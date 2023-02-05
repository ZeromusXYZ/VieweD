using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VieweD.Engine;
using VieweD.Engine.Common;
using VieweD.Helpers.System;

namespace VieweD.Forms
{
    public partial class ProjectInfoForm : Form
    {
        PacketTabPage tp ;
        int lastTagId ;
        private string currentArchive = string.Empty;

        private const string WindingsNotOk = "\xCE";
        private const string WindingsIsOk = "\x81";
        
        public ProjectInfoForm()
        {
            InitializeComponent();
        }

        private void AddTag(string name)
        {
            if (name.Trim(' ') == string.Empty)
                return;
            lastTagId++;
            var label = new Label();
            label.Tag = lastTagId;
            label.BorderStyle = BorderStyle.Fixed3D;
            label.BackColor = SystemColors.Highlight;
            label.ForeColor = SystemColors.HighlightText;
            tagContainer.Controls.Add(label);
            label.Text = name;
            label.AutoSize = true;
            label.Cursor = Cursors.No;
            label.Click += LTagLabel_Click;
        }

        private void ClearTags()
        {
            for(int i = tagContainer.Controls.Count-1; i >= 0; i--)
            {
                Label c = (tagContainer.Controls[i] is Label) ? (tagContainer.Controls[i] as Label) : null;
                if ((c?.Tag != null) && ((int)c.Tag > 0))
                {
                    tagContainer.Controls.RemoveAt(i);
                }
            }
        }

        private void CreateVisualTags(string tagString)
        {
            ClearTags();
            var tags = tagString.Split(',').ToList();
            foreach(string t in tags)
            {
                var s = t.Trim(' ');
                AddTag(s);
            }
        }

        private string VisualTagsToString(string spacer = ",")
        {
            string res = string.Empty;
            foreach(Control c in tagContainer.Controls)
            {
                if ( (c is Label label) && (label.Tag != null) && ((int)label.Tag > 0) )
                {
                    if (res != string.Empty)
                        res += spacer;
                    res += label.Text;
                }
            }
            return res;
        }

        private void AddLogOption(string fullDir)
        {
            var shortDir = Helper.MakeRelative(tp.ProjectFolder,fullDir);
            cbOpenedLog.Items.Add(shortDir);
        }

        private void PopulateOpenedLogDropDownList()
        {
            cbOpenedLog.Items.Clear();
            AddLogOption(tp.LoadedLogFile); // Current Log File
            cbOpenedLog.Text = tp.LoadedLogFile;
            List<string> files = new List<string>();
            foreach (var ext in tp.Engine.FileExtensions)
            {
                var eFiles = Directory.GetFiles(tp.ProjectFolder, "*" + ext.Key, SearchOption.AllDirectories);
                files.AddRange(eFiles);
            }

            foreach (var f in files)
            {
                if (!Path.GetFileName(f).ToLower().StartsWith("0x"))
                    AddLogOption(f);
            }
        }

        public void LoadFromPacketTapPage(PacketTabPage sourceTp)
        {
            tp = sourceTp;

            cbAADecryptor.Items.Clear();

            if (tp != null)
            {
                if (!Directory.Exists(tp.ProjectFolder))
                {
                    MessageBox.Show(@"This is not a project !");
                    DialogResult = DialogResult.Abort;
                    return;
                }
                // Try to load engine by file name
                if (File.Exists(tp.LoadedLogFile))
                    tp.Engine = Engines.GetEngineByFileName(tp.LoadedLogFile);
                else
                {
                    tp.Engine = EngineSelectForm.SelectEngine(false);
                }

                // Safety check
                if (tp.Engine == null)
                {
                    tp.Engine = new EngineBase(tp);
                }

                lEngineName.Text = tp.Engine.EngineName ?? "None";

                // Populate decryptors list
                foreach (var d in tp.Engine.DecryptionHandlerList)
                    cbAADecryptor.Items.Add(d);
                
                cbAADecryptor.Sorted = true;
                cbAADecryptor.Enabled = tp.Engine.HasDecrypt;
                tRulesFile.Enabled = tp.Engine.HasRulesFile;
                btnChangeRules.Enabled = tp.Engine.HasRulesFile;
                
                CreateVisualTags(tp.ProjectTags);
                
                tTagBox.Text = "";
                tProjectFolder.Text = tp.ProjectFolder;
                cbOpenedLog.Text = tp.LoadedLogFile;
                tSourceVideo.Text = tp.LinkVideoFileName;
                tVideoURL.Text = tp.LinkYoutubeUrl;
                tRulesFile.Text = tp.LoadedRulesFile;
                tPackedLogsURL.Text = tp.LinkPacketsDownloadUrl;

                PopulateOpenedLogDropDownList();

                var decryptVersion = tp.DecryptVersion;
                if (ValidateRulesFile(Helper.TryMakeFullPath(tp.ProjectFolder,tRulesFile.Text), ref decryptVersion))
                    tp.DecryptVersion = decryptVersion;
                SelectDecryptorByVersion(tp);
                
                gbProjectInfo.Text = @"Project Information: " + Path.GetFileName(tp.ProjectFile);
            }
        }

        private bool ValidateRulesFile(string fileName, ref string decryptVersion)
        {
            var res = false;
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                var xmlData = File.ReadAllText(fileName, Encoding.UTF8);
                var doc = new XmlDocument();
                doc.Load(new StringReader(xmlData));
                /*
                var testNode = doc.SelectSingleNode("/root/rule/s2c/packet/data"); // select something at random that should always be in there
                if (testNode?.Attributes?["type"] != null)
                {
                    res = true;
                }
                */
                var testNode = doc.SelectSingleNode("/root/rule/s2c"); // select something at random that should always be in there
                if (testNode != null)
                {
                    res = true;
                }
                // Check if client version present
                var versionNode = doc.SelectSingleNode("/root/version"); // select something at random that should always be in there
                if (versionNode != null)
                {
                    var clientName = versionNode.Attributes?["client"].Value;
                    // If it's a supported version, select it
                    if (!string.IsNullOrEmpty(clientName) && tp.Engine.DecryptionHandlerList.Contains(clientName))
                        decryptVersion = clientName;
                }
            }
            catch
            {
                res = false;
            }


            return res;
        }

        private void SelectDecryptorByVersion(PacketTabPage tabPage)
        {
            foreach (var i in cbAADecryptor.Items)
                if ((string)i == tabPage.DecryptVersion)
                {
                    cbAADecryptor.SelectedItem = i;
                    break;
                }
        }
        
        public void ApplyPacketTapPage()
        {
            if (tp != null)
            {
                tp.ProjectTags = VisualTagsToString();
                tp.ProjectFolder = tProjectFolder.Text;
                if (File.Exists(tProjectFolder.Text + cbOpenedLog.Text))
                    tp.LoadedLogFile = tProjectFolder.Text + cbOpenedLog.Text;
                else
                    tp.LoadedLogFile = cbOpenedLog.Text;
                tp.LinkVideoFileName = tSourceVideo.Text;
                tp.LinkYoutubeUrl = tVideoURL.Text;
                tp.LinkPacketsDownloadUrl = tPackedLogsURL.Text;
                tp.LoadedRulesFile = tRulesFile.Text;
                tp.DecryptVersion = tp.Engine.DecryptionHandlerList.Contains(cbAADecryptor.Text) ? cbAADecryptor.Text : "_None" ;
            }
        }

        private void ProjectInfoForm_Load(object sender, EventArgs e)
        {
            tcProjectInfo.SelectedTab = tpMainInfo;
            // Populate Autocomplete for tags
            tTagBox.AutoCompleteCustomSource.Clear();
            if (tp != null)
            {
                tTagBox.AutoCompleteCustomSource.AddRange(tp.Engine.DataLookups.AllValues.ToArray());
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnAddTag_Click(object sender, EventArgs e)
        {
            AddTag(tTagBox.Text);
            tTagBox.Text = "";
            tTagBox.Focus();
        }

        private void TTagBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                BtnAddTag_Click(null,null);
            }
        }

        private void LTagLabel_Click(object sender, EventArgs e)
        {
            if ((sender is Label label) && ((int)label.Tag > 0))
            {
                string oldTag = label.Text;
                tagContainer.Controls.Remove(label);
                tTagBox.Text = oldTag;
            }
        }

        private void ProjectInfoForm_Shown(object sender, EventArgs e)
        {
            //tTagBox.Focus();
            btnSave.Focus();
        }

        internal class InlineProgress : IProgress<double>, IDisposable
        {
            public readonly LoadingForm Loading;

            public InlineProgress()
            {
                Loading = new LoadingForm(MainForm.ThisMainForm);
                Loading.Text = @"Downloading ... ";
                Loading.pb.Minimum = 0;
                Loading.pb.Maximum = 10000;
                Loading.pb.Value = 0;
                Loading.Show();
            }

            public void Report(double progress)
            {
                var newVal = (int)Math.Round(progress * 10000f);
                Console.WriteLine($"{progress}%");
                // Don't bother updating on low progress
                if (newVal <= Loading.pb.Value)
                    return;
                if (Loading.InvokeRequired)
                {
                    Loading.Invoke(new MethodInvoker(delegate
                    {
                        Loading.pb.Value = newVal;
                        Loading.pb.Refresh();
                    }));
                }
                else
                {
                    Loading.pb.Value = newVal;
                    Loading.pb.Refresh();
                }
            }

            public void Dispose()
            {
                Loading.Dispose();
            }
        }

        private void BtnDownloadYoutube_Click(object sender, EventArgs e)
        {
            tVideoURL.Enabled = false;
            btnDownloadVideo.Enabled = false;
            bgwDownloads.RunWorkerAsync();
        }

        private void DoVideoDownload()
        { 
            if (tVideoURL.Text != string.Empty)
            {
                var fName = tSourceVideo.Text;

                if (fName == string.Empty)
                    fName = Path.ChangeExtension(cbOpenedLog.Text, ".mp4");

                if (Path.GetDirectoryName(fName) == string.Empty)
                    fName = Path.Combine(tp.ProjectFolder, fName);


                if (File.Exists(fName))
                {
                    MessageBox.Show(@"Already found a valid local video for this project, no download required.", @"No download needed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var targetFile = Helper.DownloadFileFromURL(tVideoURL.Text, fName);
                if (!string.IsNullOrEmpty(targetFile) && File.Exists(targetFile))
                {
                    tp.LinkVideoFileName = targetFile;
                    this.Invoke(new MethodInvoker(delegate
                    {
                        tSourceVideo.Text = targetFile;
                    }));
                    MessageBox.Show($"VideoLinkFileName updated to: {targetFile}");
                }
            }
        }

        private void BtnDownloadSource_Click(object sender, EventArgs e)
        {
            if (tPackedLogsURL.Text != string.Empty)
            {
                Application.UseWaitCursor = true;
                Cursor = Cursors.WaitCursor;
                var archiveFileName = Path.ChangeExtension(tp.ProjectFile, ".7z");

                var dlFile = Helper.DownloadFileFromURL(tPackedLogsURL.Text, archiveFileName);
                if (!string.IsNullOrEmpty(dlFile))
                {
                    var dlExt = Path.GetExtension(dlFile).ToLower();
                    if (dlExt != Path.GetExtension(archiveFileName).ToLower())
                    {
                        var oldFileName = archiveFileName;
                        archiveFileName = Path.ChangeExtension(archiveFileName, dlExt);
                        if (File.Exists(archiveFileName))
                            File.Delete(archiveFileName);
                        File.Move(oldFileName, archiveFileName);
                    }
                }

                if (!File.Exists(archiveFileName))
                {
                    MessageBox.Show($"Error downloading file !\r\n{archiveFileName}", @"Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ProjectInfo_TextChanged(null, null);
                Cursor = Cursors.Default;
                Application.UseWaitCursor = false;
            }
        }

        private void ProjectInfo_TextChanged(object sender, EventArgs e)
        {
            bool res = true;

            // Online button stuff
            btnDownloadSource.Enabled = Helper.GuessURLType(tPackedLogsURL.Text) != DownloadURLType.Invalid;
            var ut = Helper.GuessURLType(tVideoURL.Text);
            btnDownloadVideo.Enabled = (!bgwDownloads.IsBusy && ((ut == DownloadURLType.YouTube) || (ut == DownloadURLType.MEGA)));

            // Project Folder
            var pUpOneDirName = tProjectFolder.Text.TrimEnd(Path.DirectorySeparatorChar);
            if ((tp.ProjectFile == string.Empty) || !File.Exists(tp.ProjectFile))
            {
                tp.ProjectFile = Path.Combine(pUpOneDirName, Path.GetFileNameWithoutExtension(pUpOneDirName) + ".pvd");
            }
            var pLocalDirName = Path.Combine(Path.GetDirectoryName(tp.ProjectFile) ?? string.Empty, Path.GetFileNameWithoutExtension(tp.ProjectFile));
            if (Directory.Exists(pUpOneDirName))
            {
                lProjectFolderOK.Text = WindingsIsOk ;
                lProjectFolderOK.ForeColor = Color.LimeGreen;
                btnMake7zip.Enabled = true;

                if (File.Exists(pLocalDirName + ".7z"))
                {
                    currentArchive = pLocalDirName + ".7z";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pLocalDirName + ".zip"))
                {
                    currentArchive = pLocalDirName + ".zip";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pLocalDirName + ".rar"))
                {
                    currentArchive = pLocalDirName + ".rar";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pUpOneDirName + ".7z"))
                {
                    currentArchive = pUpOneDirName + ".7z";
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pUpOneDirName + ".zip"))
                {
                    currentArchive = pUpOneDirName + ".zip" ;
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                if (File.Exists(pUpOneDirName + ".rar"))
                {
                    currentArchive = pUpOneDirName + ".rar" ;
                    btnExtractZip.Enabled = true;
                    btnMake7zip.Enabled = false;
                }
                else
                {
                    currentArchive = string.Empty;
                    btnExtractZip.Enabled = false;
                }

            }
            else
            {
                lProjectFolderOK.Text = WindingsNotOk ;
                lProjectFolderOK.ForeColor = Color.Red;
                btnMake7zip.Enabled = false;
                btnExtractZip.Enabled = false;
                currentArchive = string.Empty;
                res = false;
            }

            lCurrentArchiveName.Text = currentArchive;
            if ((currentArchive == string.Empty) || (!File.Exists(currentArchive)))
            {
                lCurrentArchiveName.ForeColor = Color.Red;
                lCurrentArchiveName.Cursor = Cursors.Default;
            }
            else
            {
                lCurrentArchiveName.ForeColor = SystemColors.ControlText;
                lCurrentArchiveName.Cursor = Cursors.Hand;
            }

            // Attached Log file
            if (File.Exists(cbOpenedLog.Text))
            {
                lOpenedLogOK.Text = WindingsIsOk;
                lOpenedLogOK.ForeColor = Color.LimeGreen;
                // Disable download/extract when we have a valid file
                btnExtractZip.Enabled = false;
                btnDownloadSource.Enabled = false;
            }
            else
            if (File.Exists(tProjectFolder.Text + cbOpenedLog.Text))
            {
                lOpenedLogOK.Text = WindingsIsOk;
                lOpenedLogOK.ForeColor = Color.LimeGreen;
                // Disable download/extract when we have a valid file
                btnExtractZip.Enabled = false;
                btnDownloadSource.Enabled = false;
            }
            else
            {
                lOpenedLogOK.Text = WindingsNotOk;
                lOpenedLogOK.ForeColor = Color.Red;
                res = false;
                // Disable zip creating if we don't have a valid file we could open
                btnMake7zip.Enabled = false;
            }

            // Linked Local Video
            if ( (tSourceVideo.Text == string.Empty) || (File.Exists(tSourceVideo.Text)) )
            {
                lVideoSourceOK.Text = WindingsIsOk;
                lVideoSourceOK.ForeColor = Color.LimeGreen;
            }
            else
            {
                lVideoSourceOK.Text = WindingsNotOk;
                lVideoSourceOK.ForeColor = Color.Red;
            }

            // Rules File
            var aaVer = "_None_";
            var fnRules = Helper.TryMakeFullPath(tp.ProjectFolder, tRulesFile.Text);
            if ((tRulesFile.Text == string.Empty) || ((File.Exists(fnRules) && ValidateRulesFile(fnRules, ref aaVer) )))
            {
                lRulesFileOK.Text = WindingsIsOk;
                lRulesFileOK.ForeColor = Color.LimeGreen;
                tp.DecryptVersion = aaVer;
                SelectDecryptorByVersion(tp);
            }
            else
            {
                lRulesFileOK.Text = WindingsNotOk;
                lRulesFileOK.ForeColor = Color.Red;
            }

            btnSave.Enabled = res;
        }


        private void BtnCopySummary_Click(object sender, EventArgs e)
        {
            string clipText = "";
            clipText += "Name: " + Path.GetFileNameWithoutExtension(tp.ProjectFile) + "\n" ;
            if (tPackedLogsURL.Text != string.Empty)
                clipText += "> Logs: <" + tPackedLogsURL.Text + ">\n";
            if (tVideoURL.Text != string.Empty)
                clipText += "> Video: " + tVideoURL.Text + "\n";
            var t = VisualTagsToString(", ");
            if (t != string.Empty)
                clipText += "> Tags: " + t + "\n";
            try
            {
                // Because nothing is ever as simple as the next line >.>
                // Clipboard.SetText(s);
                // Helper will (try to) prevent errors when copying to clipboard because of threading issues
                var clipHelp = new SetClipboardHelper(DataFormats.Text, clipText)
                {
                    DontRetryWorkOnFailed = false
                };
                clipHelp.Go();
            }
            catch
            {
                // Ignore
            }

        }

        private bool ExploreFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            //Clean up file path so it can be navigated OK
            filePath = Path.GetFullPath(filePath);
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            return true;
        }

        private void BtnMake7zip_Click(object sender, EventArgs e)
        {
            using(var zipForm = new CompressForm())
            {
                zipForm.task = CompressForm.ZipTaskType.doZip;
                string aName;
                if (tp.ProjectFile != string.Empty)
                {
                    aName = Path.GetFileNameWithoutExtension(tp.ProjectFile) + ".7z";
                }
                else
                {
                    aName = Path.GetFileNameWithoutExtension(tp.ProjectFolder.TrimEnd(Path.DirectorySeparatorChar)) + ".7z";
                }
                zipForm.ArchiveFileName = Path.Combine(tp.ProjectFolder,aName);
                
                if (zipForm.BuildArchieveFilesList(tProjectFolder.Text) <= 0)
                {
                    MessageBox.Show(@"Nothing to add", @"Make .7z", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (zipForm.ShowDialog() == DialogResult.OK)
                {
                    ProjectInfo_TextChanged(null, null);
                    if (!ExploreFile(zipForm.ArchiveFileName))
                        MessageBox.Show(zipForm.ArchiveFileName + @"not found !");
                }
                else
                {
                    try
                    {
                        if (File.Exists(zipForm.ArchiveFileName))
                            File.Delete(zipForm.ArchiveFileName);
                    }
                    catch
                    {
                        // ignored
                    }

                    MessageBox.Show(@"Error creating " + zipForm.ArchiveFileName, @"Make .7z", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExtractZip_Click(object sender, EventArgs e)
        {
            using (var zipForm = new CompressForm())
            {
                zipForm.task = CompressForm.ZipTaskType.doUnZip;
                zipForm.ArchiveFileName = currentArchive;
                zipForm.ProjectName = Path.GetFileNameWithoutExtension(tp.ProjectFile);

                if (zipForm.ShowDialog() == DialogResult.OK)
                {
                    // reload the project file after extraction
                    var oldDownloadUrl = tp.LinkPacketsDownloadUrl;
                    var oldFile = tp.ProjectFile;
                    tp.LoadProjectFile(oldFile);
                    LoadFromPacketTapPage(tp);
                    tp.LinkPacketsDownloadUrl = oldDownloadUrl;
                    tPackedLogsURL.Text = oldDownloadUrl;
                    ProjectInfo_TextChanged(null, null);
                    MessageBox.Show(@"Done extracting " + zipForm.ArchiveFileName, @"Extract Archive", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (MessageBox.Show($"Error extracting {zipForm.ArchiveFileName} + \r\nDo you want to open the file in another program instead ?", @"Extract Archive", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        ExploreFile(zipForm.ArchiveFileName);
                    }
                }
            }
        }

        private void LCurrentArchiveName_Click(object sender, EventArgs e)
        {
            ExploreFile(currentArchive);
        }

        private void ProjectInfoForm_Enter(object sender, EventArgs e)
        {
            // Do Nothing
        }

        private void ProjectInfoForm_Activated(object sender, EventArgs e)
        {
            ProjectInfo_TextChanged(null, null);
        }

        private void BtnUploadToYoutube_Click(object sender, EventArgs e)
        {
            // Use Google's Youtube API somehow ?
        }

        private void bgwDownloads_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do Downloading
            DoVideoDownload();
        }

        private void bgwDownloads_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                ProjectInfo_TextChanged(null, null);
            }));
        }

        private void btnChangeRules_Click(object sender, EventArgs e)
        {
            var rulesFile = RulesSelectForm.SelectRulesFile(cbOpenedLog.Text, tp.Engine);
            if (File.Exists(rulesFile) && (rulesFile != tRulesFile.Text))
            {
                tRulesFile.Text = rulesFile;
                MessageBox.Show($"Rules file has been changed to\r\n{rulesFile}\n\nYou need to re-open the project for changes to take place", @"Rules Changed", MessageBoxButtons.OK);
            }
        }
    }
}
