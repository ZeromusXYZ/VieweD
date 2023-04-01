namespace VieweD.Forms
{
    partial class ExportVpxDialog
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
            GBExportDataSelect = new System.Windows.Forms.GroupBox();
            RbBoth = new System.Windows.Forms.RadioButton();
            RbBytes = new System.Windows.Forms.RadioButton();
            RbParsed = new System.Windows.Forms.RadioButton();
            BtnOk = new System.Windows.Forms.Button();
            BtnCancel = new System.Windows.Forms.Button();
            CbExportOnlyVisible = new System.Windows.Forms.CheckBox();
            GBExportDataSelect.SuspendLayout();
            SuspendLayout();
            // 
            // GBExportDataSelect
            // 
            GBExportDataSelect.Controls.Add(RbBoth);
            GBExportDataSelect.Controls.Add(RbBytes);
            GBExportDataSelect.Controls.Add(RbParsed);
            GBExportDataSelect.Location = new System.Drawing.Point(12, 12);
            GBExportDataSelect.Name = "GBExportDataSelect";
            GBExportDataSelect.Size = new System.Drawing.Size(255, 105);
            GBExportDataSelect.TabIndex = 0;
            GBExportDataSelect.TabStop = false;
            GBExportDataSelect.Text = "What to export";
            // 
            // RbBoth
            // 
            RbBoth.AutoSize = true;
            RbBoth.Checked = true;
            RbBoth.Location = new System.Drawing.Point(6, 72);
            RbBoth.Name = "RbBoth";
            RbBoth.Size = new System.Drawing.Size(163, 19);
            RbBoth.TabIndex = 2;
            RbBoth.TabStop = true;
            RbBoth.Text = "Both parsed and byte data";
            RbBoth.UseVisualStyleBackColor = true;
            RbBoth.CheckedChanged += RbExportSettings_CheckedChanged;
            // 
            // RbBytes
            // 
            RbBytes.AutoSize = true;
            RbBytes.Location = new System.Drawing.Point(6, 47);
            RbBytes.Name = "RbBytes";
            RbBytes.Size = new System.Drawing.Size(99, 19);
            RbBytes.TabIndex = 1;
            RbBytes.Text = "Raw byte data";
            RbBytes.UseVisualStyleBackColor = true;
            // 
            // RbParsed
            // 
            RbParsed.AutoSize = true;
            RbParsed.Location = new System.Drawing.Point(6, 22);
            RbParsed.Name = "RbParsed";
            RbParsed.Size = new System.Drawing.Size(86, 19);
            RbParsed.TabIndex = 0;
            RbParsed.Text = "Parsed data";
            RbParsed.UseVisualStyleBackColor = true;
            // 
            // BtnOk
            // 
            BtnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            BtnOk.Location = new System.Drawing.Point(12, 147);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new System.Drawing.Size(75, 23);
            BtnOk.TabIndex = 1;
            BtnOk.Text = "Ok";
            BtnOk.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            BtnCancel.Location = new System.Drawing.Point(192, 147);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(75, 23);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // CbExportOnlyVisible
            // 
            CbExportOnlyVisible.AutoSize = true;
            CbExportOnlyVisible.Checked = true;
            CbExportOnlyVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            CbExportOnlyVisible.Location = new System.Drawing.Point(18, 122);
            CbExportOnlyVisible.Name = "CbExportOnlyVisible";
            CbExportOnlyVisible.Size = new System.Drawing.Size(215, 19);
            CbExportOnlyVisible.TabIndex = 3;
            CbExportOnlyVisible.Text = "Export only currently visible packets";
            CbExportOnlyVisible.UseVisualStyleBackColor = true;
            // 
            // ExportVpxDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(279, 178);
            Controls.Add(CbExportOnlyVisible);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOk);
            Controls.Add(GBExportDataSelect);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Name = "ExportVpxDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Export VPX";
            GBExportDataSelect.ResumeLayout(false);
            GBExportDataSelect.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox GBExportDataSelect;
        private System.Windows.Forms.RadioButton RbBoth;
        private System.Windows.Forms.RadioButton RbBytes;
        private System.Windows.Forms.RadioButton RbParsed;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.Button BtnCancel;
        public System.Windows.Forms.CheckBox CbExportOnlyVisible;
    }
}