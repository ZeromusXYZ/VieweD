using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Ionic.Zlib;
using VieweD.Forms;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.data.aa.engine;

public class AaDInputReader : AaBaseInputReader
{
    public override string Name => "ArcheAge D-file Reader";
    public override string Description => "Supports .d files containing ArcheAge capture data made by PDEC";
    public override string DataFolder => "aa";

    public AaDInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".d");

        InitReader(parentProject);
    }

    public AaDInputReader()
    {
        ExpectedFileExtensions.Add(".d");
    }

    public override bool Open(Stream source)
    {
        try
        {
            Reader = new BinaryReader(source);
            Reader.BaseStream.Seek(0, SeekOrigin.Begin);
            // Magic number check
            var magicNumberCheck = Reader.ReadUInt32();
            if (magicNumberCheck != 0x16CB844E)
                return false;

            // At time of writing only version 1 exists
            var fileVersionCheck = Reader.ReadUInt32();
            if (fileVersionCheck != 0x01)
                return false;

            XorKey = Reader.ReadUInt32();
            AesKey = Reader.ReadBytes(16);

            return true;
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return false;
        }
    }

    public override int ReadAllData()
    {
        if (Reader == null)
            return -1;

        if (ParentProject == null)
            return -1;

        var localStreamToPortMapping = new Dictionary<byte, ushort>();

        ParentProject.TimeStampFormat = "HH:mm:ss.fff";
        var minTime = DateTime.MaxValue;
        var maxTime = DateTime.MinValue;

        var packetCounter = 0;
        ParentProject.LoadedPacketList.Clear();
        try
        {
            var ipMapping = new Dictionary<byte, string>();
            var dummySync = 0;

            while (Reader.BaseStream.Position < Reader.BaseStream.Length)
            {
                // update position
                if ((packetCounter % 250) == 0)
                    ViewedProjectTab.OnInputProgressUpdate(this, (int)Reader.BaseStream.Position, (int)Reader.BaseStream.Length);

                var fileCmd = Reader.ReadByte();
                var sourceStreamId = (byte)(Reader.ReadUInt32() % 0x100);
                var offsetTime = Reader.ReadSingle();


                switch (fileCmd)
                {
                    case 1: // Connect command
                        var ip4 = Reader.ReadBytes(4);
                        var currentIpv4 = string.Join('.', ip4);
                        var currentPort = Reader.ReadUInt16();
                        ipMapping.TryAdd(sourceStreamId, currentIpv4);
                        localStreamToPortMapping.TryAdd(sourceStreamId, currentPort);
                        ParentProject.RegisterPort(currentPort, "Port " + currentPort, "?");
                        // currentStreamInfo = ParentProject.GetExpectedStreamIdByPort(currentPort, sourceStreamId);
                        break;
                    case 2: // out
                    case 3: // in
                        var dataSize = Reader.ReadInt32();
                        var rawData = Reader.ReadBytes(dataSize);


                        var data = new BasePacketData(ParentProject);
                        if (fileCmd == 2)
                            data.PacketDataDirection = PacketDataDirection.Outgoing;
                        if (fileCmd == 3)
                            data.PacketDataDirection = PacketDataDirection.Incoming;

                        // Get Port for this stream
                        if (localStreamToPortMapping.TryGetValue(sourceStreamId, out var localPort))
                            data.SourcePort = localPort;

                        var streamInfoFromPort = ParentProject.GetExpectedStreamIdByPort(data.SourcePort, 0);

                        if (ipMapping.TryGetValue(sourceStreamId, out var ip))
                            data.SourceIp = ip;
                        data.TimeStamp = DateTime.MinValue.AddMilliseconds(offsetTime * 1000.0);

                        // grab min and max timestamp used
                        if (data.TimeStamp > maxTime)
                            maxTime = data.TimeStamp;
                        if (data.TimeStamp < minTime)
                            minTime = data.TimeStamp;

                        data.ByteData.AddRange(rawData);
                        data.SyncId = dummySync;
                        dummySync++;

                        // Get Base info
                        data.Cursor = 0;
                        byte dirByte3;
                        if (streamInfoFromPort.Item3 != "?")
                        {
                            data.PacketDataSize = data.GetUInt16AtPos(data.Cursor);
                            data.PacketId = 0xFFFF;
                            dirByte3 = data.GetByteAtPos(data.Cursor);
                            data.CompressionLevel = data.GetByteAtPos(data.Cursor);
                        }
                        else
                        {
                            dirByte3 = 0;
                            data.PacketDataSize = data.ByteData.Count;
                            data.PacketId = 0x0000;
                            data.CompressionLevel = 0;
                        }

                        switch (data.CompressionLevel)
                        {
                            case 0: // Simple packet
                                data.PacketId = dirByte3;
                                break;
                            case 1: // Packet with Hash
                                _ = data.GetUInt16AtPos(data.Cursor); // Hash L1
                                data.PacketId = data.GetUInt16AtPos(data.Cursor);
                                //pd.PacketLevel = 0;
                                break;
                            case 2: // Extra Packet
                                data.PacketId = data.GetUInt16AtPos(data.Cursor);
                                break;
                            case 3: // Do Decompress L3
                                if (DecompressL3Data(data) == false)
                                {
                                    data.MarkedAsInvalid = true;
                                    //return false;
                                }
                                break;
                            case 4: // Do Decompress L4
                                if (DecompressL4Data(data) == false)
                                {
                                    data.MarkedAsInvalid = true;
                                    //return false;
                                }
                                break;
                            case 5: // Do decode L5
                                if (DecodeL5Data(data) == false)
                                {
                                    data.MarkedAsInvalid = true;
                                    // return false;
                                }
                                break;
                        }

                        if (data.UnParseSubPacketCount > 0)
                            ParentProject.RequiresSubPacketCreation = true;

                        if (CompileData(data))
                            ParentProject.LoadedPacketList.Add(data);

                        break;
                    case 4: // Disconnect
                        // Do nothing
                        break;
                    default:
                        MessageBox.Show(string.Format(Resources.ReadAllDataUnknownCommand, fileCmd, Reader.BaseStream.Position), Resources.ReadAllDataFileFormatErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                }

            }

            // shorten the timestamp if the total capture is less than 1 hour
            var dMin = minTime - DateTime.MinValue;
            var dMax = maxTime - DateTime.MinValue;
            if ((dMin.TotalHours < 1.0) && (dMax.TotalHours < 1.0))
                ParentProject.TimeStampFormat = "mm:ss.fff";

            // end progress update
            ViewedProjectTab.OnInputProgressUpdate(this, (int)Reader.BaseStream.Length, (int)Reader.BaseStream.Length);
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return -1;
        }

        return packetCounter;
    }

}