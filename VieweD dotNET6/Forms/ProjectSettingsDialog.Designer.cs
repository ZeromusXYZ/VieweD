namespace VieweD.Forms
{
    partial class ProjectSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectSettingsDialog));
            this.GbProjectFile = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextVideoFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TextProjectFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TextLogFile = new System.Windows.Forms.TextBox();
            this.GbInput = new System.Windows.Forms.GroupBox();
            this.CBRules = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.CBInputReader = new System.Windows.Forms.ComboBox();
            this.CBParser = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnOK = new System.Windows.Forms.Button();
            this.GbProjectFile.SuspendLayout();
            this.GbInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // GbProjectFile
            // 
            this.GbProjectFile.Controls.Add(this.label2);
            this.GbProjectFile.Controls.Add(this.TextVideoFile);
            this.GbProjectFile.Controls.Add(this.label6);
            this.GbProjectFile.Controls.Add(this.TextProjectFile);
            this.GbProjectFile.Controls.Add(this.label1);
            this.GbProjectFile.Controls.Add(this.TextLogFile);
            resources.ApplyResources(this.GbProjectFile, "GbProjectFile");
            this.GbProjectFile.Name = "GbProjectFile";
            this.GbProjectFile.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TextVideoFile
            // 
            resources.ApplyResources(this.TextVideoFile, "TextVideoFile");
            this.TextVideoFile.Name = "TextVideoFile";
            this.TextVideoFile.ReadOnly = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // TextProjectFile
            // 
            resources.ApplyResources(this.TextProjectFile, "TextProjectFile");
            this.TextProjectFile.Name = "TextProjectFile";
            this.TextProjectFile.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TextLogFile
            // 
            resources.ApplyResources(this.TextLogFile, "TextLogFile");
            this.TextLogFile.Name = "TextLogFile";
            this.TextLogFile.ReadOnly = true;
            // 
            // GbInput
            // 
            this.GbInput.Controls.Add(this.CBRules);
            this.GbInput.Controls.Add(this.label4);
            this.GbInput.Controls.Add(this.label8);
            this.GbInput.Controls.Add(this.CBInputReader);
            this.GbInput.Controls.Add(this.CBParser);
            this.GbInput.Controls.Add(this.label3);
            resources.ApplyResources(this.GbInput, "GbInput");
            this.GbInput.Name = "GbInput";
            this.GbInput.TabStop = false;
            // 
            // CBRules
            // 
            this.CBRules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBRules.FormattingEnabled = true;
            resources.ApplyResources(this.CBRules, "CBRules");
            this.CBRules.Name = "CBRules";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // CBInputReader
            // 
            this.CBInputReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBInputReader.FormattingEnabled = true;
            resources.ApplyResources(this.CBInputReader, "CBInputReader");
            this.CBInputReader.Name = "CBInputReader";
            // 
            // CBParser
            // 
            this.CBParser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBParser.FormattingEnabled = true;
            resources.ApplyResources(this.CBParser, "CBParser");
            this.CBParser.Name = "CBParser";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // BtnOK
            // 
            resources.ApplyResources(this.BtnOK, "BtnOK");
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.UseVisualStyleBackColor = true;
            // 
            // ProjectSettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.GbInput);
            this.Controls.Add(this.GbProjectFile);
            this.Name = "ProjectSettingsDialog";
            this.Load += new System.EventHandler(this.ProjectSettingsDialog_Load);
            this.GbProjectFile.ResumeLayout(false);
            this.GbProjectFile.PerformLayout();
            this.GbInput.ResumeLayout(false);
            this.GbInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox GbProjectFile;
        private TextBox TextProjectFile;
        private GroupBox GbInput;
        private Label label2;
        private TextBox TextVideoFile;
        private Label label1;
        private TextBox TextLogFile;
        private Label label3;
        private Label label6;
        private ComboBox CBInputReader;
        private ComboBox CBParser;
        private ComboBox CBRules;
        private Label label4;
        private Label label8;
        private Button BtnOK;
    }
}