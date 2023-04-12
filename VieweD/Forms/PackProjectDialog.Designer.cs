using System.Windows.Forms;
using System.Drawing;

namespace VieweD.Forms
{
    partial class PackProjectDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SelectedFilesListBox = new CheckedListBox();
            label1 = new Label();
            BtnDefaultSelection = new Button();
            BtnSelectAll = new Button();
            BtnUnselectAll = new Button();
            ExportFileDialog = new SaveFileDialog();
            BtnSave = new Button();
            BtnClose = new Button();
            CBIncludeProjectFolder = new CheckBox();
            SuspendLayout();
            // 
            // SelectedFilesListBox
            // 
            SelectedFilesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SelectedFilesListBox.FormattingEnabled = true;
            SelectedFilesListBox.Location = new Point(12, 27);
            SelectedFilesListBox.Name = "SelectedFilesListBox";
            SelectedFilesListBox.Size = new Size(580, 256);
            SelectedFilesListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(133, 15);
            label1.TabIndex = 1;
            label1.Text = "Select what files to pack";
            // 
            // BtnDefaultSelection
            // 
            BtnDefaultSelection.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnDefaultSelection.Location = new Point(444, 289);
            BtnDefaultSelection.Name = "BtnDefaultSelection";
            BtnDefaultSelection.Size = new Size(148, 23);
            BtnDefaultSelection.TabIndex = 2;
            BtnDefaultSelection.Text = "Default Selection";
            BtnDefaultSelection.UseVisualStyleBackColor = true;
            BtnDefaultSelection.Click += BtnDefaultSelection_Click;
            // 
            // BtnSelectAll
            // 
            BtnSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnSelectAll.Location = new Point(12, 289);
            BtnSelectAll.Name = "BtnSelectAll";
            BtnSelectAll.Size = new Size(148, 23);
            BtnSelectAll.TabIndex = 3;
            BtnSelectAll.Text = "Select All";
            BtnSelectAll.UseVisualStyleBackColor = true;
            BtnSelectAll.Click += BtnSelectAll_Click;
            // 
            // BtnUnselectAll
            // 
            BtnUnselectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnUnselectAll.Location = new Point(166, 289);
            BtnUnselectAll.Name = "BtnUnselectAll";
            BtnUnselectAll.Size = new Size(148, 23);
            BtnUnselectAll.TabIndex = 4;
            BtnUnselectAll.Text = "Unselect All";
            BtnUnselectAll.UseVisualStyleBackColor = true;
            BtnUnselectAll.Click += BtnUnselectAll_Click;
            // 
            // ExportFileDialog
            // 
            ExportFileDialog.DefaultExt = "zip";
            ExportFileDialog.Filter = "Zip Files|*.zip|All files|*.*";
            ExportFileDialog.Title = "Export file";
            // 
            // BtnSave
            // 
            BtnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnSave.Image = Properties.Resources.document_save_16;
            BtnSave.ImageAlign = ContentAlignment.MiddleLeft;
            BtnSave.Location = new Point(12, 331);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(114, 23);
            BtnSave.TabIndex = 6;
            BtnSave.Text = "Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnExport_Click;
            // 
            // BtnClose
            // 
            BtnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnClose.Location = new Point(519, 331);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(75, 23);
            BtnClose.TabIndex = 7;
            BtnClose.Text = "Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // CBIncludeProjectFolder
            // 
            CBIncludeProjectFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            CBIncludeProjectFolder.AutoSize = true;
            CBIncludeProjectFolder.Checked = true;
            CBIncludeProjectFolder.CheckState = CheckState.Checked;
            CBIncludeProjectFolder.Location = new Point(132, 334);
            CBIncludeProjectFolder.Name = "CBIncludeProjectFolder";
            CBIncludeProjectFolder.Size = new Size(195, 19);
            CBIncludeProjectFolder.TabIndex = 8;
            CBIncludeProjectFolder.Text = "Include project name as a folder";
            CBIncludeProjectFolder.UseVisualStyleBackColor = true;
            // 
            // PackProjectDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 361);
            Controls.Add(CBIncludeProjectFolder);
            Controls.Add(BtnClose);
            Controls.Add(BtnSave);
            Controls.Add(BtnUnselectAll);
            Controls.Add(BtnSelectAll);
            Controls.Add(BtnDefaultSelection);
            Controls.Add(label1);
            Controls.Add(SelectedFilesListBox);
            MinimumSize = new Size(400, 300);
            Name = "PackProjectDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Pack project";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckedListBox SelectedFilesListBox;
        private Label label1;
        private Button BtnDefaultSelection;
        private Button BtnSelectAll;
        private Button BtnUnselectAll;
        private SaveFileDialog ExportFileDialog;
        private Button BtnSave;
        private Button BtnClose;
        private CheckBox CBIncludeProjectFolder;
    }
}