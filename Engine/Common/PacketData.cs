using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VieweD.Engine.Common
{
    public class PacketData
    {
        public PacketList Parent { get; set; }
        public PacketData Creator { get; set; }
        public List<String> RawText { get; set; }
        public string HeaderText { get; set; }
        public string OriginalHeaderText { get; set; }
        public List<byte> RawBytes { get; set; }
        public PacketLogTypes PacketLogType { get; set; }
        public byte PacketLevel { get; set; }
        public byte OriginalPacketLevel { get; set; }
        public byte StreamId { get; set; }
        public UInt16 PacketId { get; set; }
        public UInt16 PacketDataSize { get; set; }
        public uint PacketSync { get; set; } // Only UInt16 is used in FFXI
        public DateTime TimeStamp { get; set; }
        public DateTime VirtualTimeStamp { get; set; }
        public string OriginalTimeString { get; set; }
        public ushort CapturedZoneId { get; set; }
        public bool MarkedAsInvalid { get; set; }
        public bool MarkedAsDimmed { get; set; }

        public PacketParser PP;
        public int Cursor { get => cursor; set { cursor = value; BitCursor = 0; } }
        public byte BitCursor { get; set; }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=netframework-4.7.2#System_DateTime_Parse_System_String_System_IFormatProvider_System_Globalization_DateTimeStyles_
        // Assume a date and time string formatted for the fr-FR culture is the local 
        // time and convert it to UTC.
        // dateString = "2008-03-01 10:00";
        public CultureInfo cultureForDateTimeParse = CultureInfo.CreateSpecificCulture("fr-FR"); // French seems to best match for what we need here
        public DateTimeStyles stylesForDateTimeParse = DateTimeStyles.AssumeLocal;
        private int cursor;

        public PacketData(PacketList parent)
        {
            Parent = parent;
            Creator = null; 
            RawText = new List<string>();
            HeaderText = "Unknown Header";
            OriginalHeaderText = "";
            RawBytes = new List<byte>();
            PacketLogType = PacketLogTypes.Unknown;
            PacketLevel = 0x00;
            PacketId = 0x000;
            PacketDataSize = 0x0000;
            PacketSync = 0x0000;
            StreamId = 0;
            TimeStamp = new DateTime(0);
            VirtualTimeStamp = new DateTime(0);
            OriginalTimeString = "";
            CapturedZoneId = 0;
            PP = null;
            Cursor = 0;
            BitCursor = 0;
        }

        ~PacketData()
        {
            RawText.Clear();
            RawBytes = null;
        }

        public int AddRawLineAsBytes(string s)
        {

            var simpleLine = s.Replace(" ", "").Replace("\t", "");

            var dataStartPos = simpleLine.IndexOf("|", StringComparison.InvariantCulture) + 1;
            if (simpleLine.Length < dataStartPos + 32)
            {
                // Data seems too short
                return 0;
            }
            var dataString = simpleLine.Substring(dataStartPos, 32); // max 32 hex digits expect

            var c = 0;
            for (var i = 0; i <= 0xf; i++)
            {
                var h = dataString.Substring(i * 2, 2);
                if (h != "--")
                {
                    try
                    {
                        byte b = byte.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        RawBytes.Add(b);
                    }
                    catch
                    {
                        // ignored
                    }

                    c++;
                }
            }
            return c;

            /* Example:
            //        1         2         3         4         5         6         7         8         9
            01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890

            [2018-05-16 18:11:35] Outgoing packet 0x015:
                    |  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F      | 0123456789ABCDEF
                -----------------------------------------------------  ----------------------
                  0 | 15 10 9E 00 CF 50 A0 C3 04 0E 1C C2 46 BF 33 43    0 | .....P......F.3C
                  1 | 00 00 02 00 5D 00 00 00 49 97 B8 69 00 00 00 00    1 | ....]...I..i....
            ...
                  5 | 00 00 00 00 -- -- -- -- -- -- -- -- -- -- -- --    5 | ....------------
            */

            /*
            // if (s.Length < 81)
            if (s.Length < 57)
            {
                // Doesn't look like a correct format
                return 0;
            }

            int c = 0;
            for (int i = 0; i <= 0xf; i++)
            {
                var h = s.Substring(10 + (i * 3), 2);
                if (h != "--")
                {
                    try
                    {
                        byte b = byte.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        RawBytes.Add(b);
                    }
                    catch { }
                    c++;
                }
            }
            return c;
            */
        }

        public int AddRawPacketeerLineAsBytes(string s)
        {
            /* Example:
            //           1         2         3         4         5         6         7         8         9
            // 0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // [C->S] Id: 001A | Size: 28
            //     1A 0E ED 24 D5 10 10 01 D5 00 00 00 00 00 00 00  ..í$Õ...Õ.......
            */

            if (s.StartsWith("--"))
            {
                // Still a comment, get out of here
                return 0;
            }

            if (s.Length < 51)
            {
                // Doesn't look like a correct format
                return 0;
            }

            int c = 0;
            for (int i = 0; i <= 0xf; i++)
            {
                var h = s.Substring(4 + (i * 3), 2);
                // If this fails, we're probably at the end of the packet
                // Unlike windower, Packeteer doesn't add dashes for the blanks
                if ((h != "--") && (h != "  ") && (h != " "))
                {
                    byte b = byte.Parse(h, System.Globalization.NumberStyles.HexNumber);
                    //if (!byte.TryParse("0x" + h, out byte b))
                    //    break;
                    RawBytes.Add(b);
                    c++;
                }
            }
            return c;
        }

        public int AddRawHexStringDataAsBytes(string hexData)
        {
            int res = 0;
            try
            {
                RawBytes.Clear();
                string dataLine = hexData.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                for (int i = 0; i < (dataLine.Length - 1); i += 2)
                {
                    string num = dataLine.Substring(i, 2);
                    byte b = byte.Parse(num, System.Globalization.NumberStyles.HexNumber);
                    RawBytes.Add(b);
                    res++;
                }
                /*
                string[] nums = hexData.Split(' ');
                foreach(string num in nums)
                {
                    byte b = byte.Parse(num, System.Globalization.NumberStyles.HexNumber);
                    RawBytes.Add(b);
                    res++;
                }
                */
            }
            catch
            {
                //
            }
            return res;
        }

        public string PrintRawBytesAsHex()
        {
            const int ValuesPerRow = 16;
            string res = "";
            res += "   |  0  1  2  3   4  5  6  7   8  9  A  B   C  D  E  F\r\n";
            res += "---+----------------------------------------------------\r\n";
            int lineNumber = 0;
            for (int i = 0; i < RawBytes.Count; i++)
            {
                if ((i % ValuesPerRow) == 0)
                    res += lineNumber.ToString("X2") + " | ";

                res += RawBytes[i].ToString("X2");

                if ((i % ValuesPerRow) == (ValuesPerRow - 1))
                {
                    res += "\r\n";
                    lineNumber++;
                }
                else
                {
                    res += " ";
                    if ((i % 4) == 3)
                        res += " "; // Extra space every 4 bytes
                }
            }
            return res;
        }

        public byte GetByteAtPos(int pos)
        {
            if (pos > (RawBytes.Count - 1))
                return 0;
            Cursor = pos + 1;
            return RawBytes[pos];
        }

        public sbyte GetSByteAtPos(int pos)
        {
            if (pos > (RawBytes.Count - 1))
                return 0;
            Cursor = pos + 1;
            return unchecked((sbyte)RawBytes[pos]);
        }


        public bool GetBitAtPos(int pos, int bit)
        {
            if ((pos > (RawBytes.Count - 1)) || ((bit < 0) || (bit > 7)))
                return false;
            byte b = RawBytes[(int)pos];
            byte bitmask = (byte)(0x01 << (int)bit);
            Cursor = pos;
            BitCursor = (byte)bit;
            return ((b & bitmask) != 0);
        }

        public UInt16 GetUInt16AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 2))
                return 0;
            Cursor = pos + 2;
            return BitConverter.ToUInt16(RawBytes.GetRange(pos, 2).ToArray(), 0);
        }

        public Int16 GetInt16AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 2))
                return 0;
            Cursor = pos + 2;
            return BitConverter.ToInt16(RawBytes.GetRange(pos, 2).ToArray(), 0);
        }

        public UInt32 GetUInt32AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 4))
                return 0;
            Cursor = pos + 4;
            return BitConverter.ToUInt32(RawBytes.GetRange(pos, 4).ToArray(), 0);
        }

        public Int32 GetInt32AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 4))
                return 0;
            Cursor = pos + 4;
            return BitConverter.ToInt32(RawBytes.GetRange(pos, 4).ToArray(), 0);
        }

        public UInt64 GetUInt64AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 8))
                return 0;
            Cursor = pos + 8;
            return BitConverter.ToUInt64(RawBytes.GetRange(pos, 8).ToArray(), 0);
        }

        public Int64 GetInt64AtPos(int pos)
        {
            if (pos > (RawBytes.Count - 8))
                return 0;
            Cursor = pos + 8;
            return BitConverter.ToInt64(RawBytes.GetRange(pos, 8).ToArray(), 0);
        }


        public string GetTimeStampAtPos(int pos)
        {
            string res = "???";
            if (pos > (RawBytes.Count - 4))
                return res;

            try
            {
                UInt32 DT = GetUInt32AtPos(pos);
                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(DT).ToLocalTime();
                // res = dtDateTime.ToLongTimeString();
                res = dtDateTime.ToLongDateString() + " " + dtDateTime.ToLongTimeString();
                res = dtDateTime.ToString("yyyy/MM(MMM)/dd - HH:mm:ss");
            }
            catch
            {
                res = "ERROR";
            }
            Cursor = pos + 4;
            return res;
        }

        public string GetStringAtPos(int pos, int maxSize = -1)
        {
            string res = string.Empty;
            int i = 0;
            while (((i + pos) < RawBytes.Count) && (RawBytes[pos + i] != 0) && ((maxSize == -1) || (res.Length < maxSize)))
            {
                res += (char)RawBytes[pos + i];
                i++;
            }
            if (maxSize < 0)
                Cursor = pos + res.Length;
            else
                Cursor = pos + maxSize;
            return res;
        }

        public string GetDataAtPos(int pos, int size)
        {
            string res = "";
            int i = 0;
            while (((i + pos) < RawBytes.Count) && (i < size) && (i < 256))
            {
                res += RawBytes[i + pos].ToString("X2") + " ";
                i++;
            }
            Cursor = pos + size;
            return res;
        }

        public byte[] GetDataBytesAtPos(int pos, int size)
        {
            List<Byte> res = new List<byte>();
            int i = 0;
            while (((i + pos) < RawBytes.Count) && (i < size) && (i < 256))
            {
                res.Add(RawBytes[i + pos]);
                i++;
            }
            Cursor = pos + size;
            return res.ToArray();
        }


        public string GetIP4AtPos(int pos)
        {
            if (pos > RawBytes.Count - 4)
                return "";
            Cursor = pos + 4;
            return RawBytes[pos + 0].ToString() + "." + RawBytes[pos + 1].ToString() + "." + RawBytes[pos + 2].ToString() + "." + RawBytes[pos + 3].ToString();
        }

        public Int64 GetBitsAtPos(int pos, int BitOffset, int BitsSize)
        {
            Int64 res = 0;
            int P = pos;
            int B = BitOffset;
            int RestBits = BitsSize;
            // Minimum 1 bit
            if (RestBits < 1)
                RestBits = 1;
            Int64 Mask = 1;
            while (RestBits > 0)
            {
                while (B >= 8)
                {
                    B -= 8;
                    P++;
                }
                // Add Mask Value if Bit is set
                if (GetBitAtPos(P, B))
                    res += Mask;
                RestBits--;
                Mask = Mask << 1;
                B++;
            }
            return res;
        }

        public Int64 GetBitsAtBitPos(int BitOffset, int BitsSize)
        {
            return GetBitsAtPos(BitOffset / 8, BitOffset % 8, BitsSize);
        }

        public string GetPackedString16AtPos(int pos, char[] Encoded6BitKey)
        {
            string res = "";
            // Hex: B8 81 68 24  72 14 4F 10  54 0C 8F 00  00 00 00 00
            // Bit: 101110 00
            // 1000 0001
            // 01 101000
            // 001001 00
            // 0111 0010
            // 00 010100
            // 010011 11
            // 0001 0000
            // 01 010100
            // 000011 00
            // 1000 1111
            // 00 000000

            // PackedString: TheNightsWatch (with no spaces)
            // PackedNum: 2E 08 05 ...
            // 101110  T
            // 001000  h
            // 000101  e
            //

            // A_  6F F0    011011 11-1111 0000  =>  1B 3F 00  =>  A
            // B_  73 F0    011100 11-1111 0000  =>  1C 3F 00  =>  B
            // F_  83 F0    100000 11-1111 0000  =>  20 3F 00  =>  F

            // EncodeLSStr : Array [0..63] of Char = (
            // #0 ,'a','b','c','d','e','f','g',  'h','i','j','k','l','m','n','o', // $00
            // 'p','q','r','s','t','u','v','w',  'x','y','z','A','B','C','D','E', // $10
            // 'F','G','H','I','J','K','L','M',  'N','O','P','Q','R','S','T','U', // $20
            // 'V','W','X','Y','Z',' ',' ',' ',  ' ',' ',' ',' ',' ',' ',' ', #0  // $30
            //  0   1   2   3   4   5   6   7     8   9   A   B   C   D   E   F
            // );
            int Offset = 0;
            while ((Offset / 8) < 15)
            {
                byte encodedChar = 0;
                byte bitMask = 0b00100000;
                for (int bit = 0; bit < 6; bit++)
                {
                    bool isSet = GetBitAtPos(pos + ((Offset + bit) / 8), 7 - ((Offset + bit) % 8));
                    if (isSet)
                        encodedChar += bitMask;
                    bitMask >>= 1;
                }
                // GetBitsAtPos(pos + (Offset / 8), (Offset % 8), 6);
                if ((encodedChar >= Encoded6BitKey.Length) || (encodedChar < 0))
                    break;
                var c = Encoded6BitKey[encodedChar];
                if (c == 0)
                    break;
                res += c;
                Offset += 6;
            }
            return res;
        }

        public Single GetFloatAtPos(int pos)
        {
            if (pos > (RawBytes.Count - 4))
                return 0;
            Cursor = pos + 4;
            return BitConverter.ToSingle(RawBytes.GetRange(pos, 4).ToArray(), 0);
        }

        public Double GetDoubleAtPos(int pos)
        {
            if (pos > (RawBytes.Count - 8))
                return 0;
            Cursor = pos + 8;
            return BitConverter.ToDouble(RawBytes.GetRange(pos, 8).ToArray(), 0);
        }

        public int FindByte(byte aByte)
        {
            return RawBytes.IndexOf(aByte);
        }

        public int FindUInt16(UInt16 aUInt16)
        {
            for (int i = 0; i < (RawBytes.Count - 2); i++)
            {
                if (GetUInt16AtPos(i) == aUInt16)
                    return i;
            }
            return -1;
        }

        public int FindUInt32(UInt32 aUInt32)
        {
            for (int i = 0; i < (RawBytes.Count - 4); i++)
            {
                if (GetUInt32AtPos(i) == aUInt32)
                    return i;
            }
            return -1;
        }

        public int FindUInt64(UInt64 aUInt64)
        {
            for (int i = 0; i < (RawBytes.Count - 8); i++)
            {
                if (GetUInt64AtPos(i) == aUInt64)
                    return i;
            }
            return -1;
        }

        public bool DateTimeParse(string s, out DateTime res)
        {
            res = new DateTime(0);
            if (s.Length != 19)
                return false;
            try
            {
                // 0         1
                // 01234567890123456789
                // 2018-05-16 18:11:35
                var yyyy = int.Parse(s.Substring(0, 4));
                var mm = int.Parse(s.Substring(5, 2));
                var dd = int.Parse(s.Substring(8, 2));
                var hh = int.Parse(s.Substring(11, 2));
                var nn = int.Parse(s.Substring(14, 2));
                var ss = int.Parse(s.Substring(17, 2));
                res = new DateTime(yyyy, mm, dd, hh, nn, ss);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool CompileData(string packetLogFileFormats)
        {
            return Parent?._parentTab?.Engine?.CompileData(this, packetLogFileFormats) ?? false;
        }

        public void CompileSpecial(PacketList packetList)
        {
            Parent?._parentTab?.Engine?.CompileSpecial(this, packetList);
        }

        public bool MatchesSearch(SearchParameters p)
        {
            if ((PacketLogType == PacketLogTypes.Incoming) && (!p.SearchIncoming))
                return false;
            if ((PacketLogType == PacketLogTypes.Outgoing) && (!p.SearchOutgoing))
                return false;

            bool res = true;

            if ((res) && (p.SearchByPacketId))
            {
                res = false;
                if (PacketId == p.SearchPacketId)
                    res = true;
            }

            if ((res) && (p.SearchByPacketLevel))
            {
                res = false;
                if ((PacketLevel == p.SearchPacketLevel) || (OriginalPacketLevel == p.SearchPacketLevel))
                    res = true;
            }

            if ((res) && (p.SearchBySync))
            {
                res = false;
                if (PacketSync == p.SearchSync)
                    res = true;
            }

            if ((res) && (p.SearchByByte))
            {
                res = false;
                if (RawBytes.IndexOf(p.SearchByte) >= 0)
                    res = true;
            }

            if ((res) && (p.SearchByUInt16))
            {
                res = false;
                for (int i = 0; i < RawBytes.Count - 2; i++)
                {
                    var n = GetUInt16AtPos(i);
                    if (n == p.SearchUInt16)
                    {
                        res = true;
                        break;
                    }
                }
            }

            if ((res) && (p.SearchByUInt24))
            {
                res = false;
                for (int i = 0; i < RawBytes.Count - 3; i++)
                {
                    var rd = GetDataBytesAtPos(i, 3).ToList();
                    rd.Add(0);
                    UInt32 d = BitConverter.ToUInt32(rd.ToArray(), 0);
                    if (d == p.SearchUInt24)
                    {
                        res = true;
                        break;
                    }
                }
            }

            if ((res) && (p.SearchByUInt32))
            {
                res = false;
                for (int i = 0; i < RawBytes.Count - 4; i++)
                {
                    var n = GetUInt32AtPos(i);
                    if (n == p.SearchUInt32)
                    {
                        res = true;
                        break;
                    }
                }
            }

            if ((res) && (PP != null) && (p.SearchByParsedData) && (p.SearchParsedFieldValue != string.Empty))
            {
                res = false;
                foreach (var f in PP.ParsedView)
                {
                    if (p.SearchParsedFieldName != string.Empty)
                    {
                        // Field Name Specified
                        res = (f.Var.ToLower().Contains(p.SearchParsedFieldName) && f.Data.ToLower().Contains(p.SearchParsedFieldValue));
                    }
                    else
                    {
                        // No field name defined
                        res = (f.Data.ToLower().IndexOf(p.SearchParsedFieldValue) >= 0);
                    }
                    if (res)
                        break;
                }
            }

            return res;
        }

    }
}