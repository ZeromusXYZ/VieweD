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
            GbProjectFile = new GroupBox();
            label2 = new Label();
            TextVideoFile = new TextBox();
            label6 = new Label();
            TextProjectFile = new TextBox();
            label1 = new Label();
            TextLogFile = new TextBox();
            GbInput = new GroupBox();
            CBRules = new ComboBox();
            label4 = new Label();
            label8 = new Label();
            CBInputReader = new ComboBox();
            CBParser = new ComboBox();
            label3 = new Label();
            BtnOK = new Button();
            GbTags = new GroupBox();
            BtnAddTag = new Button();
            TagTextBox = new TextBox();
            TagLayout = new FlowLayoutPanel();
            GbProjectFile.SuspendLayout();
            GbInput.SuspendLayout();
            GbTags.SuspendLayout();
            SuspendLayout();
            // 
            // GbProjectFile
            // 
            GbProjectFile.Controls.Add(label2);
            GbProjectFile.Controls.Add(TextVideoFile);
            GbProjectFile.Controls.Add(label6);
            GbProjectFile.Controls.Add(TextProjectFile);
            GbProjectFile.Controls.Add(label1);
            GbProjectFile.Controls.Add(TextLogFile);
            resources.ApplyResources(GbProjectFile, "GbProjectFile");
            GbProjectFile.Name = "GbProjectFile";
            GbProjectFile.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // TextVideoFile
            // 
            resources.ApplyResources(TextVideoFile, "TextVideoFile");
            TextVideoFile.Name = "TextVideoFile";
            TextVideoFile.ReadOnly = true;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // TextProjectFile
            // 
            resources.ApplyResources(TextProjectFile, "TextProjectFile");
            TextProjectFile.Name = "TextProjectFile";
            TextProjectFile.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // TextLogFile
            // 
            resources.ApplyResources(TextLogFile, "TextLogFile");
            TextLogFile.Name = "TextLogFile";
            TextLogFile.ReadOnly = true;
            // 
            // GbInput
            // 
            GbInput.Controls.Add(CBRules);
            GbInput.Controls.Add(label4);
            GbInput.Controls.Add(label8);
            GbInput.Controls.Add(CBInputReader);
            GbInput.Controls.Add(CBParser);
            GbInput.Controls.Add(label3);
            resources.ApplyResources(GbInput, "GbInput");
            GbInput.Name = "GbInput";
            GbInput.TabStop = false;
            // 
            // CBRules
            // 
            CBRules.DropDownStyle = ComboBoxStyle.DropDownList;
            CBRules.FormattingEnabled = true;
            resources.ApplyResources(CBRules, "CBRules");
            CBRules.Name = "CBRules";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // CBInputReader
            // 
            CBInputReader.DropDownStyle = ComboBoxStyle.DropDownList;
            CBInputReader.FormattingEnabled = true;
            resources.ApplyResources(CBInputReader, "CBInputReader");
            CBInputReader.Name = "CBInputReader";
            // 
            // CBParser
            // 
            CBParser.DropDownStyle = ComboBoxStyle.DropDownList;
            CBParser.FormattingEnabled = true;
            resources.ApplyResources(CBParser, "CBParser");
            CBParser.Name = "CBParser";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // BtnOK
            // 
            resources.ApplyResources(BtnOK, "BtnOK");
            BtnOK.DialogResult = DialogResult.OK;
            BtnOK.Name = "BtnOK";
            BtnOK.UseVisualStyleBackColor = true;
            // 
            // GbTags
            // 
            resources.ApplyResources(GbTags, "GbTags");
            GbTags.Controls.Add(BtnAddTag);
            GbTags.Controls.Add(TagTextBox);
            GbTags.Controls.Add(TagLayout);
            GbTags.Name = "GbTags";
            GbTags.TabStop = false;
            // 
            // BtnAddTag
            // 
            BtnAddTag.Image = Properties.Resources.add;
            resources.ApplyResources(BtnAddTag, "BtnAddTag");
            BtnAddTag.Name = "BtnAddTag";
            BtnAddTag.UseVisualStyleBackColor = true;
            BtnAddTag.Click += BtnAddTag_Click;
            // 
            // TagTextBox
            // 
            resources.ApplyResources(TagTextBox, "TagTextBox");
            TagTextBox.Name = "TagTextBox";
            // 
            // TagLayout
            // 
            resources.ApplyResources(TagLayout, "TagLayout");
            TagLayout.Name = "TagLayout";
            // 
            // ProjectSettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(GbTags);
            Controls.Add(BtnOK);
            Controls.Add(GbInput);
            Controls.Add(GbProjectFile);
            Name = "ProjectSettingsDialog";
            Load += ProjectSettingsDialog_Load;
            GbProjectFile.ResumeLayout(false);
            GbProjectFile.PerformLayout();
            GbInput.ResumeLayout(false);
            GbInput.PerformLayout();
            GbTags.ResumeLayout(false);
            GbTags.PerformLayout();
            ResumeLayout(false);
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
        private Label label4;
        private Label label8;
        private Button BtnOK;
        private GroupBox GbTags;
        private Button BtnAddTag;
        private TextBox TagTextBox;
        private FlowLayoutPanel TagLayout;
        public ComboBox CBRules;
    }
}