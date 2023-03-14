namespace VieweD.Forms
{
    partial class RulesSelectDialog
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
            cbRulesList = new ComboBox();
            label1 = new Label();
            btnOK = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // cbRulesList
            // 
            cbRulesList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbRulesList.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRulesList.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point);
            cbRulesList.FormattingEnabled = true;
            cbRulesList.Location = new Point(14, 47);
            cbRulesList.Margin = new Padding(4, 3, 4, 3);
            cbRulesList.Name = "cbRulesList";
            cbRulesList.Size = new Size(514, 24);
            cbRulesList.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 10);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(335, 15);
            label1.TabIndex = 1;
            label1.Text = "Please select which rule set you want to use to load for this file";
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(441, 77);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 2;
            btnOK.Text = "Select";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.GrayText;
            label2.Location = new Point(13, 83);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(331, 15);
            label2.TabIndex = 3;
            label2.Text = "Rule marked like >> this << is the currently loaded active rule";
            // 
            // RulesSelectDialog
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(542, 115);
            Controls.Add(label2);
            Controls.Add(btnOK);
            Controls.Add(label1);
            Controls.Add(cbRulesList);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RulesSelectDialog";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rules Select";
            Load += RulesSelectForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cbRulesList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
    }
}