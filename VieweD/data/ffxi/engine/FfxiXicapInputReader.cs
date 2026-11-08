﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Crypto;
using VieweD.engine.common;
using ZstdSharp;

namespace VieweD.data.ffxi.engine;

// ReSharper disable once UnusedMember.Global
/// <inheritdoc />
public class FfxiXicapInputReader : BaseInputReader
{
    public override string Name => "FFXI xicap";
    public override string Description => "Supports .xicap files containing Final Fantasy XI capture data (format version 5)";
    public override string DataFolder => "ffxi";

    // Magic and format version
    private static readonly byte[] Signature = { 0x89, 0x58, 0x43, 0x50, 0x0D, 0x0A, 0x1A, 0x0A, 0x05 };

    private byte[] FileData { get; set; } = Array.Empty<byte>();

    public FfxiXicapInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".xicap");

        ParentProject!.PortToStreamIdMapping.Clear();
        ParentProject.RegisterPort(0, "Game", "G"); // 0 - Base Game
    }

    public FfxiXicapInputReader()
    {
        ExpectedFileExtensions.Add(".xicap");
    }

    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new FfxiXicapInputReader(parentProject);
    }

    public override bool Open(Stream source, string fileName)
    {
        using var memoryStream = new MemoryStream();
        source.CopyTo(memoryStream);
        FileData = memoryStream.ToArray();

        if (FileData.AsSpan().StartsWith(Signature))
            return true;

        ParentProject?.OnInputError(this, "Not a version 5 xicap file");
        return false;
    }

    public override int ReadAllData()
    {
        if (ParentProject == null)
            return -1;

        ParentProject.TimeStampFormat = "HH:mm:ss.fff";

        try
        {
            // Skip the blocks after the 160 byte header
            var pos = 160;
            for (var i = 0; i < FileData[11]; i++)
                pos += 12 + BinaryPrimitives.ReadInt32LittleEndian(FileData.AsSpan(pos + 4));

            // The payloads of all frames together form a single zstd stream
            var frames = new List<int>();
            using var payloads = new MemoryStream();
            while (pos + 28 <= FileData.Length && FileData.AsSpan(pos).StartsWith("XCFR"u8))
            {
                // A cut-off capture ends at its first incomplete or damaged frame
                var crcPos = pos + 28L + BinaryPrimitives.ReadUInt32LittleEndian(FileData.AsSpan(pos + 4));
                if (crcPos + 4 > FileData.Length ||
                    Crc32Stream.Compute(FileData[pos..(int)crcPos]) != BinaryPrimitives.ReadUInt32LittleEndian(FileData.AsSpan((int)crcPos)))
                    break;

                frames.Add(pos);
                payloads.Write(FileData, pos + 28, (int)crcPos - pos - 28);
                pos = (int)crcPos + 4;
            }

            payloads.Position = 0;
            using var decompressor = new DecompressionStream(payloads, checkEndOfStream: false);

            for (var f = 0; f < frames.Count; f++)
            {
                ViewedProjectTab.OnInputProgressUpdate(this, f, frames.Count);
                var header = FileData.AsSpan(frames[f]);
                var plain = new byte[BinaryPrimitives.ReadInt32LittleEndian(header[8..])];
                decompressor.ReadExactly(plain);
                ReadFrame(plain, BinaryPrimitives.ReadInt32LittleEndian(header[12..]),
                    BinaryPrimitives.ReadInt32LittleEndian(header[16..]), BinaryPrimitives.ReadUInt64LittleEndian(header[20..]));
            }

            ViewedProjectTab.OnInputProgressUpdate(this, frames.Count, frames.Count);
        }
        catch (Exception ex)
        {
            ParentProject.OnInputError(this, ex.Message);
            return -1;
        }

        return ParentProject.LoadedPacketList.Count;
    }

    private void ReadFrame(byte[] plain, int packetCount, int markerCount, ulong timeStamp)
    {
        var pos = 0;
        var deltaMs = ReadVarints(plain, ref pos, packetCount);
        var flags = ReadVarints(plain, ref pos, packetCount);
        var channels = ReadVarints(plain, ref pos, packetCount);
        var opcodes = ReadVarints(plain, ref pos, packetCount);
        var lengths = ReadVarints(plain, ref pos, packetCount);
        var syncs = ReadVarints(plain, ref pos, channels.Count(c => c == 0));

        // Skip the markers: packet index, kind, text
        for (var i = 0; i < markerCount; i++)
        {
            ReadVarint(plain, ref pos);
            pos++;
            var textLength = (int)ReadVarint(plain, ref pos);
            pos += textLength;
        }

        // Packets with the same channel, direction and opcode share a group of bytes
        var keys = Enumerable.Range(0, packetCount).Select(i => (channels[i], flags[i] & 1, opcodes[i])).ToArray();
        var groups = new List<(uint, uint, uint)>();
        foreach (var key in keys)
            if (!groups.Contains(key))
                groups.Add(key);

        var groupSizes = ReadVarints(plain, ref pos, groups.Count);
        var groupCursors = new Dictionary<(uint, uint, uint), int>();
        for (var g = 0; g < groups.Count; g++)
        {
            groupCursors[groups[g]] = pos;
            pos += (int)groupSizes[g];
        }

        var lastSync = new ushort[2];
        var syncIndex = 0;
        for (var i = 0; i < packetCount; i++)
        {
            timeStamp += deltaMs[i];
            // Only the game channel holds FFXI packets
            if (channels[i] != 0)
                continue;

            var start = groupCursors[keys[i]];
            var size = lengths[i] == 0 ? (plain[start + 1] >> 1) * 4 : (int)lengths[i] - 1;
            groupCursors[keys[i]] = start + size;

            // The sync is stored zigzag encoded, as the change from the previous packet in the same direction
            var direction = (int)(flags[i] & 1);
            var zigzag = syncs[syncIndex++];
            lastSync[direction] = (ushort)(lastSync[direction] + ((int)(zigzag >> 1) ^ -(int)(zigzag & 1)));

            var packetData = new BasePacketData(ParentProject!)
            {
                PacketDataDirection = direction == 0 ? PacketDataDirection.Incoming : PacketDataDirection.Outgoing,
                TimeStamp = DateTimeOffset.FromUnixTimeMilliseconds((long)timeStamp).LocalDateTime,
                SyncId = lastSync[direction]
            };
            packetData.ByteData.AddRange(new ArraySegment<byte>(plain, start, size));
            packetData.ByteData[2] = (byte)lastSync[direction];
            packetData.ByteData[3] = (byte)(lastSync[direction] >> 8);
            packetData.PacketId = (ushort)(packetData.GetByteAtPos(0) + (packetData.GetByteAtPos(1) & 0x01) * 0x100);
            packetData.PacketDataSize = (ushort)((packetData.GetByteAtPos(1) & 0xFE) * 2);

            if (packetData.ParentProject.LoadedPacketList.Count > 0)
            {
                var firstPacketTime = packetData.ParentProject.LoadedPacketList[0].TimeStamp;
                packetData.OffsetFromStart = packetData.TimeStamp - firstPacketTime;
                packetData.VirtualOffsetFromStart = packetData.OffsetFromStart;
            }

            packetData.BuildHeaderText();
            ParentProject!.OnInputDataRead(this, packetData);
        }
    }

    private static uint[] ReadVarints(byte[] data, ref int pos, int count)
    {
        var values = new uint[count];
        for (var i = 0; i < count; i++)
            values[i] = ReadVarint(data, ref pos);
        return values;
    }

    private static uint ReadVarint(byte[] data, ref int pos)
    {
        uint value = 0;
        for (var shift = 0; ; shift += 7)
        {
            var b = data[pos++];
            value |= (uint)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
                return value;
        }
    }
}
