using VieweD.engine.common;
using VieweD.Forms;
using VieweD.Helpers.System;

namespace VieweD
{
    public partial class GameViewForm : Form
    {
        private DataLookupList? LastLookupList { get; set; }
        public ViewedProjectTab ParentProject { get; set; }

        public class FilterEntry
        {
            public string Display { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }

        public GameViewForm(ViewedProjectTab parent)
        {
            ParentProject = parent;
            parent.GameView = this;
            InitializeComponent();
            //warningTextBox.Visible = (parent != null);
        }

        private void GameViewForm_Load(object sender, EventArgs e)
        {
        }

        private void GameViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ParentProject.GameView = null;
        }

        private void BtnRefreshLookups_Click(object sender, EventArgs e)
        {
            lbLookupValues.DataSource = null;
            lbLookupGroups.Items.Clear();

            lbLookupGroups.Items.AddRange(ParentProject.DataLookup.LookupLists.Keys.ToArray());
            lbLookupGroups.Sorted = true;
        }

        private void LbLookupGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            this.Cursor = Cursors.WaitCursor;
            Refresh();
            var item = lbLookupGroups.SelectedItem;
            if (item == null)
            {
                LastLookupList = null;
                return;
            }
            LastLookupList = ParentProject.DataLookup.NLU((string)item);
            // lbLookupValues.Items.Clear();
            lbLookupValues.BeginUpdate();
            lbLookupValues.DataSource = null;

            var newList = new List<FilterEntry>();
            foreach (var d in LastLookupList.Data)
            {
                var newEntry = new FilterEntry();
                newEntry.Value = d.Value.Id.ToString();
                string t = string.Empty;
                if (cbHexIndex.Checked)
                    t = "0x" + d.Value.Id.ToString("X8") + " => " + d.Value.Val;
                else
                    t = d.Value.Id + " => " + d.Value.Val;
                newEntry.Display = t;

                if ((eTextFilter.Text != string.Empty) && (!t.ToLower().Contains(eTextFilter.Text.ToLower())))
                    continue;
                newList.Add(newEntry);
                // lbLookupValues.Items.Add(t);
            }
            lbLookupValues.DataSource = newList.ToArray();
            lbLookupValues.ValueMember = "Value";
            lbLookupValues.DisplayMember = "Display";
            lbLookupValues.EndUpdate();
            UseWaitCursor = false;
            this.Cursor = Cursors.Default;
        }

        private void GameViewForm_Shown(object sender, EventArgs e)
        {
            BtnRefreshLookups_Click(sender, e);
        }

        private void SendToClipBoard(string clipText)
        {
            try
            {
                // Because nothing is ever as simple as the next line >.>
                // Clipboard.SetText(s);
                // Helper will (try to) prevent errors when copying to clipboard because of threading issues
                var clipHelp = new SetClipboardHelper(DataFormats.Text, clipText);
                clipHelp.DontRetryWorkOnFailed = false;
                clipHelp.Go();
            }
            catch
            {
                // Ignore
            }
        }

        private void BtnCopyID_Click(object sender, EventArgs e)
        {
            /*
            var n = lbLookupValues.SelectedIndex;
            if ((n >= LastLookupList.data.Count) || (n < 0))
                return;
            var val = LastLookupList.data.ElementAt(n).Value.ID;
            */
            if (lbLookupValues.SelectedItem == null)
                return;
            var val = ulong.Parse((lbLookupValues.SelectedItem as FilterEntry)?.Value ?? "0");
            string s;
            if (cbHexIndex.Checked)
            {
                s = "0x" + val.ToString("X");
            }
            else
            {
                s = val.ToString();
            }
            SendToClipBoard(s);
        }

        private void BtnCopyVal_Click(object sender, EventArgs e)
        {
            if (lbLookupValues.SelectedItem == null)
                return;
            var n = lbLookupValues.SelectedIndex;
            if ((n >= LastLookupList?.Data.Count) || (n < 0))
                return;
            // var s = LastLookupList.data.ElementAt(n).Value.Val;
            var val = ulong.Parse((lbLookupValues.SelectedItem as FilterEntry)?.Value ?? "0");
            var s = LastLookupList?.GetValue(val) ?? string.Empty;
            // var s = (lbLookupValues.SelectedItem as FilterEntry).Value;
            SendToClipBoard(s);
        }
    }
}
