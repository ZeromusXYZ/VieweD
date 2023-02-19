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
using Microsoft.Win32;
using SevenZip;
using System.Reflection;

namespace VieweD
{
    public partial class CompressForm : Form
    {
        public Dictionary<string, string> FilesToAdd = new Dictionary<string, string>();
        public string ArchiveFileName = "logs.7z" ;
        public string ProjectName = string.Empty;
        public static string SevenZipDLLPath = string.Empty;
        private int thisFileSize = 0;
        SevenZipCompressor zipper;
        SevenZipExtractor unzipper;
        public enum ZipTaskType { doZip, doUnZip };
        public ZipTaskType task = ZipTaskType.doZip;
        public bool overrideZipFormat = false;

        public int BuildArchieveFilesList(string ProjectFolder)
        {
            if (!Directory.Exists(ProjectFolder))
                return 0;
            int res = 0;

            var startdir = new DirectoryInfo(ProjectFolder);
            var allfiles = startdir.GetFiles("*", SearchOption.AllDirectories);
            string ProjectFolderName = Path.GetFileName(ProjectFolder.TrimEnd(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar ;

            pb.Minimum = 0;
            pb.Maximum = 1;
            foreach(var fn in allfiles)
            {
                bool skipThis = false;
                var checkExt = Path.GetExtension(fn.FullName).ToLower();

                // skip project files
                // if (checkExt == ".pvd") skipThis = true;
                // skip movies
                if (checkExt == ".avi") skipThis = true;
                if (checkExt == ".mp4") skipThis = true;
                if (checkExt == ".mpg") skipThis = true;
                if (checkExt == ".mpeg") skipThis = true;
                if (checkExt == ".ts") skipThis = true;
                if (checkExt == ".mkv") skipThis = true;
                if (checkExt == ".mov") skipThis = true;
                // skip self
                if (fn.FullName.ToLower() == ArchiveFileName.ToLower()) skipThis = true;

                if (!skipThis)
                {
                    var addFile = ProjectFolderName + fn.FullName.Substring(ProjectFolder.Length);
                    FilesToAdd.Add(addFile, fn.FullName);
                    res++;
                    pb.Maximum += (int)fn.Length;
                }
            }

            return res;
        }

        public CompressForm()
        {
            InitializeComponent();
        }

        private static bool TryLoad7ZipLibrary(string dll)
        {
            try
            {
                if (File.Exists(dll))
                {
                    SevenZipBase.SetLibraryPath(dll);
                    LibraryFeature features = SevenZipBase.CurrentLibraryFeatures;

                    return (features.HasFlag(LibraryFeature.Compress7z) && features.HasFlag(LibraryFeature.Extract7z));
                }
                else
                {
                    //MessageBox.Show("7zip library not found:\r\n" + dll, "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false ;
                }
            }
            catch // (Exception ex)
            {
                //MessageBox.Show("Couldn't load 7zip library Exception:\r\n" + ex.Message+"\r\n"+dll, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static string TryGet7ZipLibrary()
        {
            string res = string.Empty ;


            // Try local 7z.dll
            res = Path.Combine(Application.StartupPath, "7z.dll");
            if (!TryLoad7ZipLibrary(res))
                res = string.Empty;

            // Try from installed version of 7-zip
            if (res == string.Empty)
            {
                string InstallPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\7-Zip", (Environment.Is64BitProcess ? "Path64" : "Path"), null);
                if (InstallPath != null)
                {
                    res = Path.Combine(InstallPath, "7z.dll");
                    if (!TryLoad7ZipLibrary(res))
                        res = string.Empty;
                }
            }

            // Try local x86 and x64 dirs ?
            if (res == string.Empty)
            {
                res = Path.Combine(Application.StartupPath, Environment.Is64BitProcess ? "x64" : "x86", "7z.dll");
                if (!TryLoad7ZipLibrary(res))
                    res = string.Empty;
            }

            /*
            if (InstallPath != null)
            {
                res = Path.Combine(InstallPath, (Environment.Is64BitProcess ? "7-Zip.dll" : "7-Zip32.dll"));
                if (!TryLoad7ZipLibrary(res))
                    res = string.Empty;
            }
            */

            return res;
        }

        private void CompressForm_Shown(object sender, EventArgs e)
        {
            switch (task)
            {
                case ZipTaskType.doZip:
                    Text = "Add to archive";
                    break;
                case ZipTaskType.doUnZip:
                    Text = "Extract from archive";
                    break;
                default:
                    Text = "Unknown command";
                    DialogResult = DialogResult.Abort;
                    return;
            }
            // Start here
            // Toggle between the x86 and x64 bit dll
            if ((SevenZipDLLPath == null) || (SevenZipDLLPath == string.Empty))
                SevenZipDLLPath = TryGet7ZipLibrary();

            if (SevenZipDLLPath == string.Empty)
            {
                DialogResult = DialogResult.Abort;
                return;
            }
            LibraryFeature features = SevenZipBase.CurrentLibraryFeatures;
            lZipFile.Text = ArchiveFileName;

            switch (task)
            {
                case ZipTaskType.doZip:

                    if (!features.HasFlag(LibraryFeature.Compress7z))
                    {
                        MessageBox.Show("Currently loaded 7-zip library doesn't support 7z compression, cannot continue !\r\n"+SevenZipDLLPath, "Method not supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        DialogResult = DialogResult.Abort;
                        return;
                    }

                    if (File.Exists(ArchiveFileName))
                    {
                        if (MessageBox.Show("Target 7z file already exists, do you want to overwrite it ?\r\n" + ArchiveFileName, "Overwrite File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            DialogResult = DialogResult.Cancel;
                            return;
                        }
                    }

                    try
                    {
                        zipper = new SevenZipCompressor();
                        zipper.CompressionLevel = SevenZip.CompressionLevel.Ultra;
                        zipper.CompressionMethod = CompressionMethod.Lzma;
                        zipper.CompressionMode = CompressionMode.Create;
                        zipper.TempFolderPath = Path.GetTempPath();
                        zipper.ArchiveFormat = OutArchiveFormat.SevenZip;
                        bgwZipper.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception:\r\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DialogResult = DialogResult.Abort;
                    }
                    break;
                case ZipTaskType.doUnZip:
                    if (!File.Exists(ArchiveFileName))
                    {
                        MessageBox.Show("Source 7z file not found ?\r\n" + ArchiveFileName, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        DialogResult = DialogResult.Cancel;
                        return;
                    }

                    if ((Path.GetExtension(ArchiveFileName).ToLower() == ".7z") && (!features.HasFlag(LibraryFeature.Extract7z)))
                    {
                        MessageBox.Show("Currently loaded 7-zip library doesn't support .7z extraction, cannot continue !\r\n" + SevenZipDLLPath, "Method not supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        DialogResult = DialogResult.Abort;
                        return;
                    }

                    if ((Path.GetExtension(ArchiveFileName).ToLower() == ".zip") && (!features.HasFlag(LibraryFeature.ExtractZip)))
                    {
                        MessageBox.Show("Currently loaded 7-zip library doesn't support .zip extraction, cannot continue !\r\n" + SevenZipDLLPath, "Method not supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        DialogResult = DialogResult.Abort;
                        return;
                    }

                    if ((Path.GetExtension(ArchiveFileName).ToLower() == ".rar") && (!features.HasFlag(LibraryFeature.ExtractRar)))
                    {
                        MessageBox.Show("Currently loaded 7-zip library doesn't support .rar extraction, cannot continue !\r\n" + SevenZipDLLPath, "Method not supported", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        DialogResult = DialogResult.Abort;
                        return;
                    }

                    try
                    {
                        if (overrideZipFormat)
                            unzipper = new SevenZipExtractor(ArchiveFileName,InArchiveFormat.SevenZip);
                        else
                            unzipper = new SevenZipExtractor(ArchiveFileName);

                        if (unzipper.FilesCount <= 0)
                        {
                            MessageBox.Show("Nothing to extract", "No files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            DialogResult = DialogResult.Cancel;
                        }
                        else
                        {
                            pb.Minimum = 0;
                            pb.Maximum = (int)unzipper.FilesCount+1 ;
                            pb.Step = 1;
                            pb.Value = 0;
                            // unzipper.FileExtractionStarted += new EventHandler<FileInfoEventArgs>(FileExtractStarted);
                            unzipper.FileExtractionFinished += new EventHandler<FileInfoEventArgs>(FileExtractFinished);
                            unzipper.FileExists += new EventHandler<FileOverwriteEventArgs>(FileExtractFileExists);
                            bgwUnZipper.RunWorkerAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception:\r\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DialogResult = DialogResult.Abort;
                    }
                    break;
                default:
                    DialogResult = DialogResult.Abort;
                    return;
            }

        }

        private void CompressForm_Load(object sender, EventArgs e)
        {
            // Init
        }

        private void BgwZipper_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void FileCompressionStarted(object sender, FileNameEventArgs e)
        {
            if (bgwZipper.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            if (FilesToAdd.TryGetValue(e.FileName,out string sourceFile))
            {
                var fi = new FileInfo(sourceFile);
                thisFileSize = (int)fi.Length;
            }
            else
            {
                thisFileSize = 0;
            }
            
            this.Invoke(new MethodInvoker(delegate
            {
                lInfo.Text = e.FileName;
            }));
        }

        private void FileCompressionFinished(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                pb.Value += thisFileSize;
            }));
            // System.Threading.Thread.Sleep(200);
        }

        private void BgwZipper_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                File.Delete(ArchiveFileName);
            }
            catch
            {
                // Ignore
            }
            zipper.FileCompressionStarted += new EventHandler<FileNameEventArgs>(FileCompressionStarted);
            zipper.FileCompressionFinished += new EventHandler<EventArgs>(FileCompressionFinished);
            zipper.CompressFileDictionary(FilesToAdd, ArchiveFileName);
        }

        private void CompressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwZipper.IsBusy)
                bgwZipper.CancelAsync();
            if (bgwUnZipper.IsBusy)
                bgwUnZipper.CancelAsync();
        }

        private void FileExtractStarted(object sender, FileInfoEventArgs e)
        {
            if (bgwUnZipper.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (e.PercentDone == 0)
            {
                //this.Invoke(new MethodInvoker(delegate
                //{
                lInfo.Text = e.FileInfo.FileName;
                lInfo.Refresh();
                //}));
            }
        }

        private void FileExtractFinished(object sender, FileInfoEventArgs e)
        {
            if (bgwUnZipper.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (e.PercentDone == 100)
            {
                //this.Invoke(new MethodInvoker(delegate
                //{
                pb.Value += (int)e.FileInfo.Size ;
                //}));
                //System.Threading.Thread.Sleep(200);
            }

        }

        private void FileExtractFileExists(object sender, FileOverwriteEventArgs e)
        {
            if (bgwUnZipper.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = false;
        }

        private void DoUnZip()
        {
            string targetDir = Path.GetDirectoryName(ArchiveFileName);
            string expectedRootDir = ProjectName ;
            expectedRootDir = expectedRootDir.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            targetDir = targetDir.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var filelist = unzipper.ArchiveFileData.ToArray();
            ulong totsize = 0;
            foreach (var fd in filelist)
            {
                totsize += fd.Size;
            }

            this.Invoke(new MethodInvoker(delegate
            {
                pb.Maximum = (int)totsize + 1;
                pb.Value = 0;
            }));
            foreach (var fd in filelist)
            {
                // Skip directories
                if ((fd.Attributes & 0x10) != 0)
                    continue;

                try
                {
                    var zippedName = string.Empty;
                    if (fd.FileName.StartsWith(expectedRootDir))
                    {
                        zippedName = fd.FileName.Substring(expectedRootDir.Length);
                    }
                    else
                    {
                        zippedName = fd.FileName;
                    }
                    var targetName = targetDir + zippedName;
                    var targetFileDir = Path.GetDirectoryName(targetName);
                    if (!Directory.Exists(targetFileDir))
                        Directory.CreateDirectory(targetFileDir);
                    var fs = File.Create(targetName);
                    //this.Invoke(new MethodInvoker(delegate
                    //{
                        lInfo.Text = targetName;
                        lInfo.Refresh();
                        unzipper.ExtractFile(fd.FileName, fs);
                    //}));
                    fs.Close();
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show("Exception extracting file:\r\n"+ex+"\r\n"+fd+"\r\nDo you want to continue ?","Exception",MessageBoxButtons.YesNo,MessageBoxIcon.Error) == DialogResult.No)
                    {
                        break;
                    }
                }
            }
            // unzipper.ExtractFiles(tDir, filelist);
            // unzipper.ExtractArchive(tDir);
        }

        private void BgwUnZipper_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                DoUnZip();
            }));
        }

        private void BgwUnZipper_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CompressForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (zipper != null)
            {
                zipper = null;
            }
            if (unzipper != null)
            {
                unzipper.Dispose();
                unzipper = null;
            }
        }
    }
}
