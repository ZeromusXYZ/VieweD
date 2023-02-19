using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class SettingsForm : Form
    {
        List<Color> localFieldColors = new List<Color>();

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveButtonsIntoColorSettings();

            Properties.Settings.Default.ExternalParseEditor = cbUseExternalEditor.Checked ;
            Properties.Settings.Default.AutoOpenVideoForm = cbAutoOpenVideoForm.Checked;
            if (rbAutoLoadVideoNever.Checked)
                Properties.Settings.Default.AutoLoadVideo = 0;
            if (rbAutoLoadVideoLocalOnly.Checked)
                Properties.Settings.Default.AutoLoadVideo = 1;
            if (rbAutoLoadVideoYoutube.Checked)
                Properties.Settings.Default.AutoLoadVideo = 2;
            if (rbListStyleText.Checked)
                Properties.Settings.Default.PacketListStyle = 0;
            if (rbListStyleSolid.Checked)
                Properties.Settings.Default.PacketListStyle = 1;
            if (rbListStyleTransparent.Checked)
                Properties.Settings.Default.PacketListStyle = 2;

            Properties.Settings.Default.PreParseData = cbPreParseData.Checked;
            Properties.Settings.Default.ShowStringHexData = cbShowHexStringData.Checked;
            Properties.Settings.Default.AskCreateNewProjectFile = cbAskNewProject.Checked;
            Properties.Settings.Default.ParserDataUpdateZipURL = eParserDataUpdateZipURL.Text;
            Properties.Settings.Default.UseGameClientData = cbUseGameClientData.Checked ;
            Properties.Settings.Default.ShowDebugInfo = cbShowDebug.Checked;

            Properties.Settings.Default.PacketListFont = btnPacketListFont.Font;
            Properties.Settings.Default.GridViewFont = btnGridViewFont.Font;
            Properties.Settings.Default.RawViewFont = btnRawViewFont.Font;

            foreach (var tab in tcSettings.TabPages)
            {
                if (tab is EngineSettingsTab engineTab)
                    engineTab.OnSettingsTabSave();
            }
            
            PluginSettingsManager.SavePluginSetting();

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettingsIntoForm(false);
        }

        private void LoadSettingsIntoForm(bool pressedDefaultButton)
        {
            // Manual loading
            btnBackIN.BackColor = Properties.Settings.Default.ColBackIN;
            btnBackOUT.BackColor = Properties.Settings.Default.ColBackOUT;
            btnBackUNK.BackColor = Properties.Settings.Default.ColBackUNK;
            btnBarIN.BackColor = Properties.Settings.Default.ColBarIN;
            btnBarOUT.BackColor = Properties.Settings.Default.ColBarOUT;
            btnBarUNK.BackColor = Properties.Settings.Default.ColBarUNK;
            btnFontIN.BackColor = Properties.Settings.Default.ColFontIN;
            btnFontOUT.BackColor = Properties.Settings.Default.ColFontOUT;
            btnFontUNK.BackColor = Properties.Settings.Default.ColFontUNK;
            btnSelectedFontIN.BackColor = Properties.Settings.Default.ColSelectedFontIN;
            btnSelectedFontOUT.BackColor = Properties.Settings.Default.ColSelectedFontOUT;
            btnSelectedFontUNK.BackColor = Properties.Settings.Default.ColSelectedFontUNK;
            btnSelectIN.BackColor = Properties.Settings.Default.ColSelectIN;
            btnSelectOUT.BackColor = Properties.Settings.Default.ColSelectOUT;
            btnSelectUNK.BackColor = Properties.Settings.Default.ColSelectUNK;
            btnSyncIN.BackColor = Properties.Settings.Default.ColSyncIN;
            btnSyncOUT.BackColor = Properties.Settings.Default.ColSyncOUT;
            btnSyncUNK.BackColor = Properties.Settings.Default.ColSyncUNK;

            btnPacketListFont.Font = Properties.Settings.Default.PacketListFont;
            btnPacketListFont.Text = btnPacketListFont.Font.Name + ", " + btnPacketListFont.Font.SizeInPoints + "pt";
            labelPacketListArrows.Font = Properties.Settings.Default.PacketListFont;

            localFieldColors.Clear();
            localFieldColors.Add(SystemColors.ControlText);
            localFieldColors.Add(Properties.Settings.Default.ColField1);
            localFieldColors.Add(Properties.Settings.Default.ColField2);
            localFieldColors.Add(Properties.Settings.Default.ColField3);
            localFieldColors.Add(Properties.Settings.Default.ColField4);
            localFieldColors.Add(Properties.Settings.Default.ColField5);
            localFieldColors.Add(Properties.Settings.Default.ColField6);
            localFieldColors.Add(Properties.Settings.Default.ColField7);
            localFieldColors.Add(Properties.Settings.Default.ColField8);
            localFieldColors.Add(Properties.Settings.Default.ColField9);
            localFieldColors.Add(Properties.Settings.Default.ColField10);
            localFieldColors.Add(Properties.Settings.Default.ColField11);
            localFieldColors.Add(Properties.Settings.Default.ColField12);
            localFieldColors.Add(Properties.Settings.Default.ColField13);
            localFieldColors.Add(Properties.Settings.Default.ColField14);
            localFieldColors.Add(Properties.Settings.Default.ColField15);

            btnGridViewFont.Font = Properties.Settings.Default.GridViewFont;
            btnGridViewFont.Text = btnGridViewFont.Font.Name + ", " + btnGridViewFont.Font.SizeInPoints + "pt";

            tbFieldColorCount.Value = Properties.Settings.Default.ColFieldCount;
            UpdateFieldColorGrid();

            btnRawViewFont.Font = Properties.Settings.Default.RawViewFont;
            btnRawViewFont.Text = btnRawViewFont.Font.Name + ", " + btnRawViewFont.Font.SizeInPoints + "pt";

            cbUseExternalEditor.Checked = Properties.Settings.Default.ExternalParseEditor;
            cbAutoOpenVideoForm.Checked = Properties.Settings.Default.AutoOpenVideoForm;
            rbAutoLoadVideoLocalOnly.Checked = (Properties.Settings.Default.AutoLoadVideo == 1);
            rbAutoLoadVideoYoutube.Checked = (Properties.Settings.Default.AutoLoadVideo == 2);
            rbAutoLoadVideoNever.Checked = (!rbAutoLoadVideoLocalOnly.Checked && !rbAutoLoadVideoYoutube.Checked);
            rbListStyleText.Checked = (Properties.Settings.Default.PacketListStyle == 0);
            rbListStyleSolid.Checked = (Properties.Settings.Default.PacketListStyle == 1);
            rbListStyleTransparent.Checked = (Properties.Settings.Default.PacketListStyle == 2);
            cbPreParseData.Checked = Properties.Settings.Default.PreParseData;
            cbShowHexStringData.Checked = Properties.Settings.Default.ShowStringHexData;
            cbAskNewProject.Checked = Properties.Settings.Default.AskCreateNewProjectFile;
            eParserDataUpdateZipURL.Text = Properties.Settings.Default.ParserDataUpdateZipURL;

            cbUseGameClientData.Checked = Properties.Settings.Default.UseGameClientData;
            cbShowDebug.Checked = Properties.Settings.Default.ShowDebugInfo;

            // Add Engine-specific Tab Pages
            if (pressedDefaultButton)
            {
                foreach (var tp in tcSettings.TabPages)
                {
                    if (tp is EngineSettingsTab engineSettingsTab)
                        engineSettingsTab.OnSettingsResetDefaults();
                }
            }
            else
            {
                foreach (var engine in Engines.AllEngines)
                {
                    var engineSettingsTab = engine.CreateSettingsTab(tcSettings);
                    engineSettingsTab.OnSettingsLoaded();
                }
            }
        }

        private void SaveButtonsIntoColorSettings()
        {

            // Manual loading
            Properties.Settings.Default.ColBackIN = btnBackIN.BackColor;
            Properties.Settings.Default.ColBackOUT = btnBackOUT.BackColor;
            Properties.Settings.Default.ColBackUNK = btnBackUNK.BackColor;
            Properties.Settings.Default.ColBarIN = btnBarIN.BackColor;
            Properties.Settings.Default.ColBarOUT = btnBarOUT.BackColor;
            Properties.Settings.Default.ColBarUNK = btnBarUNK.BackColor;
            Properties.Settings.Default.ColFontIN = btnFontIN.BackColor;
            Properties.Settings.Default.ColFontOUT = btnFontOUT.BackColor;
            Properties.Settings.Default.ColFontUNK = btnFontUNK.BackColor;
            Properties.Settings.Default.ColSelectedFontIN = btnSelectedFontIN.BackColor;
            Properties.Settings.Default.ColSelectedFontOUT = btnSelectedFontOUT.BackColor;
            Properties.Settings.Default.ColSelectedFontUNK = btnSelectedFontUNK.BackColor;
            Properties.Settings.Default.ColSelectIN = btnSelectIN.BackColor;
            Properties.Settings.Default.ColSelectOUT = btnSelectOUT.BackColor;
            Properties.Settings.Default.ColSelectUNK = btnSelectUNK.BackColor;
            Properties.Settings.Default.ColSyncIN = btnSyncIN.BackColor;
            Properties.Settings.Default.ColSyncOUT = btnSyncOUT.BackColor;
            Properties.Settings.Default.ColSyncUNK = btnSyncUNK.BackColor;

            Properties.Settings.Default.ColFieldCount = tbFieldColorCount.Value;
            Properties.Settings.Default.ColField1 = localFieldColors[1];
            Properties.Settings.Default.ColField2 = localFieldColors[2];
            Properties.Settings.Default.ColField3 = localFieldColors[3];
            Properties.Settings.Default.ColField4 = localFieldColors[4];
            Properties.Settings.Default.ColField5 = localFieldColors[5];
            Properties.Settings.Default.ColField6 = localFieldColors[6];
            Properties.Settings.Default.ColField7 = localFieldColors[7];
            Properties.Settings.Default.ColField8 = localFieldColors[8];
            Properties.Settings.Default.ColField9 = localFieldColors[9];
            Properties.Settings.Default.ColField10 = localFieldColors[10];
            Properties.Settings.Default.ColField11 = localFieldColors[11];
            Properties.Settings.Default.ColField12 = localFieldColors[12];
            Properties.Settings.Default.ColField13 = localFieldColors[13];
            Properties.Settings.Default.ColField14 = localFieldColors[14];
            Properties.Settings.Default.ColField15 = localFieldColors[15];
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            LoadSettingsIntoForm(true);
        }

        private void btnColorButton_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            colorDlg.Color = btn.BackColor;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                btn.BackColor = colorDlg.Color;
            }
        }

        private void RbAutoLoadVideoNever_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAutoLoadVideoNever.Checked)
                cbAutoOpenVideoForm.Checked = false;
        }

        private void BtnSetDarkMode_Click(object sender, EventArgs e)
        {
            // Dark Mode suggested colors

            btnBackIN.BackColor = Color.FromArgb(0, 16, 0);
            btnBackOUT.BackColor = Color.FromArgb(0, 0, 16);
            btnBackUNK.BackColor = Color.FromArgb(16, 16, 16);
            btnBarIN.BackColor = Color.FromArgb(200, 255, 200);
            btnBarOUT.BackColor = Color.FromArgb(200, 200, 255);
            btnBarUNK.BackColor = Color.FromArgb(255, 200, 200);
            btnFontIN.BackColor = Color.FromArgb(128, 255, 128);
            btnFontOUT.BackColor = Color.FromArgb(64, 128, 255);
            btnFontUNK.BackColor = Color.FromArgb(192, 64, 64);
            btnSelectedFontIN.BackColor = Color.FromArgb(16, 200, 16);
            btnSelectedFontOUT.BackColor = Color.FromArgb(64, 128, 255);
            btnSelectedFontUNK.BackColor = Color.FromArgb(200, 16, 16);
            btnSelectIN.BackColor = Color.FromArgb(0, 64, 0);
            btnSelectOUT.BackColor = Color.FromArgb(8, 16, 64);
            btnSelectUNK.BackColor = Color.FromArgb(64, 0, 0);
            btnSyncIN.BackColor = Color.FromArgb(16, 92, 16);
            btnSyncOUT.BackColor = Color.FromArgb(16, 16, 92);
            btnSyncUNK.BackColor = Color.FromArgb(92, 16, 16);
        }

        private void TbFieldColorCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateFieldColorGrid();
        }

        private void UpdateFieldColorGrid()
        {
            var n = tbFieldColorCount.Value;
            lFieldColCount.Text = n.ToString();

            if (n >= 1)
            {
                lFieldCol0.ForeColor = localFieldColors[0];
                lFieldCol0.BackColor = SystemColors.Control;
            }
            else
            {
                lFieldCol0.ForeColor = Color.Gray;
                lFieldCol0.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 2)
            {
                btnColField1.ForeColor = localFieldColors[1];
                btnColField1.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField1.ForeColor = Color.Gray;
                btnColField1.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 3)
            {
                btnColField2.ForeColor = localFieldColors[2];
                btnColField2.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField2.ForeColor = Color.Gray;
                btnColField2.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 4)
            {
                btnColField3.ForeColor = localFieldColors[3];
                btnColField3.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField3.ForeColor = Color.Gray;
                btnColField3.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 5)
            {
                btnColField4.ForeColor = localFieldColors[4];
                btnColField4.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField4.ForeColor = Color.Gray;
                btnColField4.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 6)
            {
                btnColField5.ForeColor = localFieldColors[5];
                btnColField5.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField5.ForeColor = Color.Gray;
                btnColField5.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 7)
            {
                btnColField6.ForeColor = localFieldColors[6];
                btnColField6.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField6.ForeColor = Color.Gray;
                btnColField6.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 8)
            {
                btnColField7.ForeColor = localFieldColors[7];
                btnColField7.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField7.ForeColor = Color.Gray;
                btnColField7.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 9)
            {
                btnColField8.ForeColor = localFieldColors[8];
                btnColField8.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField8.ForeColor = Color.Gray;
                btnColField8.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 10)
            {
                btnColField9.ForeColor = localFieldColors[9];
                btnColField9.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField9.ForeColor = Color.Gray;
                btnColField9.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 11)
            {
                btnColField10.ForeColor = localFieldColors[10];
                btnColField10.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField10.ForeColor = Color.Gray;
                btnColField10.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 12)
            {
                btnColField11.ForeColor = localFieldColors[11];
                btnColField11.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField11.ForeColor = Color.Gray;
                btnColField11.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 13)
            {
                btnColField12.ForeColor = localFieldColors[12];
                btnColField12.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField12.ForeColor = Color.Gray;
                btnColField12.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 14)
            {
                btnColField13.ForeColor = localFieldColors[13];
                btnColField13.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField13.ForeColor = Color.Gray;
                btnColField13.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 15)
            {
                btnColField14.ForeColor = localFieldColors[14];
                btnColField14.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField14.ForeColor = Color.Gray;
                btnColField14.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 16)
            {
                btnColField15.ForeColor = localFieldColors[15];
                btnColField15.BackColor = SystemColors.Control;
            }
            else
            {
                btnColField15.ForeColor = Color.Gray;
                btnColField15.BackColor = SystemColors.ControlDarkDark;
            }

            foreach (var control in layoutGridColors.Controls)
            {
                if (control is Label l)
                    l.Font = btnGridViewFont.Font;
                else
                    if (control is Button b)
                    b.Font = btnGridViewFont.Font;
            }

        }

        private void BtnColField_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            int t = int.Parse((string)btn.Tag);
            colorDlg.Color = localFieldColors[t];
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                localFieldColors[t] = colorDlg.Color;
                UpdateFieldColorGrid();
            }
        }

        private void btnDefaultUpdateURL_Click(object sender, EventArgs e)
        {
            eParserDataUpdateZipURL.Text = ""; // None available atm
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
        }

        private void btnPacketListFont_Click(object sender, EventArgs e)
        {
            fontDlg.Font = Properties.Settings.Default.PacketListFont;
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                btnPacketListFont.Font = fontDlg.Font;
                btnPacketListFont.Text = btnPacketListFont.Font.Name + ", " + btnPacketListFont.Font.SizeInPoints + "pt" ;
                labelPacketListArrows.Font = fontDlg.Font;
            }
        }

        private void btnGridViewFont_Click(object sender, EventArgs e)
        {
            fontDlg.Font = Properties.Settings.Default.GridViewFont;
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                btnGridViewFont.Font = fontDlg.Font;
                btnGridViewFont.Text = btnGridViewFont.Font.Name + ", " + btnGridViewFont.Font.SizeInPoints + "pt";
            }

            UpdateFieldColorGrid();
        }

        private void btnRawViewFont_Click(object sender, EventArgs e)
        {
            fontDlg.Font = Properties.Settings.Default.RawViewFont;
            if (fontDlg.ShowDialog() == DialogResult.OK)
            {
                btnRawViewFont.Font = fontDlg.Font;
                btnRawViewFont.Text = btnRawViewFont.Font.Name + ", " + btnRawViewFont.Font.SizeInPoints + "pt";
            }

        }
    }
}
