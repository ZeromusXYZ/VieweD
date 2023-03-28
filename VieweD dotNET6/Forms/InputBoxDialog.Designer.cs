namespace VieweD.Forms
{
    partial class InputBoxDialog
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
            BtnOK = new System.Windows.Forms.Button();
            BtnCancel = new System.Windows.Forms.Button();
            PromptLabel = new System.Windows.Forms.Label();
            InputText = new System.Windows.Forms.TextBox();
            BtnRevert = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // BtnOK
            // 
            BtnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnOK.Image = Properties.Resources.document_save_16;
            BtnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnOK.Location = new System.Drawing.Point(12, 59);
            BtnOK.Name = "BtnOK";
            BtnOK.Size = new System.Drawing.Size(100, 23);
            BtnOK.TabIndex = 1;
            BtnOK.Text = "OK";
            BtnOK.UseVisualStyleBackColor = true;
            BtnOK.Click += BtnOK_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            BtnCancel.Image = Properties.Resources.document_close_16;
            BtnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnCancel.Location = new System.Drawing.Point(118, 59);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(100, 23);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // PromptLabel
            // 
            PromptLabel.AutoSize = true;
            PromptLabel.Location = new System.Drawing.Point(12, 9);
            PromptLabel.Name = "PromptLabel";
            PromptLabel.Size = new System.Drawing.Size(81, 15);
            PromptLabel.TabIndex = 4;
            PromptLabel.Text = "Input prompt:";
            // 
            // InputText
            // 
            InputText.Location = new System.Drawing.Point(12, 30);
            InputText.Name = "InputText";
            InputText.Size = new System.Drawing.Size(479, 23);
            InputText.TabIndex = 0;
            // 
            // BtnRevert
            // 
            BtnRevert.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            BtnRevert.Image = Properties.Resources.application_menu_16;
            BtnRevert.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            BtnRevert.Location = new System.Drawing.Point(391, 59);
            BtnRevert.Name = "BtnRevert";
            BtnRevert.Size = new System.Drawing.Size(100, 23);
            BtnRevert.TabIndex = 3;
            BtnRevert.Text = "Revert";
            BtnRevert.UseVisualStyleBackColor = true;
            BtnRevert.Click += BtnRevert_Click;
            // 
            // InputBoxDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(503, 94);
            Controls.Add(BtnRevert);
            Controls.Add(InputText);
            Controls.Add(PromptLabel);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOK);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            Name = "InputBoxDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Input";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label PromptLabel;
        private System.Windows.Forms.TextBox InputText;
        private System.Windows.Forms.Button BtnRevert;
    }
}