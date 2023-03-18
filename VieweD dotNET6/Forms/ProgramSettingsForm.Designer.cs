namespace VieweD.Forms
{
    partial class ProgramSettingsForm
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
            BtnOK = new Button();
            BtnCancel = new Button();
            ColorDlg = new ColorDialog();
            Label1 = new Label();
            Label2 = new Label();
            Label3 = new Label();
            Label4 = new Label();
            Label5 = new Label();
            Label6 = new Label();
            Label7 = new Label();
            Label8 = new Label();
            TableLayoutPanel1 = new TableLayoutPanel();
            Label10 = new Label();
            BtnFontIN = new Button();
            BtnBackIN = new Button();
            BtnSyncIN = new Button();
            BtnSelectIN = new Button();
            BtnFontOUT = new Button();
            BtnBackOUT = new Button();
            BtnSyncOUT = new Button();
            BtnSelectOUT = new Button();
            BtnBackUNK = new Button();
            BtnSyncUNK = new Button();
            BtnSelectUNK = new Button();
            BtnBarIN = new Button();
            BtnBarOUT = new Button();
            BtnBarUNK = new Button();
            Label9 = new Label();
            BtnSelectedFontIN = new Button();
            BtnSelectedFontOUT = new Button();
            BtnSelectedFontUNK = new Button();
            BtnFontUNK = new Button();
            BtnDefault = new Button();
            GbVideoSettings = new GroupBox();
            Label11 = new Label();
            RbAutoLoadVideoYoutube = new RadioButton();
            RbAutoLoadVideoLocalOnly = new RadioButton();
            RbAutoLoadVideoNever = new RadioButton();
            GbListStyle = new GroupBox();
            BtnPacketListFont = new Button();
            LabelPacketListArrows = new Label();
            Label12 = new Label();
            BtnSetDarkMode = new Button();
            PictureBox3 = new PictureBox();
            PictureBox4 = new PictureBox();
            PictureBox2 = new PictureBox();
            PictureBox1 = new PictureBox();
            RbListStyleTransparent = new RadioButton();
            RbListStyleSolid = new RadioButton();
            RbListStyleText = new RadioButton();
            LayoutGridColors = new TableLayoutPanel();
            BtnColField12 = new Button();
            BtnColField14 = new Button();
            BtnColField15 = new Button();
            BtnColField1 = new Button();
            BtnColField2 = new Button();
            BtnColField3 = new Button();
            BtnColField4 = new Button();
            BtnColField5 = new Button();
            BtnColField6 = new Button();
            BtnColField7 = new Button();
            BtnColField8 = new Button();
            BtnColField9 = new Button();
            BtnColField10 = new Button();
            BtnColField11 = new Button();
            BtnColField13 = new Button();
            LFieldCol0 = new Label();
            GbGridStyle = new GroupBox();
            BtnGridViewFont = new Button();
            Label13 = new Label();
            LFieldColCount = new Label();
            TbFieldColorCount = new TrackBar();
            GbOtherSettings = new GroupBox();
            DefaultCreditsTextBox = new TextBox();
            label15 = new Label();
            CbShowHexStringData = new CheckBox();
            CbAskNewProject = new CheckBox();
            TcSettings = new TabControl();
            TpGeneral = new TabPage();
            GroupBox1 = new GroupBox();
            TbPacketList = new TabPage();
            TpFieldGrid = new TabPage();
            GroupBox3 = new GroupBox();
            BtnRawViewFont = new Button();
            FontDlg = new FontDialog();
            label14 = new Label();
            TableLayoutPanel1.SuspendLayout();
            GbVideoSettings.SuspendLayout();
            GbListStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            LayoutGridColors.SuspendLayout();
            GbGridStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TbFieldColorCount).BeginInit();
            GbOtherSettings.SuspendLayout();
            TcSettings.SuspendLayout();
            TpGeneral.SuspendLayout();
            GroupBox1.SuspendLayout();
            TbPacketList.SuspendLayout();
            TpFieldGrid.SuspendLayout();
            GroupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // BtnOK
            // 
            BtnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnOK.Location = new Point(12, 380);
            BtnOK.Margin = new Padding(4, 3, 4, 3);
            BtnOK.Name = "BtnOK";
            BtnOK.Size = new Size(88, 27);
            BtnOK.TabIndex = 0;
            BtnOK.Text = "OK";
            BtnOK.UseVisualStyleBackColor = true;
            BtnOK.Click += BtnOK_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(108, 380);
            BtnCancel.Margin = new Padding(4, 3, 4, 3);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(88, 27);
            BtnCancel.TabIndex = 1;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // ColorDlg
            // 
            ColorDlg.AnyColor = true;
            ColorDlg.FullOpen = true;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Dock = DockStyle.Fill;
            Label1.Location = new Point(6, 46);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(81, 42);
            Label1.TabIndex = 2;
            Label1.Text = "IN packet";
            Label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Dock = DockStyle.Fill;
            Label2.Location = new Point(6, 90);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(81, 42);
            Label2.TabIndex = 3;
            Label2.Text = "OUT Packet";
            Label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Dock = DockStyle.Fill;
            Label3.Location = new Point(6, 134);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(81, 43);
            Label3.TabIndex = 4;
            Label3.Text = "Unknown Direction";
            Label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Dock = DockStyle.Fill;
            Label4.Location = new Point(188, 2);
            Label4.Margin = new Padding(4, 0, 4, 0);
            Label4.Name = "Label4";
            Label4.Size = new Size(81, 42);
            Label4.TabIndex = 5;
            Label4.Text = "Back color";
            Label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label5
            // 
            Label5.AutoSize = true;
            Label5.Dock = DockStyle.Fill;
            Label5.Location = new Point(279, 2);
            Label5.Margin = new Padding(4, 0, 4, 0);
            Label5.Name = "Label5";
            Label5.Size = new Size(81, 42);
            Label5.TabIndex = 6;
            Label5.Text = "Synced";
            Label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            Label6.AutoSize = true;
            Label6.Dock = DockStyle.Fill;
            Label6.Location = new Point(370, 2);
            Label6.Margin = new Padding(4, 0, 4, 0);
            Label6.Name = "Label6";
            Label6.Size = new Size(81, 42);
            Label6.TabIndex = 7;
            Label6.Text = "Selected back color";
            Label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label7
            // 
            Label7.AutoSize = true;
            Label7.Dock = DockStyle.Fill;
            Label7.Location = new Point(552, 2);
            Label7.Margin = new Padding(4, 0, 4, 0);
            Label7.Name = "Label7";
            Label7.Size = new Size(88, 42);
            Label7.TabIndex = 8;
            Label7.Text = "SyncBar color";
            Label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Label8
            // 
            Label8.AutoSize = true;
            Label8.Dock = DockStyle.Fill;
            Label8.Location = new Point(97, 2);
            Label8.Margin = new Padding(4, 0, 4, 0);
            Label8.Name = "Label8";
            Label8.Size = new Size(81, 42);
            Label8.TabIndex = 9;
            Label8.Text = "Font";
            Label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            TableLayoutPanel1.ColumnCount = 7;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
            TableLayoutPanel1.Controls.Add(Label10, 5, 0);
            TableLayoutPanel1.Controls.Add(Label8, 1, 0);
            TableLayoutPanel1.Controls.Add(Label6, 4, 0);
            TableLayoutPanel1.Controls.Add(Label2, 0, 2);
            TableLayoutPanel1.Controls.Add(Label5, 3, 0);
            TableLayoutPanel1.Controls.Add(Label3, 0, 3);
            TableLayoutPanel1.Controls.Add(Label4, 2, 0);
            TableLayoutPanel1.Controls.Add(BtnFontIN, 1, 1);
            TableLayoutPanel1.Controls.Add(BtnBackIN, 2, 1);
            TableLayoutPanel1.Controls.Add(BtnSyncIN, 3, 1);
            TableLayoutPanel1.Controls.Add(BtnSelectIN, 4, 1);
            TableLayoutPanel1.Controls.Add(BtnFontOUT, 1, 2);
            TableLayoutPanel1.Controls.Add(BtnBackOUT, 2, 2);
            TableLayoutPanel1.Controls.Add(BtnSyncOUT, 3, 2);
            TableLayoutPanel1.Controls.Add(BtnSelectOUT, 4, 2);
            TableLayoutPanel1.Controls.Add(BtnBackUNK, 2, 3);
            TableLayoutPanel1.Controls.Add(BtnSyncUNK, 3, 3);
            TableLayoutPanel1.Controls.Add(BtnSelectUNK, 4, 3);
            TableLayoutPanel1.Controls.Add(Label1, 0, 1);
            TableLayoutPanel1.Controls.Add(Label7, 6, 0);
            TableLayoutPanel1.Controls.Add(BtnBarIN, 6, 1);
            TableLayoutPanel1.Controls.Add(BtnBarOUT, 6, 2);
            TableLayoutPanel1.Controls.Add(BtnBarUNK, 6, 3);
            TableLayoutPanel1.Controls.Add(Label9, 0, 0);
            TableLayoutPanel1.Controls.Add(BtnSelectedFontIN, 5, 1);
            TableLayoutPanel1.Controls.Add(BtnSelectedFontOUT, 5, 2);
            TableLayoutPanel1.Controls.Add(BtnSelectedFontUNK, 5, 3);
            TableLayoutPanel1.Controls.Add(BtnFontUNK, 1, 3);
            TableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            TableLayoutPanel1.Location = new Point(7, 22);
            TableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 4;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.Size = new Size(646, 179);
            TableLayoutPanel1.TabIndex = 10;
            // 
            // Label10
            // 
            Label10.AutoSize = true;
            Label10.Dock = DockStyle.Fill;
            Label10.Location = new Point(461, 2);
            Label10.Margin = new Padding(4, 0, 4, 0);
            Label10.Name = "Label10";
            Label10.Size = new Size(81, 42);
            Label10.TabIndex = 29;
            Label10.Text = "Selected Text";
            Label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnFontIN
            // 
            BtnFontIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnFontIN.AutoSize = true;
            BtnFontIN.BackColor = Color.Lime;
            BtnFontIN.Location = new Point(97, 49);
            BtnFontIN.Margin = new Padding(4, 3, 4, 3);
            BtnFontIN.Name = "BtnFontIN";
            BtnFontIN.Size = new Size(81, 36);
            BtnFontIN.TabIndex = 10;
            BtnFontIN.Tag = "1";
            BtnFontIN.UseVisualStyleBackColor = false;
            BtnFontIN.Click += BtnColorButton_Click;
            // 
            // BtnBackIN
            // 
            BtnBackIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBackIN.AutoSize = true;
            BtnBackIN.BackColor = Color.Lime;
            BtnBackIN.Location = new Point(188, 49);
            BtnBackIN.Margin = new Padding(4, 3, 4, 3);
            BtnBackIN.Name = "BtnBackIN";
            BtnBackIN.Size = new Size(81, 36);
            BtnBackIN.TabIndex = 11;
            BtnBackIN.Tag = "1";
            BtnBackIN.UseVisualStyleBackColor = false;
            BtnBackIN.Click += BtnColorButton_Click;
            // 
            // BtnSyncIN
            // 
            BtnSyncIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSyncIN.AutoSize = true;
            BtnSyncIN.BackColor = Color.Lime;
            BtnSyncIN.Location = new Point(279, 49);
            BtnSyncIN.Margin = new Padding(4, 3, 4, 3);
            BtnSyncIN.Name = "BtnSyncIN";
            BtnSyncIN.Size = new Size(81, 36);
            BtnSyncIN.TabIndex = 12;
            BtnSyncIN.Tag = "1";
            BtnSyncIN.UseVisualStyleBackColor = false;
            BtnSyncIN.Click += BtnColorButton_Click;
            // 
            // BtnSelectIN
            // 
            BtnSelectIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectIN.AutoSize = true;
            BtnSelectIN.BackColor = Color.Lime;
            BtnSelectIN.Location = new Point(370, 49);
            BtnSelectIN.Margin = new Padding(4, 3, 4, 3);
            BtnSelectIN.Name = "BtnSelectIN";
            BtnSelectIN.Size = new Size(81, 36);
            BtnSelectIN.TabIndex = 13;
            BtnSelectIN.Tag = "1";
            BtnSelectIN.UseVisualStyleBackColor = false;
            BtnSelectIN.Click += BtnColorButton_Click;
            // 
            // BtnFontOUT
            // 
            BtnFontOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnFontOUT.AutoSize = true;
            BtnFontOUT.BackColor = Color.Blue;
            BtnFontOUT.Location = new Point(97, 93);
            BtnFontOUT.Margin = new Padding(4, 3, 4, 3);
            BtnFontOUT.Name = "BtnFontOUT";
            BtnFontOUT.Size = new Size(81, 36);
            BtnFontOUT.TabIndex = 15;
            BtnFontOUT.Tag = "1";
            BtnFontOUT.UseVisualStyleBackColor = false;
            BtnFontOUT.Click += BtnColorButton_Click;
            // 
            // BtnBackOUT
            // 
            BtnBackOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBackOUT.AutoSize = true;
            BtnBackOUT.BackColor = Color.Blue;
            BtnBackOUT.Location = new Point(188, 93);
            BtnBackOUT.Margin = new Padding(4, 3, 4, 3);
            BtnBackOUT.Name = "BtnBackOUT";
            BtnBackOUT.Size = new Size(81, 36);
            BtnBackOUT.TabIndex = 16;
            BtnBackOUT.Tag = "1";
            BtnBackOUT.UseVisualStyleBackColor = false;
            BtnBackOUT.Click += BtnColorButton_Click;
            // 
            // BtnSyncOUT
            // 
            BtnSyncOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSyncOUT.AutoSize = true;
            BtnSyncOUT.BackColor = Color.Blue;
            BtnSyncOUT.Location = new Point(279, 93);
            BtnSyncOUT.Margin = new Padding(4, 3, 4, 3);
            BtnSyncOUT.Name = "BtnSyncOUT";
            BtnSyncOUT.Size = new Size(81, 36);
            BtnSyncOUT.TabIndex = 17;
            BtnSyncOUT.Tag = "1";
            BtnSyncOUT.UseVisualStyleBackColor = false;
            BtnSyncOUT.Click += BtnColorButton_Click;
            // 
            // BtnSelectOUT
            // 
            BtnSelectOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectOUT.AutoSize = true;
            BtnSelectOUT.BackColor = Color.Blue;
            BtnSelectOUT.Location = new Point(370, 93);
            BtnSelectOUT.Margin = new Padding(4, 3, 4, 3);
            BtnSelectOUT.Name = "BtnSelectOUT";
            BtnSelectOUT.Size = new Size(81, 36);
            BtnSelectOUT.TabIndex = 18;
            BtnSelectOUT.Tag = "1";
            BtnSelectOUT.UseVisualStyleBackColor = false;
            BtnSelectOUT.Click += BtnColorButton_Click;
            // 
            // BtnBackUNK
            // 
            BtnBackUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBackUNK.AutoSize = true;
            BtnBackUNK.BackColor = Color.White;
            BtnBackUNK.Location = new Point(188, 137);
            BtnBackUNK.Margin = new Padding(4, 3, 4, 3);
            BtnBackUNK.Name = "BtnBackUNK";
            BtnBackUNK.Size = new Size(81, 37);
            BtnBackUNK.TabIndex = 21;
            BtnBackUNK.Tag = "1";
            BtnBackUNK.UseVisualStyleBackColor = false;
            BtnBackUNK.Click += BtnColorButton_Click;
            // 
            // BtnSyncUNK
            // 
            BtnSyncUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSyncUNK.AutoSize = true;
            BtnSyncUNK.BackColor = Color.White;
            BtnSyncUNK.Location = new Point(279, 137);
            BtnSyncUNK.Margin = new Padding(4, 3, 4, 3);
            BtnSyncUNK.Name = "BtnSyncUNK";
            BtnSyncUNK.Size = new Size(81, 37);
            BtnSyncUNK.TabIndex = 22;
            BtnSyncUNK.Tag = "1";
            BtnSyncUNK.UseVisualStyleBackColor = false;
            BtnSyncUNK.Click += BtnColorButton_Click;
            // 
            // BtnSelectUNK
            // 
            BtnSelectUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectUNK.AutoSize = true;
            BtnSelectUNK.BackColor = Color.White;
            BtnSelectUNK.Location = new Point(370, 137);
            BtnSelectUNK.Margin = new Padding(4, 3, 4, 3);
            BtnSelectUNK.Name = "BtnSelectUNK";
            BtnSelectUNK.Size = new Size(81, 37);
            BtnSelectUNK.TabIndex = 23;
            BtnSelectUNK.Tag = "1";
            BtnSelectUNK.UseVisualStyleBackColor = false;
            BtnSelectUNK.Click += BtnColorButton_Click;
            // 
            // BtnBarIN
            // 
            BtnBarIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBarIN.AutoSize = true;
            BtnBarIN.BackColor = Color.Lime;
            BtnBarIN.Location = new Point(552, 49);
            BtnBarIN.Margin = new Padding(4, 3, 4, 3);
            BtnBarIN.Name = "BtnBarIN";
            BtnBarIN.Size = new Size(88, 36);
            BtnBarIN.TabIndex = 14;
            BtnBarIN.Tag = "1";
            BtnBarIN.UseVisualStyleBackColor = false;
            BtnBarIN.Click += BtnColorButton_Click;
            // 
            // BtnBarOUT
            // 
            BtnBarOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBarOUT.AutoSize = true;
            BtnBarOUT.BackColor = Color.Blue;
            BtnBarOUT.Location = new Point(552, 93);
            BtnBarOUT.Margin = new Padding(4, 3, 4, 3);
            BtnBarOUT.Name = "BtnBarOUT";
            BtnBarOUT.Size = new Size(88, 36);
            BtnBarOUT.TabIndex = 19;
            BtnBarOUT.Tag = "1";
            BtnBarOUT.UseVisualStyleBackColor = false;
            BtnBarOUT.Click += BtnColorButton_Click;
            // 
            // BtnBarUNK
            // 
            BtnBarUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnBarUNK.AutoSize = true;
            BtnBarUNK.BackColor = Color.White;
            BtnBarUNK.Location = new Point(552, 137);
            BtnBarUNK.Margin = new Padding(4, 3, 4, 3);
            BtnBarUNK.Name = "BtnBarUNK";
            BtnBarUNK.Size = new Size(88, 37);
            BtnBarUNK.TabIndex = 24;
            BtnBarUNK.Tag = "1";
            BtnBarUNK.UseVisualStyleBackColor = false;
            BtnBarUNK.Click += BtnColorButton_Click;
            // 
            // Label9
            // 
            Label9.AutoSize = true;
            Label9.Dock = DockStyle.Fill;
            Label9.Location = new Point(6, 2);
            Label9.Margin = new Padding(4, 0, 4, 0);
            Label9.Name = "Label9";
            Label9.Size = new Size(81, 42);
            Label9.TabIndex = 25;
            Label9.Text = "Colors";
            Label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnSelectedFontIN
            // 
            BtnSelectedFontIN.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectedFontIN.AutoSize = true;
            BtnSelectedFontIN.BackColor = Color.Lime;
            BtnSelectedFontIN.Location = new Point(461, 49);
            BtnSelectedFontIN.Margin = new Padding(4, 3, 4, 3);
            BtnSelectedFontIN.Name = "BtnSelectedFontIN";
            BtnSelectedFontIN.Size = new Size(81, 36);
            BtnSelectedFontIN.TabIndex = 26;
            BtnSelectedFontIN.Tag = "1";
            BtnSelectedFontIN.UseVisualStyleBackColor = false;
            BtnSelectedFontIN.Click += BtnColorButton_Click;
            // 
            // BtnSelectedFontOUT
            // 
            BtnSelectedFontOUT.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectedFontOUT.AutoSize = true;
            BtnSelectedFontOUT.BackColor = Color.Blue;
            BtnSelectedFontOUT.Location = new Point(461, 93);
            BtnSelectedFontOUT.Margin = new Padding(4, 3, 4, 3);
            BtnSelectedFontOUT.Name = "BtnSelectedFontOUT";
            BtnSelectedFontOUT.Size = new Size(81, 36);
            BtnSelectedFontOUT.TabIndex = 27;
            BtnSelectedFontOUT.Tag = "1";
            BtnSelectedFontOUT.UseVisualStyleBackColor = false;
            BtnSelectedFontOUT.Click += BtnColorButton_Click;
            // 
            // BtnSelectedFontUNK
            // 
            BtnSelectedFontUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectedFontUNK.AutoSize = true;
            BtnSelectedFontUNK.BackColor = Color.White;
            BtnSelectedFontUNK.Location = new Point(461, 137);
            BtnSelectedFontUNK.Margin = new Padding(4, 3, 4, 3);
            BtnSelectedFontUNK.Name = "BtnSelectedFontUNK";
            BtnSelectedFontUNK.Size = new Size(81, 37);
            BtnSelectedFontUNK.TabIndex = 28;
            BtnSelectedFontUNK.Tag = "1";
            BtnSelectedFontUNK.UseVisualStyleBackColor = false;
            BtnSelectedFontUNK.Click += BtnColorButton_Click;
            // 
            // BtnFontUNK
            // 
            BtnFontUNK.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnFontUNK.AutoSize = true;
            BtnFontUNK.BackColor = Color.Black;
            BtnFontUNK.Location = new Point(97, 137);
            BtnFontUNK.Margin = new Padding(4, 3, 4, 3);
            BtnFontUNK.Name = "BtnFontUNK";
            BtnFontUNK.Size = new Size(81, 37);
            BtnFontUNK.TabIndex = 20;
            BtnFontUNK.Tag = "1";
            BtnFontUNK.UseVisualStyleBackColor = false;
            BtnFontUNK.Click += BtnColorButton_Click;
            // 
            // BtnDefault
            // 
            BtnDefault.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnDefault.Location = new Point(580, 380);
            BtnDefault.Margin = new Padding(4, 3, 4, 3);
            BtnDefault.Name = "BtnDefault";
            BtnDefault.Size = new Size(88, 27);
            BtnDefault.TabIndex = 11;
            BtnDefault.Text = "Default";
            BtnDefault.UseVisualStyleBackColor = true;
            BtnDefault.Click += BtnDefault_Click;
            // 
            // GbVideoSettings
            // 
            GbVideoSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GbVideoSettings.Controls.Add(Label11);
            GbVideoSettings.Controls.Add(RbAutoLoadVideoYoutube);
            GbVideoSettings.Controls.Add(RbAutoLoadVideoLocalOnly);
            GbVideoSettings.Controls.Add(RbAutoLoadVideoNever);
            GbVideoSettings.Location = new Point(8, 62);
            GbVideoSettings.Margin = new Padding(4, 3, 4, 3);
            GbVideoSettings.Name = "GbVideoSettings";
            GbVideoSettings.Padding = new Padding(4, 3, 4, 3);
            GbVideoSettings.Size = new Size(656, 64);
            GbVideoSettings.TabIndex = 13;
            GbVideoSettings.TabStop = false;
            GbVideoSettings.Text = "Video Linking Settings";
            // 
            // Label11
            // 
            Label11.AutoSize = true;
            Label11.Location = new Point(7, 19);
            Label11.Name = "Label11";
            Label11.Size = new Size(243, 15);
            Label11.TabIndex = 3;
            Label11.Text = "Open video together when the project opens";
            // 
            // RbAutoLoadVideoYoutube
            // 
            RbAutoLoadVideoYoutube.AutoSize = true;
            RbAutoLoadVideoYoutube.Enabled = false;
            RbAutoLoadVideoYoutube.Location = new Point(224, 37);
            RbAutoLoadVideoYoutube.Margin = new Padding(4, 3, 4, 3);
            RbAutoLoadVideoYoutube.Name = "RbAutoLoadVideoYoutube";
            RbAutoLoadVideoYoutube.Size = new Size(138, 19);
            RbAutoLoadVideoYoutube.TabIndex = 2;
            RbAutoLoadVideoYoutube.Text = "also for Youtube links";
            RbAutoLoadVideoYoutube.UseVisualStyleBackColor = true;
            // 
            // RbAutoLoadVideoLocalOnly
            // 
            RbAutoLoadVideoLocalOnly.AutoSize = true;
            RbAutoLoadVideoLocalOnly.Checked = true;
            RbAutoLoadVideoLocalOnly.Location = new Point(86, 37);
            RbAutoLoadVideoLocalOnly.Margin = new Padding(4, 3, 4, 3);
            RbAutoLoadVideoLocalOnly.Name = "RbAutoLoadVideoLocalOnly";
            RbAutoLoadVideoLocalOnly.Size = new Size(123, 19);
            RbAutoLoadVideoLocalOnly.TabIndex = 1;
            RbAutoLoadVideoLocalOnly.TabStop = true;
            RbAutoLoadVideoLocalOnly.Text = "for Local Files only";
            RbAutoLoadVideoLocalOnly.UseVisualStyleBackColor = true;
            // 
            // RbAutoLoadVideoNever
            // 
            RbAutoLoadVideoNever.AutoSize = true;
            RbAutoLoadVideoNever.Location = new Point(16, 37);
            RbAutoLoadVideoNever.Margin = new Padding(4, 3, 4, 3);
            RbAutoLoadVideoNever.Name = "RbAutoLoadVideoNever";
            RbAutoLoadVideoNever.Size = new Size(56, 19);
            RbAutoLoadVideoNever.TabIndex = 0;
            RbAutoLoadVideoNever.Text = "Never";
            RbAutoLoadVideoNever.UseVisualStyleBackColor = true;
            // 
            // GbListStyle
            // 
            GbListStyle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GbListStyle.Controls.Add(BtnPacketListFont);
            GbListStyle.Controls.Add(LabelPacketListArrows);
            GbListStyle.Controls.Add(Label12);
            GbListStyle.Controls.Add(BtnSetDarkMode);
            GbListStyle.Controls.Add(PictureBox3);
            GbListStyle.Controls.Add(PictureBox4);
            GbListStyle.Controls.Add(PictureBox2);
            GbListStyle.Controls.Add(PictureBox1);
            GbListStyle.Controls.Add(TableLayoutPanel1);
            GbListStyle.Controls.Add(RbListStyleTransparent);
            GbListStyle.Controls.Add(RbListStyleSolid);
            GbListStyle.Controls.Add(RbListStyleText);
            GbListStyle.Location = new Point(7, 7);
            GbListStyle.Margin = new Padding(4, 3, 4, 3);
            GbListStyle.Name = "GbListStyle";
            GbListStyle.Padding = new Padding(4, 3, 4, 3);
            GbListStyle.Size = new Size(660, 336);
            GbListStyle.TabIndex = 14;
            GbListStyle.TabStop = false;
            GbListStyle.Text = "Packet List Style";
            // 
            // BtnPacketListFont
            // 
            BtnPacketListFont.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnPacketListFont.Location = new Point(394, 255);
            BtnPacketListFont.Margin = new Padding(4, 3, 4, 3);
            BtnPacketListFont.Name = "BtnPacketListFont";
            BtnPacketListFont.Size = new Size(261, 27);
            BtnPacketListFont.TabIndex = 18;
            BtnPacketListFont.Text = "Select Font";
            BtnPacketListFont.UseVisualStyleBackColor = true;
            BtnPacketListFont.Click += BtnPacketListFont_Click;
            // 
            // LabelPacketListArrows
            // 
            LabelPacketListArrows.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LabelPacketListArrows.AutoSize = true;
            LabelPacketListArrows.Location = new Point(147, 255);
            LabelPacketListArrows.Margin = new Padding(4, 0, 4, 0);
            LabelPacketListArrows.Name = "LabelPacketListArrows";
            LabelPacketListArrows.Size = new Size(45, 15);
            LabelPacketListArrows.TabIndex = 17;
            LabelPacketListArrows.Text = "<=  =>";
            // 
            // Label12
            // 
            Label12.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Label12.AutoSize = true;
            Label12.Location = new Point(4, 234);
            Label12.Margin = new Padding(4, 0, 4, 0);
            Label12.Name = "Label12";
            Label12.Size = new Size(67, 15);
            Label12.TabIndex = 12;
            Label12.Text = "Arrow Style";
            // 
            // BtnSetDarkMode
            // 
            BtnSetDarkMode.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnSetDarkMode.Location = new Point(394, 302);
            BtnSetDarkMode.Margin = new Padding(4, 3, 4, 3);
            BtnSetDarkMode.Name = "BtnSetDarkMode";
            BtnSetDarkMode.Size = new Size(261, 27);
            BtnSetDarkMode.TabIndex = 16;
            BtnSetDarkMode.Text = "Set Dark Mode Packet Colors";
            BtnSetDarkMode.UseVisualStyleBackColor = true;
            BtnSetDarkMode.Click += BtnSetDarkMode_Click;
            // 
            // PictureBox3
            // 
            PictureBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PictureBox3.Image = Properties.Resources.mini_out_ticon;
            PictureBox3.Location = new Point(172, 312);
            PictureBox3.Margin = new Padding(4, 3, 4, 3);
            PictureBox3.Name = "PictureBox3";
            PictureBox3.Size = new Size(12, 12);
            PictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            PictureBox3.TabIndex = 6;
            PictureBox3.TabStop = false;
            // 
            // PictureBox4
            // 
            PictureBox4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PictureBox4.Image = Properties.Resources.mini_in_ticon;
            PictureBox4.Location = new Point(150, 312);
            PictureBox4.Margin = new Padding(4, 3, 4, 3);
            PictureBox4.Name = "PictureBox4";
            PictureBox4.Size = new Size(12, 12);
            PictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
            PictureBox4.TabIndex = 5;
            PictureBox4.TabStop = false;
            // 
            // PictureBox2
            // 
            PictureBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PictureBox2.Image = Properties.Resources.mini_out_icon;
            PictureBox2.Location = new Point(172, 285);
            PictureBox2.Margin = new Padding(4, 3, 4, 3);
            PictureBox2.Name = "PictureBox2";
            PictureBox2.Size = new Size(12, 12);
            PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            PictureBox2.TabIndex = 4;
            PictureBox2.TabStop = false;
            // 
            // PictureBox1
            // 
            PictureBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PictureBox1.Image = Properties.Resources.mini_in_icon;
            PictureBox1.Location = new Point(150, 285);
            PictureBox1.Margin = new Padding(4, 3, 4, 3);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new Size(12, 12);
            PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            PictureBox1.TabIndex = 3;
            PictureBox1.TabStop = false;
            // 
            // RbListStyleTransparent
            // 
            RbListStyleTransparent.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RbListStyleTransparent.AutoSize = true;
            RbListStyleTransparent.Location = new Point(7, 306);
            RbListStyleTransparent.Margin = new Padding(4, 3, 4, 3);
            RbListStyleTransparent.Name = "RbListStyleTransparent";
            RbListStyleTransparent.Size = new Size(126, 19);
            RbListStyleTransparent.TabIndex = 2;
            RbListStyleTransparent.Text = "Transparent Arrows";
            RbListStyleTransparent.UseVisualStyleBackColor = true;
            // 
            // RbListStyleSolid
            // 
            RbListStyleSolid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RbListStyleSolid.AutoSize = true;
            RbListStyleSolid.Checked = true;
            RbListStyleSolid.Location = new Point(7, 280);
            RbListStyleSolid.Margin = new Padding(4, 3, 4, 3);
            RbListStyleSolid.Name = "RbListStyleSolid";
            RbListStyleSolid.Size = new Size(91, 19);
            RbListStyleSolid.TabIndex = 1;
            RbListStyleSolid.TabStop = true;
            RbListStyleSolid.Text = "Solid Arrows";
            RbListStyleSolid.UseVisualStyleBackColor = true;
            // 
            // RbListStyleText
            // 
            RbListStyleText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RbListStyleText.AutoSize = true;
            RbListStyleText.Location = new Point(7, 253);
            RbListStyleText.Margin = new Padding(4, 3, 4, 3);
            RbListStyleText.Name = "RbListStyleText";
            RbListStyleText.Size = new Size(86, 19);
            RbListStyleText.TabIndex = 0;
            RbListStyleText.Text = "Text Arrows";
            RbListStyleText.UseVisualStyleBackColor = true;
            // 
            // LayoutGridColors
            // 
            LayoutGridColors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LayoutGridColors.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            LayoutGridColors.ColumnCount = 4;
            LayoutGridColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            LayoutGridColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            LayoutGridColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            LayoutGridColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            LayoutGridColors.Controls.Add(BtnColField12, 0, 3);
            LayoutGridColors.Controls.Add(BtnColField14, 0, 3);
            LayoutGridColors.Controls.Add(BtnColField15, 0, 3);
            LayoutGridColors.Controls.Add(BtnColField1, 1, 0);
            LayoutGridColors.Controls.Add(BtnColField2, 2, 0);
            LayoutGridColors.Controls.Add(BtnColField3, 3, 0);
            LayoutGridColors.Controls.Add(BtnColField4, 0, 1);
            LayoutGridColors.Controls.Add(BtnColField5, 1, 1);
            LayoutGridColors.Controls.Add(BtnColField6, 2, 1);
            LayoutGridColors.Controls.Add(BtnColField7, 3, 1);
            LayoutGridColors.Controls.Add(BtnColField8, 0, 2);
            LayoutGridColors.Controls.Add(BtnColField9, 1, 2);
            LayoutGridColors.Controls.Add(BtnColField10, 2, 2);
            LayoutGridColors.Controls.Add(BtnColField11, 3, 2);
            LayoutGridColors.Controls.Add(BtnColField13, 0, 3);
            LayoutGridColors.Controls.Add(LFieldCol0, 0, 0);
            LayoutGridColors.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LayoutGridColors.Location = new Point(7, 22);
            LayoutGridColors.Margin = new Padding(4, 3, 4, 3);
            LayoutGridColors.Name = "LayoutGridColors";
            LayoutGridColors.RowCount = 4;
            LayoutGridColors.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            LayoutGridColors.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            LayoutGridColors.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            LayoutGridColors.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            LayoutGridColors.Size = new Size(646, 149);
            LayoutGridColors.TabIndex = 13;
            // 
            // BtnColField12
            // 
            BtnColField12.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField12.AutoSize = true;
            BtnColField12.BackColor = SystemColors.Control;
            BtnColField12.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField12.Location = new Point(6, 113);
            BtnColField12.Margin = new Padding(4, 3, 4, 3);
            BtnColField12.Name = "BtnColField12";
            BtnColField12.Size = new Size(151, 31);
            BtnColField12.TabIndex = 36;
            BtnColField12.Tag = "13";
            BtnColField12.Text = "0x0C";
            BtnColField12.UseVisualStyleBackColor = false;
            BtnColField12.Click += BtnColField_Click;
            // 
            // BtnColField14
            // 
            BtnColField14.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField14.AutoSize = true;
            BtnColField14.BackColor = SystemColors.Control;
            BtnColField14.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField14.Location = new Point(328, 113);
            BtnColField14.Margin = new Padding(4, 3, 4, 3);
            BtnColField14.Name = "BtnColField14";
            BtnColField14.Size = new Size(151, 31);
            BtnColField14.TabIndex = 35;
            BtnColField14.Tag = "15";
            BtnColField14.Text = "0x0E";
            BtnColField14.UseVisualStyleBackColor = false;
            BtnColField14.Click += BtnColField_Click;
            // 
            // BtnColField15
            // 
            BtnColField15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField15.AutoSize = true;
            BtnColField15.BackColor = SystemColors.Control;
            BtnColField15.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField15.Location = new Point(489, 113);
            BtnColField15.Margin = new Padding(4, 3, 4, 3);
            BtnColField15.Name = "BtnColField15";
            BtnColField15.Size = new Size(151, 31);
            BtnColField15.TabIndex = 34;
            BtnColField15.Tag = "16";
            BtnColField15.Text = "0x0F";
            BtnColField15.UseVisualStyleBackColor = false;
            BtnColField15.Click += BtnColField_Click;
            // 
            // BtnColField1
            // 
            BtnColField1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField1.AutoSize = true;
            BtnColField1.BackColor = SystemColors.Control;
            BtnColField1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField1.Location = new Point(167, 5);
            BtnColField1.Margin = new Padding(4, 3, 4, 3);
            BtnColField1.Name = "BtnColField1";
            BtnColField1.Size = new Size(151, 28);
            BtnColField1.TabIndex = 22;
            BtnColField1.Tag = "1";
            BtnColField1.Text = "0x01";
            BtnColField1.UseVisualStyleBackColor = false;
            BtnColField1.Click += BtnColField_Click;
            // 
            // BtnColField2
            // 
            BtnColField2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField2.AutoSize = true;
            BtnColField2.BackColor = SystemColors.Control;
            BtnColField2.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField2.Location = new Point(328, 5);
            BtnColField2.Margin = new Padding(4, 3, 4, 3);
            BtnColField2.Name = "BtnColField2";
            BtnColField2.Size = new Size(151, 28);
            BtnColField2.TabIndex = 23;
            BtnColField2.Tag = "2";
            BtnColField2.Text = "0x02";
            BtnColField2.UseVisualStyleBackColor = false;
            BtnColField2.Click += BtnColField_Click;
            // 
            // BtnColField3
            // 
            BtnColField3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField3.AutoSize = true;
            BtnColField3.BackColor = SystemColors.Control;
            BtnColField3.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField3.Location = new Point(489, 5);
            BtnColField3.Margin = new Padding(4, 3, 4, 3);
            BtnColField3.Name = "BtnColField3";
            BtnColField3.Size = new Size(151, 28);
            BtnColField3.TabIndex = 24;
            BtnColField3.Tag = "3";
            BtnColField3.Text = "0x03";
            BtnColField3.UseVisualStyleBackColor = false;
            BtnColField3.Click += BtnColField_Click;
            // 
            // BtnColField4
            // 
            BtnColField4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField4.AutoSize = true;
            BtnColField4.BackColor = SystemColors.Control;
            BtnColField4.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField4.Location = new Point(6, 41);
            BtnColField4.Margin = new Padding(4, 3, 4, 3);
            BtnColField4.Name = "BtnColField4";
            BtnColField4.Size = new Size(151, 28);
            BtnColField4.TabIndex = 25;
            BtnColField4.Tag = "4";
            BtnColField4.Text = "0x04";
            BtnColField4.UseVisualStyleBackColor = false;
            BtnColField4.Click += BtnColField_Click;
            // 
            // BtnColField5
            // 
            BtnColField5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField5.AutoSize = true;
            BtnColField5.BackColor = SystemColors.Control;
            BtnColField5.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField5.Location = new Point(167, 41);
            BtnColField5.Margin = new Padding(4, 3, 4, 3);
            BtnColField5.Name = "BtnColField5";
            BtnColField5.Size = new Size(151, 28);
            BtnColField5.TabIndex = 26;
            BtnColField5.Tag = "5";
            BtnColField5.Text = "0x05";
            BtnColField5.UseVisualStyleBackColor = false;
            BtnColField5.Click += BtnColField_Click;
            // 
            // BtnColField6
            // 
            BtnColField6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField6.AutoSize = true;
            BtnColField6.BackColor = SystemColors.Control;
            BtnColField6.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField6.Location = new Point(328, 41);
            BtnColField6.Margin = new Padding(4, 3, 4, 3);
            BtnColField6.Name = "BtnColField6";
            BtnColField6.Size = new Size(151, 28);
            BtnColField6.TabIndex = 27;
            BtnColField6.Tag = "6";
            BtnColField6.Text = "0x06";
            BtnColField6.UseVisualStyleBackColor = false;
            BtnColField6.Click += BtnColField_Click;
            // 
            // BtnColField7
            // 
            BtnColField7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField7.AutoSize = true;
            BtnColField7.BackColor = SystemColors.Control;
            BtnColField7.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField7.Location = new Point(489, 41);
            BtnColField7.Margin = new Padding(4, 3, 4, 3);
            BtnColField7.Name = "BtnColField7";
            BtnColField7.Size = new Size(151, 28);
            BtnColField7.TabIndex = 28;
            BtnColField7.Tag = "7";
            BtnColField7.Text = "0x07";
            BtnColField7.UseVisualStyleBackColor = false;
            BtnColField7.Click += BtnColField_Click;
            // 
            // BtnColField8
            // 
            BtnColField8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField8.AutoSize = true;
            BtnColField8.BackColor = SystemColors.Control;
            BtnColField8.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField8.Location = new Point(6, 77);
            BtnColField8.Margin = new Padding(4, 3, 4, 3);
            BtnColField8.Name = "BtnColField8";
            BtnColField8.Size = new Size(151, 28);
            BtnColField8.TabIndex = 29;
            BtnColField8.Tag = "8";
            BtnColField8.Text = "0x08";
            BtnColField8.UseVisualStyleBackColor = false;
            BtnColField8.Click += BtnColField_Click;
            // 
            // BtnColField9
            // 
            BtnColField9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField9.AutoSize = true;
            BtnColField9.BackColor = SystemColors.Control;
            BtnColField9.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField9.Location = new Point(167, 77);
            BtnColField9.Margin = new Padding(4, 3, 4, 3);
            BtnColField9.Name = "BtnColField9";
            BtnColField9.Size = new Size(151, 28);
            BtnColField9.TabIndex = 30;
            BtnColField9.Tag = "9";
            BtnColField9.Text = "0x09";
            BtnColField9.UseVisualStyleBackColor = false;
            BtnColField9.Click += BtnColField_Click;
            // 
            // BtnColField10
            // 
            BtnColField10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField10.AutoSize = true;
            BtnColField10.BackColor = SystemColors.Control;
            BtnColField10.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField10.Location = new Point(328, 77);
            BtnColField10.Margin = new Padding(4, 3, 4, 3);
            BtnColField10.Name = "BtnColField10";
            BtnColField10.Size = new Size(151, 28);
            BtnColField10.TabIndex = 31;
            BtnColField10.Tag = "10";
            BtnColField10.Text = "0x0A";
            BtnColField10.UseVisualStyleBackColor = false;
            BtnColField10.Click += BtnColField_Click;
            // 
            // BtnColField11
            // 
            BtnColField11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField11.AutoSize = true;
            BtnColField11.BackColor = SystemColors.Control;
            BtnColField11.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField11.Location = new Point(489, 77);
            BtnColField11.Margin = new Padding(4, 3, 4, 3);
            BtnColField11.Name = "BtnColField11";
            BtnColField11.Size = new Size(151, 28);
            BtnColField11.TabIndex = 32;
            BtnColField11.Tag = "11";
            BtnColField11.Text = "0x0B";
            BtnColField11.UseVisualStyleBackColor = false;
            BtnColField11.Click += BtnColField_Click;
            // 
            // BtnColField13
            // 
            BtnColField13.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BtnColField13.AutoSize = true;
            BtnColField13.BackColor = SystemColors.Control;
            BtnColField13.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            BtnColField13.Location = new Point(167, 113);
            BtnColField13.Margin = new Padding(4, 3, 4, 3);
            BtnColField13.Name = "BtnColField13";
            BtnColField13.Size = new Size(151, 31);
            BtnColField13.TabIndex = 33;
            BtnColField13.Tag = "14";
            BtnColField13.Text = "0x0D";
            BtnColField13.UseVisualStyleBackColor = false;
            BtnColField13.Click += BtnColField_Click;
            // 
            // LFieldCol0
            // 
            LFieldCol0.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LFieldCol0.AutoSize = true;
            LFieldCol0.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LFieldCol0.Location = new Point(6, 2);
            LFieldCol0.Margin = new Padding(4, 0, 4, 0);
            LFieldCol0.Name = "LFieldCol0";
            LFieldCol0.Size = new Size(151, 34);
            LFieldCol0.TabIndex = 37;
            LFieldCol0.Text = "Default Color";
            LFieldCol0.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // GbGridStyle
            // 
            GbGridStyle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GbGridStyle.BackColor = Color.Transparent;
            GbGridStyle.Controls.Add(BtnGridViewFont);
            GbGridStyle.Controls.Add(Label13);
            GbGridStyle.Controls.Add(LFieldColCount);
            GbGridStyle.Controls.Add(TbFieldColorCount);
            GbGridStyle.Controls.Add(LayoutGridColors);
            GbGridStyle.Location = new Point(7, 7);
            GbGridStyle.Margin = new Padding(4, 3, 4, 3);
            GbGridStyle.Name = "GbGridStyle";
            GbGridStyle.Padding = new Padding(4, 3, 4, 3);
            GbGridStyle.Size = new Size(660, 250);
            GbGridStyle.TabIndex = 17;
            GbGridStyle.TabStop = false;
            GbGridStyle.Text = "Field Grid Colors";
            // 
            // BtnGridViewFont
            // 
            BtnGridViewFont.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnGridViewFont.Location = new Point(392, 217);
            BtnGridViewFont.Margin = new Padding(4, 3, 4, 3);
            BtnGridViewFont.Name = "BtnGridViewFont";
            BtnGridViewFont.Size = new Size(261, 27);
            BtnGridViewFont.TabIndex = 19;
            BtnGridViewFont.Text = "Select Font";
            BtnGridViewFont.UseVisualStyleBackColor = true;
            BtnGridViewFont.Click += BtnGridViewFont_Click;
            // 
            // Label13
            // 
            Label13.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Label13.AutoSize = true;
            Label13.Location = new Point(13, 174);
            Label13.Margin = new Padding(4, 0, 4, 0);
            Label13.Name = "Label13";
            Label13.Size = new Size(218, 15);
            Label13.TabIndex = 16;
            Label13.Text = "Number of colors to use in the field grid";
            // 
            // LFieldColCount
            // 
            LFieldColCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            LFieldColCount.AutoSize = true;
            LFieldColCount.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            LFieldColCount.Location = new Point(332, 204);
            LFieldColCount.Margin = new Padding(4, 0, 4, 0);
            LFieldColCount.Name = "LFieldColCount";
            LFieldColCount.Size = new Size(18, 20);
            LFieldColCount.TabIndex = 15;
            LFieldColCount.Text = "1";
            // 
            // TbFieldColorCount
            // 
            TbFieldColorCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TbFieldColorCount.Location = new Point(13, 192);
            TbFieldColorCount.Margin = new Padding(4, 3, 4, 3);
            TbFieldColorCount.Maximum = 16;
            TbFieldColorCount.Minimum = 1;
            TbFieldColorCount.Name = "TbFieldColorCount";
            TbFieldColorCount.Size = new Size(313, 45);
            TbFieldColorCount.TabIndex = 14;
            TbFieldColorCount.TickStyle = TickStyle.Both;
            TbFieldColorCount.Value = 1;
            TbFieldColorCount.ValueChanged += TbFieldColorCount_ValueChanged;
            // 
            // GbOtherSettings
            // 
            GbOtherSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GbOtherSettings.Controls.Add(label14);
            GbOtherSettings.Controls.Add(DefaultCreditsTextBox);
            GbOtherSettings.Controls.Add(label15);
            GbOtherSettings.Controls.Add(CbShowHexStringData);
            GbOtherSettings.Location = new Point(7, 132);
            GbOtherSettings.Margin = new Padding(4, 3, 4, 3);
            GbOtherSettings.Name = "GbOtherSettings";
            GbOtherSettings.Padding = new Padding(4, 3, 4, 3);
            GbOtherSettings.Size = new Size(656, 119);
            GbOtherSettings.TabIndex = 18;
            GbOtherSettings.TabStop = false;
            GbOtherSettings.Text = "Miscellaneous Settings";
            // 
            // DefaultCreditsTextBox
            // 
            DefaultCreditsTextBox.Location = new Point(17, 82);
            DefaultCreditsTextBox.Name = "DefaultCreditsTextBox";
            DefaultCreditsTextBox.Size = new Size(182, 23);
            DefaultCreditsTextBox.TabIndex = 19;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(7, 59);
            label15.Name = "label15";
            label15.Size = new Size(120, 15);
            label15.TabIndex = 18;
            label15.Text = "Default Credits Name";
            // 
            // CbShowHexStringData
            // 
            CbShowHexStringData.AutoSize = true;
            CbShowHexStringData.Location = new Point(16, 22);
            CbShowHexStringData.Margin = new Padding(4, 3, 4, 3);
            CbShowHexStringData.Name = "CbShowHexStringData";
            CbShowHexStringData.Size = new Size(183, 19);
            CbShowHexStringData.TabIndex = 16;
            CbShowHexStringData.Text = "Also show hex data on strings";
            CbShowHexStringData.UseVisualStyleBackColor = true;
            // 
            // CbAskNewProject
            // 
            CbAskNewProject.AutoSize = true;
            CbAskNewProject.Location = new Point(16, 22);
            CbAskNewProject.Margin = new Padding(4, 3, 4, 3);
            CbAskNewProject.Name = "CbAskNewProject";
            CbAskNewProject.Size = new Size(221, 19);
            CbAskNewProject.TabIndex = 17;
            CbAskNewProject.Text = "Ask before creating a new project file";
            CbAskNewProject.UseVisualStyleBackColor = true;
            // 
            // TcSettings
            // 
            TcSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TcSettings.Controls.Add(TpGeneral);
            TcSettings.Controls.Add(TbPacketList);
            TcSettings.Controls.Add(TpFieldGrid);
            TcSettings.Location = new Point(0, 0);
            TcSettings.Margin = new Padding(4, 3, 4, 3);
            TcSettings.Name = "TcSettings";
            TcSettings.SelectedIndex = 0;
            TcSettings.Size = new Size(684, 374);
            TcSettings.TabIndex = 19;
            // 
            // TpGeneral
            // 
            TpGeneral.BackColor = SystemColors.Control;
            TpGeneral.Controls.Add(GroupBox1);
            TpGeneral.Controls.Add(GbOtherSettings);
            TpGeneral.Controls.Add(GbVideoSettings);
            TpGeneral.ForeColor = SystemColors.ControlText;
            TpGeneral.Location = new Point(4, 24);
            TpGeneral.Margin = new Padding(4, 3, 4, 3);
            TpGeneral.Name = "TpGeneral";
            TpGeneral.Padding = new Padding(4, 3, 4, 3);
            TpGeneral.Size = new Size(676, 346);
            TpGeneral.TabIndex = 0;
            TpGeneral.Text = "General";
            // 
            // GroupBox1
            // 
            GroupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GroupBox1.Controls.Add(CbAskNewProject);
            GroupBox1.Location = new Point(7, 7);
            GroupBox1.Margin = new Padding(4, 3, 4, 3);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Padding = new Padding(4, 3, 4, 3);
            GroupBox1.Size = new Size(656, 49);
            GroupBox1.TabIndex = 19;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Program Settings";
            // 
            // TbPacketList
            // 
            TbPacketList.BackColor = SystemColors.Control;
            TbPacketList.Controls.Add(GbListStyle);
            TbPacketList.ForeColor = SystemColors.ControlText;
            TbPacketList.Location = new Point(4, 24);
            TbPacketList.Margin = new Padding(4, 3, 4, 3);
            TbPacketList.Name = "TbPacketList";
            TbPacketList.Padding = new Padding(4, 3, 4, 3);
            TbPacketList.Size = new Size(676, 346);
            TbPacketList.TabIndex = 1;
            TbPacketList.Text = "Packet List";
            // 
            // TpFieldGrid
            // 
            TpFieldGrid.BackColor = SystemColors.Control;
            TpFieldGrid.Controls.Add(GroupBox3);
            TpFieldGrid.Controls.Add(GbGridStyle);
            TpFieldGrid.Location = new Point(4, 24);
            TpFieldGrid.Margin = new Padding(4, 3, 4, 3);
            TpFieldGrid.Name = "TpFieldGrid";
            TpFieldGrid.Padding = new Padding(4, 3, 4, 3);
            TpFieldGrid.Size = new Size(676, 346);
            TpFieldGrid.TabIndex = 2;
            TpFieldGrid.Text = "Data Views";
            // 
            // GroupBox3
            // 
            GroupBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GroupBox3.Controls.Add(BtnRawViewFont);
            GroupBox3.Location = new Point(7, 265);
            GroupBox3.Margin = new Padding(4, 3, 4, 3);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Padding = new Padding(4, 3, 4, 3);
            GroupBox3.Size = new Size(660, 72);
            GroupBox3.TabIndex = 18;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "Raw Data View";
            // 
            // BtnRawViewFont
            // 
            BtnRawViewFont.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnRawViewFont.Location = new Point(392, 38);
            BtnRawViewFont.Margin = new Padding(4, 3, 4, 3);
            BtnRawViewFont.Name = "BtnRawViewFont";
            BtnRawViewFont.Size = new Size(261, 27);
            BtnRawViewFont.TabIndex = 20;
            BtnRawViewFont.Text = "Select Font";
            BtnRawViewFont.UseVisualStyleBackColor = true;
            BtnRawViewFont.Click += BtnRawViewFont_Click;
            // 
            // FontDlg
            // 
            FontDlg.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FontDlg.FontMustExist = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Enabled = false;
            label14.Location = new Point(207, 87);
            label14.Name = "label14";
            label14.Size = new Size(260, 15);
            label14.TabIndex = 20;
            label14.Text = "If blank, your current login name is used instead";
            // 
            // ProgramSettingsForm
            // 
            AcceptButton = BtnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = BtnCancel;
            ClientSize = new Size(681, 417);
            ControlBox = false;
            Controls.Add(TcSettings);
            Controls.Add(BtnDefault);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOK);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgramSettingsForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "VieweD Settings";
            Load += SettingsForm_Load;
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            GbVideoSettings.ResumeLayout(false);
            GbVideoSettings.PerformLayout();
            GbListStyle.ResumeLayout(false);
            GbListStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            LayoutGridColors.ResumeLayout(false);
            LayoutGridColors.PerformLayout();
            GbGridStyle.ResumeLayout(false);
            GbGridStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)TbFieldColorCount).EndInit();
            GbOtherSettings.ResumeLayout(false);
            GbOtherSettings.PerformLayout();
            TcSettings.ResumeLayout(false);
            TpGeneral.ResumeLayout(false);
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            TbPacketList.ResumeLayout(false);
            TpFieldGrid.ResumeLayout(false);
            GroupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.ColorDialog ColorDlg;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.Label Label7;
        private System.Windows.Forms.Label Label8;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.Button BtnFontIN;
        private System.Windows.Forms.Button BtnBackIN;
        private System.Windows.Forms.Button BtnSyncIN;
        private System.Windows.Forms.Button BtnSelectIN;
        private System.Windows.Forms.Button BtnBarIN;
        private System.Windows.Forms.Button BtnFontOUT;
        private System.Windows.Forms.Button BtnBackOUT;
        private System.Windows.Forms.Button BtnSyncOUT;
        private System.Windows.Forms.Button BtnSelectOUT;
        private System.Windows.Forms.Button BtnBarOUT;
        private System.Windows.Forms.Button BtnFontUNK;
        private System.Windows.Forms.Button BtnBackUNK;
        private System.Windows.Forms.Button BtnSyncUNK;
        private System.Windows.Forms.Button BtnSelectUNK;
        private System.Windows.Forms.Button BtnBarUNK;
        private System.Windows.Forms.Label Label10;
        private System.Windows.Forms.Label Label9;
        private System.Windows.Forms.Button BtnSelectedFontIN;
        private System.Windows.Forms.Button BtnSelectedFontOUT;
        private System.Windows.Forms.Button BtnSelectedFontUNK;
        private System.Windows.Forms.Button BtnDefault;
        private System.Windows.Forms.GroupBox GbVideoSettings;
        private System.Windows.Forms.RadioButton RbAutoLoadVideoYoutube;
        private System.Windows.Forms.RadioButton RbAutoLoadVideoLocalOnly;
        private System.Windows.Forms.RadioButton RbAutoLoadVideoNever;
        private System.Windows.Forms.GroupBox GbListStyle;
        private System.Windows.Forms.Label Label12;
        private System.Windows.Forms.PictureBox PictureBox3;
        private System.Windows.Forms.PictureBox PictureBox4;
        private System.Windows.Forms.PictureBox PictureBox2;
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.RadioButton RbListStyleTransparent;
        private System.Windows.Forms.RadioButton RbListStyleSolid;
        private System.Windows.Forms.RadioButton RbListStyleText;
        private System.Windows.Forms.Button BtnSetDarkMode;
        private System.Windows.Forms.TableLayoutPanel LayoutGridColors;
        private System.Windows.Forms.Button BtnColField12;
        private System.Windows.Forms.Button BtnColField14;
        private System.Windows.Forms.Button BtnColField15;
        private System.Windows.Forms.Button BtnColField1;
        private System.Windows.Forms.Button BtnColField2;
        private System.Windows.Forms.Button BtnColField3;
        private System.Windows.Forms.Button BtnColField4;
        private System.Windows.Forms.Button BtnColField5;
        private System.Windows.Forms.Button BtnColField6;
        private System.Windows.Forms.Button BtnColField7;
        private System.Windows.Forms.Button BtnColField8;
        private System.Windows.Forms.Button BtnColField9;
        private System.Windows.Forms.Button BtnColField10;
        private System.Windows.Forms.Button BtnColField11;
        private System.Windows.Forms.Button BtnColField13;
        private System.Windows.Forms.Label LFieldCol0;
        private System.Windows.Forms.GroupBox GbGridStyle;
        private System.Windows.Forms.Label LFieldColCount;
        private System.Windows.Forms.TrackBar TbFieldColorCount;
        private System.Windows.Forms.GroupBox GbOtherSettings;
        private System.Windows.Forms.CheckBox CbShowHexStringData;
        private System.Windows.Forms.CheckBox CbAskNewProject;
        private System.Windows.Forms.TabControl TcSettings;
        private System.Windows.Forms.TabPage TpGeneral;
        private System.Windows.Forms.TabPage TbPacketList;
        private System.Windows.Forms.TabPage TpFieldGrid;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.Label LabelPacketListArrows;
        private System.Windows.Forms.Label Label13;
        private System.Windows.Forms.Button BtnPacketListFont;
        private System.Windows.Forms.FontDialog FontDlg;
        private System.Windows.Forms.Button BtnGridViewFont;
        private System.Windows.Forms.GroupBox GroupBox3;
        private System.Windows.Forms.Button BtnRawViewFont;
        private Label Label11;
        private TextBox DefaultCreditsTextBox;
        private Label label15;
        private Label label14;
    }
}