using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace VieweD.Engine.Common
{
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
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
            var res = true;
            var rawBytes = BitConverter.GetBytes(DataAsUInt64);
            //var stringSearchHex = string.Empty;
            //var stringSearchDec = string.Empty;

            if (p.SearchByByte)
            {
                res = (Array.IndexOf(rawBytes, p.SearchByte) >= 0) || Data.ToLower().Contains(p.SearchByte.ToString("X2").ToLower());
            }

            if ((res) && (p.SearchByUInt16))
            {
                res = false;
                for (var i = 0; i < rawBytes.Length - 2; i++)
                {
                    
                    var n = BitConverter.ToUInt16(rawBytes,i);
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
                for (var i = 0; i < rawBytes.Length - 3; i++)
                {
                    var rd = new byte[4];
                    Array.Copy(rawBytes,i,rd,0,3);
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
                for (var i = 0; i < rawBytes.Length - 4; i++)
                {
                    var n = BitConverter.ToUInt32(rawBytes,i);
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
                // res = false;
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