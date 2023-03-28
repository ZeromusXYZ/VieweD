using System.Windows.Forms;

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
            DecryptionKeyNameLabel = new Label();
            label5 = new Label();
            CbRules = new ComboBox();
            label4 = new Label();
            label8 = new Label();
            CbInputReader = new ComboBox();
            CbParser = new ComboBox();
            label3 = new Label();
            BtnOK = new Button();
            GbTags = new GroupBox();
            BtnAddTag = new Button();
            TagTextBox = new TextBox();
            TagLayout = new FlowLayoutPanel();
            ProjectSettingsTabs = new TabControl();
            TPInputSettings = new TabPage();
            TPCommunity = new TabPage();
            GBDescription = new GroupBox();
            TextDescription = new TextBox();
            GBSummary = new GroupBox();
            CBHideUrlPreviews = new CheckBox();
            CBIncludePacketNames = new CheckBox();
            BtnCopySummary = new Button();
            CBIncludePacketIds = new CheckBox();
            GBOnlineFiles = new GroupBox();
            TextVideoURL = new TextBox();
            label9 = new Label();
            TextProjectURL = new TextBox();
            label7 = new Label();
            TableLayoutTabsButtons = new TableLayoutPanel();
            ButtonsPanel = new Panel();
            GbProjectFile.SuspendLayout();
            GbInput.SuspendLayout();
            GbTags.SuspendLayout();
            ProjectSettingsTabs.SuspendLayout();
            TPInputSettings.SuspendLayout();
            TPCommunity.SuspendLayout();
            GBDescription.SuspendLayout();
            GBSummary.SuspendLayout();
            GBOnlineFiles.SuspendLayout();
            TableLayoutTabsButtons.SuspendLayout();
            ButtonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // GbProjectFile
            // 
            resources.ApplyResources(GbProjectFile, "GbProjectFile");
            GbProjectFile.Controls.Add(label2);
            GbProjectFile.Controls.Add(TextVideoFile);
            GbProjectFile.Controls.Add(label6);
            GbProjectFile.Controls.Add(TextProjectFile);
            GbProjectFile.Controls.Add(label1);
            GbProjectFile.Controls.Add(TextLogFile);
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
            resources.ApplyResources(GbInput, "GbInput");
            GbInput.Controls.Add(DecryptionKeyNameLabel);
            GbInput.Controls.Add(label5);
            GbInput.Controls.Add(CbRules);
            GbInput.Controls.Add(label4);
            GbInput.Controls.Add(label8);
            GbInput.Controls.Add(CbInputReader);
            GbInput.Controls.Add(CbParser);
            GbInput.Controls.Add(label3);
            GbInput.Name = "GbInput";
            GbInput.TabStop = false;
            // 
            // DecryptionKeyNameLabel
            // 
            resources.ApplyResources(DecryptionKeyNameLabel, "DecryptionKeyNameLabel");
            DecryptionKeyNameLabel.Name = "DecryptionKeyNameLabel";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // CbRules
            // 
            CbRules.DropDownStyle = ComboBoxStyle.DropDownList;
            CbRules.FormattingEnabled = true;
            resources.ApplyResources(CbRules, "CbRules");
            CbRules.Name = "CbRules";
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
            // CbInputReader
            // 
            CbInputReader.DropDownStyle = ComboBoxStyle.DropDownList;
            CbInputReader.FormattingEnabled = true;
            resources.ApplyResources(CbInputReader, "CbInputReader");
            CbInputReader.Name = "CbInputReader";
            // 
            // CbParser
            // 
            CbParser.DropDownStyle = ComboBoxStyle.DropDownList;
            CbParser.FormattingEnabled = true;
            resources.ApplyResources(CbParser, "CbParser");
            CbParser.Name = "CbParser";
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
            // ProjectSettingsTabs
            // 
            ProjectSettingsTabs.Controls.Add(TPInputSettings);
            ProjectSettingsTabs.Controls.Add(TPCommunity);
            resources.ApplyResources(ProjectSettingsTabs, "ProjectSettingsTabs");
            ProjectSettingsTabs.Name = "ProjectSettingsTabs";
            ProjectSettingsTabs.SelectedIndex = 0;
            // 
            // TPInputSettings
            // 
            TPInputSettings.Controls.Add(GbProjectFile);
            TPInputSettings.Controls.Add(GbTags);
            TPInputSettings.Controls.Add(GbInput);
            resources.ApplyResources(TPInputSettings, "TPInputSettings");
            TPInputSettings.Name = "TPInputSettings";
            TPInputSettings.UseVisualStyleBackColor = true;
            // 
            // TPCommunity
            // 
            TPCommunity.Controls.Add(GBDescription);
            TPCommunity.Controls.Add(GBSummary);
            TPCommunity.Controls.Add(GBOnlineFiles);
            resources.ApplyResources(TPCommunity, "TPCommunity");
            TPCommunity.Name = "TPCommunity";
            TPCommunity.UseVisualStyleBackColor = true;
            // 
            // GBDescription
            // 
            GBDescription.Controls.Add(TextDescription);
            resources.ApplyResources(GBDescription, "GBDescription");
            GBDescription.Name = "GBDescription";
            GBDescription.TabStop = false;
            // 
            // TextDescription
            // 
            resources.ApplyResources(TextDescription, "TextDescription");
            TextDescription.Name = "TextDescription";
            // 
            // GBSummary
            // 
            resources.ApplyResources(GBSummary, "GBSummary");
            GBSummary.Controls.Add(CBHideUrlPreviews);
            GBSummary.Controls.Add(CBIncludePacketNames);
            GBSummary.Controls.Add(BtnCopySummary);
            GBSummary.Controls.Add(CBIncludePacketIds);
            GBSummary.Name = "GBSummary";
            GBSummary.TabStop = false;
            // 
            // CBHideUrlPreviews
            // 
            resources.ApplyResources(CBHideUrlPreviews, "CBHideUrlPreviews");
            CBHideUrlPreviews.Checked = true;
            CBHideUrlPreviews.CheckState = CheckState.Checked;
            CBHideUrlPreviews.Name = "CBHideUrlPreviews";
            CBHideUrlPreviews.UseVisualStyleBackColor = true;
            // 
            // CBIncludePacketNames
            // 
            resources.ApplyResources(CBIncludePacketNames, "CBIncludePacketNames");
            CBIncludePacketNames.Name = "CBIncludePacketNames";
            CBIncludePacketNames.UseVisualStyleBackColor = true;
            // 
            // BtnCopySummary
            // 
            BtnCopySummary.Image = Properties.Resources.document_properties_16;
            resources.ApplyResources(BtnCopySummary, "BtnCopySummary");
            BtnCopySummary.Name = "BtnCopySummary";
            BtnCopySummary.UseVisualStyleBackColor = true;
            BtnCopySummary.Click += BtnCopySummary_Click;
            // 
            // CBIncludePacketIds
            // 
            resources.ApplyResources(CBIncludePacketIds, "CBIncludePacketIds");
            CBIncludePacketIds.Checked = true;
            CBIncludePacketIds.CheckState = CheckState.Checked;
            CBIncludePacketIds.Name = "CBIncludePacketIds";
            CBIncludePacketIds.UseVisualStyleBackColor = true;
            // 
            // GBOnlineFiles
            // 
            resources.ApplyResources(GBOnlineFiles, "GBOnlineFiles");
            GBOnlineFiles.Controls.Add(TextVideoURL);
            GBOnlineFiles.Controls.Add(label9);
            GBOnlineFiles.Controls.Add(TextProjectURL);
            GBOnlineFiles.Controls.Add(label7);
            GBOnlineFiles.Name = "GBOnlineFiles";
            GBOnlineFiles.TabStop = false;
            // 
            // TextVideoURL
            // 
            resources.ApplyResources(TextVideoURL, "TextVideoURL");
            TextVideoURL.Name = "TextVideoURL";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // TextProjectURL
            // 
            resources.ApplyResources(TextProjectURL, "TextProjectURL");
            TextProjectURL.Name = "TextProjectURL";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // TableLayoutTabsButtons
            // 
            resources.ApplyResources(TableLayoutTabsButtons, "TableLayoutTabsButtons");
            TableLayoutTabsButtons.Controls.Add(ProjectSettingsTabs, 0, 0);
            TableLayoutTabsButtons.Controls.Add(ButtonsPanel, 0, 1);
            TableLayoutTabsButtons.Name = "TableLayoutTabsButtons";
            // 
            // ButtonsPanel
            // 
            ButtonsPanel.Controls.Add(BtnOK);
            resources.ApplyResources(ButtonsPanel, "ButtonsPanel");
            ButtonsPanel.Name = "ButtonsPanel";
            // 
            // ProjectSettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TableLayoutTabsButtons);
            Name = "ProjectSettingsDialog";
            FormClosed += ProjectSettingsDialog_FormClosed;
            Load += ProjectSettingsDialog_Load;
            GbProjectFile.ResumeLayout(false);
            GbProjectFile.PerformLayout();
            GbInput.ResumeLayout(false);
            GbInput.PerformLayout();
            GbTags.ResumeLayout(false);
            GbTags.PerformLayout();
            ProjectSettingsTabs.ResumeLayout(false);
            TPInputSettings.ResumeLayout(false);
            TPCommunity.ResumeLayout(false);
            GBDescription.ResumeLayout(false);
            GBDescription.PerformLayout();
            GBSummary.ResumeLayout(false);
            GBSummary.PerformLayout();
            GBOnlineFiles.ResumeLayout(false);
            GBOnlineFiles.PerformLayout();
            TableLayoutTabsButtons.ResumeLayout(false);
            ButtonsPanel.ResumeLayout(false);
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
        private ComboBox CbInputReader;
        private ComboBox CbParser;
        private Label label4;
        private Label label8;
        private Button BtnOK;
        private GroupBox GbTags;
        private Button BtnAddTag;
        private TextBox TagTextBox;
        private FlowLayoutPanel TagLayout;
        public ComboBox CbRules;
        private Label DecryptionKeyNameLabel;
        private Label label5;
        private TabControl ProjectSettingsTabs;
        private TabPage TPInputSettings;
        private TabPage TPCommunity;
        private TableLayoutPanel TableLayoutTabsButtons;
        private Panel ButtonsPanel;
        private Button BtnCopySummary;
        private GroupBox GBOnlineFiles;
        private Label label9;
        private Label label7;
        private CheckBox CBIncludePacketIds;
        private GroupBox GBSummary;
        private CheckBox CBIncludePacketNames;
        private CheckBox CBHideUrlPreviews;
        public TextBox TextVideoURL;
        public TextBox TextProjectURL;
        private GroupBox GBDescription;
        public TextBox TextDescription;
    }
}