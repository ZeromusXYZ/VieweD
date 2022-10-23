using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class FilterForm : Form
    {
        protected class FilterListEntry : IComparable
        {
            private string value;
            private string display;
            private string shortdisplay;
            public string Value { get => value; set => this.value = value; }
            public string Display { get => display; set => display = value; }
            public string ShortDisplay { get => shortdisplay; set => shortdisplay = value; }
            public PacketFilterListEntry FilterEntry { get; set; }

            public int CompareTo(object obj)
            {
                return Display.CompareTo((obj as FilterListEntry).Display);
            }
        }

        public PacketListFilter Filter;

        protected List<FilterListEntry> OutDataSource;
        protected List<FilterListEntry> InDataSource;
        private EngineBase _currentEngine;
        public EngineBase currentEngine
        {
            get { return _currentEngine; }
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
            //cbOutIDs.Items.Clear();
            if (currentEngine != null)
            {
                foreach (var key in currentEngine.DataLookups.NLU(DataLookups.LU_PacketOut).Data.Keys)
                {
                    var pfkEntry = new PacketFilterListEntry(key);
                    //var serverId = key / 0x1000000;
                    //var level = (key / 0x10000) & 0xFF;
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
                    var pName = currentEngine.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(key);

                    var d = pName.PadRight(32) + " " + server + "0x" + (key & 0xFFFF).ToString("X3");
                    //var d = server+"0x" + (key & 0xFFFF).ToString("X3") + " - " + pName;
                    OutDataSource.Add(new FilterListEntry() { Value = key.ToString("X"), Display = d, ShortDisplay = pName, FilterEntry = pfkEntry });
                    //cbOutIDs.Items.Add("0x" + key.ToString("X3") + " - " + DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(key));
                }
            }

            InDataSource.Clear();
            //cbInIDs.Items.Clear();
            if (currentEngine != null)
            {
                foreach (var key in currentEngine.DataLookups.NLU(DataLookups.LU_PacketIn).Data.Keys)
                {
                    var pfkEntry = new PacketFilterListEntry(key);
                    //var serverId = key / 0x1000000;
                    //var level = (key / 0x10000) & 0xFF;
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
                    var pName = currentEngine.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(key);
                    var d = pName.PadRight(32) + " " + server + "0x" + (key & 0xFFFF).ToString("X3");
                    //var d = server+"0x" + (key & 0xFFFF).ToString("X3") + " - " + pName;
                    InDataSource.Add(new FilterListEntry() { Value = key.ToString("X"), Display = d, ShortDisplay = pName, FilterEntry = pfkEntry });
                    //cbInIDs.Items.Add("0x" + key.ToString("X3") + " - " + DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(key));
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
                lbOut.Items.Add(n + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(n.AsMergedId()) ?? "???");
                // lbOut.Items.Add("0x" + n.Id.ToString("X3") + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(n.AsMergedId()) ?? "???");
            }

            rbInOff.Checked = (Filter.FilterInType == FilterType.Off);
            rbInHide.Checked = (Filter.FilterInType == FilterType.HidePackets);
            rbInShow.Checked = (Filter.FilterInType == FilterType.ShowPackets);
            rbInNone.Checked = (Filter.FilterInType == FilterType.AllowNone);
            lbIn.Items.Clear();
            foreach (var n in Filter.FilterInList)
            {
                lbIn.Items.Add(n + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(n.AsMergedId()) ?? "???");
                // lbIn.Items.Add("0x" + n.Id.ToString("X3") + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(n.AsMergedId()) ?? "???");
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
            char[] splitchars = new char[1] { '-' };
            var fields = s.Split(splitchars,2);
            if (fields.Length >= 1)
            {
                if (DataLookups.TryFieldParse(fields[0].Trim(' '),out long n))
                    res = n;
            }
            return res;
        }

        private void BtnOutAdd_Click(object sender, EventArgs e)
        {
            var s = cbOutIDs.Text ;
            // First check our data list
            foreach(var o in OutDataSource)
            {
                if (o.Display == s)
                {
                    lbOut.Items.Add(o.FilterEntry + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(o.FilterEntry.AsMergedId()) ?? "???");
                    // lbOut.Items.Add("0x" + o.Value + " - " + o.ShortDisplay);
                    return;
                }
            }

            // If nothing found, parse it
            var n = ValForID(s);
            if ((rbOutOff.Checked) && (lbOut.Items.Count == 0))
                rbOutShow.Checked = true;

            lbOut.Items.Add("0x" + n.ToString("X3") + " - " + currentEngine.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue((ulong)n));
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
            foreach (var i in InDataSource)
            {
                if (i.Display == s)
                {
                    lbIn.Items.Add(i.FilterEntry + " - " + currentEngine?.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(i.FilterEntry.AsMergedId()) ?? "???");
                    // lbIn.Items.Add("0x" + i.Value + " - " + i.ShortDisplay);
                    return;
                }
            }

            // If nothing found, parse it
            var n = ValForID(s);
            if ((rbInOff.Checked) && (lbIn.Items.Count == 0))
                rbInShow.Checked = true;

            lbIn.Items.Add("0x" + n.ToString("X3") + " - " + currentEngine.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue((UInt64)n));
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
                Filter.SaveToFile(saveFileDlg.FileName,currentEngine);
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
