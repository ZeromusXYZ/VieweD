using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using VieweD.Helpers;
using VieweD.Helpers.System;

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
        public ushort ThisPacketID { get; set; }
        public PacketLogTypes ThisPacketLogType { get; set; }
        public List<ushort> ParsedBytes { get; set; } = new List<ushort>();
        public List<ParsedViewLine> ParsedView { get; set; } = new List<ParsedViewLine>();
        public List<ushort> SelectedFields { get; set; } = new List<ushort>();
        public PacketData PD { get; set; }
        public List<string> SwitchBlocks { get; set; } = new List<string>();
        public string LastSwitchedBlock { get; set; }
        public string PreParsedSwitchBlock { get; set; } = "?" ;
        public List<string> RawParseData { get; set; } = new List<string>(); // not used by all engines
        public static List<string> AllFieldNames { get; set; } = new List<string>();

        protected static void AddFieldNameToList(string fieldName)
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
            for (var i = 0; i < PD.RawBytes.Count; i++)
                ParsedBytes.Add(0x00); // 0 = unparsed
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

        public static string MSToString(uint ms)
        {
            var r = ms % 1000;
            var v = ms / 1000;
            var res = r.ToString("0000")+"ms";
            if (v <= 0) 
                return res;

            r = v % 60;
            v /= 60;
            res = r.ToString("00") + "s " + res ;

            if (v <= 0) 
                return res;

            r = v % 60;
            v /= 60;
            res = r.ToString("00") + "m " + res;

            if (v <= 0) 
                return res;

            r = v % 24;
            v /= 24;
            res = r.ToString("00") + "h " + res;

            if (v > 0)
                res = v + "d " + res;

            return res;
        }

        public string FramesToString(uint frames)
        {
            var r = frames % 60;
            var v = frames / 60;
            var res = r.ToString("00") + "f";
            
            if (v <= 0) 
                return res;

            r = v % 60;
            v /= 60;
            res = r.ToString("00") + " / " + res;

            if (v <= 0) return res;
            r = v % 60;
            v /= 60;
            res = r.ToString("00") + "." + res;

            if (v <= 0) return res;
            r = v % 24;
            v /= 24;

            res = r.ToString("00") + ":" + res;
            if (v > 0)
                res = v + "d " + res;

            return res;
        }

        public string Lookup(string lookupName, ulong value)
        {
            if (lookupName == string.Empty)
                return "";
            return PD.Parent.ParentTab.Engine.DataLookups.NLU(lookupName).GetValue(value) + " <= ";
        }

        public string Lookup(string lookupName, ulong value, string evalString)
        {
            if (lookupName == string.Empty)
                return "";
            return PD.Parent.ParentTab.Engine.DataLookups.NLU(lookupName,evalString).GetValue(value) + " <= ";
        }

        public void ToGridView(DataGridView dataGridView)
        {
            if (dataGridView.Tag != null)
                return;

            // var startTime = DateTime.UtcNow;
            var oldFocus = dataGridView.Focused;
            dataGridView.SuspendLayout();
            dataGridView.Enabled = false;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb((int)Math.Round(dataGridView.DefaultCellStyle.BackColor.R * 0.95), (int)Math.Round(dataGridView.DefaultCellStyle.BackColor.G * 0.95), (int)Math.Round(dataGridView.DefaultCellStyle.BackColor.B * 0.95));

            dataGridView.Tag = 1;

            // Header
            //dataGridView.Rows.Clear();
            dataGridView.ColumnCount = 3;

            dataGridView.Columns[ColumnOffset].HeaderText = @"Pos";
            dataGridView.Columns[ColumnOffset].Width = 88;

            dataGridView.Columns[ColumnVar].HeaderText = @"Name";
            dataGridView.Columns[ColumnVar].Width = 192;

            dataGridView.Columns[ColumnData].HeaderText = @"Data";
            var dataWidth = dataGridView.Width - dataGridView.Columns[ColumnOffset].Width - dataGridView.Columns[ColumnVar].Width - 20;
            if (dataWidth < 128)
                dataWidth = 128;
            dataGridView.Columns[ColumnData].Width = dataWidth;

            //dataGridView.Columns[columnSize].HeaderText = "HeaderSize";
            //dataGridView.Columns[columnSize].Width = 32;
            for(var thisRow = 0;thisRow < ParsedView.Count;thisRow++)
            {
                if (dataGridView.RowCount <= thisRow)
                    dataGridView.Rows.Add();
                
                var pvl = ParsedView[thisRow];
                var isSearchResult = MainForm.SearchParameters.HasSearchForData() && pvl.MatchesSearch(MainForm.SearchParameters);

                dataGridView.Rows[thisRow].DefaultCellStyle.BackColor = isSearchResult ? Color.Yellow : SystemColors.Window ;
                dataGridView.Rows[thisRow].Cells[ColumnOffset].Value = pvl.Pos;
                dataGridView.Rows[thisRow].Cells[ColumnOffset].Style.ForeColor = pvl.FieldColor;
                dataGridView.Rows[thisRow].Cells[ColumnOffset].Value = pvl.Pos;
                dataGridView.Rows[thisRow].Cells[ColumnOffset].Style.ForeColor = pvl.FieldColor ;
                dataGridView.Rows[thisRow].Cells[ColumnVar].Value = pvl.Var;
                dataGridView.Rows[thisRow].Cells[ColumnVar].Style.ForeColor = pvl.FieldColor;
                dataGridView.Rows[thisRow].Cells[ColumnData].Value = pvl.Data ;
                // Check if this field is selected 
                dataGridView.Rows[thisRow].Selected = SelectedFields.IndexOf(pvl.FieldIndex) >= 0;
                dataGridView.Rows[thisRow].Cells[ColumnOffset].ToolTipText = pvl.ExtraInfo;
                dataGridView.Rows[thisRow].Cells[ColumnVar].ToolTipText = pvl.ExtraInfo;
                // dataGridView.Rows[thisRow].Cells[columnDATA].ToolTipText = pvl.ExtraInfo;
            }

            // Remove unused rows
            while (dataGridView.Rows.Count > ParsedView.Count)
                dataGridView.Rows.RemoveAt(dataGridView.Rows.Count - 1);

            dataGridView.Tag = null;

            // dataGridView.Refresh();

            dataGridView.Enabled = true;
            dataGridView.ResumeLayout();

            if (oldFocus)
                dataGridView.Focus();

            // var delta = DateTime.UtcNow - startTime;
            // MainForm.ThisMainForm.sbExtraInfo.Text = delta.ToString();
        }

        public void AddParseLineToView(ushort FieldIndex,string POSString, Color POSColor, string VARName, string DATAString,string EXTRAString, ulong DataUInt64)
        {
            var pvl = new ParsedViewLine
            {
                Pos = POSString,
                Var = VARName,
                Data = DATAString,
                FieldIndex = FieldIndex,
                FieldColor = POSColor,
                DataAsUInt64 = DataUInt64,
                ExtraInfo = EXTRAString == string.Empty ? DATAString : EXTRAString
            };
            ParsedView.Add(pvl);
            AddFieldNameToList(VARName);
        }

        public void AddParseLineToView(ushort FieldIndex, string POSString, Color POSColor, string VARName, string DATAString, ulong DataUInt64 = 0)
        {
            AddParseLineToView(FieldIndex, POSString, POSColor, VARName, DATAString, DATAString, DataUInt64);
        }

        public void AddParseLineToView(ushort FieldIndex, string POSString, Color POSColor, string VARName, string DATAString, string EXTRAString)
        {
            AddParseLineToView(FieldIndex, POSString, POSColor, VARName, DATAString, EXTRAString, 0);
        }

        public void MarkParsed(int offset, int byteSize, ushort fieldIndex)
        {
            if (byteSize <= 0)
                byteSize = 1;

            for(var i = 0; i < byteSize;i++)
            {
                var p = offset + i;
                if ((p >= 0) && (p < ParsedBytes.Count))
                    ParsedBytes[p] = fieldIndex;
            }
        }

        public bool ValueInStringList(int searchValue, string searchList)
        {
            if ((searchList == null) || (searchList == string.Empty))
                return false;

            var searchStrings = searchList.Split(',').ToList();
            foreach (var s in searchStrings)
            {
                if ((NumberHelper.TryFieldParse(s.Trim(' '), out int n)) && (n == searchValue))
                    return true;
            }

            return false;
        }

        public string BitFlagsToString(string lookupName, ulong value, string concatString)
        {
            var res = string.Empty;
            if (concatString == string.Empty)
                concatString = " ";

            ulong bitmask = 0x1;
            for (ulong i = 0; i < 64; i++)
            {
                if ((value & bitmask) != 0)
                {
                    var item = string.Empty;
                    if (lookupName != string.Empty)
                        item = PD.Parent.ParentTab.Engine.DataLookups.NLU(lookupName).GetValue(i);
                    if (item == string.Empty)
                        item = "Bit" + i;
                    if (res != string.Empty)
                        res += concatString;
                    res += item;
                }

                bitmask <<= 1;
            }

            if (res == string.Empty)
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
            for (var i = StartPos; (i < ParsedBytes.Count) && (i < (StartPos + FieldByteSize)); i++)
                ParsedBytes[i] = 0x0000;

            // Remove unparsed
            for (var i = ParsedView.Count-1 ; i >= 0;i--)
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
            if (ParsedBytes[StartPos] != 0) 
                return;

            DataFieldIndex++;
            for (var i = StartPos; (i < ParsedBytes.Count) && (i < (StartPos + FieldByteSize)); i++)
            {
                ParsedBytes[i] = DataFieldIndex;
            }
        }

        public ulong GetParsedBaseValue(string fieldName)
        {
            foreach(var line in ParsedView)
            {
                if (line.Var.ToLower() == fieldName)
                    return line.DataAsUInt64;
            }

            // If the fieldName is not found try to return the fieldName as a parsed uint64
            return NumberHelper.TryFieldParse(fieldName, out ulong res) ? res : 0;
        }

        public string GetParsedValue(string fieldName)
        {
            if (fieldName.StartsWith("\"") && fieldName.EndsWith("\""))
                return fieldName.Trim('\"');

            foreach (var line in ParsedView)
            {
                if (line.Var.ToLower() == fieldName)
                    return line.Data ;
            }

            return "<"+fieldName+">";
        }

        public virtual void ParseData(string ActiveSwitchBlock)
        {
            // Do nothing
        }

        public virtual void ParseUnusedData(ref ushort DataFieldIndex)
        {
            var endCursor = PD.Cursor;
            var first = true;

            // List unparsed bytes
            for (var i = 0; i < PD.RawBytes.Count(); i++)
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

            // Reset cursor to last parsed value, this is still required later in Level 4 compressed packets
            PD.Cursor = endCursor; 
        }
    }
}
