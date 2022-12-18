namespace VieweD
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MM = new System.Windows.Forms.MenuStrip();
            this.mmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mmFileAppend = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileAddFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFilePasteNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileS2 = new System.Windows.Forms.ToolStripSeparator();
            this.mmFileProjectDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileS1 = new System.Windows.Forms.ToolStripSeparator();
            this.mmFileSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFileS3 = new System.Windows.Forms.ToolStripSeparator();
            this.mmFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mmSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mmSearchSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mmSearchNext = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFilterApply = new System.Windows.Forms.ToolStripMenuItem();
            this.MMFilterApplyItem = new System.Windows.Forms.ToolStripSeparator();
            this.mmFilterHighlight = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFilterHighlightApply = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mmFilterEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mmFilterReset = new System.Windows.Forms.ToolStripMenuItem();
            this.mmExtra = new System.Windows.Forms.ToolStripMenuItem();
            this.mmVideoOpenLink = new System.Windows.Forms.ToolStripMenuItem();
            this.MMExtraGameView = new System.Windows.Forms.ToolStripMenuItem();
            this.MMExtraN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MMExtraUpdateParser = new System.Windows.Forms.ToolStripMenuItem();
            this.mmExtraExportPacketsAsCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.mmTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mmAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mmAboutGithub = new System.Windows.Forms.ToolStripMenuItem();
            this.mmAboutDiscord = new System.Windows.Forms.ToolStripMenuItem();
            this.MMAboutN1 = new System.Windows.Forms.ToolStripSeparator();
            this.MMAbout7Zip = new System.Windows.Forms.ToolStripMenuItem();
            this.MMAbout7ZipMain = new System.Windows.Forms.ToolStripMenuItem();
            this.MMAbout7ZipDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.mmAboutVideoLAN = new System.Windows.Forms.ToolStripMenuItem();
            this.MMAboutN2 = new System.Windows.Forms.ToolStripSeparator();
            this.mmAboutAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcPackets = new System.Windows.Forms.TabControl();
            this.tpPackets1 = new System.Windows.Forms.TabPage();
            this.lbPackets = new System.Windows.Forms.ListBox();
            this.pmPacketList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pmPLShowPacketName = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLS1 = new System.Windows.Forms.ToolStripSeparator();
            this.pmPLShowOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLHideThis = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLS2 = new System.Windows.Forms.ToolStripSeparator();
            this.pmPLShowOutOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.plPLShowInOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLS3 = new System.Windows.Forms.ToolStripSeparator();
            this.pmPLResetFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLS4 = new System.Windows.Forms.ToolStripSeparator();
            this.pmPLEditParser = new System.Windows.Forms.ToolStripMenuItem();
            this.pmPLExportPacket = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dGV = new System.Windows.Forms.DataGridView();
            this.cbShowBlock = new System.Windows.Forms.ComboBox();
            this.lInfo = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.rtInfo = new System.Windows.Forms.RichTextBox();
            this.flpPreviewData = new System.Windows.Forms.FlowLayoutPanel();
            this.lUint16 = new System.Windows.Forms.Label();
            this.lRawViewPos = new System.Windows.Forms.Label();
            this.btnCopyRawSource = new System.Windows.Forms.Button();
            this.cbOriginalData = new System.Windows.Forms.CheckBox();
            this.openLogFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.sbEngine = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbExtraInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbProjectInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.sbRules = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveCSVFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mmAboutKofi = new System.Windows.Forms.ToolStripMenuItem();
            this.MM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcPackets.SuspendLayout();
            this.tpPackets1.SuspendLayout();
            this.pmPacketList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.flpPreviewData.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // MM
            // 
            this.MM.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.MM.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmFile,
            this.mmSearch,
            this.mmFilter,
            this.mmExtra,
            this.mmTools,
            this.mmAbout});
            this.MM.Location = new System.Drawing.Point(0, 0);
            this.MM.Name = "MM";
            this.MM.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.MM.Size = new System.Drawing.Size(1084, 24);
            this.MM.TabIndex = 0;
            this.MM.Text = "Main Menu";
            // 
            // mmFile
            // 
            this.mmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmFileOpen,
            this.toolStripSeparator1,
            this.mmFileAppend,
            this.mmFileAddFromClipboard,
            this.mmFilePasteNew,
            this.mmFileS2,
            this.mmFileProjectDetails,
            this.mmFileClose,
            this.mmFileS1,
            this.mmFileSettings,
            this.mmFileS3,
            this.mmFileExit});
            this.mmFile.Name = "mmFile";
            this.mmFile.Size = new System.Drawing.Size(37, 20);
            this.mmFile.Text = "Fil&e";
            this.mmFile.Click += new System.EventHandler(this.MmFile_Click);
            // 
            // mmFileOpen
            // 
            this.mmFileOpen.Image = global::VieweD.Properties.Resources.Fairytale_fileopen;
            this.mmFileOpen.Name = "mmFileOpen";
            this.mmFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.mmFileOpen.Size = new System.Drawing.Size(249, 24);
            this.mmFileOpen.Text = "Open ...";
            this.mmFileOpen.Click += new System.EventHandler(this.mmFileOpen_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // mmFileAppend
            // 
            this.mmFileAppend.Image = global::VieweD.Properties.Resources.Fairytale_filenew3;
            this.mmFileAppend.Name = "mmFileAppend";
            this.mmFileAppend.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.mmFileAppend.Size = new System.Drawing.Size(249, 24);
            this.mmFileAppend.Text = "Append Log ...";
            this.mmFileAppend.Click += new System.EventHandler(this.mmFileAppend_Click);
            // 
            // mmFileAddFromClipboard
            // 
            this.mmFileAddFromClipboard.Image = global::VieweD.Properties.Resources.Fairytale_editcopy;
            this.mmFileAddFromClipboard.Name = "mmFileAddFromClipboard";
            this.mmFileAddFromClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
            this.mmFileAddFromClipboard.Size = new System.Drawing.Size(249, 24);
            this.mmFileAddFromClipboard.Text = "Add from clipboard";
            this.mmFileAddFromClipboard.Click += new System.EventHandler(this.MmAddFromClipboard_Click);
            // 
            // mmFilePasteNew
            // 
            this.mmFilePasteNew.Image = global::VieweD.Properties.Resources.Fairytale_filenew2;
            this.mmFilePasteNew.Name = "mmFilePasteNew";
            this.mmFilePasteNew.Size = new System.Drawing.Size(249, 24);
            this.mmFilePasteNew.Text = "New Tab from Clipboard";
            this.mmFilePasteNew.Click += new System.EventHandler(this.MmFilePasteNew_Click);
            // 
            // mmFileS2
            // 
            this.mmFileS2.Name = "mmFileS2";
            this.mmFileS2.Size = new System.Drawing.Size(246, 6);
            // 
            // mmFileProjectDetails
            // 
            this.mmFileProjectDetails.Name = "mmFileProjectDetails";
            this.mmFileProjectDetails.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mmFileProjectDetails.Size = new System.Drawing.Size(249, 24);
            this.mmFileProjectDetails.Text = "Project details ...";
            this.mmFileProjectDetails.Click += new System.EventHandler(this.MMFileProjectDetails_Click);
            // 
            // mmFileClose
            // 
            this.mmFileClose.Image = global::VieweD.Properties.Resources.Fairytale_button_cancel;
            this.mmFileClose.Name = "mmFileClose";
            this.mmFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.mmFileClose.Size = new System.Drawing.Size(249, 24);
            this.mmFileClose.Text = "Close";
            this.mmFileClose.Click += new System.EventHandler(this.mmFileClose_Click);
            // 
            // mmFileS1
            // 
            this.mmFileS1.Name = "mmFileS1";
            this.mmFileS1.Size = new System.Drawing.Size(246, 6);
            // 
            // mmFileSettings
            // 
            this.mmFileSettings.Name = "mmFileSettings";
            this.mmFileSettings.Size = new System.Drawing.Size(249, 24);
            this.mmFileSettings.Text = "Program settings ...";
            this.mmFileSettings.Click += new System.EventHandler(this.mmFileSettings_Click);
            // 
            // mmFileS3
            // 
            this.mmFileS3.Name = "mmFileS3";
            this.mmFileS3.Size = new System.Drawing.Size(246, 6);
            // 
            // mmFileExit
            // 
            this.mmFileExit.Name = "mmFileExit";
            this.mmFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mmFileExit.Size = new System.Drawing.Size(249, 24);
            this.mmFileExit.Text = "E&xit";
            this.mmFileExit.Click += new System.EventHandler(this.mmFileExit_Click);
            // 
            // mmSearch
            // 
            this.mmSearch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmSearchSearch,
            this.mmSearchNext});
            this.mmSearch.Name = "mmSearch";
            this.mmSearch.Size = new System.Drawing.Size(54, 20);
            this.mmSearch.Text = "&Search";
            // 
            // mmSearchSearch
            // 
            this.mmSearchSearch.Name = "mmSearchSearch";
            this.mmSearchSearch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mmSearchSearch.Size = new System.Drawing.Size(161, 22);
            this.mmSearchSearch.Text = "Search ...";
            this.mmSearchSearch.Click += new System.EventHandler(this.MmSearchSearch_Click);
            // 
            // mmSearchNext
            // 
            this.mmSearchNext.Name = "mmSearchNext";
            this.mmSearchNext.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.mmSearchNext.Size = new System.Drawing.Size(161, 22);
            this.mmSearchNext.Text = "Search next";
            this.mmSearchNext.Click += new System.EventHandler(this.MmSearchNext_Click);
            // 
            // mmFilter
            // 
            this.mmFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmFilterApply,
            this.mmFilterHighlight,
            this.toolStripMenuItem3,
            this.mmFilterEdit,
            this.mmFilterReset});
            this.mmFilter.Name = "mmFilter";
            this.mmFilter.Size = new System.Drawing.Size(45, 20);
            this.mmFilter.Text = "Fi&lter";
            // 
            // mmFilterApply
            // 
            this.mmFilterApply.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMFilterApplyItem});
            this.mmFilterApply.Name = "mmFilterApply";
            this.mmFilterApply.Size = new System.Drawing.Size(170, 22);
            this.mmFilterApply.Text = "Apply";
            this.mmFilterApply.DropDownOpening += new System.EventHandler(this.MmFilterApply_DropDownOpening);
            // 
            // MMFilterApplyItem
            // 
            this.MMFilterApplyItem.Name = "MMFilterApplyItem";
            this.MMFilterApplyItem.Size = new System.Drawing.Size(57, 6);
            this.MMFilterApplyItem.Click += new System.EventHandler(this.MMFilterApplyItem_Click);
            // 
            // mmFilterHighlight
            // 
            this.mmFilterHighlight.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmFilterHighlightApply});
            this.mmFilterHighlight.Name = "mmFilterHighlight";
            this.mmFilterHighlight.Size = new System.Drawing.Size(170, 22);
            this.mmFilterHighlight.Text = "Highlight";
            this.mmFilterHighlight.DropDownOpening += new System.EventHandler(this.MmFilterApply_DropDownOpening);
            // 
            // mmFilterHighlightApply
            // 
            this.mmFilterHighlightApply.Name = "mmFilterHighlightApply";
            this.mmFilterHighlightApply.Size = new System.Drawing.Size(57, 6);
            this.mmFilterHighlightApply.Click += new System.EventHandler(this.MMFilterHighlightItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(167, 6);
            // 
            // mmFilterEdit
            // 
            this.mmFilterEdit.Name = "mmFilterEdit";
            this.mmFilterEdit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.mmFilterEdit.Size = new System.Drawing.Size(170, 22);
            this.mmFilterEdit.Text = "Edit ...";
            this.mmFilterEdit.Click += new System.EventHandler(this.MmFilterEdit_Click);
            // 
            // mmFilterReset
            // 
            this.mmFilterReset.Name = "mmFilterReset";
            this.mmFilterReset.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.mmFilterReset.Size = new System.Drawing.Size(170, 22);
            this.mmFilterReset.Text = "Reset";
            this.mmFilterReset.Click += new System.EventHandler(this.MmFilterReset_Click);
            // 
            // mmExtra
            // 
            this.mmExtra.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmVideoOpenLink,
            this.MMExtraGameView,
            this.MMExtraN1,
            this.MMExtraUpdateParser,
            this.mmExtraExportPacketsAsCSV});
            this.mmExtra.Name = "mmExtra";
            this.mmExtra.Size = new System.Drawing.Size(45, 20);
            this.mmExtra.Text = "E&xtra";
            // 
            // mmVideoOpenLink
            // 
            this.mmVideoOpenLink.Image = global::VieweD.Properties.Resources.mini_video_icon;
            this.mmVideoOpenLink.Name = "mmVideoOpenLink";
            this.mmVideoOpenLink.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.mmVideoOpenLink.Size = new System.Drawing.Size(280, 24);
            this.mmVideoOpenLink.Text = "Video link ...";
            this.mmVideoOpenLink.Click += new System.EventHandler(this.MmVideoOpenLink_Click);
            // 
            // MMExtraGameView
            // 
            this.MMExtraGameView.Image = global::VieweD.Properties.Resources.mini_unk_ticon;
            this.MMExtraGameView.Name = "MMExtraGameView";
            this.MMExtraGameView.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
            this.MMExtraGameView.Size = new System.Drawing.Size(280, 24);
            this.MMExtraGameView.Text = "View Game Info ...";
            this.MMExtraGameView.Click += new System.EventHandler(this.MMExtraGameView_Click);
            // 
            // MMExtraN1
            // 
            this.MMExtraN1.Name = "MMExtraN1";
            this.MMExtraN1.Size = new System.Drawing.Size(277, 6);
            // 
            // MMExtraUpdateParser
            // 
            this.MMExtraUpdateParser.Name = "MMExtraUpdateParser";
            this.MMExtraUpdateParser.Size = new System.Drawing.Size(280, 24);
            this.MMExtraUpdateParser.Text = "Download parser updates from GitHub";
            this.MMExtraUpdateParser.Visible = false;
            this.MMExtraUpdateParser.Click += new System.EventHandler(this.MMExtraUpdateParser_Click);
            // 
            // mmExtraExportPacketsAsCSV
            // 
            this.mmExtraExportPacketsAsCSV.Name = "mmExtraExportPacketsAsCSV";
            this.mmExtraExportPacketsAsCSV.Size = new System.Drawing.Size(280, 24);
            this.mmExtraExportPacketsAsCSV.Text = "Export packets tab as CSV";
            this.mmExtraExportPacketsAsCSV.Click += new System.EventHandler(this.mmExtraExportPacketsAsCSV_Click);
            // 
            // mmTools
            // 
            this.mmTools.Enabled = false;
            this.mmTools.Name = "mmTools";
            this.mmTools.Size = new System.Drawing.Size(46, 20);
            this.mmTools.Text = "&Tools";
            this.mmTools.DropDownOpening += new System.EventHandler(this.mmTools_DropDownOpening);
            // 
            // mmAbout
            // 
            this.mmAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mmAboutGithub,
            this.mmAboutDiscord,
            this.mmAboutKofi,
            this.MMAboutN1,
            this.MMAbout7Zip,
            this.mmAboutVideoLAN,
            this.MMAboutN2,
            this.mmAboutAbout});
            this.mmAbout.Name = "mmAbout";
            this.mmAbout.Size = new System.Drawing.Size(52, 20);
            this.mmAbout.Text = "&About";
            // 
            // mmAboutGithub
            // 
            this.mmAboutGithub.Name = "mmAboutGithub";
            this.mmAboutGithub.Size = new System.Drawing.Size(197, 22);
            this.mmAboutGithub.Text = "Open source on Github";
            this.mmAboutGithub.Click += new System.EventHandler(this.mmAboutGithub_Click);
            // 
            // mmAboutDiscord
            // 
            this.mmAboutDiscord.Name = "mmAboutDiscord";
            this.mmAboutDiscord.Size = new System.Drawing.Size(197, 22);
            this.mmAboutDiscord.Text = "Join Discord";
            this.mmAboutDiscord.Click += new System.EventHandler(this.MmAboutDiscord_Click);
            // 
            // MMAboutN1
            // 
            this.MMAboutN1.Name = "MMAboutN1";
            this.MMAboutN1.Size = new System.Drawing.Size(194, 6);
            // 
            // MMAbout7Zip
            // 
            this.MMAbout7Zip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MMAbout7ZipMain,
            this.MMAbout7ZipDownload});
            this.MMAbout7Zip.Name = "MMAbout7Zip";
            this.MMAbout7Zip.Size = new System.Drawing.Size(197, 22);
            this.MMAbout7Zip.Text = "Visit 7-zip site";
            // 
            // MMAbout7ZipMain
            // 
            this.MMAbout7ZipMain.Name = "MMAbout7ZipMain";
            this.MMAbout7ZipMain.Size = new System.Drawing.Size(222, 22);
            this.MMAbout7ZipMain.Text = "Main site";
            this.MMAbout7ZipMain.Click += new System.EventHandler(this.MMAbout7ZipMain_Click);
            // 
            // MMAbout7ZipDownload
            // 
            this.MMAbout7ZipDownload.Name = "MMAbout7ZipDownload";
            this.MMAbout7ZipDownload.Size = new System.Drawing.Size(222, 22);
            this.MMAbout7ZipDownload.Text = "Goto V18.05 download page";
            this.MMAbout7ZipDownload.Click += new System.EventHandler(this.MMAbout7ZipDownload_Click);
            // 
            // mmAboutVideoLAN
            // 
            this.mmAboutVideoLAN.Name = "mmAboutVideoLAN";
            this.mmAboutVideoLAN.Size = new System.Drawing.Size(197, 22);
            this.mmAboutVideoLAN.Text = "Visit VideoLAN VLC site";
            this.mmAboutVideoLAN.Click += new System.EventHandler(this.mmAboutVideoLAN_Click);
            // 
            // MMAboutN2
            // 
            this.MMAboutN2.Name = "MMAboutN2";
            this.MMAboutN2.Size = new System.Drawing.Size(194, 6);
            // 
            // mmAboutAbout
            // 
            this.mmAboutAbout.Name = "mmAboutAbout";
            this.mmAboutAbout.Size = new System.Drawing.Size(197, 22);
            this.mmAboutAbout.Text = "About ...";
            this.mmAboutAbout.Click += new System.EventHandler(this.mmAboutAbout_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tcPackets);
            this.splitContainer1.Panel1MinSize = 350;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 511);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 3;
            // 
            // tcPackets
            // 
            this.tcPackets.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tcPackets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcPackets.Controls.Add(this.tpPackets1);
            this.tcPackets.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tcPackets.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.tcPackets.HotTrack = true;
            this.tcPackets.Location = new System.Drawing.Point(0, 0);
            this.tcPackets.Multiline = true;
            this.tcPackets.Name = "tcPackets";
            this.tcPackets.SelectedIndex = 0;
            this.tcPackets.Size = new System.Drawing.Size(447, 508);
            this.tcPackets.TabIndex = 1;
            this.tcPackets.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TcPackets_DrawItem);
            this.tcPackets.SelectedIndexChanged += new System.EventHandler(this.TcPackets_SelectedIndexChanged);
            this.tcPackets.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.TcPackets_ControlRemoved);
            this.tcPackets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TcPackets_MouseDown);
            // 
            // tpPackets1
            // 
            this.tpPackets1.Controls.Add(this.lbPackets);
            this.tpPackets1.Location = new System.Drawing.Point(24, 4);
            this.tpPackets1.Name = "tpPackets1";
            this.tpPackets1.Padding = new System.Windows.Forms.Padding(3);
            this.tpPackets1.Size = new System.Drawing.Size(419, 500);
            this.tpPackets1.TabIndex = 0;
            this.tpPackets1.Text = "Packets";
            this.tpPackets1.UseVisualStyleBackColor = true;
            // 
            // lbPackets
            // 
            this.lbPackets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPackets.ContextMenuStrip = this.pmPacketList;
            this.lbPackets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbPackets.FormattingEnabled = true;
            this.lbPackets.ItemHeight = 14;
            this.lbPackets.Location = new System.Drawing.Point(0, 0);
            this.lbPackets.Name = "lbPackets";
            this.lbPackets.Size = new System.Drawing.Size(419, 494);
            this.lbPackets.TabIndex = 0;
            this.lbPackets.SelectedIndexChanged += new System.EventHandler(this.lbPackets_SelectedIndexChanged);
            // 
            // pmPacketList
            // 
            this.pmPacketList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pmPLShowPacketName,
            this.pmPLS1,
            this.pmPLShowOnly,
            this.pmPLHideThis,
            this.pmPLS2,
            this.pmPLShowOutOnly,
            this.plPLShowInOnly,
            this.pmPLS3,
            this.pmPLResetFilters,
            this.pmPLS4,
            this.pmPLEditParser,
            this.pmPLExportPacket});
            this.pmPacketList.Name = "pmPacketList";
            this.pmPacketList.Size = new System.Drawing.Size(208, 204);
            // 
            // pmPLShowPacketName
            // 
            this.pmPLShowPacketName.Name = "pmPLShowPacketName";
            this.pmPLShowPacketName.Size = new System.Drawing.Size(207, 22);
            this.pmPLShowPacketName.Text = "This packet type is";
            // 
            // pmPLS1
            // 
            this.pmPLS1.Name = "pmPLS1";
            this.pmPLS1.Size = new System.Drawing.Size(204, 6);
            // 
            // pmPLShowOnly
            // 
            this.pmPLShowOnly.Name = "pmPLShowOnly";
            this.pmPLShowOnly.Size = new System.Drawing.Size(207, 22);
            this.pmPLShowOnly.Text = "Show only this type";
            // 
            // pmPLHideThis
            // 
            this.pmPLHideThis.Name = "pmPLHideThis";
            this.pmPLHideThis.Size = new System.Drawing.Size(207, 22);
            this.pmPLHideThis.Text = "Hide this type";
            // 
            // pmPLS2
            // 
            this.pmPLS2.Name = "pmPLS2";
            this.pmPLS2.Size = new System.Drawing.Size(204, 6);
            // 
            // pmPLShowOutOnly
            // 
            this.pmPLShowOutOnly.Name = "pmPLShowOutOnly";
            this.pmPLShowOutOnly.Size = new System.Drawing.Size(207, 22);
            this.pmPLShowOutOnly.Text = "Show only Outgoing";
            // 
            // plPLShowInOnly
            // 
            this.plPLShowInOnly.Name = "plPLShowInOnly";
            this.plPLShowInOnly.Size = new System.Drawing.Size(207, 22);
            this.plPLShowInOnly.Text = "Show only Incoming";
            // 
            // pmPLS3
            // 
            this.pmPLS3.Name = "pmPLS3";
            this.pmPLS3.Size = new System.Drawing.Size(204, 6);
            // 
            // pmPLResetFilters
            // 
            this.pmPLResetFilters.Name = "pmPLResetFilters";
            this.pmPLResetFilters.Size = new System.Drawing.Size(207, 22);
            this.pmPLResetFilters.Text = "Reset all filters";
            // 
            // pmPLS4
            // 
            this.pmPLS4.Name = "pmPLS4";
            this.pmPLS4.Size = new System.Drawing.Size(204, 6);
            // 
            // pmPLEditParser
            // 
            this.pmPLEditParser.Name = "pmPLEditParser";
            this.pmPLEditParser.Size = new System.Drawing.Size(207, 22);
            this.pmPLEditParser.Text = "Edit parser for this packet";
            // 
            // pmPLExportPacket
            // 
            this.pmPLExportPacket.Name = "pmPLExportPacket";
            this.pmPLExportPacket.Size = new System.Drawing.Size(207, 22);
            this.pmPLExportPacket.Text = "Export packet";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dGV);
            this.splitContainer2.Panel1.Controls.Add(this.cbShowBlock);
            this.splitContainer2.Panel1.Controls.Add(this.lInfo);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel2.Controls.Add(this.lRawViewPos);
            this.splitContainer2.Panel2.Controls.Add(this.btnCopyRawSource);
            this.splitContainer2.Panel2.Controls.Add(this.cbOriginalData);
            this.splitContainer2.Size = new System.Drawing.Size(630, 511);
            this.splitContainer2.SplitterDistance = 352;
            this.splitContainer2.TabIndex = 0;
            // 
            // dGV
            // 
            this.dGV.AllowUserToAddRows = false;
            this.dGV.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dGV.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dGV.Location = new System.Drawing.Point(3, 32);
            this.dGV.Name = "dGV";
            this.dGV.RowHeadersVisible = false;
            this.dGV.RowHeadersWidth = 8;
            this.dGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dGV.Size = new System.Drawing.Size(612, 317);
            this.dGV.TabIndex = 2;
            this.dGV.SelectionChanged += new System.EventHandler(this.dGV_SelectionChanged);
            // 
            // cbShowBlock
            // 
            this.cbShowBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShowBlock.FormattingEnabled = true;
            this.cbShowBlock.Location = new System.Drawing.Point(460, 6);
            this.cbShowBlock.Name = "cbShowBlock";
            this.cbShowBlock.Size = new System.Drawing.Size(158, 22);
            this.cbShowBlock.TabIndex = 1;
            this.cbShowBlock.Visible = false;
            this.cbShowBlock.SelectedIndexChanged += new System.EventHandler(this.CbShowBlock_SelectedIndexChanged);
            // 
            // lInfo
            // 
            this.lInfo.AutoSize = true;
            this.lInfo.Location = new System.Drawing.Point(3, 12);
            this.lInfo.Name = "lInfo";
            this.lInfo.Size = new System.Drawing.Size(35, 14);
            this.lInfo.TabIndex = 0;
            this.lInfo.Text = "Info";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(3, 4);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.rtInfo);
            this.splitContainer3.Panel1.Resize += new System.EventHandler(this.splitContainer3_Panel1_Resize);
            this.splitContainer3.Panel1MinSize = 500;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.flpPreviewData);
            this.splitContainer3.Panel2Collapsed = true;
            this.splitContainer3.Size = new System.Drawing.Size(615, 124);
            this.splitContainer3.SplitterDistance = 590;
            this.splitContainer3.TabIndex = 5;
            this.splitContainer3.Resize += new System.EventHandler(this.splitContainer3_Resize);
            // 
            // rtInfo
            // 
            this.rtInfo.DetectUrls = false;
            this.rtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtInfo.HideSelection = false;
            this.rtInfo.Location = new System.Drawing.Point(0, 0);
            this.rtInfo.Name = "rtInfo";
            this.rtInfo.ReadOnly = true;
            this.rtInfo.Size = new System.Drawing.Size(615, 124);
            this.rtInfo.TabIndex = 2;
            this.rtInfo.Text = resources.GetString("rtInfo.Text");
            this.rtInfo.SelectionChanged += new System.EventHandler(this.RtInfo_SelectionChanged);
            // 
            // flpPreviewData
            // 
            this.flpPreviewData.AutoSize = true;
            this.flpPreviewData.BackColor = System.Drawing.SystemColors.Control;
            this.flpPreviewData.Controls.Add(this.lUint16);
            this.flpPreviewData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpPreviewData.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpPreviewData.Location = new System.Drawing.Point(0, 0);
            this.flpPreviewData.Name = "flpPreviewData";
            this.flpPreviewData.Size = new System.Drawing.Size(96, 100);
            this.flpPreviewData.TabIndex = 0;
            // 
            // lUint16
            // 
            this.lUint16.AutoSize = true;
            this.lUint16.Location = new System.Drawing.Point(3, 0);
            this.lUint16.Name = "lUint16";
            this.lUint16.Size = new System.Drawing.Size(35, 14);
            this.lUint16.TabIndex = 0;
            this.lUint16.Text = "info";
            // 
            // lRawViewPos
            // 
            this.lRawViewPos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lRawViewPos.Location = new System.Drawing.Point(164, 134);
            this.lRawViewPos.Name = "lRawViewPos";
            this.lRawViewPos.Size = new System.Drawing.Size(328, 18);
            this.lRawViewPos.TabIndex = 4;
            this.lRawViewPos.Text = "Pos: ??,??";
            this.lRawViewPos.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnCopyRawSource
            // 
            this.btnCopyRawSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyRawSource.Location = new System.Drawing.Point(498, 132);
            this.btnCopyRawSource.Name = "btnCopyRawSource";
            this.btnCopyRawSource.Size = new System.Drawing.Size(120, 21);
            this.btnCopyRawSource.TabIndex = 3;
            this.btnCopyRawSource.Text = "Copy Source";
            this.btnCopyRawSource.UseVisualStyleBackColor = true;
            this.btnCopyRawSource.Click += new System.EventHandler(this.BtnCopyRawSource_Click);
            // 
            // cbOriginalData
            // 
            this.cbOriginalData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbOriginalData.AutoSize = true;
            this.cbOriginalData.Location = new System.Drawing.Point(6, 134);
            this.cbOriginalData.Name = "cbOriginalData";
            this.cbOriginalData.Size = new System.Drawing.Size(152, 18);
            this.cbOriginalData.TabIndex = 1;
            this.cbOriginalData.Text = "Show original data";
            this.cbOriginalData.UseVisualStyleBackColor = true;
            this.cbOriginalData.CheckedChanged += new System.EventHandler(this.cbOriginalData_CheckedChanged);
            // 
            // openLogFileDialog
            // 
            this.openLogFileDialog.DefaultExt = "log";
            this.openLogFileDialog.Filter = "All Files|*.*";
            this.openLogFileDialog.SupportMultiDottedExtensions = true;
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sbEngine,
            this.sbExtraInfo,
            this.sbProjectInfo,
            this.sbRules});
            this.statusBar.Location = new System.Drawing.Point(0, 537);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1084, 24);
            this.statusBar.TabIndex = 4;
            this.statusBar.Text = "statusStrip1";
            // 
            // sbEngine
            // 
            this.sbEngine.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sbEngine.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sbEngine.Name = "sbEngine";
            this.sbEngine.Size = new System.Drawing.Size(47, 19);
            this.sbEngine.Text = "Engine";
            this.sbEngine.Visible = false;
            // 
            // sbExtraInfo
            // 
            this.sbExtraInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sbExtraInfo.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sbExtraInfo.Name = "sbExtraInfo";
            this.sbExtraInfo.Size = new System.Drawing.Size(20, 19);
            this.sbExtraInfo.Text = "...";
            // 
            // sbProjectInfo
            // 
            this.sbProjectInfo.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sbProjectInfo.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sbProjectInfo.Name = "sbProjectInfo";
            this.sbProjectInfo.Size = new System.Drawing.Size(1049, 19);
            this.sbProjectInfo.Spring = true;
            this.sbProjectInfo.Text = "Project: none";
            this.sbProjectInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sbRules
            // 
            this.sbRules.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sbRules.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.sbRules.Name = "sbRules";
            this.sbRules.Size = new System.Drawing.Size(74, 19);
            this.sbRules.Text = "Rules: None";
            this.sbRules.Visible = false;
            // 
            // saveCSVFileDialog
            // 
            this.saveCSVFileDialog.DefaultExt = "csv";
            this.saveCSVFileDialog.Filter = "CSV Files|*.csv|All files|*.*";
            this.saveCSVFileDialog.RestoreDirectory = true;
            this.saveCSVFileDialog.Title = "Export CSV File";
            // 
            // mmAboutKofi
            // 
            this.mmAboutKofi.Name = "mmAboutKofi";
            this.mmAboutKofi.Size = new System.Drawing.Size(197, 22);
            this.mmAboutKofi.Text = "Buy me a coffee";
            this.mmAboutKofi.Click += new System.EventHandler(this.mmAboutKofi_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 561);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.MM);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MM;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VieweD";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.MM.ResumeLayout(false);
            this.MM.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tcPackets.ResumeLayout(false);
            this.tpPackets1.ResumeLayout(false);
            this.pmPacketList.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGV)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.flpPreviewData.ResumeLayout(false);
            this.flpPreviewData.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lRawViewPos;

        #endregion

        private System.Windows.Forms.MenuStrip MM;
        private System.Windows.Forms.ToolStripMenuItem mmFile;
        private System.Windows.Forms.ToolStripMenuItem mmFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mmFileAppend;
        private System.Windows.Forms.ToolStripMenuItem mmFileAddFromClipboard;
        private System.Windows.Forms.ToolStripSeparator mmFileS1;
        private System.Windows.Forms.ToolStripMenuItem mmFileSettings;
        private System.Windows.Forms.ToolStripSeparator mmFileS2;
        private System.Windows.Forms.ToolStripMenuItem mmFileExit;
        private System.Windows.Forms.ToolStripMenuItem mmSearch;
        private System.Windows.Forms.ToolStripMenuItem mmSearchSearch;
        private System.Windows.Forms.ToolStripMenuItem mmSearchNext;
        private System.Windows.Forms.ToolStripMenuItem mmFilter;
        private System.Windows.Forms.ToolStripMenuItem mmFilterEdit;
        private System.Windows.Forms.ToolStripMenuItem mmFilterReset;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mmFilterApply;
        private System.Windows.Forms.ToolStripSeparator MMFilterApplyItem;
        private System.Windows.Forms.ToolStripMenuItem mmExtra;
        private System.Windows.Forms.ToolStripMenuItem mmVideoOpenLink;
        private System.Windows.Forms.ToolStripMenuItem mmAbout;
        private System.Windows.Forms.ToolStripMenuItem mmAboutGithub;
        private System.Windows.Forms.ToolStripMenuItem mmAboutVideoLAN;
        private System.Windows.Forms.ToolStripSeparator MMAboutN2;
        private System.Windows.Forms.ToolStripMenuItem mmAboutAbout;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbPackets;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ComboBox cbShowBlock;
        private System.Windows.Forms.Label lInfo;
        private System.Windows.Forms.CheckBox cbOriginalData;
        private System.Windows.Forms.OpenFileDialog openLogFileDialog;
        private System.Windows.Forms.ToolStripMenuItem mmFileClose;
        private System.Windows.Forms.RichTextBox rtInfo;
        private System.Windows.Forms.TabControl tcPackets;
        private System.Windows.Forms.TabPage tpPackets1;
        private System.Windows.Forms.ToolStripSeparator mmFileS3;
        private System.Windows.Forms.ToolStripMenuItem mmFilePasteNew;
        private System.Windows.Forms.ContextMenuStrip pmPacketList;
        private System.Windows.Forms.ToolStripMenuItem pmPLShowPacketName;
        private System.Windows.Forms.ToolStripSeparator pmPLS1;
        private System.Windows.Forms.ToolStripMenuItem pmPLShowOnly;
        private System.Windows.Forms.ToolStripMenuItem pmPLHideThis;
        private System.Windows.Forms.ToolStripSeparator pmPLS2;
        private System.Windows.Forms.ToolStripMenuItem pmPLShowOutOnly;
        private System.Windows.Forms.ToolStripMenuItem plPLShowInOnly;
        private System.Windows.Forms.ToolStripSeparator pmPLS3;
        private System.Windows.Forms.ToolStripMenuItem pmPLResetFilters;
        private System.Windows.Forms.ToolStripSeparator pmPLS4;
        private System.Windows.Forms.ToolStripMenuItem pmPLEditParser;
        private System.Windows.Forms.ToolStripMenuItem pmPLExportPacket;
        private System.Windows.Forms.Button btnCopyRawSource;
        public System.Windows.Forms.DataGridView dGV;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel sbProjectInfo;
        private System.Windows.Forms.ToolStripStatusLabel sbExtraInfo;
        private System.Windows.Forms.ToolStripMenuItem mmAboutDiscord;
        private System.Windows.Forms.ToolStripMenuItem MMExtraGameView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mmFileProjectDetails;
        private System.Windows.Forms.ToolStripSeparator MMExtraN1;
        private System.Windows.Forms.ToolStripMenuItem MMExtraUpdateParser;
        private System.Windows.Forms.ToolStripSeparator MMAboutN1;
        private System.Windows.Forms.ToolStripMenuItem MMAbout7Zip;
        private System.Windows.Forms.ToolStripMenuItem MMAbout7ZipMain;
        private System.Windows.Forms.ToolStripMenuItem MMAbout7ZipDownload;
        private System.Windows.Forms.ToolStripMenuItem mmExtraExportPacketsAsCSV;
        private System.Windows.Forms.SaveFileDialog saveCSVFileDialog;
        private System.Windows.Forms.ToolStripStatusLabel sbRules;
        private System.Windows.Forms.ToolStripStatusLabel sbEngine;
        private System.Windows.Forms.ToolStripMenuItem mmTools;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.FlowLayoutPanel flpPreviewData;
        private System.Windows.Forms.Label lUint16;
        private System.Windows.Forms.ToolStripMenuItem mmFilterHighlight;
        private System.Windows.Forms.ToolStripSeparator mmFilterHighlightApply;
        private System.Windows.Forms.ToolStripMenuItem mmAboutKofi;
    }
}

