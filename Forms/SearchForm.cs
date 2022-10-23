using System;
using System.Drawing;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class SearchForm : Form
    {
        public SearchParameters SearchParameters { get; set; }

        public EngineBase Engine { get; set; }

        private bool IsValidating { get; set; } = false;

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
            rbAny.Checked = ((SearchParameters.SearchIncoming && SearchParameters.SearchOutgoing) || (!SearchParameters.SearchIncoming && !SearchParameters.SearchOutgoing));
            rbIncoming.Checked = (SearchParameters.SearchIncoming && !SearchParameters.SearchOutgoing);
            rbOutgoing.Checked = (!SearchParameters.SearchIncoming && SearchParameters.SearchOutgoing);

            if (SearchParameters.SearchByPacketId)
                ePacketID.Text = "0x"+SearchParameters.SearchPacketId.ToString("X");
            else
                ePacketID.Text = "";

            if ((Engine == null) || (Engine?.AllowedPacketLevelSearch == true))
            {
                ePacketLevel.Enabled = true;
                if (SearchParameters.SearchByPacketLevel)
                    ePacketLevel.Text = "0x" + SearchParameters.SearchPacketLevel.ToString("X");
                else
                    ePacketLevel.Text = "";
            }
            else
            {
                ePacketLevel.Enabled = false;
                ePacketLevel.Text = "";
            }

            lPacketLevel.Enabled = ePacketLevel.Enabled;

            if ((Engine == null) || (Engine?.AllowedPacketSyncSearch == true))
            {
                eSync.Enabled = true;
                if (SearchParameters.SearchBySync)
                    eSync.Text = "0x" + SearchParameters.SearchSync.ToString("X");
                else
                    eSync.Text = "";
            }
            else
            {
                eSync.Enabled = false;
                eSync.Text = "";
            }

            lPacketSync.Enabled = eSync.Enabled;

            if (SearchParameters.SearchByByte)
            {
                eValue.Text = "0x" + SearchParameters.SearchByte.ToString("X");
                rbByte.Checked = true;
            }
            else
            if (SearchParameters.SearchByUInt16)
            {
                eValue.Text = "0x" + SearchParameters.SearchUInt16.ToString("X");
                rbUInt16.Checked = true;
            }
            else
            if (SearchParameters.SearchByUInt32)
            {
                eValue.Text = "0x" + SearchParameters.SearchUInt32.ToString("X");
                rbUInt32.Checked = true;
            }
            else
            {
                eValue.Text = "";
            }

            cbFieldNames.Items.Clear();
            cbFieldNames.Items.AddRange(PacketParser.AllFieldNames.ToArray());
            cbFieldNames.Sorted = true;

            if (SearchParameters.SearchByParsedData)
            {
                cbFieldNames.Text = SearchParameters.SearchParsedFieldName;
                eFieldValue.Text = SearchParameters.SearchParsedFieldValue;
            }

            IsValidating = false;
            ValidateFields();
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
            bool isValid = true;
            bool hasData = false;
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

            UInt16 minID = 1;
            UInt16 maxID = 0x1FF;
            if (SearchParameters.FileFormat == "PDEC")
            {
                minID = 0x0000;
                maxID = 0xFFFF;
            }

            // PacketID
            if (DataLookups.TryFieldParse(ePacketID.Text, out int nPacketID))
            {
                if ((nPacketID >= minID) && (nPacketID <= maxID))
                {
                    hasData = true;
                    SearchParameters.SearchByPacketId = true;
                    SearchParameters.SearchPacketId = (UInt16)nPacketID;
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

            // PacketLevel (AA only)
            if ((SearchParameters.FileFormat == "PDEC") && (DataLookups.TryFieldParse(ePacketLevel.Text, out int nPacketLevel)))
            {
                if ((nPacketLevel >= 0) && (nPacketLevel <= 5))
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
            if (DataLookups.TryFieldParse(eSync.Text, out int nSync))
            {
                if ((nSync > 0) && (nSync < 0xFFFF))
                {
                    hasData = true;
                    SearchParameters.SearchBySync = true;
                    SearchParameters.SearchSync = (UInt16)nSync;
                    eSync.ForeColor = Color.Blue;
                }
                else
                    eSync.ForeColor = Color.Red;
            }
            else
                eSync.ForeColor = Color.DarkGray;

            // Value
            if (DataLookups.TryFieldParseUInt64(eValue.Text, out UInt64 nValue))
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

                if ((nValue >= 0) && (nValue <= 0xFF) && (rbByte.Checked))
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
                if ((nValue >= 0) && (nValue <= 0xFFFF) && (rbUInt16.Checked))
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = true;
                    SearchParameters.SearchByUInt24 = false;
                    SearchParameters.SearchByUInt32 = false;
                    SearchParameters.SearchUInt16 = (UInt16)nValue;
                    eValue.ForeColor = Color.RoyalBlue;
                }
                else
                if ((nValue >= 0) && (nValue <= 0xFFFFFF) && (rbUInt24.Checked))
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = false;
                    SearchParameters.SearchByUInt24 = true;
                    SearchParameters.SearchByUInt32 = false;
                    SearchParameters.SearchUInt24 = (UInt32)nValue;
                    eValue.ForeColor = Color.Blue;
                }
                else
                if ((nValue >= 0) && (nValue <= 0xFFFFFFFF) && (rbUInt32.Checked))
                {
                    hasData = true;
                    SearchParameters.SearchByByte = false;
                    SearchParameters.SearchByUInt16 = false;
                    SearchParameters.SearchByUInt24 = false;
                    SearchParameters.SearchByUInt32 = true;
                    SearchParameters.SearchUInt32 = (UInt32)nValue;
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
    }

}
