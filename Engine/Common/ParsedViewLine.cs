using System;
using System.Drawing;

namespace VieweD.Engine.Common
{
    public class ParsedViewLine
    {
        public string Pos ;
        public string Var ;
        public string Data ;
        public ushort FieldIndex;
        public Color FieldColor;
        public UInt64 DataAsUInt64;
        public string ExtraInfo;

        public bool MatchesSearch(SearchParameters p)
        {
            bool res = true;
            var RawBytes = BitConverter.GetBytes(DataAsUInt64);
            var stringSearchHex = string.Empty;
            var stringSearchDec = string.Empty;

            if ((res) && (p.SearchByByte))
            {
                res = false;
                if (Array.IndexOf(RawBytes,p.SearchByte) >= 0)
                    res = true;

                if (Data.ToLower().Contains(p.SearchByte.ToString("X2").ToLower()))
                    res = true;
            }

            if ((res) && (p.SearchByUInt16))
            {
                res = false;
                for (int i = 0; i < RawBytes.Length - 2; i++)
                {
                    
                    var n = BitConverter.ToUInt16(RawBytes,i);
                    if (n == p.SearchUInt16)
                    {
                        res = true;
                        break;
                    }
                }
                if (Data.ToLower().Contains(p.SearchUInt16.ToString("X4").ToLower()))
                    res = true;
            }

            if ((res) && (p.SearchByUInt24))
            {
                res = false;
                for (int i = 0; i < RawBytes.Length - 3; i++)
                {
                    var rd = new byte[4];
                    Array.Copy(RawBytes,i,rd,0,3);
                    UInt32 n = BitConverter.ToUInt32(rd,0);
                   
                    if (n == p.SearchUInt24)
                    {
                        res = true;
                        break;
                    }
                }
                if (Data.ToLower().Contains(p.SearchUInt24.ToString("X3").ToLower()))
                    res = true;
            }

            if ((res) && (p.SearchByUInt32))
            {
                res = false;
                for (int i = 0; i < RawBytes.Length - 4; i++)
                {
                    var n = BitConverter.ToUInt32(RawBytes,i);
                    if (n == p.SearchUInt32)
                    {
                        res = true;
                        break;
                    }
                }
                if (Data.ToLower().Contains(p.SearchUInt32.ToString("X8").ToLower()))
                    res = true;
            }

            if ((res) && (p.SearchByParsedData) && (p.SearchParsedFieldValue != string.Empty))
            {
                res = false;
                if (p.SearchParsedFieldName != string.Empty)
                {
                    // Field Name Specified
                    res = (Var.ToLower().Contains(p.SearchParsedFieldName) && Data.ToLower().Contains(p.SearchParsedFieldValue));
                }
                else
                {
                    // No field name defined
                    res = Data.ToLower().Contains(p.SearchParsedFieldValue);
                }
            }

            return res;
        }
    }
}