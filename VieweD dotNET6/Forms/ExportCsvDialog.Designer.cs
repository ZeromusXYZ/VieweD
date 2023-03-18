namespace VieweD.Forms
{
    partial class ExportCsvDialog
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
            SelectedFieldsListBox = new CheckedListBox();
            label1 = new Label();
            BtnDefaultSelection = new Button();
            BtnSelectAll = new Button();
            BtnUnselectAll = new Button();
            BtnSelectQuestionMarks = new Button();
            ExportFileDialog = new SaveFileDialog();
            BtnExport = new Button();
            BtnClose = new Button();
            groupBox1 = new GroupBox();
            DelimiterTab = new RadioButton();
            DelimiterSemicolon = new RadioButton();
            DelimiterComma = new RadioButton();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // SelectedFieldsListBox
            // 
            SelectedFieldsListBox.FormattingEnabled = true;
            SelectedFieldsListBox.Location = new Point(12, 42);
            SelectedFieldsListBox.Name = "SelectedFieldsListBox";
            SelectedFieldsListBox.Size = new Size(270, 256);
            SelectedFieldsListBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(149, 15);
            label1.TabIndex = 1;
            label1.Text = "Select what fields to export";
            // 
            // BtnDefaultSelection
            // 
            BtnDefaultSelection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnDefaultSelection.Location = new Point(288, 100);
            BtnDefaultSelection.Name = "BtnDefaultSelection";
            BtnDefaultSelection.Size = new Size(200, 23);
            BtnDefaultSelection.TabIndex = 2;
            BtnDefaultSelection.Text = "Default Selection";
            BtnDefaultSelection.UseVisualStyleBackColor = true;
            BtnDefaultSelection.Click += BtnDefaultSelection_Click;
            // 
            // BtnSelectAll
            // 
            BtnSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectAll.Location = new Point(288, 42);
            BtnSelectAll.Name = "BtnSelectAll";
            BtnSelectAll.Size = new Size(200, 23);
            BtnSelectAll.TabIndex = 3;
            BtnSelectAll.Text = "Select All";
            BtnSelectAll.UseVisualStyleBackColor = true;
            BtnSelectAll.Click += BtnSelectAll_Click;
            // 
            // BtnUnselectAll
            // 
            BtnUnselectAll.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnUnselectAll.Location = new Point(288, 71);
            BtnUnselectAll.Name = "BtnUnselectAll";
            BtnUnselectAll.Size = new Size(200, 23);
            BtnUnselectAll.TabIndex = 4;
            BtnUnselectAll.Text = "Unselect All";
            BtnUnselectAll.UseVisualStyleBackColor = true;
            BtnUnselectAll.Click += BtnUnselectAll_Click;
            // 
            // BtnSelectQuestionMarks
            // 
            BtnSelectQuestionMarks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnSelectQuestionMarks.Location = new Point(288, 129);
            BtnSelectQuestionMarks.Name = "BtnSelectQuestionMarks";
            BtnSelectQuestionMarks.Size = new Size(200, 23);
            BtnSelectQuestionMarks.TabIndex = 5;
            BtnSelectQuestionMarks.Text = "Add all fields with a ?";
            BtnSelectQuestionMarks.UseVisualStyleBackColor = true;
            BtnSelectQuestionMarks.Click += BtnSelectQuestionMarks_Click;
            // 
            // ExportFileDialog
            // 
            ExportFileDialog.DefaultExt = "csv";
            ExportFileDialog.Filter = "CSV Files|*.csv|All files|*.*";
            ExportFileDialog.Title = "Export file";
            // 
            // BtnExport
            // 
            BtnExport.Image = Properties.Resources.document_save_16;
            BtnExport.ImageAlign = ContentAlignment.MiddleLeft;
            BtnExport.Location = new Point(12, 304);
            BtnExport.Name = "BtnExport";
            BtnExport.Size = new Size(114, 23);
            BtnExport.TabIndex = 6;
            BtnExport.Text = "Export";
            BtnExport.UseVisualStyleBackColor = true;
            BtnExport.Click += BtnExport_Click;
            // 
            // BtnClose
            // 
            BtnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnClose.Location = new Point(413, 304);
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new Size(75, 23);
            BtnClose.TabIndex = 7;
            BtnClose.Text = "Close";
            BtnClose.UseVisualStyleBackColor = true;
            BtnClose.Click += BtnClose_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(DelimiterTab);
            groupBox1.Controls.Add(DelimiterSemicolon);
            groupBox1.Controls.Add(DelimiterComma);
            groupBox1.Location = new Point(288, 158);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(200, 100);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Delimiter";
            // 
            // DelimiterTab
            // 
            DelimiterTab.AutoSize = true;
            DelimiterTab.Location = new Point(6, 72);
            DelimiterTab.Name = "DelimiterTab";
            DelimiterTab.Size = new Size(43, 19);
            DelimiterTab.TabIndex = 2;
            DelimiterTab.Text = "Tab";
            DelimiterTab.UseVisualStyleBackColor = true;
            // 
            // DelimiterSemicolon
            // 
            DelimiterSemicolon.AutoSize = true;
            DelimiterSemicolon.Location = new Point(6, 47);
            DelimiterSemicolon.Name = "DelimiterSemicolon";
            DelimiterSemicolon.Size = new Size(137, 19);
            DelimiterSemicolon.TabIndex = 1;
            DelimiterSemicolon.Text = "Semicolon (for Excel)";
            DelimiterSemicolon.UseVisualStyleBackColor = true;
            // 
            // DelimiterComma
            // 
            DelimiterComma.AutoSize = true;
            DelimiterComma.Checked = true;
            DelimiterComma.Location = new Point(6, 22);
            DelimiterComma.Name = "DelimiterComma";
            DelimiterComma.Size = new Size(125, 19);
            DelimiterComma.TabIndex = 0;
            DelimiterComma.TabStop = true;
            DelimiterComma.Text = "Comma (standard)";
            DelimiterComma.UseVisualStyleBackColor = true;
            // 
            // ExportCsvDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(498, 334);
            Controls.Add(groupBox1);
            Controls.Add(BtnClose);
            Controls.Add(BtnExport);
            Controls.Add(BtnSelectQuestionMarks);
            Controls.Add(BtnUnselectAll);
            Controls.Add(BtnSelectAll);
            Controls.Add(BtnDefaultSelection);
            Controls.Add(label1);
            Controls.Add(SelectedFieldsListBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ExportCsvDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export as CSV";
            Load += ExportCsvDialog_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckedListBox SelectedFieldsListBox;
        private Label label1;
        private Button BtnDefaultSelection;
        private Button BtnSelectAll;
        private Button BtnUnselectAll;
        private Button BtnSelectQuestionMarks;
        private SaveFileDialog ExportFileDialog;
        private Button BtnExport;
        private Button BtnClose;
        private GroupBox groupBox1;
        private RadioButton DelimiterTab;
        private RadioButton DelimiterSemicolon;
        private RadioButton DelimiterComma;
    }
}