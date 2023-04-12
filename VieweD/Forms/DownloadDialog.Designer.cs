using VieweD.Properties;

namespace VieweD.Forms
{
    partial class DownloadDialog
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
            DownloadProgress = new System.Windows.Forms.ProgressBar();
            label1 = new System.Windows.Forms.Label();
            LabelDownloadUrl = new System.Windows.Forms.LinkLabel();
            label2 = new System.Windows.Forms.Label();
            LabelTargetFile = new System.Windows.Forms.Label();
            BtnCancel = new System.Windows.Forms.Button();
            bgw = new System.ComponentModel.BackgroundWorker();
            SuspendLayout();
            // 
            // DownloadProgress
            // 
            DownloadProgress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DownloadProgress.Location = new System.Drawing.Point(12, 12);
            DownloadProgress.Name = "DownloadProgress";
            DownloadProgress.Size = new System.Drawing.Size(577, 23);
            DownloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            DownloadProgress.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 38);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(84, 15);
            label1.TabIndex = 1;
            label1.Text = "Downloading: ";
            // 
            // LabelDownloadUrl
            // 
            LabelDownloadUrl.AutoSize = true;
            LabelDownloadUrl.Location = new System.Drawing.Point(102, 38);
            LabelDownloadUrl.Name = "LabelDownloadUrl";
            LabelDownloadUrl.Size = new System.Drawing.Size(50, 15);
            LabelDownloadUrl.TabIndex = 2;
            LabelDownloadUrl.TabStop = true;
            LabelDownloadUrl.Text = "<none>";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 53);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(42, 15);
            label2.TabIndex = 3;
            label2.Text = "Target:";
            // 
            // LabelTargetFile
            // 
            LabelTargetFile.AutoSize = true;
            LabelTargetFile.Location = new System.Drawing.Point(102, 53);
            LabelTargetFile.Name = "LabelTargetFile";
            LabelTargetFile.Size = new System.Drawing.Size(50, 15);
            LabelTargetFile.TabIndex = 4;
            LabelTargetFile.Text = "<none>";
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnCancel.Location = new System.Drawing.Point(12, 76);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(84, 23);
            BtnCancel.TabIndex = 5;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // bgw
            // 
            bgw.DoWork += bgw_DoWork;
            bgw.ProgressChanged += bgw_ProgressChanged;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            // 
            // DownloadDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(601, 111);
            Controls.Add(BtnCancel);
            Controls.Add(LabelTargetFile);
            Controls.Add(label2);
            Controls.Add(LabelDownloadUrl);
            Controls.Add(label1);
            Controls.Add(DownloadProgress);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "DownloadDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = Resources.Download;
            Load += DownloadDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ProgressBar DownloadProgress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel LabelDownloadUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelTargetFile;
        private System.Windows.Forms.Button BtnCancel;
        private System.ComponentModel.BackgroundWorker bgw;
    }
}