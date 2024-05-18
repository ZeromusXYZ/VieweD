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
using SharpPcap.LibPcap;
using SharpPcap;
using PacketDotNet;
using Newtonsoft.Json.Linq;

namespace VieweD.data.aa.engine;

public class AaPCapInputReader : AaBaseInputReader
{
    public override string Name => "ArcheAge Pcap Reader";
    public override string Description => "Supports pcap(ng) files containing ArcheAge capture data";
    public override string DataFolder => "aa";

    protected CaptureFileReaderDevice? ReaderDevice { get; set; }

    private CaptureStoppedEventStatus StopState { get; set; }
    private DateTime MinTime { get; set; }
    private DateTime MaxTime { get; set; }

    public AaPCapInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".pcapng");
        ExpectedFileExtensions.Add(".pcap");

        InitReader(parentProject);
    }

    public AaPCapInputReader()
    {
        ExpectedFileExtensions.Add(".pcapng");
        ExpectedFileExtensions.Add(".pcap");
    }

    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new AaPCapInputReader(parentProject);
    }

    public override bool Open(Stream source, string fileName)
    {
        try
        {
            // Pcap reader actually requires a file as input
            // TODO: Fine a workaround so it can read a stream without having to make a temp-file first
            if (!File.Exists(fileName))
                return false;

            ReaderDevice = new CaptureFileReaderDevice(fileName);
            if (ParentProject != null)
                ParentProject.TimeStampFormat = "HH:mm:ss.fff";
            MinTime = DateTime.MaxValue;
            MaxTime = DateTime.MinValue;
            // XorKey = 0x6D783F3C;
            // AesKey = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31 };

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
        if (ReaderDevice == null)
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

            // var handler = new PacketArrivalEventHandler(ReaderDeviceOnPacketArrival);
            StopState = CaptureStoppedEventStatus.ErrorWhileCapturing;
            ReaderDevice.OnPacketArrival += ReaderDeviceOnPacketArrival;
            ReaderDevice.OnCaptureStopped += ReaderDeviceOnPacketStopped;
            ReaderDevice.Open();
            ReaderDevice.Capture(); // 50 for testing // Reads all the data

            // shorten the timestamp if the total capture is less than 1 hour
            var dMin = minTime - DateTime.MinValue;
            var dMax = maxTime - DateTime.MinValue;
            if ((dMin.TotalHours < 1.0) && (dMax.TotalHours < 1.0))
                ParentProject.TimeStampFormat = "mm:ss.fff";

            // end progress update
            ViewedProjectTab.OnInputProgressUpdate(this, 100, 100);
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return -1;
        }
        finally
        {
            ReaderDevice.Close();
            ReaderDevice.OnPacketArrival -= ReaderDeviceOnPacketArrival;
            ReaderDevice.OnCaptureStopped -= ReaderDeviceOnPacketStopped;
        }
        if (StopState == CaptureStoppedEventStatus.ErrorWhileCapturing)
        {
            ParentProject?.OnInputError(this, "There was a error reading the capture file");
            return -1;
        }

        return packetCounter;
    }

    private void ReaderDeviceOnPacketArrival(object sender, PacketCapture e)
    {
        if (ParentProject == null)
            return;
        var rawPacket = e.GetPacket();

        if (rawPacket == null)
            return;

        var packet = TcpPacket.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var ip4Packet = packet.PayloadPacket as IPv4Packet;

        if (ip4Packet?.Protocol != ProtocolType.Tcp)
            return;

        var tcp = ip4Packet.Extract<TcpPacket>();
        if (tcp == null)
            return;

        if (tcp.PayloadData.Length <= 0)
            return;

        var data = new BasePacketData(ParentProject);
        data.SourceIp = ip4Packet.SourceAddress.ToString();
        data.DestinationIp = ip4Packet.DestinationAddress.ToString();
        var sPort = tcp.SourcePort;
        var dPort = tcp.DestinationPort;

        if (dPort == 0)
            data.PacketDataDirection = PacketDataDirection.Unknown;
        else
        if (ParentProject.PortToStreamIdMapping.ContainsKey(sPort))
        {
            data.SourcePort = sPort;
            data.DestinationPort = dPort;
            data.PacketDataDirection = PacketDataDirection.Incoming;
        }
        else
        if (ParentProject.PortToStreamIdMapping.ContainsKey(dPort))
        {
            data.SourcePort = dPort;
            data.DestinationPort = sPort;
            data.PacketDataDirection = PacketDataDirection.Outgoing;
        }
        else
        {
            data.SourcePort = sPort;
            data.DestinationPort = dPort;
            data.PacketDataDirection = PacketDataDirection.Unknown;
        }

        // Get Port for this stream
        var streamInfoFromPort = ParentProject.GetExpectedStreamIdByPort(data.SourcePort, 0);

        data.TimeStamp = DateTime.MinValue.AddSeconds(rawPacket.Timeval.Seconds);
        // grab min and max timestamp used
        if (data.TimeStamp > MaxTime)
            MaxTime = data.TimeStamp;
        if (data.TimeStamp < MinTime)
            MinTime = data.TimeStamp;

        data.ByteData.AddRange(tcp.PayloadData);
        data.SyncId = ParentProject.LoadedPacketList.Count; // dummy sync

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
                var l1Hash = data.GetUInt16AtPos(data.Cursor); // Hash L1
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

        // update position
        ViewedProjectTab.OnInputProgressUpdate(this, data.SyncId % 0x0100, 0x0100);
    }

    private void ReaderDeviceOnPacketStopped(object sender, CaptureStoppedEventStatus status)
    {
        StopState = status;
    }
}