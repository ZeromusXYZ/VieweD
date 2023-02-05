using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VieweD.Helpers.System;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class GameViewForm : Form
    {
        public static GameViewForm GV = null;
        private DataLookupList LastLookupList { get; set; }
        public PacketTabPage CurrentTab { get; set; }

        public class FilterEntry
        {
            public string Display { get; set; }
            public string Value { get; set; }
        }

        public GameViewForm(PacketTabPage parent)
        {
            CurrentTab = parent;
            InitializeComponent();
            GameViewForm.GV = this;
            warningTextBox.Visible = (parent != null);
        }

        private void GameViewForm_Load(object sender, EventArgs e)
        {
        }

        private void GameViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            GV = null;
        }

        private void BtnRefreshLookups_Click(object sender, EventArgs e)
        {
            // lbLookupValues.Items.Clear();
            lbLookupValues.DataSource = null;
            lbLookupGroups.Items.Clear();

            // Try to get current tab if none was open
            if (CurrentTab == null)
                CurrentTab = MainForm.ThisMainForm.GetCurrentPacketTabPage();

            if (CurrentTab != null)
            {
                lbLookupGroups.Items.AddRange(CurrentTab.Engine.DataLookups.LookupLists.Keys.ToArray());
                lbLookupGroups.Sorted = true;
            }
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
            LastLookupList = CurrentTab.Engine.DataLookups.NLU((string)item);
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
                    t = d.Value.Id.ToString() + " => " + d.Value.Val;
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
            BtnRefreshLookups_Click(null, null);
        }

        private void SendToClipBoard(string cliptext)
        {
            try
            {
                // Because nothing is ever as simple as the next line >.>
                // Clipboard.SetText(s);
                // Helper will (try to) prevent errors when copying to clipboard because of threading issues
                var cliphelp = new SetClipboardHelper(DataFormats.Text, cliptext);
                cliphelp.DontRetryWorkOnFailed = false;
                cliphelp.Go();
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
            var val = ulong.Parse((lbLookupValues.SelectedItem as FilterEntry).Value);
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
            if ((n >= LastLookupList.Data.Count) || (n < 0))
                return;
            // var s = LastLookupList.data.ElementAt(n).Value.Val;
            var val = ulong.Parse((lbLookupValues.SelectedItem as FilterEntry).Value);
            var s = LastLookupList.GetValue(val);
            // var s = (lbLookupValues.SelectedItem as FilterEntry).Value;
            SendToClipBoard(s);
        }
    }
}
