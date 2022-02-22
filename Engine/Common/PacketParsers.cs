using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace VieweD.Engine.Common
{
    [SuppressMessage("ReSharper", "InconsistentNaming")] 
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class PacketParser
    {
        private const int ColumnOffset = 0;
        private const int ColumnVar = 1;
        private const int ColumnData = 2;
        private const int ColumnSize = 3;
        public UInt16 ThisPacketID { get; set; }
        public PacketLogTypes ThisPacketLogType { get; set; }
        public List<ushort> ParsedBytes = new List<ushort>();
        public List<ParsedViewLine> ParsedView = new List<ParsedViewLine>();
        public List<ushort> SelectedFields = new List<ushort>();
        public PacketData PD;
        public List<string> SwitchBlocks = new List<string>();
        public string LastSwitchedBlock = "";
        public string PreParsedSwitchBlock = "?" ;
        public List<string> RawParseData = new List<string>(); // not used by all engines
        static public List<string> AllFieldNames = new List<string>();

        static protected void AddFieldNameToList(string fieldName)
        {
            var FilteredFieldName = EngineBase.StripSpacer(fieldName);
            if (FilteredFieldName.StartsWith("??"))
                return;
            if (AllFieldNames.IndexOf(FilteredFieldName) < 0)
                AllFieldNames.Add(FilteredFieldName);
        }

        public PacketParser()
        {
            PD = null;
            ThisPacketID = 0;
            ThisPacketLogType = PacketLogTypes.Unknown;
        }

        public Color GetDataColor(int fieldIndex)
        {
            if (PacketColors.DataColors.Count > 0)
            {
                return PacketColors.DataColors[fieldIndex % PacketColors.DataColors.Count];
            }
            else
            {
                return Color.MediumPurple;
            }
        }

        public void AssignPacket(PacketData PacketData)
        {
            PD = PacketData;
            ParsedBytes.Clear();
            SelectedFields.Clear();
            for (int i = 0; i < PD.RawBytes.Count(); i++)
            {
                ParsedBytes.Add(0x00); // 0 = unparsed
            }
        }

        public string ByteToBits(byte b)
        {
            string res = "";
            for(int i = 1;i < 256;i <<= 1)
            {
                if (i == 16)
                    res = " " + res;

                if ((b & i) != 0)
                {
                    res = "1" + res ;
                }
                else
                {
                    res = "0" + res ;
                }
            }

            return res;
        }

        static public string MSToString(UInt32 ms)
        {
            UInt32 r = ms % 1000;
            UInt32 v = ms / 1000;
            string res = r.ToString("0000")+"ms";
            if (v > 0)
            {
                r = v % 60;
                v = v / 60;
                res = r.ToString("00") + "s " + res ;
                if (v > 0)
                {
                    r = v % 60;
                    v = v / 60;
                    res = r.ToString("00") + "m " + res;
                    if (v > 0)
                    {
                        r = v % 24;
                        v = v / 24;
                        res = r.ToString("00") + "h " + res;
                        if (v > 0)
                        {
                            res = v.ToString() + "d " + res;
                        }
                    }
                }
            }
            return res;
        }

        public string FramesToString(UInt32 frames)
        {
            UInt32 r = frames % 60;
            UInt32 v = frames / 60;
            string res = r.ToString("00") + "f";
            if (v > 0)
            {
                r = v % 60;
                v = v / 60;
                res = r.ToString("00") + " / " + res;
                if (v > 0)
                {
                    r = v % 60;
                    v = v / 60;
                    res = r.ToString("00") + "." + res;
                    if (v > 0)
                    {
                        r = v % 24;
                        v = v / 24;
                        res = r.ToString("00") + ":" + res;
                        if (v > 0)
                        {
                            res = v.ToString() + "d " + res;
                        }
                    }
                }
            }
            return res;
        }

        public string Lookup(string lookupName,UInt64 value)
        {
            if (lookupName == string.Empty)
                return "";
            return PD.Parent._parentTab.Engine.DataLookups.NLU(lookupName).GetValue(value) + " <= " ;
        }

        public string Lookup(string lookupName, UInt64 value, string evalString)
        {
            if (lookupName == string.Empty)
                return "";
            return PD.Parent._parentTab.Engine.DataLookups.NLU(lookupName,evalString).GetValue(value) + " <= ";
        }

        public void ToGridView(DataGridView DGV)
        {
            if (DGV.Tag != null)
                return;
            DGV.SuspendLayout();
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb((int)Math.Round(DGV.DefaultCellStyle.BackColor.R * 0.95), (int)Math.Round(DGV.DefaultCellStyle.BackColor.G * 0.95), (int)Math.Round(DGV.DefaultCellStyle.BackColor.B * 0.95));

            DGV.Tag = 1;
            // Header
            //DGV.Rows.Clear();
            DGV.ColumnCount = 3;

            DGV.Columns[ColumnOffset].HeaderText = @"Pos";
            DGV.Columns[ColumnOffset].Width = 88;

            DGV.Columns[ColumnVar].HeaderText = @"Name";
            DGV.Columns[ColumnVar].Width = 192;

            DGV.Columns[ColumnData].HeaderText = @"Data";
            var dataWidth = DGV.Width - DGV.Columns[ColumnOffset].Width - DGV.Columns[ColumnVar].Width - 20;
            if (dataWidth < 128)
                dataWidth = 128;
            DGV.Columns[ColumnData].Width = dataWidth;


            //DGV.Columns[columnSize].HeaderText = "Size";
            //DGV.Columns[columnSize].Width = 32;
            for(int thisRow = 0;thisRow < ParsedView.Count;thisRow++)
            {
                if (DGV.RowCount <= thisRow)
                    DGV.Rows.Add();
                
                ParsedViewLine pvl = ParsedView[thisRow];
                var isSearchResult = MainForm.SearchParameters.HasSearchForData() && pvl.MatchesSearch(MainForm.SearchParameters);

                DGV.Rows[thisRow].DefaultCellStyle.BackColor = isSearchResult ? Color.Yellow : SystemColors.Window ;
                DGV.Rows[thisRow].Cells[ColumnOffset].Value = pvl.Pos;
                DGV.Rows[thisRow].Cells[ColumnOffset].Style.ForeColor = pvl.FieldColor;
                DGV.Rows[thisRow].Cells[ColumnOffset].Value = pvl.Pos;
                DGV.Rows[thisRow].Cells[ColumnOffset].Style.ForeColor = pvl.FieldColor ;
                DGV.Rows[thisRow].Cells[ColumnVar].Value = pvl.Var;
                DGV.Rows[thisRow].Cells[ColumnVar].Style.ForeColor = pvl.FieldColor;
                DGV.Rows[thisRow].Cells[ColumnData].Value = pvl.Data ;
                // DGV.Rows[thisRow].Cells[3].Value = pvl.FieldIndex.ToString();
                if (SelectedFields.IndexOf(pvl.FieldIndex) >= 0)
                {
                    // this field is selected 
                    DGV.Rows[thisRow].Selected = true;
                }
                else
                {
                    DGV.Rows[thisRow].Selected = false;
                }
                DGV.Rows[thisRow].Cells[ColumnOffset].ToolTipText = pvl.ExtraInfo;
                DGV.Rows[thisRow].Cells[ColumnVar].ToolTipText = pvl.ExtraInfo;
                // DGV.Rows[thisRow].Cells[columnDATA].ToolTipText = pvl.ExtraInfo;
            }

            while (DGV.Rows.Count > ParsedView.Count)
                DGV.Rows.RemoveAt(DGV.Rows.Count - 1);

            DGV.Tag = null;
            // DGV.Refresh();
            DGV.ResumeLayout();
        }

        public void AddParseLineToView(ushort FieldIndex,string POSString, Color POSColor, string VARName, string DATAString,string EXTRAString, UInt64 DataUInt64)
        {
            ParsedViewLine pvl = new ParsedViewLine();
            pvl.Pos = POSString;
            pvl.Var = VARName;
            pvl.Data = DATAString;
            pvl.FieldIndex = FieldIndex;
            pvl.FieldColor = POSColor;
            pvl.DataAsUInt64 = DataUInt64;
            if (EXTRAString == string.Empty)
                pvl.ExtraInfo = DATAString;
            else
                pvl.ExtraInfo = EXTRAString;
            ParsedView.Add(pvl);
            AddFieldNameToList(VARName);
        }

        public void AddParseLineToView(ushort FieldIndex, string POSString, Color POSColor, string VARName, string DATAString, UInt64 DataUInt64 = 0)
        {
            AddParseLineToView(FieldIndex, POSString, POSColor, VARName, DATAString, DATAString, DataUInt64);
        }

        public void AddParseLineToView(ushort FieldIndex, string POSString, Color POSColor, string VARName, string DATAString, string EXTRAString)
        {
            AddParseLineToView(FieldIndex, POSString, POSColor, VARName, DATAString, EXTRAString, 0);
        }

        public void MarkParsed(int offset, int bytesize, ushort fieldindex)
        {
            if (bytesize <= 0)
                bytesize = 1;

            for(int i = 0; i < bytesize;i++)
            {
                var p = offset + i;
                if ((p >= 0) && (p < ParsedBytes.Count))
                    ParsedBytes[p] = fieldindex;
            }
        }

        public bool ValueInStringList(int searchValue, string searchList)
        {
            if ((searchList == null) || (searchList == string.Empty))
                return false;
            var searchStrings = searchList.Split(',').ToList();
            foreach(string s in searchStrings)
            {
                if (DataLookups.TryFieldParse(s.Trim(' '), out int n))
                    if (n == searchValue)
                        return true;
            }
            return false;
        }

        public string BitFlagsToString(string lookupname,UInt64 value,string concatString)
        {
            string res = "";
            if (concatString == "")
                concatString = " ";
            UInt64 bitmask = 0x1;
            for(UInt64 i = 0; i < 64;i++)
            {
                if ((value & bitmask) != 0)
                {
                    string item = "";
                    if (lookupname != "")
                        item = PD.Parent._parentTab.Engine.DataLookups.NLU(lookupname).GetValue(i);
                    if (item == "")
                        item = "Bit" + i.ToString();
                    if (res != "")
                        res += concatString;
                    res += item;
                }
                bitmask <<= 1;
            }
            if (res == "")
                res = "No bits set";
            return res;
        }

        public void RemoveMarkedParsedBytes(int StartPos, int FieldByteSize)
        {
            if (FieldByteSize < 1)
                FieldByteSize = 1;
            if ((StartPos < 0) || (StartPos >= ParsedBytes.Count))
                return;
            // Set markers to zero
            for (int i = StartPos; (i < ParsedBytes.Count) && (i < (StartPos + FieldByteSize)); i++)
                ParsedBytes[i] = 0x0000;
            // Remove unparsed
            for (int i = ParsedView.Count-1 ; i >= 0;i--)
            {
                if (ParsedView[i].Var.StartsWith("??_"))
                    ParsedView.RemoveAt(i);
            }
        }


        public void AddDataFieldEx(int StartPos, int FieldByteSize, ref ushort DataFieldIndex)
        {
            if (FieldByteSize < 1)
                FieldByteSize = 1;
            if ((StartPos < 0) || (StartPos >= ParsedBytes.Count))
                return;
            // DataFieldIndex++;
            if (ParsedBytes[StartPos] == 0)
            {
                DataFieldIndex++;
                for (int i = StartPos; (i < ParsedBytes.Count) && (i < (StartPos + FieldByteSize)); i++)
                {
                    ParsedBytes[i] = DataFieldIndex;
                }
            }
        }

        public UInt64 GetParsedBaseValue(string fieldName)
        {
            foreach(var line in ParsedView)
            {
                if (line.Var.ToLower() == fieldName)
                {
                    return line.DataAsUInt64;
                }
            }
            // If the fieldname is not found try to return the fieldname as a parsed uint64
            if (DataLookups.TryFieldParseUInt64(fieldName, out UInt64 res))
                return res;
            else
                return 0;
        }

        public string GetParsedValue(string fieldName)
        {
            if (fieldName.StartsWith("\"") && fieldName.EndsWith("\""))
            {
                return fieldName.Trim('\"');
            }
            foreach (var line in ParsedView)
            {
                if (line.Var.ToLower() == fieldName)
                {
                    return line.Data ;
                }
            }
            return "<"+fieldName+">";
        }

        public virtual void ParseData(string ActiveSwitchBlock)
        {
            
        }

        public virtual void ParseUnusedData(ref ushort DataFieldIndex)
        {
            var endCursor = PD.Cursor;
            var first = true;
            // List unparsed bytes
            for (int i = 0; i < PD.RawBytes.Count(); i++)
            {
                if ((first) && (ParsedBytes[i] == 0))
                {
                    first = false;
                    if (PD.OriginalPacketLevel == 4)
                        AddParseLineToView(DataFieldIndex, "0x" + i.ToString("X2"), Color.Red,
                            "Unparsed", "Unparsed L4 Data Starts here",
                            "End of pre-parsed data", 0);
                }
                if ((i <= (PD.RawBytes.Count() - 4)) && (ParsedBytes[i] == 0) && (ParsedBytes[i + 1] == 0) && (ParsedBytes[i + 2] == 0) && (ParsedBytes[i + 3] == 0))
                {
                    AddDataFieldEx(i, 4, ref DataFieldIndex);
                    AddParseLineToView(DataFieldIndex,
                        "0x" + i.ToString("X2"),
                        Color.DarkGray,
                        "??_UInt32 (@" + i.ToString() + ")",
                        "0x" + PD.GetUInt32AtPos(i).ToString("X8") + " (" + PD.GetUInt32AtPos(i).ToString() + ")", "Not parsed by script", PD.GetUInt32AtPos(i));
                    MarkParsed(i, 4, DataFieldIndex);
                    i += 3; // move forward a extra 3 bytes
                }
                else
                if (ParsedBytes[i] == 0)
                {
                    AddDataFieldEx(i, 1, ref DataFieldIndex);
                    AddParseLineToView(DataFieldIndex,
                        "0x" + i.ToString("X2"),
                        Color.DarkGray,
                        "??_Byte (@" + i.ToString() + ")",
                        "0x" + PD.GetByteAtPos(i).ToString("X2") + " (" + PD.GetByteAtPos(i).ToString() + ")", "Not parsed by script", PD.GetByteAtPos(i));
                    MarkParsed(i, 1, DataFieldIndex);
                }
            }
            PD.Cursor = endCursor; // Reset cursor to last parsed value, this is still required later in Level 4 packets
        }
    }
}
