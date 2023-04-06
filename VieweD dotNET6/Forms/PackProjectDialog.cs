using System;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using CsvHelper.Configuration;
using VieweD.engine.common;
using VieweD.Helpers.System;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class PackProjectDialog : Form
    {
        private ViewedProjectTab? ParentProject { get; set; }

        public PackProjectDialog()
        {
            InitializeComponent();
        }

        public bool LoadFromProject(ViewedProjectTab project)
        {
            ParentProject = project;
            if (ParentProject == null)
                return false;

            if (!Directory.Exists(ParentProject.ProjectFolder))
            {
                MessageBox.Show("Can only pack a full project.");
                return false;
            }

            var files = Directory.GetFiles(ParentProject.ProjectFolder, "*.*", SearchOption.AllDirectories);

            SelectedFilesListBox.Items.Clear();
            foreach (var fileName in files)
            {
                var ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".bak")
                    continue;
                if (ext == ".old")
                    continue;
                if (ext == ".zip")
                    continue;
                if (ext == ".7z")
                    continue;
                var shortName = Helper.MakeRelative(ParentProject.ProjectFolder, fileName);
                SelectedFilesListBox.Items.Add(shortName);
            }

            AutoSelectFields();
            return true;
        }

        public void AutoSelectFields()
        {
            for (var i = 0; i < SelectedFilesListBox.Items.Count; i++)
            {
                var item = (SelectedFilesListBox.Items[i] as string);
                if (item == null)
                    continue;
                var doShow = true;
                var ext = Path.GetExtension(item).ToLower();
                if (ext == ".zip") doShow = false;
                if (ext == ".z7") doShow = false;
                if (ext == ".rar") doShow = false;
                if (ext == ".avi") doShow = false;
                if (ext == ".mp4") doShow = false;
                if (ext == ".mpg") doShow = false;
                if (ext == ".mpeg") doShow = false;
                if (ext == ".ogg") doShow = false;
                SelectedFilesListBox.SetItemChecked(i, doShow);
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFilesListBox.Items.Count; i++)
                SelectedFilesListBox.SetItemChecked(i, true);
        }

        private void BtnUnselectAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < SelectedFilesListBox.Items.Count; i++)
                SelectedFilesListBox.SetItemChecked(i, false);
        }

        private void BtnDefaultSelection_Click(object sender, EventArgs e)
        {
            AutoSelectFields();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (ParentProject == null)
                return;

            ExportFileDialog.InitialDirectory = ParentProject.ProjectFolder;
            ExportFileDialog.FileName = Path.GetFileName(ParentProject.ProjectFolder) + ".zip";

            if (ExportFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var selectedFileNames = SelectedFilesListBox.CheckedItems;
                MainForm.Instance?.UpdateStatusBarProgress(0, selectedFileNames.Count, Resources.AddToArchiveTitle, null);

                //selectedFieldNames.CopyTo(csv.HeaderRecord, 0);
                using var fileStream = File.Open(ExportFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                using var zip = new ZipArchive(fileStream, ZipArchiveMode.Create, false, Encoding.UTF8);

                var c = 0;
                foreach (string fileName in selectedFileNames)
                {
                    c++;
                    var sourceFile = fileName;
                    var destFile = fileName;
                    if (File.Exists(Path.Combine(ParentProject.ProjectFolder, sourceFile)))
                    {
                        sourceFile = Path.Combine(ParentProject.ProjectFolder, sourceFile);
                    }
                    else
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }

                    if (CBIncludeProjectFolder.Checked)
                    {
                        destFile = Path.Combine(Path.GetFileName(ParentProject.ProjectFolder), fileName);
                    }
                    zip.CreateEntryFromFile(sourceFile, destFile, CompressionLevel.SmallestSize);
                    MainForm.Instance?.UpdateStatusBarProgress(c, selectedFileNames.Count, Resources.AddToArchiveTitle, null);
                }
                MainForm.Instance?.UpdateStatusBarProgress(selectedFileNames.Count, selectedFileNames.Count, Resources.AddToArchiveTitle, null);
                FileHelper.ExploreFile(ExportFileDialog.FileName);
                DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                MainForm.Instance?.UpdateStatusBarProgress(1, 1, Resources.AddToArchiveTitle, null);
                MessageBox.Show(exception.Message, Resources.SaveCancelled, MessageBoxButtons.OK);
            }
        }
    }
}
