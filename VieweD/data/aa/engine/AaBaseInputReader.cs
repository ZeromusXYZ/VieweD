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

public class AaBaseInputReader : BaseInputReader
{
    public override string Name => "ArcheAge Reader Base";
    public override string Description => "Base reader class for handling ArcheAge data";
    public override string DataFolder => "aa";

    protected BinaryReader? Reader { get; set; }
    protected uint XorKey { get; set; }
    protected byte[] AesKey { get; set; } = new byte[16];
    protected byte[] Iv { get; set; } = new byte[16];
    protected uint NumberPacketCounter { get; set; }
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

    public AaBaseInputReader(ViewedProjectTab parentProject) : base(parentProject)
    {
        // ExpectedFileExtensions.Add(".d");

        // InitReader(parentProject);
    }

    public AaBaseInputReader()
    {
        // ExpectedFileExtensions.Add(".d");
    }

    protected void InitReader(ViewedProjectTab parentProject)
    {
        DecompressionHandler.InitializeInflate(false);

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
        return new AaBaseInputReader(parentProject);
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

    public bool DecompressL4Data(BasePacketData pd)
    {
        int inflateOffset;
        switch (pd.CompressionLevel)
        {
            case 3:
                inflateOffset = 2;
                break;
            case 4:
                // Read the number of contained sub-packets (after inflate)
                pd.UnParseSubPacketCount = pd.GetByteAtPos(pd.Cursor);

                // Next byte seems like it's always 0x00, might be the possible high-byte for the counter value of l4Data1
                var l4Data2 = pd.GetByteAtPos(pd.Cursor);
                if (l4Data2 > 0)
                {
                    // throw new Exception($"l4data2 was larger than zero");
                }

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
        if (ParentProject.Settings.DecryptionName == string.Empty)
        {
            var keyName = DecryptKeySelectDialog.SelectDecryptionKeyName(ParentProject.Settings.DecryptionName, ParentProject);
            ParentProject.Settings.DecryptionName = keyName;
        }

        if (ParentProject.Settings.DecryptionName != string.Empty)
            Encryption.GetValuesForVersion(ParentProject.Settings.DecryptionName, out c1, out c2);

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