using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using VieweD.engine.common;
using SharpPcap.LibPcap;
using SharpPcap;
using PacketDotNet;
using System.Globalization;
using System.Xml;

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
    private List<byte> ReadDataBuffer { get; set; } = new();

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

    /// <summary>
    /// Loads XOR and AES keys from .keys files (xml)
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool LoadKeys(string fileName)
    {
        try
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            XmlNode? aesNode = doc.SelectSingleNode("/Settings/ArcheAge/AES");
            if (aesNode != null)
            {
                var aes = Convert.FromHexString(aesNode.InnerText);
                AesKey = aes;
            }

            XmlNode? xorNode = doc.SelectSingleNode("/Settings/ArcheAge/XOR");
            if (xorNode != null)
            {
                if (uint.TryParse(xorNode.InnerText, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var xor))
                {
                    XorKey = xor;
                }
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    public override bool Open(Stream source, string fileName)
    {
        base.Open(source, fileName);
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
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return false;
        }

        var keyFile = Path.ChangeExtension(fileName, ".keys");
        if (File.Exists(keyFile))
            _ = LoadKeys(keyFile);

        return true;
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

        ParentProject.LoadedPacketList.Clear();
        ReadDataBuffer.Clear(); // Clear reading buffer
        try
        {
            var ipMapping = new Dictionary<byte, string>();

            // var handler = new PacketArrivalEventHandler(ReaderDeviceOnPacketArrival);
            StopState = CaptureStoppedEventStatus.ErrorWhileCapturing;
            ReaderDevice.OnPacketArrival += ReaderDeviceOnPacketArrival;
            ReaderDevice.OnCaptureStopped += ReaderDeviceOnPacketStopped;
            ReaderDevice.Open(); 
            ReaderDevice.Capture(); // Start reading all the data, triggers ReaderDeviceOnPacketStopped

            // Shorten the timestamp if the total capture is less than 1 hour
            var dMin = minTime - DateTime.MinValue;
            var dMax = maxTime - DateTime.MinValue;
            if ((dMin.TotalHours < 1.0) && (dMax.TotalHours < 1.0))
                ParentProject.TimeStampFormat = "mm:ss.fff";

            // End progress update
            ViewedProjectTab.OnInputProgressUpdate(this, 100, 100);
        }
        catch (Exception ex)
        {
            ParentProject?.OnInputError(this, ex.Message);
            return -1;
        }
        finally
        {
            // Close the reader, un detach the triggers
            ReaderDevice.Close();
            ReaderDevice.OnPacketArrival -= ReaderDeviceOnPacketArrival;
            ReaderDevice.OnCaptureStopped -= ReaderDeviceOnPacketStopped;
        }
        if (StopState == CaptureStoppedEventStatus.ErrorWhileCapturing)
        {
            ParentProject?.OnInputError(this, "There was a error reading the capture file");
            return -1;
        }

        return ParentProject.LoadedPacketList.Count;
    }

    private void ReaderDeviceOnPacketArrival(object sender, PacketCapture e)
    {
        if (ParentProject == null)
            return;

        // Get the raw Capture Data
        var rawPacket = e.GetPacket();

        if (rawPacket == null)
            return;

        // Get pre-parsed packet
        var packet = TcpPacket.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        // Check if it's an IPv4 one
        var ip4Packet = packet.PayloadPacket as IPv4Packet;
        if (ip4Packet?.Protocol != ProtocolType.Tcp)
            return;

        // Extract TCP data (if any)
        var tcp = ip4Packet.Extract<TcpPacket>();
        if (tcp == null)
            return;

        // Don't bother if it's empty
        if (tcp.PayloadData.Length <= 0)
            return;

        // Add to Read buffer
        ReadDataBuffer.AddRange(tcp.PayloadData);
        
        // Generate Dummy Sync ID
        var dummySync = ParentProject.LoadedPacketList.Count;

        // Keep processing until our buffer is too small
        while (ReadDataBuffer.Count > 4)
        {
            // Get the size of the next expected game packet
            var expectedCompiledPacketSize = Convert.ToUInt16((ReadDataBuffer[1] * 0x100) + ReadDataBuffer[0]);
            // Check if our current data is big enough
            if (ReadDataBuffer.Count < expectedCompiledPacketSize)
                return; // expect more data

            // Create new PacketData object
            var data = new BasePacketData(ParentProject);
            // Grab IP and port data
            var sIp = ip4Packet.SourceAddress.ToString();
            var dIp = ip4Packet.DestinationAddress.ToString();
            var sPort = tcp.SourcePort;
            var dPort = tcp.DestinationPort;
            data.SourceIp = sIp;
            data.DestinationIp = dIp;

            // Use port data to determine what direction the packet is for and adjust the port values
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
                // Swap ports
                data.SourceIp = dIp;
                data.DestinationIp = sIp;
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

            // Get StreamPort information for this port
            var streamInfoFromPort = ParentProject.GetExpectedStreamIdByPort(data.SourcePort, 0);

            // Try to generate timestamp
            try
            {
                data.TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds((double)rawPacket.Timeval.Value);
            }
            catch
            {
                data.TimeStamp = DateTime.MinValue;
            }

            // Capture min and max timestamp used
            if (data.TimeStamp > MaxTime)
                MaxTime = data.TimeStamp;
            if (data.TimeStamp < MinTime)
                MinTime = data.TimeStamp;

            // Take meaningful data from the pool, and leave the rest in the ReadDataBuffer
            var packetSize = Convert.ToUInt16((ReadDataBuffer[1] * 0x100) + ReadDataBuffer[0]);
            var addData = ReadDataBuffer.Take(packetSize+2);
            ReadDataBuffer = ReadDataBuffer.Skip(packetSize+2).ToList();

            // Add the data to the BasePacketData
            data.ByteData.AddRange(addData);
            data.SyncId = dummySync; // dummy sync

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

            // Compile the packet depending on level
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
                default:
                    // Debug.WriteLine($"Unexpected Compression Level {data.CompressionLevel}");
                    data.MarkedAsInvalid = true;
                    // continue;
                    break;
            }

            if (data.UnParseSubPacketCount > 0)
                ParentProject.RequiresSubPacketCreation = true;

            if (CompileData(data))
                ParentProject.LoadedPacketList.Add(data);

            // update position
            if (data.SyncId % 0x0100 == 0)
                ViewedProjectTab.OnInputProgressUpdate(this, data.SyncId >> 8 % 0x0100, 0x0100);
        }

    }

    private void ReaderDeviceOnPacketStopped(object sender, CaptureStoppedEventStatus status)
    {
        StopState = status;
    }
}