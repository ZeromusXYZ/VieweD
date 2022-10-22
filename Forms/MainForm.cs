using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using VieweD.Helpers.System;
using VieweD.Engine;
using VieweD.Engine.Common;
using VieweD.Forms;

namespace VieweD
{
    // Extra icons in Post-Build from: https://stackoverflow.com/questions/3485103/automatically-embed-multiple-icons-in-a-net-executable

    public partial class MainForm : Form
    {
        public static MainForm ThisMainForm;
        public readonly List<string> AllUsedTempFiles = new List<string>();

        private string defaultTitle = "";
        static readonly string UrlGitHub = "https://github.com/ZeromusXYZ/VieweD";
        static readonly string UrlDiscord = "https://discord.gg/GhVfDtK";
        static readonly string UrlVideoLan = "https://www.videolan.org/";
        static readonly string Url7Zip = "https://www.7-zip.org/";
        static readonly string Url7ZipRequiredVer = "https://sourceforge.net/p/sevenzip/discussion/45797/thread/adc65bfa/";

        public PacketParser CurrentPP;
        public static SearchParameters SearchParameters;

        private const string InfoGridHeader = "     |  0  1  2  3   4  5  6  7   8  9  A  B   C  D  E  F    | 0123456789ABCDEF\n" +
                                              "-----+----------------------------------------------------  -+------------------\n";

        public MainForm()
        {
            InitializeComponent();
            ThisMainForm = this;
            SearchParameters = new SearchParameters();
            SearchParameters.ClearValidSearchFlags();
        }

        private void mmFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mmAboutGithub_Click(object sender, EventArgs e)
        {
            Process.Start(UrlGitHub);
        }

        private void mmAboutVideoLAN_Click(object sender, EventArgs e)
        {
            Process.Start(UrlVideoLan);
        }

        private void MmAboutDiscord_Click(object sender, EventArgs e)
        {
            Process.Start(UrlDiscord);
        }

        private void mmAboutAbout_Click(object sender, EventArgs e)
        {
            using (AboutBoxForm ab = new AboutBoxForm())
            {
                ab.ShowDialog();
            }
        }

        private void RegisterFileExt()
        {
            try
            {
                // Might also need to check
                // HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\
                FileAssociations.EnsureAssociationsSet();
                //FileAssociations.EnsureURIAssociationsSet();
            }
            catch
            {
                // Set File or URI Association failed ?
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            defaultTitle = Text;

            // Handle User settings upgrades when using a newer version
            if (Properties.Settings.Default.DoUpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.DoUpdateSettings = false; // This is setting is true by default, so we reset it when done upgrading
                Properties.Settings.Default.Save();
            }

            // Create Engines Handler (and load engines)
            Engines.Instance = new Engines();

            if (Engines.PluginErrors.Count > 0)
            {
                MessageBox.Show(string.Join("\r\n", Engines.PluginErrors),"Plugin Compile Errors");
            }

            RegisterFileExt(); // Registers project files in Windows

            PacketColors.UpdateColorsFromSettings();
            dGV.Font = Properties.Settings.Default.GridViewFont;

            Application.UseWaitCursor = true;
            try
            {
                Directory.SetCurrentDirectory(Application.StartupPath);
            }
            catch (Exception x)
            {
                MessageBox.Show("Exception: " + x.Message, "Loading Lookup Data", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
                return;
            }
            tcPackets.TabPages.Clear();
            Application.UseWaitCursor = false;
        }



        private void mmFileOpen_Click(object sender, EventArgs e)
        {
            openLogFileDialog.Title = "Open log file";
            openLogFileDialog.Filter = Engines.GetRegisteredFileExtensionForOpen(true);
            if (openLogFileDialog.ShowDialog() != DialogResult.OK)
                return;
            TryOpenFile(openLogFileDialog.FileName);
        }

        private void TryOpenFile(string aFileName)
        {
            if ((Path.GetExtension(aFileName).ToLower() == ".pvd") || (Path.GetExtension(aFileName).ToLower() == ".pvlv"))
            {
                // Open Project File
                TryOpenProjectFile(aFileName);
            }
            else
            if ((Path.GetExtension(aFileName).ToLower() == ".7z") || (Path.GetExtension(aFileName).ToLower() == ".zip") || (Path.GetExtension(aFileName).ToLower() == ".rar"))
            {
                // Open Archive as a Project Folder
                TryOpenProjectArchive(aFileName);
            }
            else
            {
                TryOpenLogFile(aFileName, true);
            }

            if (GameViewForm.GV != null)
                GameViewForm.GV.btnRefreshLookups.PerformClick();
        }

        private void TryOpenProjectArchive(string projectArchive)
        {
            if (!File.Exists(projectArchive))
                return;

            if (MessageBox.Show($"Do you want to extract this archive file?", "Unpack Archive", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using (var zipForm = new CompressForm())
            {
                zipForm.task = CompressForm.ZipTaskType.doUnZip;
                zipForm.ArchiveFileName = projectArchive;
                zipForm.ProjectName = Path.GetFileNameWithoutExtension(projectArchive);

                if (zipForm.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show($"Failed to extract files from archive\r\n{projectArchive}", "Unpack Archive",
                        MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    return;
                }
            }

            var expectedProjectFile = Path.ChangeExtension(projectArchive, ".pvd");
            if (!File.Exists(expectedProjectFile))
            {
                MessageBox.Show($"The Archive got extracted, but didn't find the expected project file\r\n{projectArchive}\r\n\r\nTry to manually open the file.", "Unpack Archive", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            TryOpenProjectFile(expectedProjectFile);
        }

        private void TryOpenProjectFile(string ProjectFile)
        {
            PacketTabPage tp = CreateNewPacketsTabPage();
            tp.LoadProjectFile(ProjectFile);
            tp.Text = Helper.MakeTabName(ProjectFile);

            using (var projectDlg = new ProjectInfoForm())
            {
                projectDlg.LoadFromPacketTapPage(tp);
                projectDlg.btnSave.Text = "Open";
                projectDlg.cbOpenedLog.Enabled = true;
                if (projectDlg.ShowDialog() == DialogResult.OK)
                {
                    projectDlg.ApplyPacketTapPage();
                    TryOpenLogFile(tp.LoadedLogFile, false);
                    tp.SaveProjectFile();
                }
                else
                {
                    tcPackets.TabPages.Remove(tp);
                }
            }

        }


        private void TryOpenLogFile(string logFile, bool alsoLoadProject)
        {
            PacketTabPage tp;
            if (alsoLoadProject)
            {
                tp = CreateNewPacketsTabPage();
                tp.LoadProjectFileFromLogFile(logFile);
            }
            else
            {
                tp = GetCurrentPacketTabPage();
            }

            //tp.ProjectFolder = Helper.MakeProjectDirectoryFromLogFileName(logFile);
            tp.Text = Helper.MakeTabName(logFile);

            tp.PLLoaded.Clear();
            tp.PLLoaded.Filter.Clear();
            if (!tp.PLLoaded.LoadFromFile(logFile,tp))
            {
                MessageBox.Show("Error loading file: " + logFile, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tp.PLLoaded.Clear();
                tcPackets.TabPages.Remove(tp);
                return;
            }
            if (tp.PLLoaded.Count <= 0)
            {
                MessageBox.Show("File contains no useful data.\n" + logFile, "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tcPackets.TabPages.Remove(tp);
                return;
            }
            Text = defaultTitle + " - " + logFile;
            tp.LoadedLogFile = logFile;
            if (tp.PLLoaded.Rules != null)
                tp.LoadedRulesFile = tp.PLLoaded.Rules.LoadedRulesFileName;
            tp.PL.CopyFrom(tp.PLLoaded);
            tp.FillListBox();
            UpdateStatusBarAndTitle(tp);
            if (Properties.Settings.Default.AutoOpenVideoForm && ((tp.LinkVideoFileName != string.Empty) || (tp.LinkYoutubeUrl != string.Empty)))
            {
                MmVideoOpenLink_Click(null, null);
            }
        }

        public void lbPackets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (sender as ListBox);
            if (!(lb.Parent is PacketTabPage))
                return;
            PacketTabPage tp = (lb.Parent as PacketTabPage);
            if ((lb.SelectedIndex < 0) || (lb.SelectedIndex >= tp.PL.Count))
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }
            PacketData pd = tp.PL.GetPacket(lb.SelectedIndex);
            cbShowBlock.Enabled = false;
            UpdatePacketDetails(tp, pd, "-");
            cbShowBlock.Enabled = true;
            lb.Invalidate();
            if ((tp.VideoLink != null) && (tp.VideoLink.cbFollowPacketList.Checked))
            {
                tp.VideoLink.MoveToDateTime(pd.VirtualTimeStamp);
            }
        }


        private void cbOriginalData_CheckedChanged(object sender, EventArgs e)
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a tabpage first";
                return;
            }

            PacketData pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }

            UpdatePacketDetails(tp, pd, "-");
        }

        private void mmFileClose_Click(object sender, EventArgs e)
        {
            if ((tcPackets.SelectedIndex >= 0) && (tcPackets.SelectedIndex < tcPackets.TabCount))
            {
                tcPackets.TabPages.RemoveAt(tcPackets.SelectedIndex);
            }
            /*
            PLLoaded.Clear();
            PLLoaded.ClearFilters();
            PL.Clear();
            PL.ClearFilters();
            FillListBox(lbPackets,PL);
            */
        }

        private void mmFileAppend_Click(object sender, EventArgs e)
        {
            openLogFileDialog.Title = "Append log file";
            openLogFileDialog.Filter = Engines.GetRegisteredFileExtensionForOpen(false);
            if (openLogFileDialog.ShowDialog() != DialogResult.OK)
                return;

            PacketTabPage tp = GetCurrentOrNewPacketTabPage();
            tp.Text = "Multi";
            tp.LoadedLogFile = "?Multiple Sources";
            tp.ProjectFolder = string.Empty;

            if (!tp.PLLoaded.LoadFromFile(openLogFileDialog.FileName,tp))
            {
                MessageBox.Show("Error loading file: " + openLogFileDialog.FileName, "File Append Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tp.PLLoaded.Clear();
                return;
            }
            Text = defaultTitle + " - " + tp.LoadedLogFile;
            if (tp.PLLoaded.Rules != null)
                tp.LoadedRulesFile = tp.PLLoaded.Rules.LoadedRulesFileName;
            tp.PL.CopyFrom(tp.PLLoaded);
            tp.FillListBox();
            UpdateStatusBarAndTitle(tp);
        }

        private void RawDataToRichText(PacketParser pp, RichTextBox rt)
        {
            RichTextBox rtInfo = rt;
            string rtf = string.Empty;
            List<Color> colorTable = new List<Color>();
            int LastForeCol = -1;
            int LastBackCol = -1;

            int GetRTFColor(Color col)
            {
                var p = colorTable.IndexOf(col);
                if (p < 0)
                {
                    p = colorTable.Count;
                    colorTable.Add(col);
                }
                return p + 1;
            }

            void SetRTFColor(Color Fore, Color Back)
            {
                var f = GetRTFColor(Fore);
                var b = GetRTFColor(Back);
                //rtf += "\\cf" + f.ToString() + "\\highlight" + b.ToString();
                if ((f == LastForeCol) && (b == LastBackCol))
                    return;
                if (f != LastForeCol)
                    rtf += "\\cf" + f.ToString();
                if (b != LastBackCol)
                    rtf += "\\highlight" + b.ToString();
                rtf += " ";
                LastForeCol = f;
                LastBackCol = b;
            }


            string BuildHeaderWithColorTable()
            {
                string rtfHead = string.Empty;
                rtfHead += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 " + Properties.Settings.Default.RawViewFont.Name + ";}}";
                // rtfHead += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 Consolas;}}";
                rtfHead += "{\\colortbl;";
                foreach (var col in colorTable)
                {
                    rtfHead += "\\red" + col.R.ToString() + "\\green" + col.G.ToString() + "\\blue" + col.B.ToString() + ";";
                }
                rtfHead += "}";
                rtfHead += "\\viewkind4\\uc1\\pard\\cf1\\highlight2\\f0\\fs18 ";
                // {\colortbl ;\red169\green169\blue169;\red255\green255\blue255;\red25\green25\blue112;\red0\green0\blue0;\red210\green105\blue30;\red100\green149\blue237;\red60\green179\blue113;\red233\green150\blue122;\red165\green42\blue42;}
                return rtfHead;
            }

            void SetColorBasic(ushort n)
            {
                SetRTFColor(rtInfo.ForeColor, rtInfo.BackColor);
                //rtInfo.SelectionFont = rtInfo.Font;
                //rtInfo.SelectionColor = rtInfo.ForeColor;
                //rtInfo.SelectionBackColor = rtInfo.BackColor;
            }

            void SetColorGrid()
            {
                SetRTFColor(Color.DarkGray, rtInfo.BackColor);
                //rtInfo.SelectionFont = rtInfo.Font;
                //rtInfo.SelectionColor = Color.DarkGray;
                //rtInfo.SelectionBackColor = rtInfo.BackColor;
            }

            void SetColorSelect(ushort n, bool forchars)
            {
                //if (!forchars)
                //{
                //    rtInfo.SelectionFont = new Font(rtInfo.Font, FontStyle.Italic);
                //}
                //else
                //{
                //    rtInfo.SelectionFont = rtInfo.Font;
                //}
                SetRTFColor(Color.Yellow, Color.DarkBlue);
                //rtInfo.SelectionColor = Color.Yellow;
                //rtInfo.SelectionBackColor = Color.DarkBlue;
            }

            void SetColorNotSelect(ushort n, bool forchars)
            {
                //rtInfo.SelectionFont = rtInfo.Font;
                if ((pp.SelectedFields.Count > 0) || forchars)
                {
                    SetRTFColor(pp.GetDataColor(n), rtInfo.BackColor);
                    //rtInfo.SelectionColor = pp.GetDataColor(n);
                    //rtInfo.SelectionBackColor = rtInfo.BackColor;
                }
                else
                {
                    SetRTFColor(rtInfo.BackColor, pp.GetDataColor(n));
                    //rtInfo.SelectionColor = rtInfo.BackColor;
                    //rtInfo.SelectionBackColor = pp.GetDataColor(n);
                }
            }


            void AddChars(int startIndex)
            {
                SetColorGrid();
                rtf += "  | ";
                //rtInfo.AppendText("  | ");
                for (int c = 0; (c < 0x10) && ((startIndex + c) < pp.ParsedBytes.Count); c++)
                {
                    var n = pp.ParsedBytes[startIndex + c];
                    if (pp.SelectedFields.IndexOf(n) >= 0)
                    {
                        SetColorSelect(n, true);
                    }
                    else
                    {
                        SetColorNotSelect(n, true);
                    }
                    char ch = (char)pp.PD.GetByteAtPos(startIndex + c);
                    if (ch == 92)
                        rtf += "\\\\";
                    else
                    if (ch == 64)
                        rtf += "\\@";
                    else
                    if (ch == 123)
                        rtf += "\\{";
                    else
                    if (ch == 125)
                        rtf += "\\}";
                    else
                    if ((ch < 32) || (ch >= 128))
                        rtf += '.';
                    else
                        rtf += ch.ToString();
                    //rtInfo.AppendText(ch.ToString());
                }
            }

            rtInfo.SuspendLayout();
            rtInfo.ForeColor = SystemColors.WindowText;
            rtInfo.BackColor = SystemColors.Window;
            // rtInfo.Clear();

            SetColorGrid();

            rtf += InfoGridHeader.Replace("\n", "\\par\n");
            //rtInfo.AppendText(InfoGridHeader);
            int addCharCount = 0;
            ushort lastFieldIndex = 0;
            bool moveCursor = true;
            var endCursor = -1;

            for (int i = 0; i < pp.PD.RawBytes.Count; i += 0x10)
            {
                SetColorGrid();
                rtf += i.ToString("X").PadLeft(4, ' ') + " | ";
                //rtInfo.AppendText(i.ToString("X").PadLeft(4, ' ') + " | ");
                for (int i2 = 0; i2 < 0x10; i2++)
                {
                    if ((i + i2) < pp.ParsedBytes.Count)
                    {
                        var n = pp.ParsedBytes[i + i2];
                        lastFieldIndex = n;
                        if (pp.SelectedFields.Count > 0)
                        {
                            if (pp.SelectedFields.IndexOf(n) >= 0)
                            {
                                // Is selected field
                                SetColorSelect(n, false);
                                if (moveCursor)
                                {
                                    moveCursor = false;
                                    endCursor = i + i2;
                                }
                            }
                            else
                            {
                                // we have non-selected field
                                SetColorNotSelect(n, false);
                            }
                        }
                        else
                        {
                            // No fields selected
                            SetColorNotSelect(n, false);
                        }
                        rtf += pp.PD.GetByteAtPos(i + i2).ToString("X2");
                        //rtInfo.AppendText(pp.PD.GetByteAtPos(i + i2).ToString("X2"));
                        addCharCount++;
                    }
                    else
                    {
                        SetColorGrid();
                        rtf += "  ";
                        //rtInfo.AppendText("  ");
                    }

                    if ((i + i2 + 1) < pp.ParsedBytes.Count)
                    {
                        var n = pp.ParsedBytes[i + i2 + 1];
                        if (n != lastFieldIndex)
                        {
                            SetColorBasic(n);
                        }
                    }
                    else
                    {
                        SetColorGrid();
                    }

                    rtf += " ";
                    // rtInfo.AppendText(" ");
                    if ((i2 % 0x4) == 0x3)
                    {
                        rtf += " ";
                        //rtInfo.AppendText(" ");
                    }
                }
                if (addCharCount > 0)
                {
                    AddChars(i);
                    addCharCount = 0;
                }
                rtf += "\\par\n";
                // rtInfo.AppendText("\r\n");
            }
            rtf += "}\n";
            rtInfo.WordWrap = false;
            rtInfo.Rtf = BuildHeaderWithColorTable() + rtf;
            rtInfo.Refresh();
            rtInfo.ResumeLayout();

            if ((endCursor >= 0) && (cbOriginalData.Checked == false))
            {
                var line = (endCursor / 16)+2;
                var linePos = endCursor % 16;
                var rawPos = rtInfo.GetFirstCharIndexFromLine(line) + 7 + (linePos * 3);
                if (linePos > 3)
                    rawPos++;
                if (linePos > 7)
                    rawPos ++;
                if (linePos > 11)
                    rawPos ++;
                rtInfo.SelectionStart = rawPos;
                rtInfo.SelectionLength = 0;
            }

        }

        public void UpdatePacketDetails(PacketTabPage tp, PacketData pd, string SwitchBlockName, bool dontReloadParser = false)
        {
            if ((tp == null) || (pd == null))
                return;
            tp.CurrentSync = pd.PacketSync;
            lInfo.Text = pd.OriginalHeaderText;
            rtInfo.Clear();

            if ((dontReloadParser == false) || (pd.PP == null))
            {
                pd.PP = pd.Parent._parentTab.Engine.GetParser(pd);
                pd.PP?.AssignPacket(pd);
            }

            if (pd.PP == null)
                return;

            if ((tp.PL.IsPreParsed == false) || (pd.PP.PreParsedSwitchBlock != SwitchBlockName))
                pd.PP.ParseData(SwitchBlockName);

            CurrentPP = pd.PP;
            CurrentPP.ToGridView(dGV);
            cbShowBlock.Enabled = false;
            if (CurrentPP.SwitchBlocks.Count > 0)
            {
                cbShowBlock.Items.Clear();
                cbShowBlock.Items.Add("-");
                cbShowBlock.Items.AddRange(CurrentPP.SwitchBlocks.ToArray());
                cbShowBlock.Show();
            }
            else
            {
                cbShowBlock.Items.Clear();
                cbShowBlock.Hide();
            }
            for (int i = 0; i < cbShowBlock.Items.Count; i++)
            {
                if ((SwitchBlockName == "-") && (cbShowBlock.Items[i].ToString() == CurrentPP.LastSwitchedBlock))
                {
                    if (cbShowBlock.SelectedIndex != i)
                        cbShowBlock.SelectedIndex = i;
                    //break;
                }
                else
                if (cbShowBlock.Items[i].ToString() == SwitchBlockName)
                {
                    if (cbShowBlock.SelectedIndex != i)
                        cbShowBlock.SelectedIndex = i;
                    //break;
                }
            }
            cbShowBlock.Enabled = true;

            if (cbOriginalData.Checked)
            {
                rtInfo.SuspendLayout();
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Source:\r\n" + string.Join("\r\n", pd.RawText.ToArray());
                rtInfo.Refresh();
                rtInfo.ResumeLayout();
            }
            else
            {
                RawDataToRichText(CurrentPP, rtInfo);
            }

        }

        private void mmFileSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsDialog = new SettingsForm())
            {
                if (settingsDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.Save();
                    PacketColors.UpdateColorsFromSettings();
                    dGV.Font = Properties.Settings.Default.GridViewFont;
                    // Apply packet list font to open tabs
                    foreach (var page in tcPackets.TabPages)
                        if (page is PacketTabPage tp)
                        {
                            tp.LbPackets.Font = Properties.Settings.Default.PacketListFont;
                            tp.LbPackets.ItemHeight = (int)Math.Ceiling(tp.LbPackets.Font.GetHeight());
                            tp.LbPackets.Dock = DockStyle.Fill;
                            tp.LbPackets.Refresh();
                        }

                    //LoadDataFromGameclient();
                    //MessageBox.Show("Settings saved");
                }
                settingsDialog.Dispose();
            }
        }

        private void CbShowBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbShowBlock.Enabled)
                return;

            if (!(tcPackets.SelectedTab is PacketTabPage))
                return;
            PacketTabPage tp = (tcPackets.SelectedTab as PacketTabPage);

            cbShowBlock.Enabled = false;
            if ((tp.LbPackets.SelectedIndex < 0) || (tp.LbPackets.SelectedIndex >= tp.PL.Count))
            {
                rtInfo.SelectionColor = rtInfo.ForeColor;
                rtInfo.SelectionBackColor = rtInfo.BackColor;
                rtInfo.Text = "Please select a valid item from the list";
                return;
            }
            PacketData pd = tp.PL.GetPacket(tp.LbPackets.SelectedIndex);
            var sw = cbShowBlock.SelectedIndex;
            if (sw >= 0)
            {
                UpdatePacketDetails(tp, pd, cbShowBlock.Items[sw].ToString(), true);
            }
            else
            {
                UpdatePacketDetails(tp, pd, "-", true);
            }
            cbShowBlock.Enabled = true;
            tp.LbPackets.Invalidate();
        }

        private void dGV_SelectionChanged(object sender, EventArgs e)
        {
            if ((CurrentPP == null) || (CurrentPP.PD == null))
                return;
            if (dGV.Tag != null)
                return;
            CurrentPP.SelectedFields.Clear();
            for (int i = 0; i < dGV.RowCount; i++)
            {
                if ((dGV.Rows[i].Selected) && (i < CurrentPP.ParsedView.Count))
                {
                    var f = CurrentPP.ParsedView[i].FieldIndex;
                    //if (f != 0xFF)
                    CurrentPP.SelectedFields.Add(f);
                }
            }
            CurrentPP.ToGridView(dGV);
            RawDataToRichText(CurrentPP, rtInfo);
        }


        public void UpdateStatusBarAndTitle(PacketTabPage tp)
        {
            if (tp == null)
            {
                // Statusbar
                sbEngine.Text = "";
                sbEngine.Visible = false;
                sbProjectInfo.Text = "Not a project";
                sbExtraInfo.Text = "";
                sbExtraInfo.Visible = false;
                sbRules.Text = "";
                sbRules.Visible = false;
                // Menu
                mmFileAppend.Enabled = false;
                mmFileAddFromClipboard.Enabled = false;
                mmFilePasteNew.Enabled = true;
                mmSearch.Enabled = false;
                mmFilter.Enabled = false;
                mmFileProjectDetails.Enabled = false;
                mmFileClose.Enabled = false;
                return;
            }

            var t = tp.LoadedLogFile;
            if (t.StartsWith("?"))
                t = t.TrimStart('?');
            Text = defaultTitle + " - " + t;
            if (tp.ProjectFolder != string.Empty)
            {
                sbProjectInfo.Text = "Project Folder: " + tp.ProjectFolder;
            }
            else
            {
                sbProjectInfo.Text = "Not a project";
            }

            if (File.Exists(tp.LinkVideoFileName))
                sbExtraInfo.Text = "Local Video Linked";
            else
            if (tp.LinkYoutubeUrl != string.Empty)
                sbExtraInfo.Text = "Youtube Linked";
            else
                sbExtraInfo.Text = "";
            sbExtraInfo.Visible = (sbExtraInfo.Text != string.Empty);

            if (tp.Engine.HasRulesFile)
            {
                if (File.Exists(tp.PL.Rules.LoadedRulesFileName))
                {
                    sbRules.Text = "Rules: " + Path.GetFileNameWithoutExtension(tp.PL.Rules.LoadedRulesFileName);
                }
                else
                {
                    sbRules.Text = "No Rules Loaded";
                }
                sbRules.Visible = true;
                sbRules.ToolTipText = tp.PL.Rules.LoadedRulesFileName;
            }
            else
            {
                sbRules.Text = string.Empty;
                sbRules.ToolTipText = string.Empty;
                sbRules.Visible = false;
            }
            sbEngine.Text = tp.Engine.EngineName ;
            sbEngine.Visible = (sbEngine.Text != string.Empty);

            // Menu stuff
            mmFileAppend.Enabled = tp?.Engine?.CanAppend(tp) ?? false;
            mmFileAddFromClipboard.Enabled = mmFileAppend.Enabled;
            mmFilePasteNew.Enabled = true;
            mmFileProjectDetails.Enabled = true;
            mmFileClose.Enabled = true;
            mmSearch.Enabled = tp?.PL?.Count > 0;
            mmFilter.Enabled = tp?.PL?.Count > 0;

            mmTools.Visible = tp?.Engine.ToolNamesList.Count > 0;
            mmTools.Enabled = mmTools.Visible;
        }

        private void TcPackets_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tc = (sender as TabControl);
            if (!(tc.SelectedTab is PacketTabPage))
            {
                UpdateStatusBarAndTitle(null);
                return;
            }
            PacketTabPage tp = (tc.SelectedTab as PacketTabPage);
            UpdateStatusBarAndTitle(tp);
            PacketData pd = tp.PL.GetPacket(tp.LbPackets.SelectedIndex);
            cbShowBlock.Enabled = false;
            UpdatePacketDetails(tp, pd, "-");
            cbShowBlock.Enabled = true;
        }

        private void MmAddFromClipboard_Click(object sender, EventArgs e)
        {
            if ((!Clipboard.ContainsText()) || (Clipboard.GetText() == string.Empty))
            {
                MessageBox.Show(@"Nothing to paste", @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                PacketTabPage tp = GetCurrentOrNewPacketTabPage();
                if (tp.Engine.EngineId == "null")
                {
                    MessageBox.Show(@"No engine selected", @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!tp.Engine.CanAppend(tp))
                {
                    MessageBox.Show(tp.Text + @" using " + tp.Engine.EngineName + @" does not support pasting data", @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                tp.Text = "Clipboard   ";
                tp.LoadedLogFile = "?Paste from Clipboard";
                tp.ProjectFolder = string.Empty;
                
                var cText = Clipboard.GetText().Replace("\r", "");

                using (var clipBoardMemoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(clipBoardMemoryStream, Encoding.UTF8, cText.Length, true))
                    {
                        writer.Write(cText);
                    }
                    clipBoardMemoryStream.Seek(0, SeekOrigin.Begin);
                    
                    if (!tp.PLLoaded._parentTab.Engine.LoadFromStream(tp.PLLoaded, clipBoardMemoryStream, tp.LoadedLogFile, string.Empty,string.Empty))
                    {
                        MessageBox.Show("Error loading data from clipboard", "Clipboard Paste Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tp.PLLoaded.Clear();
                        tcPackets.TabPages.Remove(tp);
                        return;
                    }
                }

                if (tp.PLLoaded.Count <= 0)
                {
                    MessageBox.Show("Clipboard contained no useful data.", "Clipboard Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                Text = defaultTitle + " - " + tp.LoadedLogFile;
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox();
                UpdateStatusBarAndTitle(tp);
            }
            catch (Exception x)
            {
                MessageBox.Show("Paste Failed, Exception: " + x.Message, "Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private PacketTabPage CreateNewPacketsTabPage()
        {
            PacketTabPage tp = new PacketTabPage(this);
            tp.LbPackets.SelectedIndexChanged += lbPackets_SelectedIndexChanged;
            tcPackets.TabPages.Add(tp);
            tcPackets.SelectedTab = tp;
            tp.LbPackets.Focus();
            return tp;
        }

        private PacketTabPage GetCurrentOrNewPacketTabPage()
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                tp = CreateNewPacketsTabPage();
            }
            return tp;
        }

        public PacketTabPage GetCurrentPacketTabPage()
        {
            if (!(tcPackets.SelectedTab is PacketTabPage))
            {
                return null;
            }
            else
            {
                return (tcPackets.SelectedTab as PacketTabPage);
            }
        }

        private void MmFilterEdit_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            using (var filterDlg = new FilterForm())
            {
                filterDlg.btnOK.Enabled = (tp != null);
                if (tp != null)
                {
                    filterDlg.currentEngine = tp.Engine;
                    filterDlg.Filter.CopyFrom(tp.PL.Filter);
                    filterDlg.LoadLocalFromFilter();
                }
                var lastSync = tp.CurrentSync;
                switch (filterDlg.ShowDialog(this))
                {
                    case DialogResult.OK: // Apply
                        filterDlg.SaveLocalToFilter();
                        tp.PL.Filter.CopyFrom(filterDlg.Filter);
                        tp.PL.FilterFrom(tp.PLLoaded);
                        tp.FillListBox(lastSync);
                        tp.CenterListBox();
                        break;
                    case DialogResult.Yes: // Highlight
                        filterDlg.SaveLocalToFilter();
                        tp.PL.Filter.CopyFrom(filterDlg.Filter);
                        tp.PL.HightlightFilterFrom(tp.PLLoaded);
                        tp.FillListBox(lastSync);
                        tp.CenterListBox();
                        break;
                    default:
                        break;
                }
            }
        }

        private void MmFilterReset_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp != null)
            {
                var lastSync = tp.CurrentSync;
                tp.PL.Filter.Clear();
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox(lastSync);
                tp.CenterListBox();
            }

        }

        private void MMFilterApplyItem_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;

            if (sender is ToolStripMenuItem)
            {
                var mITem = (sender as ToolStripMenuItem);
                // apply filter
                var lastSync = tp.CurrentSync;
                tp.PL.Filter.LoadFromFile(Path.Combine(Application.StartupPath, "data", tp.Engine.EngineId, "filter", mITem.Text + ".pfl"));
                tp.PL.FilterFrom(tp.PLLoaded);
                tp.FillListBox(lastSync);
                tp.CenterListBox();
            }
        }

        private void MMFilterHighlightItem_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;

            if (sender is ToolStripMenuItem)
            {
                var mITem = (sender as ToolStripMenuItem);
                // apply filter
                var lastSync = tp.CurrentSync;
                tp.PL.Filter.LoadFromFile(Path.Combine(Application.StartupPath, "data", tp.Engine.EngineId, "filter", mITem.Text + ".pfl"));
                tp.PL.HightlightFilterFrom(tp.PLLoaded);
                tp.FillListBox(lastSync);
                tp.CenterListBox();
            }
        }

        private void MmFilterApply_DropDownOpening(object sender, EventArgs e)
        {
            // generate menu
            // GetFiles
            try
            {
                var tp = GetCurrentPacketTabPage();
                mmFilterApply.DropDownItems.Clear();
                mmFilterHighlight.DropDownItems.Clear();
                var di = new DirectoryInfo(Path.Combine(Application.StartupPath, "data", tp.Engine.EngineId, "filter"));
                var files = di.GetFiles("*.pfl");
                foreach (var fi in files)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fi.Name));
                    mi.Click += MMFilterApplyItem_Click;
                    mmFilterApply.DropDownItems.Add(mi);

                    ToolStripMenuItem mi2 = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fi.Name));
                    mi2.Click += MMFilterHighlightItem_Click;
                    mmFilterHighlight.DropDownItems.Add(mi2);
                }
                if (files.Length <= 0)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem("no filters found");
                    mi.Enabled = false;
                    mmFilterApply.DropDownItems.Add(mi);

                    ToolStripMenuItem mi2 = new ToolStripMenuItem("no filters found");
                    mi2.Enabled = false;
                    mmFilterHighlight.DropDownItems.Add(mi2);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private void MmSearchSearch_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;
            MainForm.SearchParameters.FileFormat = tp.PL.LoadedLogFileFormat;
            using (SearchForm SearchDlg = new SearchForm())
            {
                if (tp.PL.IsPreParsed == false)
                {
                    MainForm.SearchParameters.SearchByParsedData = false;
                    SearchDlg.gbSearchByField.Enabled = false;
                }
                SearchDlg.searchParameters.CopyFrom(MainForm.SearchParameters);
                var res = SearchDlg.ShowDialog();
                if ((res == DialogResult.OK) || (res == DialogResult.Retry))
                {
                    MainForm.SearchParameters.CopyFrom(SearchDlg.searchParameters);
                    if (res == DialogResult.OK)
                        FindNext();
                    else
                    if (res == DialogResult.Retry)
                        FindAsNewTab();
                }
            }
        }

        private void MmSearchNext_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;
            if ((SearchParameters.SearchIncoming == false) && (SearchParameters.SearchOutgoing == false))
            {
                MmSearchSearch_Click(null, null);
                return;
            }
            else
                FindNext();
        }

        private void FindNext()
        {
            var tp = GetCurrentPacketTabPage();

            if ((tp == null) || (tp.LbPackets.Items.Count <= 0))
            {
                MessageBox.Show("Nothing to search in !", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var startIndex = tp.LbPackets.SelectedIndex;
            if ((startIndex < 0) && (startIndex >= tp.LbPackets.Items.Count))
                startIndex = -1;
            int i = startIndex + 1;
            for (int c = 0; c < tp.LbPackets.Items.Count - 1; c++)
            {
                if (i >= tp.LbPackets.Items.Count)
                    i = 0;
                var pd = tp.PL.GetPacket(i);
                if (pd.MatchesSearch(SearchParameters))
                {
                    // Select index
                    tp.LbPackets.SelectedIndex = i;
                    // Move to center
                    var iHeight = tp.LbPackets.ItemHeight;
                    if (iHeight <= 0)
                        iHeight = 8;
                    var iCount = tp.LbPackets.Size.Height / iHeight;
                    var tPos = i - (iCount / 2);
                    if (tPos < 0)
                        tPos = 0;
                    tp.LbPackets.TopIndex = tPos;
                    tp.LbPackets.Focus();
                    // We're done
                    return;
                }
                i++;
            }
            MessageBox.Show("No matches found !", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FindAsNewTab()
        {
            var tp = GetCurrentPacketTabPage();

            if ((tp == null) || (tp.LbPackets.Items.Count <= 0))
            {
                MessageBox.Show("Nothing to search in !", "Search as New Tab", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PacketTabPage newtp = CreateNewPacketsTabPage();
            newtp.Text = "*" + tp.Text;
            newtp.LoadedLogFile = "Search Result";
            newtp.Engine = tp.Engine;
            newtp.LoadedRulesFile = tp.LoadedRulesFile;
            newtp.DecryptVersion = tp.DecryptVersion;

            var count = newtp.PLLoaded.SearchFrom(tp.PL, SearchParameters);

            if (count <= 0)
            {
                MessageBox.Show("No matches found !", "Search as New Tab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                newtp.PL.CopyFrom(newtp.PLLoaded);
                newtp.FillListBox();
            }
            UpdateStatusBarAndTitle(newtp);
        }

        private void MmFilePasteNew_Click(object sender, EventArgs e)
        {

            if ((!Clipboard.ContainsText()) || (Clipboard.GetText() == string.Empty))
            {
                MessageBox.Show(@"Nothing to paste", @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var tp = CreateNewPacketsTabPage();
                if (tp.Engine.EngineId == "null")
                {
                    var newEngine = EngineSelectForm.SelectEngine(true);
                    if ((newEngine == null) || (newEngine.EngineId == "null"))
                    {
                        MessageBox.Show(@"No engine selected", @"Paste from Clipboard", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                    tp.Engine = newEngine;
                    tp.PLLoaded.LoadedLogFileFormat = "Clipboard";
                    tp.PL.LoadedLogFileFormat = tp.PLLoaded.LoadedLogFileFormat;
                }
                
                tp.Text = @"Clipboard   ";
                tp.LoadedLogFile = "?Paste from Clipboard";
                tp.ProjectFolder = string.Empty;
                tcPackets.SelectedTab = tp;

                if (!tp.Engine.CanAppend(tp))
                {
                    MessageBox.Show(tp.Text + @" using " + tp.Engine.EngineName + @" does not support pasting data", @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                if (tp.Engine.HasRulesFile && (string.IsNullOrWhiteSpace(tp.LoadedRulesFile) || !File.Exists(tp.LoadedRulesFile)))
                {
                    var rulesFile = RulesSelectForm.SelectRulesFile(string.Empty, tp.Engine);
                    tp.PLLoaded.Rules = tp.Engine.CreateRulesReader(tp);
                    tp.PLLoaded.Rules.LoadRulesFromFile(rulesFile);
                    tp.LoadedRulesFile = rulesFile;
                }
                
                // Normally if you paste something it's already decrypted, so no need to select a decryptor

                var cText = Clipboard.GetText().Replace("\r", "");

                using (var clipBoardMemoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(clipBoardMemoryStream, Encoding.UTF8, cText.Length, true))
                    {
                        writer.Write(cText);
                    }
                    clipBoardMemoryStream.Seek(0, SeekOrigin.Begin);

                    if (!tp.PLLoaded._parentTab.Engine.LoadFromStream(tp.PLLoaded, clipBoardMemoryStream, tp.LoadedLogFile, tp.LoadedRulesFile, tp.DecryptVersion))
                    {
                        MessageBox.Show(@"Error loading data from clipboard", @"Clipboard Paste Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tp.PLLoaded.Clear();
                        tcPackets.TabPages.Remove(tp);
                        return;
                    }
                }

                if (tp.PLLoaded.Count <= 0)
                {
                    MessageBox.Show(@"Clipboard contained no useful data.", @"Clipboard Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tcPackets.TabPages.Remove(tp);
                    return;
                }
                Text = defaultTitle + " - " + tp.LoadedLogFile;
                tp.PL.CopyFrom(tp.PLLoaded);
                tp.FillListBox();
                UpdateStatusBarAndTitle(tp);
            }
            catch (Exception x)
            {
                MessageBox.Show(@"Paste Failed, Exception: " + x.Message, @"Paste from Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BtnCopyRawSource_Click(object sender, EventArgs e)
        {
            PacketTabPage tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                MessageBox.Show(@"No Packet List selected", @"Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PacketData pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                MessageBox.Show(@"No Packet selected", @"Copy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cliptext = "";
            foreach (string s in pd.RawText)
            {
                // re-add the linefeeds
                if (cliptext != string.Empty)
                    cliptext += "\n";
                cliptext += s;
            }
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
            }
        }

        private void TcPackets_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Source: https://social.technet.microsoft.com/wiki/contents/articles/50957.c-winform-tabcontrol-with-add-and-close-button.aspx
            // Adapted to using resources and without the add button
            try
            {
                TabControl tabControl = (sender as TabControl);
                var tabPage = tabControl.TabPages[e.Index];
                var tabRect = tabControl.GetTabRect(e.Index);
                tabRect.Inflate(-2, -2);
                var closeImage = Properties.Resources.close_icon;
                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    // for tabs at the top/bottom
                    e.Graphics.DrawImage(closeImage,
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
                    TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                        tabRect, tabPage.ForeColor, TextFormatFlags.Left);
                }
                else
                if (tabControl.Alignment == TabAlignment.Left)
                {
                    // for tabs to the left
                    e.Graphics.DrawImage(closeImage,
                        tabRect.Left + (tabRect.Width - closeImage.Width) / 2,
                        tabRect.Top);
                    var tSize = e.Graphics.MeasureString(tabPage.Text, tabPage.Font);
                    e.Graphics.TranslateTransform(tabRect.Left + tabRect.Width, tabRect.Bottom);
                    e.Graphics.RotateTransform(-90);
                    var textBrush = new SolidBrush(tabPage.ForeColor);
                    e.Graphics.DrawString(tabPage.Text, tabPage.Font, textBrush, 0, -tabRect.Width - (tSize.Height / -4), StringFormat.GenericDefault);
                }
                else
                {
                    // If you want it on the right as well, you code it >.>
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        private void TcPackets_MouseDown(object sender, MouseEventArgs e)
        {
            // Process MouseDown event only till (tabControl.TabPages.Count - 1) excluding the last TabPage
            TabControl tabControl = (sender as TabControl);
            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                var tabRect = tabControl.GetTabRect(i);
                tabRect.Inflate(-2, -2);
                var closeImage = Properties.Resources.close_icon;
                Rectangle imageRect;
                if ((tabControl.Alignment == TabAlignment.Top) || (tabControl.Alignment == TabAlignment.Bottom))
                {
                    imageRect = new Rectangle(
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                        closeImage.Width,
                        closeImage.Height);
                }
                else
                {
                    imageRect = new Rectangle(
                        tabRect.Left + (tabRect.Width - closeImage.Width) / 2,
                        tabRect.Top,
                        closeImage.Width,
                        closeImage.Height);
                }
                if (imageRect.Contains(e.Location))
                {
                    tabControl.TabPages.RemoveAt(i);
                    break;
                }
            }
        }

        public void OpenBasicParseEditor(string parseFileName)
        {
            string editFile = Application.StartupPath + Path.DirectorySeparatorChar + parseFileName;
            if (!File.Exists(editFile))
            {
                if (MessageBox.Show($"Parser \"{parseFileName}\" doesn't exists, create one ?", @"Edit Parse File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                var s = "file;" + Path.GetFileNameWithoutExtension(parseFileName) + ";unnamed package";
                s += "\r\n\r\n";
                s += "rem;insert your parser fields here";
                try
                {
                    File.WriteAllText(editFile, s);
                }
                catch
                {
                    MessageBox.Show("Failed to create new parser file", "Edit Parse File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (Properties.Settings.Default.ExternalParseEditor)
            {
                Process.Start(editFile);
            }
            else
            {
                // Open in-app editor
                var editDlg = new ParseEditorForm(GetCurrentPacketTabPage());
                editDlg.LoadFromFile(editFile);
                editDlg.Show();
            }
        }

        public void OpenXmlRulesParseEditor(PacketRule rule)
        {
            if (rule == null)
            {
                MessageBox.Show("No rule attached to this packet, cannot edit", "Edit Parse File", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Open in-app editor, since we're editing a node, we can't reliably use a external editor for this
            var editDlg = new ParseEditorForm(GetCurrentPacketTabPage());
            editDlg.LoadFromRule(rule);
            editDlg.Show();
        }

        private void RtInfo_SelectionChanged(object sender, EventArgs e)
        {
            List<string> lookupResults = new List<string>();

            // Used for uint24 that doesn't support negative values
            void AddLabelUnsigned(string typeName, int hexWidth, UInt64 val)
            {
                var l = new Label();
                l.AutoSize = true;
                l.Text = typeName + ": 0x" + val.ToString("X"+hexWidth) + " - " + val.ToString();
                foreach(var ll in CurrentPP.PD.Parent._parentTab.Engine.DataLookups.LookupLists)
                {
                    if (ll.Value.Data.TryGetValue(val, out var v))
                        if (v.Id > 0)
                        {
                            var s = ll.Key + "(0x" + val.ToString("X") + ") => " + v.Val;
                            if (!lookupResults.Contains(s))
                                lookupResults.Add(s);
                        }
                }
                flpPreviewData.Controls.Add(l);
            }

            // Displays both signed and it's unsigned equivalent if they are different
            void AddLabelSigned(string typeName, int hexWidth, Int64 val)
            {
                var l = new Label();
                l.AutoSize = true;
                var h = val.ToString("X" + hexWidth);
                if (h.Length >= hexWidth)
                    h = h.Substring(h.Length - hexWidth, hexWidth);
                else
                    h = h.PadLeft(hexWidth, 'F');
                if (!Int64.TryParse(h, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var positiveVal))
                    positiveVal = val;

                if (positiveVal != val)
                    l.Text = typeName + ": 0x" + h + " - " + positiveVal + " (" + val.ToString() + ")";
                else
                    l.Text = typeName + ": 0x" + h + " - " + val.ToString();

                // Only lookups for negative values on signed
                if (val < 0)
                {
                    foreach (var ll in CurrentPP.PD.Parent._parentTab.Engine.DataLookups.LookupLists)
                    {
                        if (ll.Value.Data.TryGetValue((ulong)val, out var v))
                            if (v.Id > 0)
                            {
                                var s = ll.Key + "(" + val.ToString() + ") => " + v.Val;
                                if (!lookupResults.Contains(s))
                                    lookupResults.Add(s);
                            }
                    }
                }
                flpPreviewData.Controls.Add(l);
            }

            void AddLabel(string val)
            {
                var l = new Label();
                l.AutoSize = true;
                l.Text = val;
                flpPreviewData.Controls.Add(l);
            }

            var firstPos = rtInfo.SelectionStart;
            var line = rtInfo.GetLineFromCharIndex(firstPos);
            var lineFirst = rtInfo.GetFirstCharIndexFromLine(line);
            var linePos = firstPos - lineFirst;

            var rawPos = -1;
            var maxPos = 0;

            // Only calculate position if there is data
            if (CurrentPP?.PD?.RawBytes?.Count > 0)
            {
                maxPos = CurrentPP.PD.RawBytes.Count;
                if (!cbOriginalData.Checked)
                {
                    rawPos = ((line - 2) * 16);
                    if ((linePos >= 6) && (linePos < 58)) // normal hex view location
                    {
                        var off = linePos - 6;
                        var blockNumber = off / 12;
                        off -= blockNumber;
                        var p = off / 3;
                        if (p < 0)
                            p = 0;
                        if (linePos == 57)
                            p++;
                        rawPos += p;
                    }
                    else if ((linePos >= 63) && (linePos < 80)) // string hex view
                    {
                        var off = linePos - 63;
                        rawPos += off;
                    }
                }
                else
                {
                    // Viewing raw source data
                    rawPos = ((line - 1) * 16) + (linePos / 3);
                }
            }

            var cursorPosText = "Pos: " + line.ToString() + "," + linePos.ToString();
            if ((rawPos >= 0) && (rawPos < maxPos))
            {
                cursorPosText += " (0x" + rawPos.ToString("X2") + ")";
                var sizeLeft = CurrentPP.PD.RawBytes.Count - rawPos;

                flpPreviewData.Controls.Clear();

                if (sizeLeft >= 2)
                {
                    //AddLabelUnsigned("uint16",4,CurrentPP.PD.GetUInt16AtPos(rawPos));
                    AddLabelSigned("int16 ",4,CurrentPP.PD.GetInt16AtPos(rawPos));
                }
                if (sizeLeft >= 3)
                {
                    var rd = CurrentPP.PD.GetDataBytesAtPos(rawPos, 3).ToList();
                    rd.Add(0);
                    UInt32 d = BitConverter.ToUInt32(rd.ToArray(), 0);
                    AddLabelUnsigned("uint24", 6, d);
                }
                if (sizeLeft >= 4)
                {
                    //AddLabelUnsigned("uint32", 8, CurrentPP.PD.GetUInt32AtPos(rawPos));
                    AddLabelSigned("int32 ", 8, CurrentPP.PD.GetInt32AtPos(rawPos));
                    AddLabel("float : " + CurrentPP.PD.GetFloatAtPos(rawPos).ToString());
                    AddLabel("datetime: " + CurrentPP.PD.GetTimeStampAtPos(rawPos));
                }
                if (sizeLeft >= 8)
                {
                    //AddLabelUnsigned("uint64", 16, CurrentPP.PD.GetUInt64AtPos(rawPos));
                    AddLabelSigned("int64 ", 16, CurrentPP.PD.GetInt64AtPos(rawPos));
                    AddLabel("double: " + CurrentPP.PD.GetDoubleAtPos(rawPos).ToString());
                }

                foreach (var lr in lookupResults)
                    AddLabel(lr);
            }

            lRawViewPos.Text = cursorPosText;
        }

        private void MmVideoOpenLink_Click(object sender, EventArgs e)
        {
            if (VideoLinkForm.GetVLCLibPath() == string.Empty)
            {
                MessageBox.Show("VideoLAN VLC needs to be installed on your PC to use the video linking feature", "libvlc not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Application.UseWaitCursor = true;
            Cursor = Cursors.WaitCursor;
            VideoLinkForm videoLink = null;
            try
            {
                PacketTabPage thisTP = GetCurrentPacketTabPage();
                if ((thisTP != null) && (thisTP.VideoLink != null))
                {
                    thisTP.VideoLink.BringToFront();
                    Cursor = Cursors.Default;
                    Application.UseWaitCursor = false;
                    return;
                }
                // Create our virtualtime stamps now
                if (thisTP != null)
                {
                    // thisTP.PL.BuildVirtualTimeStamps();
                    thisTP.PLLoaded.BuildVirtualTimeStamps();
                }
                videoLink = new VideoLinkForm();
                videoLink.sourceTP = thisTP;
                videoLink.Show();
                videoLink.BringToFront();
                UpdateStatusBarAndTitle(thisTP);
            }
            catch (Exception x)
            {
                if (videoLink != null)
                    videoLink.Dispose();
                MessageBox.Show("Could not create video link, likely libvlc not correcty installed !\r\n" + x.Message, "Video Link Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
                Application.UseWaitCursor = false;
                return;
            }
            Cursor = Cursors.Default;
            Application.UseWaitCursor = false;
        }

        private void MmVideoViewProject_Click(object sender, EventArgs e)
        {
        }

        private void TcPackets_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is PacketTabPage)
            {
                PacketTabPage tp = (e.Control as PacketTabPage);
                if (tp.PLLoaded.Count > 0)
                    tp.SaveProjectFile();

                var gsp = tp.GetSelectedPacket();
                if ((CurrentPP != null) && (gsp != null) && (gsp.PP != null) && (gsp.PP == CurrentPP))
                {
                    CurrentPP = null;
                    dGV.Rows.Clear();
                    rtInfo.Clear();
                    lInfo.Text = "";
                    cbShowBlock.Visible = false;
                }
                try
                {
                    if (tp.VideoLink != null)
                        tp.VideoLink.Close();
                }
                catch { }

                if (tcPackets.TabCount <= 1)
                    Text = defaultTitle;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Properly draw everything before we start
            this.Refresh();
            // Handle arguments
            var args = Environment.GetCommandLineArgs().ToList();
            args.RemoveAt(0);
            foreach (string arg in args)
            {
                if (File.Exists(arg))
                {
                    // open log
                    TryOpenFile(arg);
                }
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            // trying to add some file dropping
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s == null)
                return;
            for (int i = 0; i < s.Length; i++)
            {
                if (File.Exists(s[i]))
                    TryOpenFile(s[i]);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // try deleting all created temp-files when closing
            foreach (var fn in AllUsedTempFiles)
                try
                {
                    File.Delete(fn);
                }
                catch
                {
                    // ignored
                }
        }

        private void MMExtraGameView_Click(object sender, EventArgs e)
        {
            if (GameViewForm.GV == null)
            {
                _ = new GameViewForm(GetCurrentPacketTabPage());
            }
            GameViewForm.GV?.Show();
            GameViewForm.GV?.BringToFront();
        }

        private void MmFile_Click(object sender, EventArgs e)
        {

        }

        private void MMFileProjectDetails_Click(object sender, EventArgs e)
        {
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
            {
                MessageBox.Show("You need to open a log file first before you can view it's project settings", "View Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(tp.ProjectFolder))
            {
                MessageBox.Show("Current tab is not project. You can only create a project from single files. Appended or pasted data does not generate a project!", "Not a project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var projectDlg = new ProjectInfoForm())
            {
                projectDlg.LoadFromPacketTapPage(tp);
                if (projectDlg.ShowDialog() == DialogResult.OK)
                {
                    projectDlg.ApplyPacketTapPage();
                    if (!tp.SaveProjectFile())
                    {
                        MessageBox.Show("Project file was NOT saved !\r\nEither you don't have write permission,\r\nor are not able to save the file because of restrictions placed by this program", "Project NOT saved", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MMExtraUpdateParser_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ParserDataUpdateZipURL == string.Empty)
            {
                MessageBox.Show("No update URL has been set, please go to program settings to set one up", "No update URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Do you want to download packet data ?\r\n" +
                "\r\n" +
                "This will update your lookup and parse data from the PVLV-Data repository on GitHub at \r\n" +
                Properties.Settings.Default.ParserDataUpdateZipURL + "\r\n" +
                "\r\n" +
                "Any changes you have made will be overwritten if you do.\r\n" +
                "This does NOT check for version updates of the program itself !\r\n" +
                "Also note that it is possible that this data is OLDER than your current one.",
                "Update data ?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            using (var loadform = new LoadingForm(this))
            {
                try
                {
                    if ((CompressForm.SevenZipDLLPath == null) || (CompressForm.SevenZipDLLPath == string.Empty))
                        CompressForm.SevenZipDLLPath = CompressForm.TryGet7ZipLibrary();

                    loadform.Text = "Updating data ...";
                    loadform.pb.Hide();
                    loadform.lTextInfo.Text = "Downloading ...";
                    loadform.lTextInfo.Show();
                    loadform.Show();
                    loadform.lTextInfo.Refresh();

                    System.Threading.Thread.Sleep(250);
                    // Delete the old download data if there
                    var localDataDir = Path.Combine(Application.StartupPath, "data");
                    var tempFile = Path.GetTempFileName();

                    Helpers.System.FileDownloader.DownloadFileFromURLToPath(Properties.Settings.Default.ParserDataUpdateZipURL, tempFile);

                    loadform.lTextInfo.Text = "Unpacking ...";
                    loadform.lTextInfo.Refresh();
                    System.Threading.Thread.Sleep(500);

                    var unzipper = new SevenZip.SevenZipExtractor(tempFile, SevenZip.InArchiveFormat.SevenZip);
                    var filelist = unzipper.ArchiveFileData;

                    loadform.pb.Minimum = 0;
                    loadform.pb.Maximum = filelist.Count;
                    loadform.pb.Step = 1;
                    loadform.pb.Show();

                    foreach (var fd in filelist)
                    {
                        // Skip directories
                        if ((fd.Attributes & 0x10) != 0)
                            continue;

                        try
                        {
                            var zippedName = fd.FileName;
                            var targetName = Path.Combine(localDataDir, zippedName);
                            var targetFileDir = Path.GetDirectoryName(targetName);
                            if (!Directory.Exists(targetFileDir))
                                Directory.CreateDirectory(targetFileDir);
                            var fs = File.Create(targetName);
                            unzipper.ExtractFile(fd.FileName, fs);
                            fs.Close();
                            loadform.pb.PerformStep();
                            System.Threading.Thread.Sleep(25);
                        }
                        catch (Exception x)
                        {
                            if (MessageBox.Show("Exception extracting file:\r\n" + x + "\r\n" + fd + "\r\nDo you want to continue ?", "Exception", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                            {
                                break;
                            }
                        }
                    }
                    unzipper.Dispose();
                    loadform.pb.Hide();

                    loadform.lTextInfo.Text = "Done ...";
                    loadform.lTextInfo.Refresh();
                    System.Threading.Thread.Sleep(1000);
                    File.Delete(tempFile);

                    MessageBox.Show("Done downloading and unpacking data from \r\n" +
                        Properties.Settings.Default.ParserDataUpdateZipURL + "\r\n\r\n" +
                        "Some changes will only be visible after you restart the program.", "Update data", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                catch (Exception x)
                {
                    MessageBox.Show("Exception updating:\r\n" + x.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MMAbout7ZipMain_Click(object sender, EventArgs e)
        {
            Process.Start(Url7Zip);
        }

        private void MMAbout7ZipDownload_Click(object sender, EventArgs e)
        {
            Process.Start(Url7ZipRequiredVer);
        }

        private void mmExtraExportPacketsAsCSV_Click(object sender, EventArgs e)
        {
            PacketTabPage thisTP = GetCurrentPacketTabPage();
            if (thisTP == null)
                return;

            if (!thisTP.PL.IsPreParsed)
            {
                MessageBox.Show("This function requires the pre-parse setting to be enabled","Export to CSV",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return ;
            }

            saveCSVFileDialog.FileName = Path.GetFileNameWithoutExtension(thisTP.ProjectFile) + ".csv";

            if (saveCSVFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ExportCSVHelper.ExportPacketToCSV(thisTP.PL, saveCSVFileDialog.FileName))
                    MessageBox.Show("Exported as:\r\n" + saveCSVFileDialog.FileName, "Export CSV",MessageBoxButtons.OK,MessageBoxIcon.Information);
                else
                    MessageBox.Show("Export failed !", "Export CSV", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void splitContainer3_Resize(object sender, EventArgs e)
        {
            var maxRawSizeWidth = (int)splitContainer3.CreateGraphics().MeasureString(InfoGridHeader, Properties.Settings.Default.RawViewFont).Width + 60 ;
            splitContainer3.SplitterDistance = Math.Min(maxRawSizeWidth, splitContainer3.SplitterDistance);
            splitContainer3.Panel2Collapsed = (splitContainer3.Width  < (maxRawSizeWidth + 100));
        }

        private void splitContainer3_Panel1_Resize(object sender, EventArgs e)
        {
            var maxRawSizeWidth = (int)splitContainer3.CreateGraphics().MeasureString(InfoGridHeader, Properties.Settings.Default.RawViewFont).Width + 60;
            if (splitContainer3.SplitterDistance > maxRawSizeWidth)
                splitContainer3.SplitterDistance = maxRawSizeWidth;
        }
        
        private void MMToolsRun_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi)
            {
                var currentTabPage = GetCurrentPacketTabPage();
                currentTabPage?.Engine?.RunTool(currentTabPage, mi.Text);
            }
        }

        private void mmTools_DropDownOpening(object sender, EventArgs e)
        {
            // Populate the tools menu
            mmTools.DropDownItems.Clear();
            var tp = GetCurrentPacketTabPage();
            if (tp == null)
                return;
            
            foreach (var toolName in tp?.Engine.ToolNamesList)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(toolName);
                mi.Click += MMToolsRun_Click;
                mmTools.DropDownItems.Add(mi);
            }
        }
    }
}
