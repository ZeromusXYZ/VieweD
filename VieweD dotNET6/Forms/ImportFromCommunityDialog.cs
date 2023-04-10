using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SharpCompress.Archives;
using VieweD.engine.common;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class ImportFromCommunityDialog : Form
    {
        // create some vars
        private string ProjectFile { get; set; } = string.Empty;
        private string ProjectArchive { get; set; } = string.Empty;
        private string ProjectVideo { get; set; } = string.Empty;
        private string ProjectFolder { get; set; } = string.Empty;
        private string ProjectArchiveUrl { get; set; } = string.Empty;
        private string ProjectVideoUrl { get; set; } = string.Empty;
        private string ProjectSaveVideoUrl { get; set; } = string.Empty;
        private string ProjectTitle { get; set; } = string.Empty;

        public ImportFromCommunityDialog()
        {
            InitializeComponent();
        }

        enum LabelTaskState
        {
            Blank,
            Busy,
            Complete,
            Failed,
            Skipped
        }

        private void TaskLabelUpdate(Label l, LabelTaskState state)
        {
            void MethodInvokerDelegate()
            {
                switch (state)
                {
                    case LabelTaskState.Blank:
                        l.Text = @" ";
                        l.ForeColor = SystemColors.ControlText;
                        break;
                    case LabelTaskState.Busy:
                        l.Text = @">>";
                        l.ForeColor = Color.Navy;
                        break;
                    case LabelTaskState.Complete:
                        l.Text = @"√";
                        l.ForeColor = Color.Green;
                        break;
                    case LabelTaskState.Failed:
                        l.Text = @"fail";
                        l.ForeColor = Color.Red;
                        break;
                    case LabelTaskState.Skipped:
                        l.Text = @"skip";
                        l.ForeColor = SystemColors.GrayText;
                        break;
                    default:
                        l.Text = @"?";
                        l.ForeColor = Color.Red;
                        break;
                }
            }

            //This will be true if Current thread is not UI thread.
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)MethodInvokerDelegate);
            else
                MethodInvokerDelegate();
        }

        /// <summary>
        /// Generates import data from (clipboard) text
        /// </summary>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        public bool ImportFromCommunityPost(string sourceText)
        {
            BtnStart.Enabled = false;
            BtnSkipVideo.Enabled = false;
            TaskLabelUpdate(LabelTaskDownloadArchive, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskSaveProject, LabelTaskState.Blank);

            #region analyze_clipboard

            // url, full line, expected type (0 = unknown, 1=packets, 2=video)
            List<(string, string, byte)> urlLines = new();

            sourceText = sourceText.Replace("\r", "");
            var lines = sourceText.Split("\n");
            ProjectArchiveUrl = string.Empty;
            ProjectVideoUrl = string.Empty;
            ProjectTitle = string.Empty;
            foreach (var line in lines)
            {
                var urls = Regex.Matches(line,
                    @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");
                if (urls.Count <= 0)
                {
                    if (string.IsNullOrWhiteSpace(ProjectTitle))
                        ProjectTitle = line;
                    continue;
                }

                var url = string.Empty;
                foreach (Match match in urls)
                {
                    url = match.Value;
                }

                byte lineType = 0;
                if (line.Contains("video", StringComparison.InvariantCultureIgnoreCase))
                    lineType = 2;
                if (line.Contains("packet", StringComparison.InvariantCultureIgnoreCase))
                    lineType = 1;
                if (line.Contains("logs", StringComparison.InvariantCultureIgnoreCase))
                    lineType = 1;

                if (url.Contains("youtu.be", StringComparison.InvariantCultureIgnoreCase))
                    lineType = 2;
                if (url.Contains("youtube", StringComparison.InvariantCultureIgnoreCase))
                    lineType = 2;

                urlLines.Add((url, line, lineType));
            }

            // First grab from already guessed data
            foreach (var urlLine in urlLines)
            {
                if (urlLine.Item3 == 1)
                    ProjectArchiveUrl = urlLine.Item1;
                if (urlLine.Item3 == 2)
                    ProjectVideoUrl = urlLine.Item1;
            }

            // Next go over all remaining urls, to try and fill the gaps
            foreach (var urlLine in urlLines)
            {
                if (urlLine.Item3 != 0)
                    continue; // If already guessed, skip

                if (string.IsNullOrWhiteSpace(ProjectVideoUrl) && (urlLines.Count > 1))
                {
                    ProjectVideoUrl = urlLine.Item1;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(ProjectArchiveUrl))
                {
                    ProjectArchiveUrl = urlLine.Item1;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(ProjectArchiveUrl) && !string.IsNullOrWhiteSpace(ProjectVideoUrl))
                    break;
            }
            #endregion

            if (string.IsNullOrWhiteSpace(ProjectArchiveUrl))
            {
                MessageBox.Show("No archive urls found on the clipboard", Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            LabelTitle.Text = ProjectTitle;
            LabelArchive.Text = ProjectArchiveUrl;
            if (string.IsNullOrWhiteSpace(ProjectVideoUrl))
            {
                LabelVideo.Text = @"<no video>";
                TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Skipped);
            }
            else
            {
                LabelVideo.Text = ProjectVideoUrl;
                BtnSkipVideo.Enabled = true;
            }

            ProjectSaveVideoUrl = ProjectVideoUrl;

            // strip all non-alpha characters
            var suggestedProjectName = Regex.Replace(ProjectTitle, @"[^a-zA-Z0-9 (){}+-]", "");
            if (suggestedProjectName.Length > 64)
                suggestedProjectName = suggestedProjectName[..64];

            // Set default import folder if available
            if (!string.IsNullOrWhiteSpace(Settings.Default.DefaultImportFolder))
                ImportFileDialog.InitialDirectory = Settings.Default.DefaultImportFolder;
            ImportFileDialog.FileName = suggestedProjectName + ".pvd";

            BtnSelectTarget.Enabled = true;

            /*
            // Show import summary of what is found
            var res = MessageBox.Show($"Import from community post using the following settings?\n\n" +
                                      $"Title:\n{ProjectTitle}\n\n" +
                                      $"Archive:\n{ProjectArchiveUrl}\n\n" +
                                      $"Video:\n{ProjectVideoUrl}", "Import from community post",
                MessageBoxButtons.YesNo);

            if (res != DialogResult.Yes)
                return false;
            */
            return true;
        }

        private void ImportFromCommunityDialog_Load(object sender, EventArgs e)
        {

        }

        private void BtnSelectTarget_Click(object sender, EventArgs e)
        {
            // strip all non-alpha characters
            var suggestedProjectName = Regex.Replace(ProjectTitle, @"[^a-zA-Z0-9 (){}+-]", "");
            if (suggestedProjectName.Length > 64)
                suggestedProjectName = suggestedProjectName[..64];

            // Set default import folder if available
            if (!string.IsNullOrWhiteSpace(Settings.Default.DefaultImportFolder))
                ImportFileDialog.InitialDirectory = Settings.Default.DefaultImportFolder;

            // Select where to save the import
            ImportFileDialog.FileName = suggestedProjectName + ".pvd";
            if (ImportFileDialog.ShowDialog() != DialogResult.OK)
                return;

            // Set some vars
            ProjectFile = ImportFileDialog.FileName;
            ProjectArchive = Path.ChangeExtension(ProjectFile, ".zip");
            ProjectVideo = Path.ChangeExtension(ProjectFile, ".mp4");
            ProjectFolder = Path.GetDirectoryName(ProjectFile) ?? "";

            LabelImportTarget.Text = ProjectFile;
            BtnStart.Enabled = true;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            BtnSelectTarget.Enabled = false;
            BtnCancel.Enabled = false;
            BtnStart.Enabled = false;
            BtnSkipVideo.Enabled = false;
            bgwImport.RunWorkerAsync();
        }

        private void bgwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            TaskLabelUpdate(LabelTaskDownloadArchive, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Blank);
            TaskLabelUpdate(LabelTaskSaveProject, LabelTaskState.Blank);

            // Check if the target folder is empty
            var checkFiles = Directory.GetFiles(ProjectFolder, "*.*", SearchOption.TopDirectoryOnly);
            if (checkFiles.Length > 0)
            {
                if (MessageBox.Show(
                        $"Target directory already contains files, are you sure you want to continue?\n{ProjectFolder}",
                        Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    BtnSelectTarget.Enabled = true; // Re-enable the target button to allow a new selection
                    BtnCancel.Enabled = true;
                    return;
                }
            }

            TaskLabelUpdate(LabelTaskDownloadArchive, LabelTaskState.Busy);

            // Download archive (required)
            using (var downloadDialog = new DownloadDialog())
            {
                downloadDialog.SetDownloadJob(ProjectArchiveUrl, ProjectArchive, "Download project archive");
                if (downloadDialog.BeginDownload() != DialogResult.OK)
                {
                    MessageBox.Show(string.Format(Resources.DownloadFileError, ProjectArchive),
                        "Download project archive",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Re-enable buttons for retry
                    BtnSelectTarget.Enabled = true;
                    BtnStart.Enabled = true;
                    BtnCancel.Enabled = true;
                    TaskLabelUpdate(LabelTaskDownloadArchive, LabelTaskState.Failed);
                    return;
                }
                ProjectArchive = downloadDialog.TargetFile;
            }

            TaskLabelUpdate(LabelTaskDownloadArchive, LabelTaskState.Complete);
            TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Busy);

            // Try to unpack the archive
            try
            {
                var stripBeginningPath = string.Empty;
                var firstFile = true;

                using (var archive = ArchiveFactory.Open(ProjectArchive))
                {
                    // Check if there are "empty folders" in the archive tree
                    foreach (var zipArchiveEntry in archive.Entries)
                    {
                        // Might seem weird, but for extracting directory simplification, we skip checking directories
                        if (zipArchiveEntry.IsDirectory)
                            continue;

                        var aFolder = Path.GetDirectoryName(zipArchiveEntry.Key) ?? "";
                        aFolder = aFolder.Replace('\\', '/').Replace('/', Path.DirectorySeparatorChar);
                        var aDirs = aFolder.Split(Path.DirectorySeparatorChar);

                        if (firstFile)
                        {
                            stripBeginningPath = aFolder;
                            firstFile = false;
                        }

                        stripBeginningPath = stripBeginningPath.Replace('\\', '/')
                            .Replace('/', Path.DirectorySeparatorChar);
                        var stripDirs = stripBeginningPath.Split(Path.DirectorySeparatorChar);

                        var newStrip = string.Empty;
                        for (var d = 0; (d < stripDirs.Length) && (d < aDirs.Length); d++)
                        {
                            if (stripDirs[d] == aDirs[d])
                                newStrip = Path.Combine(newStrip, aDirs[d]);
                            else
                                break;
                        }

                        stripBeginningPath = newStrip;
                    }

                    /*
                    var extractOption = new ExtractionOptions()
                    {
                        ExtractFullPath = true, 
                        PreserveAttributes = true, 
                        Overwrite = true, 
                        PreserveFileTime = true
                    };
                    */
                    // Extract all files, and strip of root folders that don't have any files in them
                    foreach (var zipArchiveEntry in archive.Entries)
                    {
                        if (zipArchiveEntry.IsDirectory)
                            continue;

                        var targetFile = zipArchiveEntry.Key;
                        if ((stripBeginningPath.Length > 0) && targetFile.StartsWith(stripBeginningPath))
                            targetFile = targetFile.Substring(stripBeginningPath.Length + 1);

                        targetFile = Path.Combine(ProjectFolder, targetFile);
                        var targetFileDir = Path.GetDirectoryName(targetFile) ?? "";

                        if (!Directory.Exists(targetFileDir))
                            Directory.CreateDirectory(targetFileDir);
                        if (File.Exists(targetFile))
                            File.Delete(targetFile);
                        zipArchiveEntry.WriteToFile(targetFile);
                    }
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80131501)
                {
                    // This is usually that it downloaded a page instead of the actual archive
                    MessageBox.Show($"It looks like the downloaded file is not a archive!\n" +
                                    $"The importer has probably downloaded a page instead of the archive.\n\n" +
                                    $"{ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Archive error trying to extract archive!\n\n{ex.Message}",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Failed);
                DialogResult = DialogResult.Cancel;
                return;
            }

            TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Complete);

            // Download video if available
            if (!string.IsNullOrWhiteSpace(ProjectVideoUrl))
            {
                TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Busy);
                using (var downloadDialog = new DownloadDialog())
                {
                    downloadDialog.SetDownloadJob(ProjectVideoUrl, ProjectVideo, "Download project video");
                    if (downloadDialog.BeginDownload() != DialogResult.OK)
                    {
                        MessageBox.Show(string.Format(Resources.DownloadFileError, ProjectVideo),
                            "Download project video",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        TaskLabelUpdate(LabelTaskUnpackArchive, LabelTaskState.Failed);
                    }
                    else
                    {
                        ProjectVideo = downloadDialog.TargetFile;
                        TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Complete);
                    }
                }
            }
            else
            {
                TaskLabelUpdate(LabelTaskDownloadVideo, LabelTaskState.Skipped);
            }

            TaskLabelUpdate(LabelTaskSaveProject, LabelTaskState.Busy);

            // Check if there is only one project file after extracting
            // If so, directly load that, else open the file open dialog starting in newly created directory
            var readyProjects = Directory.GetFiles(ProjectFolder, "*.pvd", SearchOption.AllDirectories).ToList();
            readyProjects.AddRange(Directory.GetFiles(ProjectFolder, "*.pvlv", SearchOption.AllDirectories));

            ViewedProjectTab? newProject = null;
            if (readyProjects.Count == 1)
            {
                // Exactly one project file found, use it
                this.Invoke(delegate
                {
                    newProject = MainForm.Instance?.OpenFile(readyProjects[0]) ?? null;
                });
            }
            else
            {
                // The warning text is different if there are multiple project files, but the effect is the same.
                // You need to manually select a file to open
                if (readyProjects.Count == 0)
                {
                    MessageBox.Show(
                        "It looks like the archive didn't contain any project files, please select a new log data file manually to create a project from!",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        "Not sure which file is your main project file, please select it manually!",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                OpenProjectFileDialog.InitialDirectory = ProjectFolder;
                OpenProjectFileDialog.FileName = Path.GetFileName(ProjectFile);
                this.Invoke(delegate
                {
                    if (OpenProjectFileDialog.ShowDialog() == DialogResult.OK)
                        newProject = MainForm.Instance?.OpenFile(OpenProjectFileDialog.FileName) ?? null;
                });
            }

            // Update the project settings to reflect the actually downloaded data
            if (newProject != null)
            {
                if (!string.IsNullOrWhiteSpace(ProjectArchiveUrl))
                    newProject.Settings.ProjectUrl = ProjectArchiveUrl;

                if (!string.IsNullOrWhiteSpace(ProjectSaveVideoUrl))
                    newProject.Settings.VideoSettings.VideoUrl = ProjectSaveVideoUrl;

                if (!string.IsNullOrWhiteSpace(ProjectVideo) && File.Exists(ProjectVideo))
                    newProject.Settings.VideoSettings.VideoFile = ProjectVideo;

                newProject.ProjectFile = ProjectFile;
                newProject.Settings.Description = ProjectTitle;
                newProject.OpenSettings();
                newProject.SaveProjectSettingsFile(newProject.ProjectFile, newProject.ProjectFolder);
                this.Invoke(delegate
                {
                    MainForm.Instance!.SaveProject(newProject, false);
                    MainForm.Instance.UpdateStatusBar(newProject);
                });
                TaskLabelUpdate(LabelTaskSaveProject, LabelTaskState.Complete);
                BtnCancel.Text = @"Close";
                BtnCancel.Enabled = true;
                return;
            }
            TaskLabelUpdate(LabelTaskSaveProject, LabelTaskState.Failed);
            BtnCancel.Enabled = true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnSkipVideo_Click(object sender, EventArgs e)
        {
            BtnSkipVideo.Enabled = false;
            ProjectVideoUrl = string.Empty;
            LabelVideo.Text = @"<skip>";
        }

        private void bgwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BtnCancel.Enabled = true;
        }
    }
}
