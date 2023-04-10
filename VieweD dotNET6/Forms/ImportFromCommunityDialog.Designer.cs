namespace VieweD.Forms
{
    partial class ImportFromCommunityDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFromCommunityDialog));
            BtnStart = new System.Windows.Forms.Button();
            LabelTitle = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            LabelArchive = new System.Windows.Forms.LinkLabel();
            LabelVideo = new System.Windows.Forms.LinkLabel();
            label3 = new System.Windows.Forms.Label();
            BtnSelectTarget = new System.Windows.Forms.Button();
            LabelImportTarget = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            BtnSkipVideo = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            LabelTaskSaveProject = new System.Windows.Forms.Label();
            LabelTaskDownloadVideo = new System.Windows.Forms.Label();
            LabelTaskUnpackArchive = new System.Windows.Forms.Label();
            LabelTaskDownloadArchive = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            BtnCancel = new System.Windows.Forms.Button();
            bgwImport = new System.ComponentModel.BackgroundWorker();
            ImportFileDialog = new System.Windows.Forms.SaveFileDialog();
            OpenProjectFileDialog = new System.Windows.Forms.OpenFileDialog();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // BtnStart
            // 
            resources.ApplyResources(BtnStart, "BtnStart");
            BtnStart.Image = Properties.Resources.document_import_16;
            BtnStart.Name = "BtnStart";
            BtnStart.UseVisualStyleBackColor = true;
            BtnStart.Click += BtnStart_Click;
            // 
            // LabelTitle
            // 
            resources.ApplyResources(LabelTitle, "LabelTitle");
            LabelTitle.Name = "LabelTitle";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // LabelArchive
            // 
            resources.ApplyResources(LabelArchive, "LabelArchive");
            LabelArchive.Name = "LabelArchive";
            LabelArchive.TabStop = true;
            // 
            // LabelVideo
            // 
            resources.ApplyResources(LabelVideo, "LabelVideo");
            LabelVideo.Name = "LabelVideo";
            LabelVideo.TabStop = true;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // BtnSelectTarget
            // 
            resources.ApplyResources(BtnSelectTarget, "BtnSelectTarget");
            BtnSelectTarget.Image = Properties.Resources.document_open_folder_16;
            BtnSelectTarget.Name = "BtnSelectTarget";
            BtnSelectTarget.UseVisualStyleBackColor = true;
            BtnSelectTarget.Click += BtnSelectTarget_Click;
            // 
            // LabelImportTarget
            // 
            resources.ApplyResources(LabelImportTarget, "LabelImportTarget");
            LabelImportTarget.Name = "LabelImportTarget";
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(BtnSkipVideo);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(LabelTitle);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(LabelVideo);
            groupBox1.Controls.Add(LabelArchive);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // BtnSkipVideo
            // 
            resources.ApplyResources(BtnSkipVideo, "BtnSkipVideo");
            BtnSkipVideo.Image = Properties.Resources.close;
            BtnSkipVideo.Name = "BtnSkipVideo";
            BtnSkipVideo.UseVisualStyleBackColor = true;
            BtnSkipVideo.Click += BtnSkipVideo_Click;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(BtnSelectTarget);
            groupBox2.Controls.Add(LabelImportTarget);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Controls.Add(LabelTaskSaveProject);
            groupBox3.Controls.Add(LabelTaskDownloadVideo);
            groupBox3.Controls.Add(LabelTaskUnpackArchive);
            groupBox3.Controls.Add(LabelTaskDownloadArchive);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label2);
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // LabelTaskSaveProject
            // 
            resources.ApplyResources(LabelTaskSaveProject, "LabelTaskSaveProject");
            LabelTaskSaveProject.Name = "LabelTaskSaveProject";
            // 
            // LabelTaskDownloadVideo
            // 
            resources.ApplyResources(LabelTaskDownloadVideo, "LabelTaskDownloadVideo");
            LabelTaskDownloadVideo.Name = "LabelTaskDownloadVideo";
            // 
            // LabelTaskUnpackArchive
            // 
            resources.ApplyResources(LabelTaskUnpackArchive, "LabelTaskUnpackArchive");
            LabelTaskUnpackArchive.Name = "LabelTaskUnpackArchive";
            // 
            // LabelTaskDownloadArchive
            // 
            resources.ApplyResources(LabelTaskDownloadArchive, "LabelTaskDownloadArchive");
            LabelTaskDownloadArchive.Name = "LabelTaskDownloadArchive";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // BtnCancel
            // 
            resources.ApplyResources(BtnCancel, "BtnCancel");
            BtnCancel.Image = Properties.Resources.document_close_16;
            BtnCancel.Name = "BtnCancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // bgwImport
            // 
            bgwImport.DoWork += bgwImport_DoWork;
            bgwImport.RunWorkerCompleted += bgwImport_RunWorkerCompleted;
            // 
            // ImportFileDialog
            // 
            ImportFileDialog.DefaultExt = "pvd";
            resources.ApplyResources(ImportFileDialog, "ImportFileDialog");
            // 
            // OpenProjectFileDialog
            // 
            resources.ApplyResources(OpenProjectFileDialog, "OpenProjectFileDialog");
            // 
            // ImportFromCommunityDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(BtnCancel);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(BtnStart);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "ImportFromCommunityDialog";
            Load += ImportFromCommunityDialog_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label LabelTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel LabelArchive;
        private System.Windows.Forms.LinkLabel LabelVideo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtnSelectTarget;
        private System.Windows.Forms.Label LabelImportTarget;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelTaskSaveProject;
        private System.Windows.Forms.Label LabelTaskDownloadVideo;
        private System.Windows.Forms.Label LabelTaskUnpackArchive;
        private System.Windows.Forms.Label LabelTaskDownloadArchive;
        private System.ComponentModel.BackgroundWorker bgwImport;
        private System.Windows.Forms.SaveFileDialog ImportFileDialog;
        internal System.Windows.Forms.OpenFileDialog OpenProjectFileDialog;
        private System.Windows.Forms.Button BtnSkipVideo;
    }
}