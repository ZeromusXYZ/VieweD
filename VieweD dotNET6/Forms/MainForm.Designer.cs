namespace VieweD.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            MM = new MenuStrip();
            MMFile = new ToolStripMenuItem();
            MMFileOpen = new ToolStripMenuItem();
            MMFileN1 = new ToolStripSeparator();
            MMFileSettings = new ToolStripMenuItem();
            MMFileN2 = new ToolStripSeparator();
            MMFileExit = new ToolStripMenuItem();
            MMVersion = new ToolStripMenuItem();
            MMProject = new ToolStripMenuItem();
            MMProjectSave = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            MMProjectGameData = new ToolStripMenuItem();
            MMProjectVideo = new ToolStripMenuItem();
            MMProjectN1 = new ToolStripSeparator();
            MMProjectSettings = new ToolStripMenuItem();
            MMProjectN2 = new ToolStripSeparator();
            MMProjectClose = new ToolStripMenuItem();
            MMSearch = new ToolStripMenuItem();
            MMSearchFind = new ToolStripMenuItem();
            MMSearchFindNext = new ToolStripMenuItem();
            MMSearchN1 = new ToolStripSeparator();
            MMSearchEditFilter = new ToolStripMenuItem();
            MMSearchApplyMenu = new ToolStripMenuItem();
            MMSearchApplyReset = new ToolStripMenuItem();
            MMSearchApplyMenuN1 = new ToolStripSeparator();
            MMSearchHighlightMenu = new ToolStripMenuItem();
            MMSearchHighlightReset = new ToolStripMenuItem();
            MMSearchHighlightMenuN1 = new ToolStripSeparator();
            MMLinks = new ToolStripMenuItem();
            MMLinksGitHub = new ToolStripMenuItem();
            MMLinksDikscord = new ToolStripMenuItem();
            MMLinksN1 = new ToolStripSeparator();
            MMLinksKoFi = new ToolStripMenuItem();
            MMLinksN2 = new ToolStripSeparator();
            MMLinks7Zip = new ToolStripMenuItem();
            MMLinks7ZipMain = new ToolStripMenuItem();
            MMLinks7ZipDownload = new ToolStripMenuItem();
            MMLinksVLC = new ToolStripMenuItem();
            MMLinksWireshark = new ToolStripMenuItem();
            StatusBar = new StatusStrip();
            StatusBarProgressBar = new ToolStripProgressBar();
            StatusBarEngineName = new ToolStripStatusLabel();
            StatusBarProject = new ToolStripStatusLabel();
            TCProjects = new TabControl();
            TPWelcome = new TabPage();
            TPWelcomeLayout = new TableLayoutPanel();
            RichTextWelcome = new RichTextBox();
            TPWelcomeBtnClose = new Button();
            ILTabs = new ImageList(components);
            DgvParsed = new DataGridView();
            DgvPosition = new DataGridViewTextBoxColumn();
            DgvName = new DataGridViewTextBoxColumn();
            DgvValue = new DataGridViewTextBoxColumn();
            MiFieldView = new ContextMenuStrip(components);
            MiFieldFields = new ToolStripMenuItem();
            MIFieldLocalVars = new ToolStripMenuItem();
            MiFieldN1 = new ToolStripSeparator();
            MiFieldDebug = new ToolStripMenuItem();
            OpenProjectFileDialog = new OpenFileDialog();
            SplitContainerListAndFields = new SplitContainer();
            SplitContainerFieldsAndRawData = new SplitContainer();
            LayoutRawAndSuggested = new TableLayoutPanel();
            RichTextData = new RichTextBox();
            SaveProjectFileDialog = new SaveFileDialog();
            MM.SuspendLayout();
            StatusBar.SuspendLayout();
            TCProjects.SuspendLayout();
            TPWelcome.SuspendLayout();
            TPWelcomeLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvParsed).BeginInit();
            MiFieldView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainerListAndFields).BeginInit();
            SplitContainerListAndFields.Panel1.SuspendLayout();
            SplitContainerListAndFields.Panel2.SuspendLayout();
            SplitContainerListAndFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainerFieldsAndRawData).BeginInit();
            SplitContainerFieldsAndRawData.Panel1.SuspendLayout();
            SplitContainerFieldsAndRawData.Panel2.SuspendLayout();
            SplitContainerFieldsAndRawData.SuspendLayout();
            LayoutRawAndSuggested.SuspendLayout();
            SuspendLayout();
            // 
            // MM
            // 
            MM.Items.AddRange(new ToolStripItem[] { MMFile, MMVersion, MMProject, MMSearch, MMLinks });
            resources.ApplyResources(MM, "MM");
            MM.Name = "MM";
            // 
            // MMFile
            // 
            MMFile.DropDownItems.AddRange(new ToolStripItem[] { MMFileOpen, MMFileN1, MMFileSettings, MMFileN2, MMFileExit });
            MMFile.Name = "MMFile";
            resources.ApplyResources(MMFile, "MMFile");
            // 
            // MMFileOpen
            // 
            MMFileOpen.Image = Properties.Resources.document_open_16;
            MMFileOpen.Name = "MMFileOpen";
            resources.ApplyResources(MMFileOpen, "MMFileOpen");
            MMFileOpen.Click += MMFileOpen_Click;
            // 
            // MMFileN1
            // 
            MMFileN1.Name = "MMFileN1";
            resources.ApplyResources(MMFileN1, "MMFileN1");
            // 
            // MMFileSettings
            // 
            MMFileSettings.Name = "MMFileSettings";
            resources.ApplyResources(MMFileSettings, "MMFileSettings");
            MMFileSettings.Click += MMFileSettings_Click;
            // 
            // MMFileN2
            // 
            MMFileN2.Name = "MMFileN2";
            resources.ApplyResources(MMFileN2, "MMFileN2");
            // 
            // MMFileExit
            // 
            MMFileExit.Image = Properties.Resources.application_exit_16;
            MMFileExit.Name = "MMFileExit";
            resources.ApplyResources(MMFileExit, "MMFileExit");
            MMFileExit.Click += MMFileExit_Click;
            // 
            // MMVersion
            // 
            MMVersion.Alignment = ToolStripItemAlignment.Right;
            resources.ApplyResources(MMVersion, "MMVersion");
            MMVersion.Name = "MMVersion";
            // 
            // MMProject
            // 
            MMProject.DropDownItems.AddRange(new ToolStripItem[] { MMProjectSave, toolStripMenuItem1, MMProjectGameData, MMProjectVideo, MMProjectN1, MMProjectSettings, MMProjectN2, MMProjectClose });
            MMProject.Name = "MMProject";
            resources.ApplyResources(MMProject, "MMProject");
            // 
            // MMProjectSave
            // 
            MMProjectSave.Image = Properties.Resources.document_save_16;
            MMProjectSave.Name = "MMProjectSave";
            resources.ApplyResources(MMProjectSave, "MMProjectSave");
            MMProjectSave.Click += MMProjectSave_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // MMProjectGameData
            // 
            MMProjectGameData.Name = "MMProjectGameData";
            resources.ApplyResources(MMProjectGameData, "MMProjectGameData");
            // 
            // MMProjectVideo
            // 
            MMProjectVideo.Name = "MMProjectVideo";
            resources.ApplyResources(MMProjectVideo, "MMProjectVideo");
            MMProjectVideo.Click += MMProjectVideo_Click;
            // 
            // MMProjectN1
            // 
            MMProjectN1.Name = "MMProjectN1";
            resources.ApplyResources(MMProjectN1, "MMProjectN1");
            // 
            // MMProjectSettings
            // 
            MMProjectSettings.Image = Properties.Resources.document_properties_16;
            MMProjectSettings.Name = "MMProjectSettings";
            resources.ApplyResources(MMProjectSettings, "MMProjectSettings");
            MMProjectSettings.Click += MMProjectSettings_Click;
            // 
            // MMProjectN2
            // 
            MMProjectN2.Name = "MMProjectN2";
            resources.ApplyResources(MMProjectN2, "MMProjectN2");
            // 
            // MMProjectClose
            // 
            MMProjectClose.Image = Properties.Resources.view_close_16;
            MMProjectClose.Name = "MMProjectClose";
            resources.ApplyResources(MMProjectClose, "MMProjectClose");
            MMProjectClose.Click += MMProjectClose_Click;
            // 
            // MMSearch
            // 
            MMSearch.DropDownItems.AddRange(new ToolStripItem[] { MMSearchFind, MMSearchFindNext, MMSearchN1, MMSearchEditFilter, MMSearchApplyMenu, MMSearchHighlightMenu });
            MMSearch.Name = "MMSearch";
            resources.ApplyResources(MMSearch, "MMSearch");
            // 
            // MMSearchFind
            // 
            MMSearchFind.Name = "MMSearchFind";
            resources.ApplyResources(MMSearchFind, "MMSearchFind");
            MMSearchFind.Click += MMSearchFind_Click;
            // 
            // MMSearchFindNext
            // 
            MMSearchFindNext.Name = "MMSearchFindNext";
            resources.ApplyResources(MMSearchFindNext, "MMSearchFindNext");
            MMSearchFindNext.Click += MMSearchFindNext_Click;
            // 
            // MMSearchN1
            // 
            MMSearchN1.Name = "MMSearchN1";
            resources.ApplyResources(MMSearchN1, "MMSearchN1");
            // 
            // MMSearchEditFilter
            // 
            MMSearchEditFilter.Name = "MMSearchEditFilter";
            resources.ApplyResources(MMSearchEditFilter, "MMSearchEditFilter");
            MMSearchEditFilter.Click += MMSearchEditFilter_Click;
            // 
            // MMSearchApplyMenu
            // 
            MMSearchApplyMenu.DropDownItems.AddRange(new ToolStripItem[] { MMSearchApplyReset, MMSearchApplyMenuN1 });
            MMSearchApplyMenu.Name = "MMSearchApplyMenu";
            resources.ApplyResources(MMSearchApplyMenu, "MMSearchApplyMenu");
            MMSearchApplyMenu.Tag = "ap";
            MMSearchApplyMenu.DropDownOpening += MMSearchFilterMenu_DropDownOpening;
            // 
            // MMSearchApplyReset
            // 
            MMSearchApplyReset.Name = "MMSearchApplyReset";
            resources.ApplyResources(MMSearchApplyReset, "MMSearchApplyReset");
            MMSearchApplyReset.Click += MMSearchFilterApplyFile_Click;
            // 
            // MMSearchApplyMenuN1
            // 
            MMSearchApplyMenuN1.Name = "MMSearchApplyMenuN1";
            resources.ApplyResources(MMSearchApplyMenuN1, "MMSearchApplyMenuN1");
            // 
            // MMSearchHighlightMenu
            // 
            MMSearchHighlightMenu.DropDownItems.AddRange(new ToolStripItem[] { MMSearchHighlightReset, MMSearchHighlightMenuN1 });
            MMSearchHighlightMenu.Name = "MMSearchHighlightMenu";
            resources.ApplyResources(MMSearchHighlightMenu, "MMSearchHighlightMenu");
            MMSearchHighlightMenu.Tag = "hl";
            MMSearchHighlightMenu.DropDownOpening += MMSearchFilterMenu_DropDownOpening;
            // 
            // MMSearchHighlightReset
            // 
            MMSearchHighlightReset.Name = "MMSearchHighlightReset";
            resources.ApplyResources(MMSearchHighlightReset, "MMSearchHighlightReset");
            MMSearchHighlightReset.Click += MMSearchFilterHighlightApplyFile_Click;
            // 
            // MMSearchHighlightMenuN1
            // 
            MMSearchHighlightMenuN1.Name = "MMSearchHighlightMenuN1";
            resources.ApplyResources(MMSearchHighlightMenuN1, "MMSearchHighlightMenuN1");
            // 
            // MMLinks
            // 
            MMLinks.DropDownItems.AddRange(new ToolStripItem[] { MMLinksGitHub, MMLinksDikscord, MMLinksN1, MMLinksKoFi, MMLinksN2, MMLinks7Zip, MMLinksVLC, MMLinksWireshark });
            MMLinks.Name = "MMLinks";
            resources.ApplyResources(MMLinks, "MMLinks");
            // 
            // MMLinksGitHub
            // 
            MMLinksGitHub.Image = Properties.Resources.github_mark;
            MMLinksGitHub.Name = "MMLinksGitHub";
            resources.ApplyResources(MMLinksGitHub, "MMLinksGitHub");
            MMLinksGitHub.Tag = "https://github.com/ZeromusXYZ/VieweD";
            MMLinksGitHub.Click += MMLinksOpen_Click;
            // 
            // MMLinksDikscord
            // 
            MMLinksDikscord.Image = Properties.Resources.discord_x32;
            MMLinksDikscord.Name = "MMLinksDikscord";
            resources.ApplyResources(MMLinksDikscord, "MMLinksDikscord");
            MMLinksDikscord.Tag = "https://discord.gg/GhVfDtK";
            MMLinksDikscord.Click += MMLinksOpen_Click;
            // 
            // MMLinksN1
            // 
            MMLinksN1.Name = "MMLinksN1";
            resources.ApplyResources(MMLinksN1, "MMLinksN1");
            // 
            // MMLinksKoFi
            // 
            MMLinksKoFi.Image = Properties.Resources.kofi_p_logo_nolabel_x32;
            MMLinksKoFi.Name = "MMLinksKoFi";
            resources.ApplyResources(MMLinksKoFi, "MMLinksKoFi");
            MMLinksKoFi.Tag = "https://ko-fi.com/zeromusxyz";
            MMLinksKoFi.Click += MMLinksOpen_Click;
            // 
            // MMLinksN2
            // 
            MMLinksN2.Name = "MMLinksN2";
            resources.ApplyResources(MMLinksN2, "MMLinksN2");
            // 
            // MMLinks7Zip
            // 
            MMLinks7Zip.DropDownItems.AddRange(new ToolStripItem[] { MMLinks7ZipMain, MMLinks7ZipDownload });
            resources.ApplyResources(MMLinks7Zip, "MMLinks7Zip");
            MMLinks7Zip.Image = Properties.Resources._7zip_x64;
            MMLinks7Zip.Name = "MMLinks7Zip";
            // 
            // MMLinks7ZipMain
            // 
            MMLinks7ZipMain.Name = "MMLinks7ZipMain";
            resources.ApplyResources(MMLinks7ZipMain, "MMLinks7ZipMain");
            MMLinks7ZipMain.Tag = "https://www.7-zip.org/";
            MMLinks7ZipMain.Click += MMLinksOpen_Click;
            // 
            // MMLinks7ZipDownload
            // 
            MMLinks7ZipDownload.Name = "MMLinks7ZipDownload";
            resources.ApplyResources(MMLinks7ZipDownload, "MMLinks7ZipDownload");
            MMLinks7ZipDownload.Tag = "https://sourceforge.net/p/sevenzip/discussion/45797/thread/adc65bfa/";
            MMLinks7ZipDownload.Click += MMLinksOpen_Click;
            // 
            // MMLinksVLC
            // 
            resources.ApplyResources(MMLinksVLC, "MMLinksVLC");
            MMLinksVLC.Image = Properties.Resources.cone_altglass_2_x48;
            MMLinksVLC.Name = "MMLinksVLC";
            MMLinksVLC.Tag = "https://www.videolan.org/";
            MMLinksVLC.Click += MMLinksOpen_Click;
            // 
            // MMLinksWireshark
            // 
            resources.ApplyResources(MMLinksWireshark, "MMLinksWireshark");
            MMLinksWireshark.Image = Properties.Resources.wireshark_x48;
            MMLinksWireshark.Name = "MMLinksWireshark";
            MMLinksWireshark.Tag = "https://www.wireshark.org/";
            MMLinksWireshark.Click += MMLinksOpen_Click;
            // 
            // StatusBar
            // 
            StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarProgressBar, StatusBarEngineName, StatusBarProject });
            resources.ApplyResources(StatusBar, "StatusBar");
            StatusBar.Name = "StatusBar";
            // 
            // StatusBarProgressBar
            // 
            resources.ApplyResources(StatusBarProgressBar, "StatusBarProgressBar");
            StatusBarProgressBar.Name = "StatusBarProgressBar";
            // 
            // StatusBarEngineName
            // 
            StatusBarEngineName.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusBarEngineName.BorderStyle = Border3DStyle.SunkenInner;
            resources.ApplyResources(StatusBarEngineName, "StatusBarEngineName");
            StatusBarEngineName.Name = "StatusBarEngineName";
            // 
            // StatusBarProject
            // 
            StatusBarProject.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusBarProject.BorderStyle = Border3DStyle.SunkenInner;
            resources.ApplyResources(StatusBarProject, "StatusBarProject");
            StatusBarProject.Name = "StatusBarProject";
            StatusBarProject.Spring = true;
            // 
            // TCProjects
            // 
            resources.ApplyResources(TCProjects, "TCProjects");
            TCProjects.Controls.Add(TPWelcome);
            TCProjects.ImageList = ILTabs;
            TCProjects.Multiline = true;
            TCProjects.Name = "TCProjects";
            TCProjects.SelectedIndex = 0;
            TCProjects.DrawItem += TCProjects_DrawItem;
            TCProjects.SelectedIndexChanged += TCProjects_SelectedIndexChanged;
            TCProjects.MouseDoubleClick += TCProjects_MouseDoubleClick;
            // 
            // TPWelcome
            // 
            TPWelcome.Controls.Add(TPWelcomeLayout);
            resources.ApplyResources(TPWelcome, "TPWelcome");
            TPWelcome.Name = "TPWelcome";
            TPWelcome.UseVisualStyleBackColor = true;
            // 
            // TPWelcomeLayout
            // 
            resources.ApplyResources(TPWelcomeLayout, "TPWelcomeLayout");
            TPWelcomeLayout.Controls.Add(RichTextWelcome, 0, 0);
            TPWelcomeLayout.Controls.Add(TPWelcomeBtnClose, 0, 1);
            TPWelcomeLayout.Name = "TPWelcomeLayout";
            // 
            // RichTextWelcome
            // 
            resources.ApplyResources(RichTextWelcome, "RichTextWelcome");
            RichTextWelcome.Name = "RichTextWelcome";
            RichTextWelcome.ReadOnly = true;
            // 
            // TPWelcomeBtnClose
            // 
            resources.ApplyResources(TPWelcomeBtnClose, "TPWelcomeBtnClose");
            TPWelcomeBtnClose.Name = "TPWelcomeBtnClose";
            TPWelcomeBtnClose.UseVisualStyleBackColor = true;
            TPWelcomeBtnClose.Click += TPWelcomeBtnClose_Click;
            // 
            // ILTabs
            // 
            ILTabs.ColorDepth = ColorDepth.Depth8Bit;
            ILTabs.ImageStream = (ImageListStreamer)resources.GetObject("ILTabs.ImageStream");
            ILTabs.TransparentColor = Color.Transparent;
            ILTabs.Images.SetKeyName(0, "close.png");
            ILTabs.Images.SetKeyName(1, "found_it!x44.png");
            ILTabs.Images.SetKeyName(2, "add.png");
            ILTabs.Images.SetKeyName(3, "mini_unk_icon.png");
            ILTabs.Images.SetKeyName(4, "view-visible-16.png");
            // 
            // DgvParsed
            // 
            DgvParsed.AllowUserToAddRows = false;
            DgvParsed.AllowUserToDeleteRows = false;
            DgvParsed.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            DgvParsed.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            DgvParsed.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            DgvParsed.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvParsed.Columns.AddRange(new DataGridViewColumn[] { DgvPosition, DgvName, DgvValue });
            DgvParsed.ContextMenuStrip = MiFieldView;
            resources.ApplyResources(DgvParsed, "DgvParsed");
            DgvParsed.EditMode = DataGridViewEditMode.EditOnF2;
            DgvParsed.Name = "DgvParsed";
            DgvParsed.RowHeadersVisible = false;
            DgvParsed.RowTemplate.Height = 25;
            DgvParsed.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvParsed.SelectionChanged += DgvParsed_SelectionChanged;
            // 
            // DgvPosition
            // 
            DgvPosition.FillWeight = 20F;
            DgvPosition.Frozen = true;
            resources.ApplyResources(DgvPosition, "DgvPosition");
            DgvPosition.MaxInputLength = 64;
            DgvPosition.Name = "DgvPosition";
            // 
            // DgvName
            // 
            DgvName.FillWeight = 30F;
            resources.ApplyResources(DgvName, "DgvName");
            DgvName.MaxInputLength = 128;
            DgvName.Name = "DgvName";
            // 
            // DgvValue
            // 
            DgvValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(DgvValue, "DgvValue");
            DgvValue.MaxInputLength = 4096;
            DgvValue.Name = "DgvValue";
            // 
            // MiFieldView
            // 
            MiFieldView.Items.AddRange(new ToolStripItem[] { MiFieldFields, MIFieldLocalVars, MiFieldN1, MiFieldDebug });
            MiFieldView.Name = "MiFieldView";
            resources.ApplyResources(MiFieldView, "MiFieldView");
            MiFieldView.Opening += MiFieldView_Opening;
            // 
            // MiFieldFields
            // 
            MiFieldFields.Checked = true;
            MiFieldFields.CheckState = CheckState.Checked;
            MiFieldFields.Name = "MiFieldFields";
            resources.ApplyResources(MiFieldFields, "MiFieldFields");
            MiFieldFields.Click += MiFieldFields_Click;
            // 
            // MIFieldLocalVars
            // 
            MIFieldLocalVars.Name = "MIFieldLocalVars";
            resources.ApplyResources(MIFieldLocalVars, "MIFieldLocalVars");
            MIFieldLocalVars.Click += MIFieldLocalVars_Click;
            // 
            // MiFieldN1
            // 
            MiFieldN1.Name = "MiFieldN1";
            resources.ApplyResources(MiFieldN1, "MiFieldN1");
            // 
            // MiFieldDebug
            // 
            MiFieldDebug.Name = "MiFieldDebug";
            resources.ApplyResources(MiFieldDebug, "MiFieldDebug");
            MiFieldDebug.Click += MiFieldDebug_Click;
            // 
            // OpenProjectFileDialog
            // 
            resources.ApplyResources(OpenProjectFileDialog, "OpenProjectFileDialog");
            // 
            // SplitContainerListAndFields
            // 
            resources.ApplyResources(SplitContainerListAndFields, "SplitContainerListAndFields");
            SplitContainerListAndFields.Name = "SplitContainerListAndFields";
            // 
            // SplitContainerListAndFields.Panel1
            // 
            SplitContainerListAndFields.Panel1.Controls.Add(TCProjects);
            // 
            // SplitContainerListAndFields.Panel2
            // 
            SplitContainerListAndFields.Panel2.Controls.Add(SplitContainerFieldsAndRawData);
            // 
            // SplitContainerFieldsAndRawData
            // 
            resources.ApplyResources(SplitContainerFieldsAndRawData, "SplitContainerFieldsAndRawData");
            SplitContainerFieldsAndRawData.Name = "SplitContainerFieldsAndRawData";
            // 
            // SplitContainerFieldsAndRawData.Panel1
            // 
            SplitContainerFieldsAndRawData.Panel1.Controls.Add(DgvParsed);
            // 
            // SplitContainerFieldsAndRawData.Panel2
            // 
            SplitContainerFieldsAndRawData.Panel2.Controls.Add(LayoutRawAndSuggested);
            // 
            // LayoutRawAndSuggested
            // 
            resources.ApplyResources(LayoutRawAndSuggested, "LayoutRawAndSuggested");
            LayoutRawAndSuggested.Controls.Add(RichTextData, 0, 0);
            LayoutRawAndSuggested.Name = "LayoutRawAndSuggested";
            // 
            // RichTextData
            // 
            resources.ApplyResources(RichTextData, "RichTextData");
            RichTextData.Name = "RichTextData";
            RichTextData.ReadOnly = true;
            // 
            // SaveProjectFileDialog
            // 
            SaveProjectFileDialog.DefaultExt = "pvd";
            resources.ApplyResources(SaveProjectFileDialog, "SaveProjectFileDialog");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(SplitContainerListAndFields);
            Controls.Add(StatusBar);
            Controls.Add(MM);
            MainMenuStrip = MM;
            Name = "MainForm";
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            MM.ResumeLayout(false);
            MM.PerformLayout();
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            TCProjects.ResumeLayout(false);
            TPWelcome.ResumeLayout(false);
            TPWelcomeLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DgvParsed).EndInit();
            MiFieldView.ResumeLayout(false);
            SplitContainerListAndFields.Panel1.ResumeLayout(false);
            SplitContainerListAndFields.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainerListAndFields).EndInit();
            SplitContainerListAndFields.ResumeLayout(false);
            SplitContainerFieldsAndRawData.Panel1.ResumeLayout(false);
            SplitContainerFieldsAndRawData.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainerFieldsAndRawData).EndInit();
            SplitContainerFieldsAndRawData.ResumeLayout(false);
            LayoutRawAndSuggested.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip MM;
        private ToolStripMenuItem MMFile;
        private ToolStripMenuItem MMFileOpen;
        private ToolStripSeparator MMFileN1;
        private ToolStripMenuItem MMFileSettings;
        private ToolStripSeparator MMFileN2;
        private ToolStripMenuItem MMFileExit;
        private StatusStrip StatusBar;
        private ToolStripStatusLabel StatusBarEngineName;
        private ToolStripStatusLabel StatusBarProject;
        private ToolStripMenuItem MMVersion;
        private TabControl TCProjects;
        private TabPage TPWelcome;
        private OpenFileDialog OpenProjectFileDialog;
        private ToolStripProgressBar StatusBarProgressBar;
        private DataGridView DgvParsed;
        private DataGridViewTextBoxColumn DgvPosition;
        private DataGridViewTextBoxColumn DgvName;
        private DataGridViewTextBoxColumn DgvValue;
        private SplitContainer SplitContainerListAndFields;
        private SplitContainer SplitContainerFieldsAndRawData;
        private TableLayoutPanel LayoutRawAndSuggested;
        private RichTextBox RichTextData;
        private ToolStripMenuItem MMProject;
        private ToolStripMenuItem MMProjectSettings;
        private ToolStripMenuItem MMProjectClose;
        private ToolStripSeparator MMProjectN2;
        private RichTextBox RichTextWelcome;
        private TableLayoutPanel TPWelcomeLayout;
        private Button TPWelcomeBtnClose;
        private ToolStripMenuItem MMProjectSave;
        private ToolStripSeparator MMProjectN1;
        private ImageList ILTabs;
        private ToolStripMenuItem MMProjectVideo;
        private ToolStripMenuItem MMProjectGameData;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem MMLinks;
        private ToolStripMenuItem MMLinksGitHub;
        private ToolStripMenuItem MMLinksDikscord;
        private ToolStripSeparator MMLinksN1;
        private ToolStripMenuItem MMLinksKoFi;
        private ToolStripSeparator MMLinksN2;
        private ToolStripMenuItem MMLinks7Zip;
        private ToolStripMenuItem MMLinksVLC;
        private ToolStripMenuItem MMLinksWireshark;
        private ToolStripMenuItem MMLinks7ZipMain;
        private ToolStripMenuItem MMLinks7ZipDownload;
        private ContextMenuStrip MiFieldView;
        private ToolStripMenuItem MiFieldFields;
        private ToolStripMenuItem MIFieldLocalVars;
        private ToolStripSeparator MiFieldN1;
        private ToolStripMenuItem MiFieldDebug;
        private ToolStripMenuItem MMSearch;
        private ToolStripMenuItem MMSearchFind;
        private ToolStripMenuItem MMSearchFindNext;
        private ToolStripSeparator MMSearchN1;
        private ToolStripMenuItem MMSearchEditFilter;
        private ToolStripMenuItem MMSearchApplyMenu;
        private ToolStripMenuItem MMSearchApplyReset;
        private ToolStripMenuItem MMSearchHighlightMenu;
        private ToolStripMenuItem MMSearchHighlightReset;
        private ToolStripSeparator MMSearchApplyMenuN1;
        private ToolStripSeparator MMSearchHighlightMenuN1;
        private SaveFileDialog SaveProjectFileDialog;
    }
}