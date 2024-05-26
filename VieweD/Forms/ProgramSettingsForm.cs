using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class ProgramSettingsForm : Form
    {
        private List<Color> LocalFieldColors { get; set; } = new();

        public ProgramSettingsForm()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            SaveButtonsIntoColorSettings();

            if (RbAutoLoadVideoNever.Checked)
                Settings.Default.AutoLoadVideo = 0;
            if (RbAutoLoadVideoLocalOnly.Checked)
                Settings.Default.AutoLoadVideo = 1;
            // if (RbAutoLoadVideoYoutube.Checked)
            //     Settings.Default.AutoLoadVideo = 2;
            if (RbListStyleText.Checked)
                Settings.Default.PacketListStyle = 0;
            if (RbListStyleSolid.Checked)
                Settings.Default.PacketListStyle = 1;
            if (RbListStyleTransparent.Checked)
                Settings.Default.PacketListStyle = 2;

            Settings.Default.ShowStringHexData = CbShowHexStringData.Checked;
            Settings.Default.SkipUnparsed = CbSkipUnparsed.Checked;
            // Settings.Default.AskCreateNewProjectFile = CbAskNewProject.Checked;

            Settings.Default.PacketListFont = BtnPacketListFont.Font;
            Settings.Default.FieldViewFont = BtnGridViewFont.Font;
            Settings.Default.RawViewFont = BtnRawViewFont.Font;

            Settings.Default.CreditsName = DefaultCreditsTextBox.Text;
            Settings.Default.DefaultImportFolder = LabelDefaultImportFolder.Text == @"<none>" ? "" : LabelDefaultImportFolder.Text;

            /*
            foreach (var tab in TcSettings.TabPages)
            {
                if (tab is EngineSettingsTab engineTab)
                    engineTab.OnSettingsTabSave();
            }
            */

            PluginSettingsManager.SavePluginSetting();

            DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
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
            BtnBackIN.BackColor = Settings.Default.ColBackIN;
            BtnBackOUT.BackColor = Settings.Default.ColBackOUT;
            BtnBackUNK.BackColor = Settings.Default.ColBackUNK;
            BtnBarIN.BackColor = Settings.Default.ColBarIN;
            BtnBarOUT.BackColor = Settings.Default.ColBarOUT;
            BtnBarUNK.BackColor = Settings.Default.ColBarUNK;
            BtnFontIN.BackColor = Settings.Default.ColFontIN;
            BtnFontOUT.BackColor = Settings.Default.ColFontOUT;
            BtnFontUNK.BackColor = Settings.Default.ColFontUNK;
            BtnSelectedFontIN.BackColor = Settings.Default.ColSelectedFontIN;
            BtnSelectedFontOUT.BackColor = Settings.Default.ColSelectedFontOUT;
            BtnSelectedFontUNK.BackColor = Settings.Default.ColSelectedFontUNK;
            BtnSelectIN.BackColor = Settings.Default.ColSelectIN;
            BtnSelectOUT.BackColor = Settings.Default.ColSelectOUT;
            BtnSelectUNK.BackColor = Settings.Default.ColSelectUNK;
            BtnSyncIN.BackColor = Settings.Default.ColSyncIN;
            BtnSyncOUT.BackColor = Settings.Default.ColSyncOUT;
            BtnSyncUNK.BackColor = Settings.Default.ColSyncUNK;

            BtnPacketListFont.Font = Settings.Default.PacketListFont;
            BtnPacketListFont.Text = BtnPacketListFont.Font.Name + @", " + BtnPacketListFont.Font.SizeInPoints + @"pt";
            LabelPacketListArrows.Font = Settings.Default.PacketListFont;

            LocalFieldColors.Clear();
            LocalFieldColors.Add(SystemColors.ControlText);
            LocalFieldColors.Add(Settings.Default.ColField1);
            LocalFieldColors.Add(Settings.Default.ColField2);
            LocalFieldColors.Add(Settings.Default.ColField3);
            LocalFieldColors.Add(Settings.Default.ColField4);
            LocalFieldColors.Add(Settings.Default.ColField5);
            LocalFieldColors.Add(Settings.Default.ColField6);
            LocalFieldColors.Add(Settings.Default.ColField7);
            LocalFieldColors.Add(Settings.Default.ColField8);
            LocalFieldColors.Add(Settings.Default.ColField9);
            LocalFieldColors.Add(Settings.Default.ColField10);
            LocalFieldColors.Add(Settings.Default.ColField11);
            LocalFieldColors.Add(Settings.Default.ColField12);
            LocalFieldColors.Add(Settings.Default.ColField13);
            LocalFieldColors.Add(Settings.Default.ColField14);
            LocalFieldColors.Add(Settings.Default.ColField15);

            BtnGridViewFont.Font = Settings.Default.FieldViewFont;
            BtnGridViewFont.Text = BtnGridViewFont.Font.Name + @", " + BtnGridViewFont.Font.SizeInPoints + @"pt";

            TbFieldColorCount.Value = Settings.Default.ColFieldCount;
            UpdateFieldColorGrid();

            BtnRawViewFont.Font = Settings.Default.RawViewFont;
            BtnRawViewFont.Text = BtnRawViewFont.Font.Name + @", " + BtnRawViewFont.Font.SizeInPoints + @"pt";

            RbAutoLoadVideoLocalOnly.Checked = (Settings.Default.AutoLoadVideo == 1);
            // RbAutoLoadVideoYoutube.Checked = (Settings.Default.AutoLoadVideo == 2);
            RbAutoLoadVideoNever.Checked = (!RbAutoLoadVideoLocalOnly.Checked); //&& !RbAutoLoadVideoYoutube.Checked);
            RbListStyleText.Checked = (Settings.Default.PacketListStyle == 0);
            RbListStyleSolid.Checked = (Settings.Default.PacketListStyle == 1);
            RbListStyleTransparent.Checked = (Settings.Default.PacketListStyle == 2);
            CbShowHexStringData.Checked = Settings.Default.ShowStringHexData;
            CbSkipUnparsed.Checked = Settings.Default.SkipUnparsed;
            // CbAskNewProject.Checked = Settings.Default.AskCreateNewProjectFile;

            DefaultCreditsTextBox.Text = Settings.Default.CreditsName;
            LabelDefaultImportFolder.Text = Settings.Default.DefaultImportFolder == ""
                ? @"<none>"
                : Settings.Default.DefaultImportFolder;

            // Add Engine-specific Tab Pages
            if (pressedDefaultButton)
            {
                /*
                foreach (var tp in TcSettings.TabPages)
                {
                    if (tp is EngineSettingsTab engineSettingsTab)
                        engineSettingsTab.OnSettingsResetDefaults();
                }
                */
            }
            else
            {
                /*
                foreach (var engine in Engines.AllEngines)
                {
                    var engineSettingsTab = engine.CreateSettingsTab(TcSettings);
                    engineSettingsTab.OnSettingsLoaded();
                }
                */
            }
        }

        private void SaveButtonsIntoColorSettings()
        {

            // Manual loading
            Settings.Default.ColBackIN = BtnBackIN.BackColor;
            Settings.Default.ColBackOUT = BtnBackOUT.BackColor;
            Settings.Default.ColBackUNK = BtnBackUNK.BackColor;
            Settings.Default.ColBarIN = BtnBarIN.BackColor;
            Settings.Default.ColBarOUT = BtnBarOUT.BackColor;
            Settings.Default.ColBarUNK = BtnBarUNK.BackColor;
            Settings.Default.ColFontIN = BtnFontIN.BackColor;
            Settings.Default.ColFontOUT = BtnFontOUT.BackColor;
            Settings.Default.ColFontUNK = BtnFontUNK.BackColor;
            Settings.Default.ColSelectedFontIN = BtnSelectedFontIN.BackColor;
            Settings.Default.ColSelectedFontOUT = BtnSelectedFontOUT.BackColor;
            Settings.Default.ColSelectedFontUNK = BtnSelectedFontUNK.BackColor;
            Settings.Default.ColSelectIN = BtnSelectIN.BackColor;
            Settings.Default.ColSelectOUT = BtnSelectOUT.BackColor;
            Settings.Default.ColSelectUNK = BtnSelectUNK.BackColor;
            Settings.Default.ColSyncIN = BtnSyncIN.BackColor;
            Settings.Default.ColSyncOUT = BtnSyncOUT.BackColor;
            Settings.Default.ColSyncUNK = BtnSyncUNK.BackColor;

            Settings.Default.ColFieldCount = TbFieldColorCount.Value;
            Settings.Default.ColField1 = LocalFieldColors[1];
            Settings.Default.ColField2 = LocalFieldColors[2];
            Settings.Default.ColField3 = LocalFieldColors[3];
            Settings.Default.ColField4 = LocalFieldColors[4];
            Settings.Default.ColField5 = LocalFieldColors[5];
            Settings.Default.ColField6 = LocalFieldColors[6];
            Settings.Default.ColField7 = LocalFieldColors[7];
            Settings.Default.ColField8 = LocalFieldColors[8];
            Settings.Default.ColField9 = LocalFieldColors[9];
            Settings.Default.ColField10 = LocalFieldColors[10];
            Settings.Default.ColField11 = LocalFieldColors[11];
            Settings.Default.ColField12 = LocalFieldColors[12];
            Settings.Default.ColField13 = LocalFieldColors[13];
            Settings.Default.ColField14 = LocalFieldColors[14];
            Settings.Default.ColField15 = LocalFieldColors[15];
        }

        private void BtnDefault_Click(object sender, EventArgs e)
        {
            Settings.Default.Reset();
            LoadSettingsIntoForm(true);
        }

        private void BtnColorButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn)
                return;
            ColorDlg.Color = btn.BackColor;
            if (ColorDlg.ShowDialog() == DialogResult.OK)
            {
                btn.BackColor = ColorDlg.Color;
            }
        }

        private void BtnSetDarkMode_Click(object sender, EventArgs e)
        {
            // Dark Mode suggested colors

            BtnBackIN.BackColor = Color.FromArgb(0, 16, 0);
            BtnBackOUT.BackColor = Color.FromArgb(0, 0, 16);
            BtnBackUNK.BackColor = Color.FromArgb(16, 16, 16);
            BtnBarIN.BackColor = Color.FromArgb(200, 255, 200);
            BtnBarOUT.BackColor = Color.FromArgb(200, 200, 255);
            BtnBarUNK.BackColor = Color.FromArgb(255, 200, 200);
            BtnFontIN.BackColor = Color.FromArgb(128, 255, 128);
            BtnFontOUT.BackColor = Color.FromArgb(64, 128, 255);
            BtnFontUNK.BackColor = Color.FromArgb(192, 64, 64);
            BtnSelectedFontIN.BackColor = Color.FromArgb(16, 200, 16);
            BtnSelectedFontOUT.BackColor = Color.FromArgb(64, 128, 255);
            BtnSelectedFontUNK.BackColor = Color.FromArgb(200, 16, 16);
            BtnSelectIN.BackColor = Color.FromArgb(0, 64, 0);
            BtnSelectOUT.BackColor = Color.FromArgb(8, 16, 64);
            BtnSelectUNK.BackColor = Color.FromArgb(64, 0, 0);
            BtnSyncIN.BackColor = Color.FromArgb(16, 92, 16);
            BtnSyncOUT.BackColor = Color.FromArgb(16, 16, 92);
            BtnSyncUNK.BackColor = Color.FromArgb(92, 16, 16);
        }

        private void TbFieldColorCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateFieldColorGrid();
        }

        private void UpdateFieldColorGrid()
        {
            var n = TbFieldColorCount.Value;
            LFieldColCount.Text = n.ToString();

            if (n >= 1)
            {
                LFieldCol0.ForeColor = LocalFieldColors[0];
                LFieldCol0.BackColor = SystemColors.Control;
            }
            else
            {
                LFieldCol0.ForeColor = Color.Gray;
                LFieldCol0.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 2)
            {
                BtnColField1.ForeColor = LocalFieldColors[1];
                BtnColField1.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField1.ForeColor = Color.Gray;
                BtnColField1.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 3)
            {
                BtnColField2.ForeColor = LocalFieldColors[2];
                BtnColField2.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField2.ForeColor = Color.Gray;
                BtnColField2.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 4)
            {
                BtnColField3.ForeColor = LocalFieldColors[3];
                BtnColField3.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField3.ForeColor = Color.Gray;
                BtnColField3.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 5)
            {
                BtnColField4.ForeColor = LocalFieldColors[4];
                BtnColField4.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField4.ForeColor = Color.Gray;
                BtnColField4.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 6)
            {
                BtnColField5.ForeColor = LocalFieldColors[5];
                BtnColField5.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField5.ForeColor = Color.Gray;
                BtnColField5.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 7)
            {
                BtnColField6.ForeColor = LocalFieldColors[6];
                BtnColField6.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField6.ForeColor = Color.Gray;
                BtnColField6.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 8)
            {
                BtnColField7.ForeColor = LocalFieldColors[7];
                BtnColField7.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField7.ForeColor = Color.Gray;
                BtnColField7.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 9)
            {
                BtnColField8.ForeColor = LocalFieldColors[8];
                BtnColField8.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField8.ForeColor = Color.Gray;
                BtnColField8.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 10)
            {
                BtnColField9.ForeColor = LocalFieldColors[9];
                BtnColField9.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField9.ForeColor = Color.Gray;
                BtnColField9.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 11)
            {
                BtnColField10.ForeColor = LocalFieldColors[10];
                BtnColField10.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField10.ForeColor = Color.Gray;
                BtnColField10.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 12)
            {
                BtnColField11.ForeColor = LocalFieldColors[11];
                BtnColField11.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField11.ForeColor = Color.Gray;
                BtnColField11.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 13)
            {
                BtnColField12.ForeColor = LocalFieldColors[12];
                BtnColField12.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField12.ForeColor = Color.Gray;
                BtnColField12.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 14)
            {
                BtnColField13.ForeColor = LocalFieldColors[13];
                BtnColField13.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField13.ForeColor = Color.Gray;
                BtnColField13.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 15)
            {
                BtnColField14.ForeColor = LocalFieldColors[14];
                BtnColField14.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField14.ForeColor = Color.Gray;
                BtnColField14.BackColor = SystemColors.ControlDarkDark;
            }

            if (n >= 16)
            {
                BtnColField15.ForeColor = LocalFieldColors[15];
                BtnColField15.BackColor = SystemColors.Control;
            }
            else
            {
                BtnColField15.ForeColor = Color.Gray;
                BtnColField15.BackColor = SystemColors.ControlDarkDark;
            }

            foreach (var control in LayoutGridColors.Controls)
            {
                if (control is Label l)
                    l.Font = BtnGridViewFont.Font;
                else
                    if (control is Button b)
                    b.Font = BtnGridViewFont.Font;
            }

        }

        private void BtnColField_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn)
                return;

            int t = btn.Tag != null ? int.Parse((string)btn.Tag) : 0;
            ColorDlg.Color = LocalFieldColors[t];
            if (ColorDlg.ShowDialog() == DialogResult.OK)
            {
                LocalFieldColors[t] = ColorDlg.Color;
                UpdateFieldColorGrid();
            }
        }

        private void BtnPacketListFont_Click(object sender, EventArgs e)
        {
            FontDlg.Font = Settings.Default.PacketListFont;
            if (FontDlg.ShowDialog() == DialogResult.OK)
            {
                BtnPacketListFont.Font = FontDlg.Font;
                BtnPacketListFont.Text = BtnPacketListFont.Font.Name + @", " + BtnPacketListFont.Font.SizeInPoints + @"pt";
                LabelPacketListArrows.Font = FontDlg.Font;
            }
        }

        private void BtnGridViewFont_Click(object sender, EventArgs e)
        {
            FontDlg.Font = Settings.Default.FieldViewFont;
            if (FontDlg.ShowDialog() == DialogResult.OK)
            {
                BtnGridViewFont.Font = FontDlg.Font;
                BtnGridViewFont.Text = BtnGridViewFont.Font.Name + @", " + BtnGridViewFont.Font.SizeInPoints + @"pt";
            }

            UpdateFieldColorGrid();
        }

        private void BtnRawViewFont_Click(object sender, EventArgs e)
        {
            FontDlg.Font = Settings.Default.RawViewFont;
            if (FontDlg.ShowDialog() == DialogResult.OK)
            {
                BtnRawViewFont.Font = FontDlg.Font;
                BtnRawViewFont.Text = BtnRawViewFont.Font.Name + @", " + BtnRawViewFont.Font.SizeInPoints + @"pt";
            }

        }

        private void BtnDefaultImportFolder_Click(object sender, EventArgs e)
        {
            if (DefaultFolderDialog.ShowDialog() == DialogResult.OK)
                LabelDefaultImportFolder.Text = DefaultFolderDialog.SelectedPath;
        }
    }
}
