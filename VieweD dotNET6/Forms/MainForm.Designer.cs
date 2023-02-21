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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MM = new System.Windows.Forms.MenuStrip();
            this.MMFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MMFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MMFileN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MMFileSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MMFileN2 = new System.Windows.Forms.ToolStripSeparator();
            this.MMFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MMVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.MMProject = new System.Windows.Forms.ToolStripMenuItem();
            this.MMProjectSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.projectDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MMProjectN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MMProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MMProjectN2 = new System.Windows.Forms.ToolStripSeparator();
            this.MMProjectClose = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinks = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksGitHub = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksDikscord = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MMLinksKoFi = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksN2 = new System.Windows.Forms.ToolStripSeparator();
            this.MMLinks7Zip = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinks7ZipMain = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinks7ZipDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksVLC = new System.Windows.Forms.ToolStripMenuItem();
            this.MMLinksWireshark = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusBarProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusBarEngineName = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.TCProjects = new System.Windows.Forms.TabControl();
            this.TPWelcome = new System.Windows.Forms.TabPage();
            this.TPWelcomeLayout = new System.Windows.Forms.TableLayoutPanel();
            this.RichTextWelcome = new System.Windows.Forms.RichTextBox();
            this.TPWelcomeBtnClose = new System.Windows.Forms.Button();
            this.ILTabs = new System.Windows.Forms.ImageList(this.components);
            this.DgvParsed = new System.Windows.Forms.DataGridView();
            this.DgvPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgvName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgvValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MiFieldView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MiFieldFields = new System.Windows.Forms.ToolStripMenuItem();
            this.MIFieldLocalVars = new System.Windows.Forms.ToolStripMenuItem();
            this.MiFieldN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MiFieldDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SplitContainerListAndFields = new System.Windows.Forms.SplitContainer();
            this.SplitContainerFieldsAndRawData = new System.Windows.Forms.SplitContainer();
            this.LayoutRawAndSuggested = new System.Windows.Forms.TableLayoutPanel();
            this.RichTextData = new System.Windows.Forms.RichTextBox();
            this.MM.SuspendLayout();
            this.StatusBar.SuspendLayout();
            this.TCProjects.SuspendLayout();
            this.TPWelcome.SuspendLayout();
            this.TPWelcomeLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvParsed)).BeginInit();
            this.MiFieldView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerListAndFields)).BeginInit();
            this.SplitContainerListAndFields.Panel1.SuspendLayout();
            this.SplitContainerListAndFields.Panel2.SuspendLayout();
            this.SplitContainerListAndFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerFieldsAndRawData)).BeginInit();
            this.SplitContainerFieldsAndRawData.Panel1.SuspendLayout();
            this.SplitContainerFieldsAndRawData.Panel2.SuspendLayout();
            this.SplitContainerFieldsAndRawData.SuspendLayout();
            this.LayoutRawAndSuggested.SuspendLayout();
            this.SuspendLayout();
            // 
            // MM
            // 
            this.MM.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMFile,
            this.MMVersion,
            this.MMProject,
            this.MMLinks});
            resources.ApplyResources(this.MM, "MM");
            this.MM.Name = "MM";
            // 
            // MMFile
            // 
            this.MMFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMFileOpen,
            this.MMFileN1,
            this.MMFileSettings,
            this.MMFileN2,
            this.MMFileExit});
            this.MMFile.Name = "MMFile";
            resources.ApplyResources(this.MMFile, "MMFile");
            // 
            // MMFileOpen
            // 
            this.MMFileOpen.Image = global::VieweD.Properties.Resources.document_open_16;
            this.MMFileOpen.Name = "MMFileOpen";
            resources.ApplyResources(this.MMFileOpen, "MMFileOpen");
            this.MMFileOpen.Click += new System.EventHandler(this.MMFileOpen_Click);
            // 
            // MMFileN1
            // 
            this.MMFileN1.Name = "MMFileN1";
            resources.ApplyResources(this.MMFileN1, "MMFileN1");
            // 
            // MMFileSettings
            // 
            this.MMFileSettings.Name = "MMFileSettings";
            resources.ApplyResources(this.MMFileSettings, "MMFileSettings");
            // 
            // MMFileN2
            // 
            this.MMFileN2.Name = "MMFileN2";
            resources.ApplyResources(this.MMFileN2, "MMFileN2");
            // 
            // MMFileExit
            // 
            this.MMFileExit.Image = global::VieweD.Properties.Resources.application_exit_16;
            this.MMFileExit.Name = "MMFileExit";
            resources.ApplyResources(this.MMFileExit, "MMFileExit");
            this.MMFileExit.Click += new System.EventHandler(this.MMFileExit_Click);
            // 
            // MMVersion
            // 
            this.MMVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.MMVersion, "MMVersion");
            this.MMVersion.Name = "MMVersion";
            // 
            // MMProject
            // 
            this.MMProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMProjectSave,
            this.toolStripMenuItem1,
            this.projectDataToolStripMenuItem,
            this.videoLinkToolStripMenuItem,
            this.MMProjectN1,
            this.MMProjectSettings,
            this.MMProjectN2,
            this.MMProjectClose});
            this.MMProject.Name = "MMProject";
            resources.ApplyResources(this.MMProject, "MMProject");
            // 
            // MMProjectSave
            // 
            this.MMProjectSave.Image = global::VieweD.Properties.Resources.document_save_16;
            this.MMProjectSave.Name = "MMProjectSave";
            resources.ApplyResources(this.MMProjectSave, "MMProjectSave");
            this.MMProjectSave.Click += new System.EventHandler(this.MMProjectSave_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // projectDataToolStripMenuItem
            // 
            this.projectDataToolStripMenuItem.Name = "projectDataToolStripMenuItem";
            resources.ApplyResources(this.projectDataToolStripMenuItem, "projectDataToolStripMenuItem");
            // 
            // videoLinkToolStripMenuItem
            // 
            this.videoLinkToolStripMenuItem.Name = "videoLinkToolStripMenuItem";
            resources.ApplyResources(this.videoLinkToolStripMenuItem, "videoLinkToolStripMenuItem");
            // 
            // MMProjectN1
            // 
            this.MMProjectN1.Name = "MMProjectN1";
            resources.ApplyResources(this.MMProjectN1, "MMProjectN1");
            // 
            // MMProjectSettings
            // 
            this.MMProjectSettings.Image = global::VieweD.Properties.Resources.document_properties_16;
            this.MMProjectSettings.Name = "MMProjectSettings";
            resources.ApplyResources(this.MMProjectSettings, "MMProjectSettings");
            this.MMProjectSettings.Click += new System.EventHandler(this.MMProjectSettings_Click);
            // 
            // MMProjectN2
            // 
            this.MMProjectN2.Name = "MMProjectN2";
            resources.ApplyResources(this.MMProjectN2, "MMProjectN2");
            // 
            // MMProjectClose
            // 
            this.MMProjectClose.Image = global::VieweD.Properties.Resources.view_close_16;
            this.MMProjectClose.Name = "MMProjectClose";
            resources.ApplyResources(this.MMProjectClose, "MMProjectClose");
            this.MMProjectClose.Click += new System.EventHandler(this.MMProjectClose_Click);
            // 
            // MMLinks
            // 
            this.MMLinks.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMLinksGitHub,
            this.MMLinksDikscord,
            this.MMLinksN1,
            this.MMLinksKoFi,
            this.MMLinksN2,
            this.MMLinks7Zip,
            this.MMLinksVLC,
            this.MMLinksWireshark});
            this.MMLinks.Name = "MMLinks";
            resources.ApplyResources(this.MMLinks, "MMLinks");
            // 
            // MMLinksGitHub
            // 
            this.MMLinksGitHub.Image = global::VieweD.Properties.Resources.github_mark;
            this.MMLinksGitHub.Name = "MMLinksGitHub";
            resources.ApplyResources(this.MMLinksGitHub, "MMLinksGitHub");
            this.MMLinksGitHub.Tag = "https://github.com/ZeromusXYZ/VieweD";
            this.MMLinksGitHub.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinksDikscord
            // 
            this.MMLinksDikscord.Image = global::VieweD.Properties.Resources.discord_x32;
            this.MMLinksDikscord.Name = "MMLinksDikscord";
            resources.ApplyResources(this.MMLinksDikscord, "MMLinksDikscord");
            this.MMLinksDikscord.Tag = "https://discord.gg/GhVfDtK";
            this.MMLinksDikscord.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinksN1
            // 
            this.MMLinksN1.Name = "MMLinksN1";
            resources.ApplyResources(this.MMLinksN1, "MMLinksN1");
            // 
            // MMLinksKoFi
            // 
            this.MMLinksKoFi.Image = global::VieweD.Properties.Resources.kofi_p_logo_nolabel_x32;
            this.MMLinksKoFi.Name = "MMLinksKoFi";
            resources.ApplyResources(this.MMLinksKoFi, "MMLinksKoFi");
            this.MMLinksKoFi.Tag = "https://ko-fi.com/zeromusxyz";
            this.MMLinksKoFi.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinksN2
            // 
            this.MMLinksN2.Name = "MMLinksN2";
            resources.ApplyResources(this.MMLinksN2, "MMLinksN2");
            // 
            // MMLinks7Zip
            // 
            this.MMLinks7Zip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMLinks7ZipMain,
            this.MMLinks7ZipDownload});
            resources.ApplyResources(this.MMLinks7Zip, "MMLinks7Zip");
            this.MMLinks7Zip.Image = global::VieweD.Properties.Resources._7zip_x64;
            this.MMLinks7Zip.Name = "MMLinks7Zip";
            // 
            // MMLinks7ZipMain
            // 
            this.MMLinks7ZipMain.Name = "MMLinks7ZipMain";
            resources.ApplyResources(this.MMLinks7ZipMain, "MMLinks7ZipMain");
            this.MMLinks7ZipMain.Tag = "https://www.7-zip.org/";
            this.MMLinks7ZipMain.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinks7ZipDownload
            // 
            this.MMLinks7ZipDownload.Name = "MMLinks7ZipDownload";
            resources.ApplyResources(this.MMLinks7ZipDownload, "MMLinks7ZipDownload");
            this.MMLinks7ZipDownload.Tag = "https://sourceforge.net/p/sevenzip/discussion/45797/thread/adc65bfa/";
            this.MMLinks7ZipDownload.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinksVLC
            // 
            resources.ApplyResources(this.MMLinksVLC, "MMLinksVLC");
            this.MMLinksVLC.Image = global::VieweD.Properties.Resources.cone_altglass_2_x48;
            this.MMLinksVLC.Name = "MMLinksVLC";
            this.MMLinksVLC.Tag = "https://www.videolan.org/";
            this.MMLinksVLC.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // MMLinksWireshark
            // 
            resources.ApplyResources(this.MMLinksWireshark, "MMLinksWireshark");
            this.MMLinksWireshark.Image = global::VieweD.Properties.Resources.wireshark_x48;
            this.MMLinksWireshark.Name = "MMLinksWireshark";
            this.MMLinksWireshark.Tag = "https://www.wireshark.org/";
            this.MMLinksWireshark.Click += new System.EventHandler(this.MMLinksOpen_Click);
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBarProgressBar,
            this.StatusBarEngineName,
            this.StatusBarProject});
            resources.ApplyResources(this.StatusBar, "StatusBar");
            this.StatusBar.Name = "StatusBar";
            // 
            // StatusBarProgressBar
            // 
            resources.ApplyResources(this.StatusBarProgressBar, "StatusBarProgressBar");
            this.StatusBarProgressBar.Name = "StatusBarProgressBar";
            // 
            // StatusBarEngineName
            // 
            this.StatusBarEngineName.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusBarEngineName.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            resources.ApplyResources(this.StatusBarEngineName, "StatusBarEngineName");
            this.StatusBarEngineName.Name = "StatusBarEngineName";
            // 
            // StatusBarProject
            // 
            this.StatusBarProject.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.StatusBarProject.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            resources.ApplyResources(this.StatusBarProject, "StatusBarProject");
            this.StatusBarProject.Name = "StatusBarProject";
            this.StatusBarProject.Spring = true;
            // 
            // TCProjects
            // 
            resources.ApplyResources(this.TCProjects, "TCProjects");
            this.TCProjects.Controls.Add(this.TPWelcome);
            this.TCProjects.ImageList = this.ILTabs;
            this.TCProjects.Multiline = true;
            this.TCProjects.Name = "TCProjects";
            this.TCProjects.SelectedIndex = 0;
            this.TCProjects.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TCProjects_DrawItem);
            this.TCProjects.SelectedIndexChanged += new System.EventHandler(this.TCProjects_SelectedIndexChanged);
            this.TCProjects.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TCProjects_MouseDoubleClick);
            // 
            // TPWelcome
            // 
            this.TPWelcome.Controls.Add(this.TPWelcomeLayout);
            resources.ApplyResources(this.TPWelcome, "TPWelcome");
            this.TPWelcome.Name = "TPWelcome";
            this.TPWelcome.UseVisualStyleBackColor = true;
            // 
            // TPWelcomeLayout
            // 
            resources.ApplyResources(this.TPWelcomeLayout, "TPWelcomeLayout");
            this.TPWelcomeLayout.Controls.Add(this.RichTextWelcome, 0, 0);
            this.TPWelcomeLayout.Controls.Add(this.TPWelcomeBtnClose, 0, 1);
            this.TPWelcomeLayout.Name = "TPWelcomeLayout";
            // 
            // RichTextWelcome
            // 
            resources.ApplyResources(this.RichTextWelcome, "RichTextWelcome");
            this.RichTextWelcome.Name = "RichTextWelcome";
            this.RichTextWelcome.ReadOnly = true;
            // 
            // TPWelcomeBtnClose
            // 
            resources.ApplyResources(this.TPWelcomeBtnClose, "TPWelcomeBtnClose");
            this.TPWelcomeBtnClose.Name = "TPWelcomeBtnClose";
            this.TPWelcomeBtnClose.UseVisualStyleBackColor = true;
            this.TPWelcomeBtnClose.Click += new System.EventHandler(this.TPWelcomeBtnClose_Click);
            // 
            // ILTabs
            // 
            this.ILTabs.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ILTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ILTabs.ImageStream")));
            this.ILTabs.TransparentColor = System.Drawing.Color.Transparent;
            this.ILTabs.Images.SetKeyName(0, "close.png");
            this.ILTabs.Images.SetKeyName(1, "found_it!x44.png");
            this.ILTabs.Images.SetKeyName(2, "add.png");
            this.ILTabs.Images.SetKeyName(3, "mini_unk_icon.png");
            // 
            // DgvParsed
            // 
            this.DgvParsed.AllowUserToAddRows = false;
            this.DgvParsed.AllowUserToDeleteRows = false;
            this.DgvParsed.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.DgvParsed.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvParsed.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.DgvParsed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvParsed.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgvPosition,
            this.DgvName,
            this.DgvValue});
            this.DgvParsed.ContextMenuStrip = this.MiFieldView;
            resources.ApplyResources(this.DgvParsed, "DgvParsed");
            this.DgvParsed.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.DgvParsed.Name = "DgvParsed";
            this.DgvParsed.RowHeadersVisible = false;
            this.DgvParsed.RowTemplate.Height = 25;
            this.DgvParsed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvParsed.SelectionChanged += new System.EventHandler(this.DgvParsed_SelectionChanged);
            // 
            // DgvPosition
            // 
            this.DgvPosition.FillWeight = 20F;
            this.DgvPosition.Frozen = true;
            resources.ApplyResources(this.DgvPosition, "DgvPosition");
            this.DgvPosition.MaxInputLength = 64;
            this.DgvPosition.Name = "DgvPosition";
            // 
            // DgvName
            // 
            this.DgvName.FillWeight = 30F;
            resources.ApplyResources(this.DgvName, "DgvName");
            this.DgvName.MaxInputLength = 128;
            this.DgvName.Name = "DgvName";
            // 
            // DgvValue
            // 
            this.DgvValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.DgvValue, "DgvValue");
            this.DgvValue.MaxInputLength = 4096;
            this.DgvValue.Name = "DgvValue";
            // 
            // MiFieldView
            // 
            this.MiFieldView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MiFieldFields,
            this.MIFieldLocalVars,
            this.MiFieldN1,
            this.MiFieldDebug});
            this.MiFieldView.Name = "MiFieldView";
            resources.ApplyResources(this.MiFieldView, "MiFieldView");
            this.MiFieldView.Opening += new System.ComponentModel.CancelEventHandler(this.MiFieldView_Opening);
            // 
            // MiFieldFields
            // 
            this.MiFieldFields.Checked = true;
            this.MiFieldFields.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MiFieldFields.Name = "MiFieldFields";
            resources.ApplyResources(this.MiFieldFields, "MiFieldFields");
            this.MiFieldFields.Click += new System.EventHandler(this.MiFieldFields_Click);
            // 
            // MIFieldLocalVars
            // 
            this.MIFieldLocalVars.Name = "MIFieldLocalVars";
            resources.ApplyResources(this.MIFieldLocalVars, "MIFieldLocalVars");
            this.MIFieldLocalVars.Click += new System.EventHandler(this.MIFieldLocalVars_Click);
            // 
            // MiFieldN1
            // 
            this.MiFieldN1.Name = "MiFieldN1";
            resources.ApplyResources(this.MiFieldN1, "MiFieldN1");
            // 
            // MiFieldDebug
            // 
            this.MiFieldDebug.Name = "MiFieldDebug";
            resources.ApplyResources(this.MiFieldDebug, "MiFieldDebug");
            this.MiFieldDebug.Click += new System.EventHandler(this.MiFieldDebug_Click);
            // 
            // openProjectFileDialog
            // 
            resources.ApplyResources(this.openProjectFileDialog, "openProjectFileDialog");
            // 
            // SplitContainerListAndFields
            // 
            resources.ApplyResources(this.SplitContainerListAndFields, "SplitContainerListAndFields");
            this.SplitContainerListAndFields.Name = "SplitContainerListAndFields";
            // 
            // SplitContainerListAndFields.Panel1
            // 
            this.SplitContainerListAndFields.Panel1.Controls.Add(this.TCProjects);
            // 
            // SplitContainerListAndFields.Panel2
            // 
            this.SplitContainerListAndFields.Panel2.Controls.Add(this.SplitContainerFieldsAndRawData);
            // 
            // SplitContainerFieldsAndRawData
            // 
            resources.ApplyResources(this.SplitContainerFieldsAndRawData, "SplitContainerFieldsAndRawData");
            this.SplitContainerFieldsAndRawData.Name = "SplitContainerFieldsAndRawData";
            // 
            // SplitContainerFieldsAndRawData.Panel1
            // 
            this.SplitContainerFieldsAndRawData.Panel1.Controls.Add(this.DgvParsed);
            // 
            // SplitContainerFieldsAndRawData.Panel2
            // 
            this.SplitContainerFieldsAndRawData.Panel2.Controls.Add(this.LayoutRawAndSuggested);
            // 
            // LayoutRawAndSuggested
            // 
            resources.ApplyResources(this.LayoutRawAndSuggested, "LayoutRawAndSuggested");
            this.LayoutRawAndSuggested.Controls.Add(this.RichTextData, 0, 0);
            this.LayoutRawAndSuggested.Name = "LayoutRawAndSuggested";
            // 
            // RichTextData
            // 
            resources.ApplyResources(this.RichTextData, "RichTextData");
            this.RichTextData.Name = "RichTextData";
            this.RichTextData.ReadOnly = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SplitContainerListAndFields);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.MM);
            this.MainMenuStrip = this.MM;
            this.Name = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MM.ResumeLayout(false);
            this.MM.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.TCProjects.ResumeLayout(false);
            this.TPWelcome.ResumeLayout(false);
            this.TPWelcomeLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvParsed)).EndInit();
            this.MiFieldView.ResumeLayout(false);
            this.SplitContainerListAndFields.Panel1.ResumeLayout(false);
            this.SplitContainerListAndFields.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerListAndFields)).EndInit();
            this.SplitContainerListAndFields.ResumeLayout(false);
            this.SplitContainerFieldsAndRawData.Panel1.ResumeLayout(false);
            this.SplitContainerFieldsAndRawData.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainerFieldsAndRawData)).EndInit();
            this.SplitContainerFieldsAndRawData.ResumeLayout(false);
            this.LayoutRawAndSuggested.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private OpenFileDialog openProjectFileDialog;
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
        private ToolStripMenuItem videoLinkToolStripMenuItem;
        private ToolStripMenuItem projectDataToolStripMenuItem;
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
    }
}