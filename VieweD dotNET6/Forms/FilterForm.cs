using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.Forms
{
    public partial class FilterForm : Form
    {
        protected class FilterListEntry : IComparable
        {
            public FilterListEntry(string value, string display, string shortDisplay, ulong key)
            {
                Value = value;
                Display = display;
                ShortDisplay = shortDisplay;
                FilterEntry = new PacketFilterListEntry(key);
            }

            public string Value { get; set; }

            public string Display { get; set; }

            public string ShortDisplay { get; set; }

            public PacketFilterListEntry FilterEntry { get; set; }

            public int CompareTo(object? obj)
            {
                return string.Compare(Display, (obj as FilterListEntry)?.Display, StringComparison.Ordinal);
            }
        }

        public PacketListFilter Filter;
        public ViewedProjectTab? ParentProject { get; set; }

        protected List<FilterListEntry> OutDataSource;
        protected List<FilterListEntry> InDataSource;

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
            LbOut.Font = Properties.Settings.Default.PacketListFont;
            LbIn.Font = Properties.Settings.Default.PacketListFont;
            cbOutIDs.Font = Properties.Settings.Default.PacketListFont;
            cbInIDs.Font = Properties.Settings.Default.PacketListFont;
            OutDataSource.Clear();
            if (ParentProject == null)
                return;

            foreach (var key in ParentProject.DataLookup.NLU(DataLookups.LuPacketOut).Data.Keys)
            {
                var pfkEntry = new PacketFilterListEntry(key);

                var server = ParentProject.InputParser?.Rules?.UsesMultipleStreams ?? false
                    ? ParentProject.GetStreamIdName(pfkEntry.StreamId) : string.Empty;
                if (!string.IsNullOrWhiteSpace(server))
                    server += " ";

                if (pfkEntry.CompressionLevel > 0)
                    server += "L" + pfkEntry.CompressionLevel + " ";
                var pName = ParentProject.DataLookup.NLU(DataLookups.LuPacketOut).GetValue(pfkEntry.FilterKey);

                var d = pName.PadRight(32) + " " + server + pfkEntry.PacketId.ToHex(3);
                OutDataSource.Add(new FilterListEntry(key.ToString("X"), d, pName, key));
            }


            InDataSource.Clear();
            foreach (var key in ParentProject.DataLookup.NLU(DataLookups.LuPacketIn).Data.Keys)
            {
                var pfkEntry = new PacketFilterListEntry(key);

                var server = ParentProject.InputParser?.Rules?.UsesMultipleStreams ?? false
                    ? ParentProject.GetStreamIdName(pfkEntry.StreamId)
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(server))
                    server += " ";

                if (pfkEntry.CompressionLevel > 0)
                    server += "L" + pfkEntry.CompressionLevel.ToString() + " ";

                var pName = ParentProject.DataLookup.NLU(DataLookups.LuPacketIn).GetValue(pfkEntry.FilterKey);
                var d = pName.PadRight(32) + " " + server + pfkEntry.PacketId.ToHex(3);
                InDataSource.Add(new FilterListEntry(key.ToString("X"), d, pName, key));
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
            LbOut.Items.Clear();

            rbInOff.Checked = true;
            rbInHide.Checked = false;
            rbInShow.Checked = false;
            rbInNone.Checked = false;
            LbIn.Items.Clear();
        }

        public void LoadLocalFromFilter()
        {
            rbOutOff.Checked = (Filter.FilterOutType == FilterType.Off);
            rbOutHide.Checked = (Filter.FilterOutType == FilterType.HidePackets);
            rbOutShow.Checked = (Filter.FilterOutType == FilterType.ShowPackets);
            rbOutNone.Checked = (Filter.FilterOutType == FilterType.AllowNone);
            LbOut.Items.Clear();
            foreach (var n in Filter.FilterOutList)
            {
                LbOut.Items.Add(n + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketOut).GetValue(n.FilterKey));
            }

            rbInOff.Checked = (Filter.FilterInType == FilterType.Off);
            rbInHide.Checked = (Filter.FilterInType == FilterType.HidePackets);
            rbInShow.Checked = (Filter.FilterInType == FilterType.ShowPackets);
            rbInNone.Checked = (Filter.FilterInType == FilterType.AllowNone);
            LbIn.Items.Clear();
            foreach (var n in Filter.FilterInList)
            {
                LbIn.Items.Add(n + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketIn).GetValue(n.FilterKey));
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
            foreach (string line in LbOut.Items)
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
            foreach (string line in LbIn.Items)
            {
                Filter.AddInFilterValueToList(new PacketFilterListEntry(line));
            }
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
                    LbOut.Items.Add(o.FilterEntry + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketOut).GetValue(o.FilterEntry.FilterKey));
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (NumberHelper.TryFieldParse(s, out ulong unknownValue))
                {
                    var unknownFilter = new PacketFilterListEntry(unknownValue);
                    LbOut.Items.Add(unknownFilter + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketOut).GetValue(unknownFilter.FilterKey));
                }
            }

            if ((rbOutOff.Checked) && (LbOut.Items.Count == 0))
                rbOutShow.Checked = true;
        }

        private void BtnRemoveOut_Click(object sender, EventArgs e)
        {
            for (var i = LbOut.Items.Count - 1; i >= 0; i--)
                if (LbOut.GetSelected(i))
                    LbOut.Items.RemoveAt(i);
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
                    LbIn.Items.Add(i.FilterEntry + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketIn).GetValue(i.FilterEntry.FilterKey));
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                if (NumberHelper.TryFieldParse(s, out ulong unknownValue))
                {
                    var unknownFilter = new PacketFilterListEntry(unknownValue);
                    LbIn.Items.Add(unknownFilter + " - " + ParentProject?.DataLookup.NLU(DataLookups.LuPacketIn).GetValue(unknownFilter.FilterKey));
                }
            }

            if ((rbInOff.Checked) && (LbIn.Items.Count == 0))
                rbInShow.Checked = true;
        }

        private void BtnRemoveIn_Click(object sender, EventArgs e)
        {
            for (var i = LbIn.Items.Count - 1; i >= 0; i--)
                if (LbIn.GetSelected(i))
                    LbIn.Items.RemoveAt(i);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                SaveLocalToFilter();
                if (ParentProject != null)
                    Filter.SaveToFile(saveFileDlg.FileName, ParentProject);
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

        private void CbOutIDs_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!cbOutIDs.DroppedDown) && (e.KeyCode == Keys.Enter))
                BtnOutAdd_Click(sender, e);
        }

        private void CbInIDs_KeyDown(object sender, KeyEventArgs e)
        {
            if ((!cbInIDs.DroppedDown) && (e.KeyCode == Keys.Enter))
                BtnInAdd_Click(sender, e);
        }

        private void BtnHighlight_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }
    }
}
