namespace VieweD
{
    partial class CompressForm
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
            this.pb = new System.Windows.Forms.ProgressBar();
            this.lZipFile = new System.Windows.Forms.Label();
            this.lInfo = new System.Windows.Forms.Label();
            this.bgwZipper = new System.ComponentModel.BackgroundWorker();
            this.bgwUnZipper = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb.Location = new System.Drawing.Point(12, 25);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(532, 23);
            this.pb.TabIndex = 0;
            // 
            // lZipFile
            // 
            this.lZipFile.AutoSize = true;
            this.lZipFile.Location = new System.Drawing.Point(12, 9);
            this.lZipFile.Name = "lZipFile";
            this.lZipFile.Size = new System.Drawing.Size(26, 13);
            this.lZipFile.TabIndex = 1;
            this.lZipFile.Text = "File:";
            // 
            // lInfo
            // 
            this.lInfo.AutoSize = true;
            this.lInfo.Location = new System.Drawing.Point(12, 67);
            this.lInfo.Name = "lInfo";
            this.lInfo.Size = new System.Drawing.Size(28, 13);
            this.lInfo.TabIndex = 2;
            this.lInfo.Text = "Info:";
            // 
            // bgwZipper
            // 
            this.bgwZipper.WorkerSupportsCancellation = true;
            this.bgwZipper.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwZipper_DoWork);
            this.bgwZipper.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwZipper_RunWorkerCompleted);
            // 
            // bgwUnZipper
            // 
            this.bgwUnZipper.WorkerSupportsCancellation = true;
            this.bgwUnZipper.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwUnZipper_DoWork);
            this.bgwUnZipper.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwUnZipper_RunWorkerCompleted);
            // 
            // CompressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 104);
            this.Controls.Add(this.lInfo);
            this.Controls.Add(this.lZipFile);
            this.Controls.Add(this.pb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompressForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add to archieve";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CompressForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CompressForm_FormClosed);
            this.Load += new System.EventHandler(this.CompressForm_Load);
            this.Shown += new System.EventHandler(this.CompressForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Label lZipFile;
        private System.Windows.Forms.Label lInfo;
        private System.ComponentModel.BackgroundWorker bgwZipper;
        private System.ComponentModel.BackgroundWorker bgwUnZipper;
    }
}