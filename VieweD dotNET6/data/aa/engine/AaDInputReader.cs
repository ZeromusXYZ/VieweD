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

public class AaDInputReader : BaseInputReader
{
    public override string Name => "ArcheAge D-file Reader";
    public override string Description => "Supports .d files containing ArcheAge capture data made by PDEC";
    public override string DataFolder => "aa";

    private BinaryReader? Reader { get; set; }
    private uint XorKey { get; set; }
    private byte[] AesKey { get; set; } = new byte[16];
    private byte[] Iv { get; set; } = new byte[16];
    private uint NumberPacketCounter { get; set; }
    public string DecryptVersion { get; set; } = string.Empty;

    // Source: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=netframework-4.7.2#System_DateTime_Parse_System_String_System_IFormatProvider_System_Globalization_DateTimeStyles_
    // Assume a date and time string formatted for the fr-FR culture is the local 
    // time and convert it to UTC.
    // dateString = "2008-03-01 10:00";
    public static readonly CultureInfo
        CultureForDateTimeParse =
            CultureInfo.CreateSpecificCulture("fr-FR"); // French seems to best match for what we need here

    public static readonly DateTimeStyles StylesForDateTimeParse = DateTimeStyles.AssumeLocal;
    protected ZlibCodec DecompressionHandler { get; set; } = new ZlibCodec(CompressionMode.Decompress);

    private AaEncryptionsBase? Encryption { get; set; }

    public AaDInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        ExpectedFileExtensions.Add(".d");

        InitReader(parentProject);
    }

    public AaDInputReader()
    {
        ExpectedFileExtensions.Add(".d");
    }

    private void InitReader(ViewedProjectTab parentProject)
    {
        // Load encryption handler (if present), otherwise use base null handler
        var enc = AaEncryptionsBase.CreateEncryptionByName("VieweD.data.aa.engine.AaEncryptions");
        if (enc != null)
            Encryption = enc;
        else
            Encryption = new AaEncryptionsBase();

        /*
        var t = Type.GetType("VieweD.data.aa.engine.AaEncryptions");
        if (t != null)
        {
            if (Activator.CreateInstance(t) is AaEncryptionsBase e)
                Encryption = e;
            else
                Encryption = new AaEncryptionsBase();
        }
        */

        _ = Encryption?.LoadKeysFromFolder(Path.Combine(Application.StartupPath, "data", DataFolder, "keys"));

        // _ = DecryptCs.LoadKeysFromFolder(Path.Combine(Application.StartupPath, "data", DataFolder, "rules", "decryption.keys"));

        #region portmappings

        parentProject.PortToStreamIdMapping.Clear();
        parentProject.RegisterPort(0, "NULL","?"); // 0 - Unknown
        parentProject.RegisterPort(1237, "Auth","A"); // 1 - Login Server
        parentProject.RegisterPort(1239, "Game","G"); // 2 - Game Server
        parentProject.RegisterPort(1250, "Stream","S"); // 3 - Data Streaming Server

        // HTTP stuff, ignored
        parentProject.RegisterPort(80, "HTTP", ""); // 4
        parentProject.RegisterPort(8080, "HTTP", ""); // 5
        parentProject.RegisterPort(443, "SSL", ""); // 6

        // AAFree specific ports
        parentProject.PortToStreamIdMapping.Add(2527, (1, "AAFree Auth","a")); // 1 - Login Server
        parentProject.PortToStreamIdMapping.Add(2529, (2, "AAFree Game","g")); // 2 - Game Server
        parentProject.PortToStreamIdMapping.Add(2560, (3, "AAFree Stream","s")); // 3 - Data Streaming Server

        #endregion
    }

    public override BaseInputReader CreateNew(ViewedProjectTab parentProject)
    {
        return new AaDInputReader(parentProject);
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

            // At time of writing it only version 1 exists
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

    public bool CompileData(BasePacketData packetData)
    {
        if (packetData.ByteData.Count < 4)
        {
            packetData.PacketId = uint.MaxValue;
            packetData.PacketDataSize = 0;
            packetData.HeaderText = "Invalid Packet Size < 4";
            return false;
        }

        packetData.BuildHeaderText();

        return true;
    }

    public bool DecompressL3Data(BasePacketData pd)
    {
        var sourceData = pd.ByteData.GetRange(pd.Cursor, pd.ByteData.Count - pd.Cursor);
        // var decompressedData = new List<byte>();
        var resultData = new List<byte>();
        try
        {
            var buffer = new byte[32768];
            var compressedBytes = sourceData.ToArray();
            var decompressedBytes = new byte[32768];
            var ms = new MemoryStream(decompressedBytes);
            DecompressionHandler.InputBuffer = compressedBytes;
            DecompressionHandler.NextIn = 0;
            DecompressionHandler.AvailableBytesIn = compressedBytes.Length;

            DecompressionHandler.OutputBuffer = buffer;
            do
            {
                DecompressionHandler.NextOut = 0;
                DecompressionHandler.AvailableBytesOut = buffer.Length;
                var rc = DecompressionHandler.Inflate(FlushType.None);

                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new Exception("inflating: " + DecompressionHandler.Message);

                ms.Write(DecompressionHandler.OutputBuffer, 0, buffer.Length - DecompressionHandler.AvailableBytesOut);
            }
            while (DecompressionHandler.AvailableBytesIn > 0 || DecompressionHandler.AvailableBytesOut == 0);

            // pass 2: finish and flush
            do
            {
                DecompressionHandler.NextOut = 0;
                DecompressionHandler.AvailableBytesOut = buffer.Length;
                var rc = DecompressionHandler.Inflate(FlushType.Finish);

                if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
                    throw new Exception("inflating: " + DecompressionHandler.Message);

                if (buffer.Length - DecompressionHandler.AvailableBytesOut > 0)
                    ms.Write(buffer, 0, buffer.Length - DecompressionHandler.AvailableBytesOut);
            }
            while (DecompressionHandler.AvailableBytesIn > 0 || DecompressionHandler.AvailableBytesOut == 0);

            // decompressedData.AddRange(DecompressedBytes.ToList().GetRange(2, (int)ms.Position - 2));

            resultData.Add((byte)(ms.Position % 0x100));
            resultData.Add((byte)(ms.Position / 0x100));
            resultData.AddRange(pd.ByteData.GetRange(2, 2));
            //resultData.AddRange(decompressedBytes.Skip(2).Take((int)ms.Position-2));
            resultData.AddRange(decompressedBytes.Take((int)ms.Position));
            pd.PacketId = BitConverter.ToUInt16(new[] { decompressedBytes[2], decompressedBytes[3] }, 0);

            pd.ByteData = resultData;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DecompressL4Data(BasePacketData pd)
    {
        int inflateOffset;
        switch (pd.CompressionLevel)
        {
            case 3:
                inflateOffset = 2;
                break;
            case 4:
                pd.UnParseSubPacketCount = pd.GetUInt16AtPos(pd.Cursor);
                // l4Data1 = pd.GetByteAtPos(pd.Cursor); // This is the number of contained sub-packets (after inflate)
                // l4Data2 = pd.GetByteAtPos(pd.Cursor); // Seems like it's always 0x00, might be the possible high-byte for the counter value of l4Data1
                inflateOffset = 1;
                break;
            default:
                return false;
        }
        var packetIdOffset = pd.Cursor;

        var sourceData = pd.ByteData.GetRange(pd.Cursor, pd.ByteData.Count - pd.Cursor);
        var decompressedData = new List<byte>();
        var resultData = new List<byte>();

        try
        {
            using var destinationMemoryStream = new MemoryStream();
            using var sourceMemoryStream = new MemoryStream(sourceData.ToArray());
            using var ds = new DeflateStream(sourceMemoryStream, CompressionMode.Decompress, true);
            ds.CopyTo(destinationMemoryStream);
            destinationMemoryStream.Flush();
            decompressedData.AddRange(destinationMemoryStream.ToArray());

            resultData.Add((byte)(decompressedData.Count % 0x100));
            resultData.Add((byte)(decompressedData.Count / 0x100));
            //resultData.AddRange(pd.RawBytes.GetRange(0, 2));
            resultData.AddRange(pd.ByteData.GetRange(2, 4 - inflateOffset));
            resultData.AddRange(decompressedData.GetRange(inflateOffset, decompressedData.Count - inflateOffset));
            
            // Get new PacketTypeID
            // pd.CompressionLevel = decompressedData[0];
            pd.CompressionLevel = 0;
            var pArr = new byte[2];
            pArr[0] = decompressedData[3];
            pArr[1] = decompressedData[2];

            pd.PacketId = BitConverter.ToUInt16(resultData.ToArray(), packetIdOffset);

            //pd.PacketID = Convert.ToUInt16((pArr[0] * 0x100) + pArr[1]);
            pd.ByteData = resultData;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DecodeL5Data(BasePacketData pd)
    {
        if (ParentProject == null)
            return false;

        if (Encryption == null)
            return false;

        uint c1 = 0;
        uint c2 = 0;

        // Ask for key if not defined yet
        if (ParentProject.DecryptionKeyName == string.Empty)
        {
            var keyName = DecryptKeySelectDialog.SelectDecryptionKeyName(ParentProject.DecryptionKeyName, ParentProject);
            ParentProject.DecryptionKeyName = keyName;
        }

        if (ParentProject.DecryptionKeyName != string.Empty)
            Encryption.GetValuesForVersion(ParentProject.DecryptionKeyName, out c1, out c2);

        // var sourceData = pd.ByteData.GetRange(pd.Cursor, pd.ByteData.Count - pd.Cursor);
        // var decodeData = new List<byte>();
        var resultData = new List<byte>();

        if (pd.PacketDataDirection == PacketDataDirection.Incoming)
        {
            // S2C
            if (pd.ByteData[2] == 0xDD && pd.ByteData[3] == 0x05)
            {
                try
                {
                    var input = pd.ByteData.GetRange(4, pd.ByteData.Count - 4).ToArray();
                    //пакет от сервера шифрованный XOR
                    //packet from server encrypted XOR
                    var output = Encryption.S2CEncrypt(input);
                    resultData.AddRange(pd.ByteData.GetRange(0, 4));
                    resultData.AddRange(output);
                    pd.ByteData = resultData;
                    pd.PacketDataSize = pd.GetUInt16AtPos(4);
                    pd.PacketId = pd.GetUInt16AtPos(pd.Cursor);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            // Marked as incoming level 5, but doesn't have the DD or 05 set correctly, or some other error
            return false;
        }
        else
        if (pd.PacketDataDirection == PacketDataDirection.Outgoing)
        {
            // Check the packet length
            if (pd.ByteData.Count % 16 != 5)
            {
                return false;
                // TODO: так не должно быть! слипшиеся пакеты прилетели
                // throw new Exception("It should not be! Sticky packages arrived!");
            }

            // Initialize IV if it's the first packet ?
            if (NumberPacketCounter == 0)
            {
                //Initialize IV for the first packet
                Iv = new byte[16];
            }

            // Get normal constants and input
            // DecryptCs.GetConstantsForVersion(DecryptVersion, out var c1, out var c2);

            var input = pd.ByteData.GetRange(2, pd.ByteData.Count - 2).ToArray();

            try
            {
                var output = Encryption.S2CDecrypt(input, XorKey, AesKey, Iv, NumberPacketCounter, c1, c2);

                // C2S L5 packet counter
                NumberPacketCounter++;

                resultData.AddRange(pd.ByteData.GetRange(0, 5));
                resultData.AddRange(output.Skip(1));
                
                pd.ByteData = resultData;
                pd.PacketDataSize = (ushort)output.Length;
                pd.PacketId = pd.GetUInt16AtPos(6);
            }
            catch (Exception ex)
            {
                throw new Exception($"Decryption Exception: {ex.Message}");
            }

            return true;
        }
        else
        {
            // Can't handle unknown directions
            return false;
        }

    }
}