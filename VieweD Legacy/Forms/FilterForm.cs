using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using VieweD.Engine.Common;
using System.Drawing;
using VieweD.Helpers;
using VieweD.Helpers.System;

namespace VieweD
{
    public partial class FilterForm : Form
    {
        protected class FilterListEntry : IComparable
        {
            public string Value { get; set; }

            public string Display { get; set; }

            public string ShortDisplay { get; set; }

            public PacketFilterListEntry FilterEntry { get; set; }

            public int CompareTo(object obj)
            {
                return string.Compare(Display, (obj as FilterListEntry)?.Display, StringComparison.Ordinal);
            }
        }

        public PacketListFilter Filter;

        protected List<FilterListEntry> OutDataSource;
        protected List<FilterListEntry> InDataSource;
        private EngineBase _currentEngine;
        public EngineBase CurrentEngine
        {
            get => _currentEngine;
            set
            {
                _currentEngine = value;
                if (value != null)
                {
                    saveFileDlg.InitialDirectory = Path.Combine(Application.StartupPath, "data", value.EngineId, "filter");
                    loadFileDlg.InitialDirectory = Path.Combine(Application.StartupPath, "data", value.EngineId, "filter");
                }
            }
        }

        public FilterForm()
        {
            InitializeComponent();
            Filter = new PacketListFilter();
            OutDataSource = new List<FilterListEntry>();
            InDataSource = new List<FilterListEntry>();
            ClearFilters();
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            lbOut.Font = Properties.Settings.Default.PacketListFont;
            lbIn.Font = Properties.Settings.Default.PacketListFont;
            cbOutIDs.Font = Properties.Settings.Default.PacketListFont;
            cbInIDs.Font = Properties.Settings.Default.PacketListFont;
            OutDataSource.Clear();
            if (CurrentEngine != null)
            {
                foreach (var key in CurrentEngine.DataLookups.NLU(DataLookups.LU_PacketOut).Data.Keys)
                {
                    var pfkEntry = new PacketFilterListEntry(key);
                    var server = string.Empty;
                    switch (pfkEntry.StreamId)
                    {
                        case 1:
                            server = "Auth ";
                            break;
                        case 2:
                            server = "Game ";
                            break;
                        case 3:
                            server = "Stream ";
                            break;
                    }
                    if (pfkEntry.Level > 0)
                        server += "L" + pfkEntry.Level.ToString() + " ";
                    var pName = CurrentEngine.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(key);

                    var d = pName.PadRight(32) + " " + server + "0x" + (key & 0xFFFF).ToString("X3");
                    OutDataSource.Add(new FilterListEntry() { Value = key.ToString("X"), Display = d, ShortDisplay = pName, FilterEntry = pfkEntry });
                }
            }

            InDataSource.Clear();
            if (CurrentEngine != null)
            {
                foreach (var key in CurrentEngine.DataLookups.NLU(DataLookups.LU_PacketIn).Data.Keys)
                {
                    var pfkEntry = new PacketFilterListEntry(key);
                    var server = string.Empty;
                    switch (pfkEntry.StreamId)
                    {
                        case 1:
                            server = "Auth ";
                            break;
                        case 2:
                            server = "Game ";
                            break;
                        case 3:
                            server = "Stream ";
                            break;
                    }
                    if (pfkEntry.Level > 0)
                        server += "L" + pfkEntry.Level.ToString() + " ";
                    var pName = CurrentEngine.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(key);
                    var d = pName.PadRight(32) + " " + server + "0x" + (key & 0xFFFF).ToString("X3");
                    InDataSource.Add(new FilterListEntry() { Value = key.ToString("X"), Display = d, ShortDisplay = pName, FilterEntry = pfkEntry });
                }
            }

            OutDataSource.Sort();
            InDataSource.Sort();

            cbOutIDs.DataSource = OutDataSource;
            cbOutIDs.ValueMember = "Value";
            cbOutIDs.DisplayMember = "Display";

            cbInIDs.DataSource = InDataSource;
            cbInIDs.ValueMember = "Value";
            cbInIDs.DisplayMember = "Display";
        }

        private void ClearFilters()
        {
            rbOutOff.Checked = true;
            rbOutHide.Checked = false;
            rbOutShow.Checked = false;
            rbOutNone.Checked = false;
            lbOut.Items.Clear();

            rbInOff.Checked = true;
            rbInHide.Checked = false;
            rbInShow.Checked = false;
            rbInNone.Checked = false;
            lbIn.Items.Clear();
        }

        public void LoadLocalFromFilter()
        {
            rbOutOff.Checked = (Filter.FilterOutType == FilterType.Off);
            rbOutHide.Checked = (Filter.FilterOutType == FilterType.HidePackets);
            rbOutShow.Checked = (Filter.FilterOutType == FilterType.ShowPackets);
            rbOutNone.Checked = (Filter.FilterOutType == FilterType.AllowNone);
            lbOut.Items.Clear();
            foreach (var n in Filter.FilterOutList)
            {
                lbOut.Items.Add(n + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(n.AsMergedId()) ?? "???");
            }

            rbInOff.Checked = (Filter.FilterInType == FilterType.Off);
            rbInHide.Checked = (Filter.FilterInType == FilterType.HidePackets);
            rbInShow.Checked = (Filter.FilterInType == FilterType.ShowPackets);
            rbInNone.Checked = (Filter.FilterInType == FilterType.AllowNone);
            lbIn.Items.Clear();
            foreach (var n in Filter.FilterInList)
            {
                lbIn.Items.Add(n + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(n.AsMergedId()) ?? "???");
            }
        }

        public void SaveLocalToFilter()
        {
            if (rbOutOff.Checked)
                Filter.FilterOutType = FilterType.Off;
            if (rbOutHide.Checked)
                Filter.FilterOutType = FilterType.HidePackets;
            if (rbOutShow.Checked)
                Filter.FilterOutType = FilterType.ShowPackets;
            if (rbOutNone.Checked)
                Filter.FilterOutType = FilterType.AllowNone;
            Filter.FilterOutList.Clear();
            foreach (string line in lbOut.Items)
            {
                Filter.AddOutFilterValueToList(new PacketFilterListEntry(line));
            }

            if (rbInOff.Checked)
                Filter.FilterInType = FilterType.Off;
            if (rbInHide.Checked)
                Filter.FilterInType = FilterType.HidePackets;
            if (rbInShow.Checked)
                Filter.FilterInType = FilterType.ShowPackets;
            if (rbInNone.Checked)
                Filter.FilterInType = FilterType.AllowNone;
            Filter.FilterInList.Clear();
            foreach (string line in lbIn.Items)
            {
                Filter.AddInFilterValueToList(new PacketFilterListEntry(line));
            }
        }

        private long ValForID(string s)
        {
            if (s == string.Empty)
                return 0;

            long res = 0;
            var splitChars = new [] { '-' };
            var fields = s.Split(splitChars,2);
            if (fields.Length >= 1)
            {
                if (NumberHelper.TryFieldParse(fields[0].Trim(' '),out long n))
                    res = n;
            }

            return res;
        }

        private void BtnOutAdd_Click(object sender, EventArgs e)
        {
            var s = cbOutIDs.Text ;
            // First check our data list
            var found = false;
            foreach(var o in OutDataSource)
            {
                if (o.Display == s)
                {
                    lbOut.Items.Add(o.FilterEntry + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(o.FilterEntry.AsMergedId()) ?? "???");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (NumberHelper.TryFieldParse(s, out ulong unknownValue))
                {
                    var unknownFilter = new PacketFilterListEntry(unknownValue);
                    lbOut.Items.Add(unknownFilter + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(unknownFilter.AsMergedId()) ?? "???");
                }
            }

            if ((rbOutOff.Checked) && (lbOut.Items.Count == 0))
                rbOutShow.Checked = true;
        }

        private void BtnRemoveOut_Click(object sender, EventArgs e)
        {
            for (var i = lbOut.Items.Count - 1; i >= 0; i--)
                if (lbOut.GetSelected(i))
                    lbOut.Items.RemoveAt(i);
        }

        private void BtnInAdd_Click(object sender, EventArgs e)
        {
            var s = cbInIDs.Text;
            // First check our data list
            var found = false;
            foreach (var i in InDataSource)
            {
                if (i.Display == s)
                {
                    lbIn.Items.Add(i.FilterEntry + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(i.FilterEntry.AsMergedId()) ?? "???");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (NumberHelper.TryFieldParse(s, out ulong unknownValue))
                {
                    var unknownFilter = new PacketFilterListEntry(unknownValue);
                    lbIn.Items.Add(unknownFilter + " - " + CurrentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(unknownFilter.AsMergedId()) ?? "???");
                }
            }

            if ((rbInOff.Checked) && (lbIn.Items.Count == 0))
                rbInShow.Checked = true;
        }

        private void BtnRemoveIn_Click(object sender, EventArgs e)
        {
            for (var i = lbIn.Items.Count - 1; i >= 0; i--)
                if (lbIn.GetSelected(i))
                    lbIn.Items.RemoveAt(i);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                SaveLocalToFilter();
                Filter.SaveToFile(saveFileDlg.FileName,CurrentEngine);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (loadFileDlg.ShowDialog() == DialogResult.OK)
            {
                if (!Filter.LoadFromFile(loadFileDlg.FileName))
                    Filter.Clear();
                LoadLocalFromFilter();
            }

        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            Filter.Clear();
            LoadLocalFromFilter();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cbOutIDs_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!cbOutIDs.DroppedDown) && (e.KeyCode == Keys.Enter))
                BtnOutAdd_Click(null, null);
        }

        private void cbInIDs_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!cbInIDs.DroppedDown) && (e.KeyCode == Keys.Enter))
                BtnInAdd_Click(null, null);
        }

        private void btnHighlight_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}
