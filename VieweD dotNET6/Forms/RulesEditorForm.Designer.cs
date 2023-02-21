namespace VieweD.Forms
{
    partial class RulesEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RulesEditorForm));
            this.RuleEdit = new FastColoredTextBoxNS.FastColoredTextBox();
            this.MiInsert = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.RuleDocumentMap = new FastColoredTextBoxNS.DocumentMap();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.RuleEdit)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // RuleEdit
            // 
            this.RuleEdit.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.RuleEdit.AutoIndentCharsPatterns = "";
            this.RuleEdit.AutoScrollMinSize = new System.Drawing.Size(123, 14);
            this.RuleEdit.BackBrush = null;
            this.RuleEdit.CharHeight = 14;
            this.RuleEdit.CharWidth = 8;
            this.RuleEdit.CommentPrefix = null;
            this.RuleEdit.ContextMenuStrip = this.MiInsert;
            this.RuleEdit.DefaultMarkerSize = 8;
            this.RuleEdit.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.RuleEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RuleEdit.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.RuleEdit.IsReplaceMode = false;
            this.RuleEdit.Language = FastColoredTextBoxNS.Language.XML;
            this.RuleEdit.LeftBracket = '<';
            this.RuleEdit.LeftBracket2 = '(';
            this.RuleEdit.Location = new System.Drawing.Point(0, 0);
            this.RuleEdit.Name = "RuleEdit";
            this.RuleEdit.Paddings = new System.Windows.Forms.Padding(0);
            this.RuleEdit.RightBracket = '>';
            this.RuleEdit.RightBracket2 = ')';
            this.RuleEdit.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.RuleEdit.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("RuleEdit.ServiceColors")));
            this.RuleEdit.Size = new System.Drawing.Size(599, 399);
            this.RuleEdit.TabIndex = 0;
            this.RuleEdit.Text = "<ruledata />";
            this.RuleEdit.Zoom = 100;
            this.RuleEdit.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.RuleEdit_TextChanged);
            // 
            // MiInsert
            // 
            this.MiInsert.Name = "MiInsert";
            this.MiInsert.Size = new System.Drawing.Size(181, 26);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(688, 453);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(682, 399);
            this.panel1.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.RuleEdit);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(599, 399);
            this.panel4.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.RuleDocumentMap);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(599, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(83, 399);
            this.panel3.TabIndex = 4;
            // 
            // RuleDocumentMap
            // 
            this.RuleDocumentMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RuleDocumentMap.ForeColor = System.Drawing.Color.Maroon;
            this.RuleDocumentMap.Location = new System.Drawing.Point(0, 0);
            this.RuleDocumentMap.Name = "RuleDocumentMap";
            this.RuleDocumentMap.Size = new System.Drawing.Size(83, 399);
            this.RuleDocumentMap.TabIndex = 3;
            this.RuleDocumentMap.Target = this.RuleEdit;
            this.RuleDocumentMap.Text = "documentMap1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BtnSave);
            this.panel2.Controls.Add(this.BtnCancel);
            this.panel2.Controls.Add(this.BtnTest);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 408);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(682, 42);
            this.panel2.TabIndex = 2;
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.Image = global::VieweD.Properties.Resources.document_save_16;
            this.BtnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnSave.Location = new System.Drawing.Point(9, 10);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(90, 23);
            this.BtnSave.TabIndex = 2;
            this.BtnSave.Text = "&Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.Image = global::VieweD.Properties.Resources.view_close_16;
            this.BtnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnCancel.Location = new System.Drawing.Point(487, 10);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(90, 23);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnTest
            // 
            this.BtnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnTest.Image = global::VieweD.Properties.Resources.application_menu_16;
            this.BtnTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnTest.Location = new System.Drawing.Point(583, 10);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(90, 23);
            this.BtnTest.TabIndex = 0;
            this.BtnTest.Text = "Test";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // RulesEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 453);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RulesEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rule Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RulesEditorForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.RuleEdit)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox RuleEdit;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private FastColoredTextBoxNS.DocumentMap RuleDocumentMap;
        private Panel panel2;
        private Button BtnSave;
        private Button BtnCancel;
        private Button BtnTest;
        private Panel panel4;
        private Panel panel3;
        private ContextMenuStrip MiInsert;
    }
}