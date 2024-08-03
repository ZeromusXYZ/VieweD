using System;
using System.Drawing;
using System.Windows.Forms;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.Forms
{
    public partial class SearchForm : Form
    {
        public SearchParameters SearchParameters { get; set; }

        public ViewedProjectTab? ParentProject { get; set; }

        private bool IsValidating { get; set; }

        public SearchForm()
        {
            InitializeComponent();
            SearchParameters = new SearchParameters();
            SearchParameters.ClearValidSearchFlags();
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            // temporary disable validate
            IsValidating = true;
            rbAny.Checked = SearchParameters is { SearchIncoming: true, SearchOutgoing: true } or { SearchIncoming: false, SearchOutgoing: false };
            rbIncoming.Checked = SearchParameters is { SearchIncoming: true, SearchOutgoing: false };
            rbOutgoing.Checked = SearchParameters is { SearchIncoming: false, SearchOutgoing: true };

            ePacketID.Text = SearchParameters.SearchByPacketId ? SearchParameters.SearchPacketId.ToHex(1) : "";

            if (ParentProject?.InputParser?.Rules?.UsesCompressionLevels ?? false)
            {
                ePacketLevel.Enabled = true;
                ePacketLevel.Text = SearchParameters.SearchByPacketLevel ? SearchParameters.SearchPacketLevel.ToHex(1) : "";
            }
            else
            {
                ePacketLevel.Enabled = false;
                ePacketLevel.Text = "";
            }

            lPacketLevel.Enabled = ePacketLevel.Enabled;

            if (ParentProject?.InputParser?.AllowSyncSearch ?? false)
            {
                eSync.Enabled = true;
                eSync.Text = SearchParameters.SearchBySync ? SearchParameters.SearchSync.ToHex(1) : "";
            }
            else
            {
                eSync.Enabled = false;
                eSync.Text = "";
            }

            lPacketSync.Enabled = eSync.Enabled;

            if (SearchParameters.SearchByByte)
            {
                eValue.Text = SearchParameters.SearchByte.ToHex(1);
                rbByte.Checked = true;
            }
            else
            if (SearchParameters.SearchByUInt16)
            {
                eValue.Text = SearchParameters.SearchUInt16.ToHex(1);
                rbUInt16.Checked = true;
            }
            else
            if (SearchParameters.SearchByUInt24)
            {
                eValue.Text = SearchParameters.SearchUInt24.ToHex(1);
                rbUInt24.Checked = true;
            }
            else
            if (SearchParameters.SearchByUInt32)
            {
                eValue.Text = SearchParameters.SearchUInt32.ToHex(1);
                rbUInt32.Checked = true;
            }
            else
            {
                eValue.Text = "";
            }

            cbFieldNames.Items.Clear();
            if (ParentProject != null)
                foreach (var fieldName in ParentProject.AllFieldNames)
                    cbFieldNames.Items.Add(fieldName);

            cbFieldNames.Sorted = true;

            if (SearchParameters.SearchByParsedData)
            {
                cbFieldNames.Text = SearchParameters.SearchParsedFieldName;
                eFieldValue.Text = SearchParameters.SearchParsedFieldValue;
            }

            IsValidating = false;
            ValidateFields();
            if (!string.IsNullOrWhiteSpace(eFieldValue.Text))
            {
                eFieldValue.Focus();
                eFieldValue.SelectAll();
            }
            else if (!string.IsNullOrWhiteSpace(eValue.Text))
            {
                eValue.Focus();
                eValue.SelectAll();
            }
        }

        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnAsNewTab_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void SearchFieldsChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }

        private void ValidateFields()
        {
            var isValid = true;
            var hasData = false;
            if (IsValidating)
                return;
            IsValidating = true;

            // Packet directions
            SearchParameters.SearchIncoming = (rbAny.Checked || rbIncoming.Checked);
            SearchParameters.SearchOutgoing = (rbAny.Checked || rbOutgoing.Checked);

            // Make sure no search options are on before validating
            SearchParameters.SearchByPacketId = false;
            SearchParameters.SearchByPacketLevel = false;
            SearchParameters.SearchBySync = false;
            SearchParameters.SearchByByte = false;
            SearchParameters.SearchByUInt16 = false;
            SearchParameters.SearchByUInt24 = false;
            SearchParameters.SearchByUInt32 = false;
            SearchParameters.SearchByParsedData = false;
            SearchParameters.SearchParsedFieldName = string.Empty;
            SearchParameters.SearchParsedFieldValue = string.Empty;

            var minId = ParentProject?.InputParser?.PacketIdMinimum ?? 1;
            var maxId = ParentProject?.InputParser?.PacketIdMaximum ?? 0xFFF;

            // PacketID
            if (NumberHelper.TryFieldParse(ePacketID.Text, out int nPacketId))
            {
                if ((nPacketId >= minId) && (nPacketId <= maxId))
                {
                    hasData = true;
                    SearchParameters.SearchByPacketId = true;
                    SearchParameters.SearchPacketId = (ushort)nPacketId;
                    ePacketID.ForeColor = Color.Blue;
                }
                else
                {
                    ePacketID.ForeColor = Color.Red;
                }
            }
            else
            {
                ePacketID.ForeColor = Color.DarkGray;
            }

            // PacketLevel if allowed
            if ((ParentProject?.InputParser?.PacketCompressionLevelMaximum > 0) && (NumberHelper.TryFieldParse(ePacketLevel.Text, out int nPacketLevel)))
            {
                if ((nPacketLevel >= 0) && (nPacketLevel <= ParentProject.InputParser.PacketCompressionLevelMaximum))
                {
                    hasData = true;
                    SearchParameters.SearchByPacketLevel = true;
                    SearchParameters.SearchPacketLevel = (byte)nPacketLevel;
                    ePacketLevel.ForeColor = Color.Blue;
                }
                else
                {
                    ePacketLevel.ForeColor = Color.Red;
                }
            }
            else
            {
                ePacketLevel.ForeColor = Color.DarkGray;
            }

            // Sync
            if (NumberHelper.TryFieldParse(eSync.Text, out long nSync))
            {
                if (nSync is > 0 and < 0xFFFF)
                {
                    hasData = true;
                    SearchParameters.SearchBySync = true;
                    SearchParameters.SearchSync = (ushort)nSync;
                    eSync.ForeColor = Color.Blue;
                }
                else
                    eSync.ForeColor = Color.Red;
            }
            else
                eSync.ForeColor = Color.DarkGray;

            // Value
            if (NumberHelper.TryFieldParse(eValue.Text, out long nValue))
            {
                // Check the correct type
                if ((nValue > 0xFFFFFF) && (rbByte.Checked || rbUInt16.Checked || rbUInt24.Checked))
                    rbUInt32.Checked = true;
                else
                if ((nValue > 0xFFFF) && (rbByte.Checked || rbUInt16.Checked))
                    rbUInt24.Checked = true;
                else
                if ((nValue > 0xFF) && (rbByte.Checked))
                    rbUInt16.Checked = true;

                if ((nValue is >= 0 and <= 0xFF) && rbByte.Checked)
                {
                    hasData = true;
                    SearchParameters.SearchByByte = true;
                    SearchParameters.SearchByUInt16 = false;
                    SearchParameters.SearchByUInt24 = false;
                    SearchParameters.SearchByUInt32 = false;
                    SearchParameters.SearchByte = (byte)nValue;
                    eValue.ForeColor = Color.Navy;
                }
                else
                if ((nValue is >= 0 and <= 0xFFFF) && rbUInt16.Checked)
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = true;
                    SearchParameters.SearchByUInt24 = false;
                    SearchParameters.SearchByUInt32 = false;
                    SearchParameters.SearchUInt16 = (ushort)nValue;
                    eValue.ForeColor = Color.RoyalBlue;
                }
                else
                if ((nValue is >= 0 and <= 0xFFFFFF) && rbUInt24.Checked)
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = false;
                    SearchParameters.SearchByUInt24 = true;
                    SearchParameters.SearchByUInt32 = false;
                    SearchParameters.SearchUInt24 = (uint)nValue;
                    eValue.ForeColor = Color.Blue;
                }
                else
                if ((nValue is >= 0 and <= 0xFFFFFFFF) && rbUInt32.Checked)
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = false;
                    SearchParameters.SearchByUInt24 = false;
                    SearchParameters.SearchByUInt32 = true;
                    SearchParameters.SearchUInt32 = (uint)nValue;
                    eValue.ForeColor = Color.Blue;
                }
                else
                {
                    rbByte.Checked = true;
                    eValue.ForeColor = Color.Red;
                }

            }
            else
            {
                rbByte.Checked = true;
                eValue.ForeColor = Color.DarkGray;
            }

            if (eFieldValue.Text != string.Empty)
            {
                hasData = true;
                SearchParameters.SearchByParsedData = true;
                SearchParameters.SearchParsedFieldName = cbFieldNames.Text.ToLower();
                SearchParameters.SearchParsedFieldValue = eFieldValue.Text.ToLower();
            }

            if ((!isValid) || (!hasData))
                SearchParameters.ClearValidSearchFlags();

            btnFindNext.Enabled = isValid && hasData;
            btnAsNewTab.Enabled = isValid && hasData;
            IsValidating = false;
        }

        private void SearchForm_Shown(object sender, EventArgs e)
        {
            ValidateFields();
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            SearchParameters.ClearValidSearchFlags();
            DialogResult = DialogResult.OK;
        }
    }

}
