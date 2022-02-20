namespace VieweD
{
    partial class ParseEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParseEditorForm));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.lPosInfo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tComment = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tName = new System.Windows.Forms.TextBox();
            this.lPos = new System.Windows.Forms.Label();
            this.tPos = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbLookup = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFieldType = new System.Windows.Forms.ComboBox();
            this.editBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.pmEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.miBlank1 = new System.Windows.Forms.ToolStripSeparator();
            this.docMap = new FastColoredTextBoxNS.DocumentMap();
            this.pOldEditControl = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.editBox)).BeginInit();
            this.pmEdit.SuspendLayout();
            this.pOldEditControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(502, 495);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(87, 25);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(595, 495);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(688, 495);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(87, 25);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsert.Location = new System.Drawing.Point(12, 494);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(229, 26);
            this.btnInsert.TabIndex = 29;
            this.btnInsert.Text = "Insert Line";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // lPosInfo
            // 
            this.lPosInfo.AutoSize = true;
            this.lPosInfo.Location = new System.Drawing.Point(124, 46);
            this.lPosInfo.Name = "lPosInfo";
            this.lPosInfo.Size = new System.Drawing.Size(182, 14);
            this.lPosInfo.TabIndex = 28;
            this.lPosInfo.Text = "byte[:startbit[-bitsize]]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(124, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(259, 14);
            this.label6.TabIndex = 27;
            this.label6.Text = "Leave Lookup empty for normal values";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(371, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(231, 14);
            this.label5.TabIndex = 26;
            this.label5.Text = "Comment (or Compare switchblock)";
            // 
            // tComment
            // 
            this.tComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tComment.Location = new System.Drawing.Point(374, 21);
            this.tComment.Name = "tComment";
            this.tComment.Size = new System.Drawing.Size(386, 22);
            this.tComment.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 24;
            this.label4.Text = "Display Name";
            // 
            // tName
            // 
            this.tName.Location = new System.Drawing.Point(238, 21);
            this.tName.Name = "tName";
            this.tName.Size = new System.Drawing.Size(130, 22);
            this.tName.TabIndex = 23;
            this.tName.Text = "MyField";
            // 
            // lPos
            // 
            this.lPos.AutoSize = true;
            this.lPos.Location = new System.Drawing.Point(124, 4);
            this.lPos.Name = "lPos";
            this.lPos.Size = new System.Drawing.Size(63, 14);
            this.lPos.TabIndex = 22;
            this.lPos.Text = "Position";
            // 
            // tPos
            // 
            this.tPos.Location = new System.Drawing.Point(127, 21);
            this.tPos.Name = "tPos";
            this.tPos.Size = new System.Drawing.Size(105, 22);
            this.tPos.TabIndex = 21;
            this.tPos.Text = "0x04";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 20;
            this.label2.Text = "Lookup";
            // 
            // cbLookup
            // 
            this.cbLookup.FormattingEnabled = true;
            this.cbLookup.Location = new System.Drawing.Point(4, 66);
            this.cbLookup.Name = "cbLookup";
            this.cbLookup.Size = new System.Drawing.Size(115, 22);
            this.cbLookup.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 18;
            this.label1.Text = "Type";
            // 
            // cbFieldType
            // 
            this.cbFieldType.DropDownWidth = 200;
            this.cbFieldType.FormattingEnabled = true;
            this.cbFieldType.Items.AddRange(new object[] {
            "uint32",
            "int32",
            "uint16",
            "int16",
            "byte",
            "float",
            "pos",
            "dir",
            "switchblock",
            "showblock",
            "info",
            "bit",
            "bits",
            "string",
            "data",
            "ms",
            "frames",
            "vanatime",
            "ip4",
            "linkshellstring",
            "inscribestring",
            "bitflaglist",
            "bitflaglist2",
            "combatskill",
            "craftskill",
            "equipsetitem",
            "equipsetitemlist",
            "abilityrecastlist",
            "blacklistentry",
            "meritentries",
            "playercheckitems",
            "bufficons",
            "bufftimers",
            "buffs",
            "jobpointentries",
            "shopitems",
            "guildshopitems",
            "jobpoints",
            "roequest",
            "packet-in-0x028"});
            this.cbFieldType.Location = new System.Drawing.Point(3, 21);
            this.cbFieldType.Name = "cbFieldType";
            this.cbFieldType.Size = new System.Drawing.Size(115, 22);
            this.cbFieldType.TabIndex = 17;
            this.cbFieldType.Text = "byte";
            // 
            // editBox
            // 
            this.editBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editBox.AutoCompleteBracketsList = new char[] {
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
            this.editBox.AutoIndentCharsPatterns = "";
            this.editBox.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.editBox.BackBrush = null;
            this.editBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.editBox.CharHeight = 14;
            this.editBox.CharWidth = 8;
            this.editBox.CommentPrefix = "";
            this.editBox.ContextMenuStrip = this.pmEdit;
            this.editBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.editBox.DescriptionFile = "";
            this.editBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.editBox.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.editBox.IsReplaceMode = false;
            this.editBox.Language = FastColoredTextBoxNS.Language.XML;
            this.editBox.LeftBracket = '<';
            this.editBox.LeftBracket2 = '(';
            this.editBox.Location = new System.Drawing.Point(14, 12);
            this.editBox.Name = "editBox";
            this.editBox.Paddings = new System.Windows.Forms.Padding(0);
            this.editBox.RightBracket = '>';
            this.editBox.RightBracket2 = ')';
            this.editBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.editBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("editBox.ServiceColors")));
            this.editBox.Size = new System.Drawing.Size(698, 368);
            this.editBox.TabIndex = 18;
            this.editBox.Zoom = 100;
            // 
            // pmEdit
            // 
            this.pmEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miInsert,
            this.miBlank1});
            this.pmEdit.Name = "pmEdit";
            this.pmEdit.Size = new System.Drawing.Size(104, 32);
            this.pmEdit.Opening += new System.ComponentModel.CancelEventHandler(this.pmEdit_Opening);
            // 
            // miInsert
            // 
            this.miInsert.Name = "miInsert";
            this.miInsert.Size = new System.Drawing.Size(103, 22);
            this.miInsert.Text = "Insert";
            // 
            // miBlank1
            // 
            this.miBlank1.Name = "miBlank1";
            this.miBlank1.Size = new System.Drawing.Size(100, 6);
            this.miBlank1.Click += new System.EventHandler(this.miCustomInsert_Click);
            // 
            // docMap
            // 
            this.docMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.docMap.ForeColor = System.Drawing.Color.Maroon;
            this.docMap.Location = new System.Drawing.Point(718, 12);
            this.docMap.Name = "docMap";
            this.docMap.Size = new System.Drawing.Size(57, 368);
            this.docMap.TabIndex = 30;
            this.docMap.Target = this.editBox;
            this.docMap.Text = "documentMap1";
            // 
            // pOldEditControl
            // 
            this.pOldEditControl.Controls.Add(this.label1);
            this.pOldEditControl.Controls.Add(this.label2);
            this.pOldEditControl.Controls.Add(this.tPos);
            this.pOldEditControl.Controls.Add(this.cbLookup);
            this.pOldEditControl.Controls.Add(this.lPos);
            this.pOldEditControl.Controls.Add(this.lPosInfo);
            this.pOldEditControl.Controls.Add(this.cbFieldType);
            this.pOldEditControl.Controls.Add(this.label6);
            this.pOldEditControl.Controls.Add(this.tName);
            this.pOldEditControl.Controls.Add(this.label4);
            this.pOldEditControl.Controls.Add(this.label5);
            this.pOldEditControl.Controls.Add(this.tComment);
            this.pOldEditControl.Location = new System.Drawing.Point(12, 386);
            this.pOldEditControl.Name = "pOldEditControl";
            this.pOldEditControl.Size = new System.Drawing.Size(763, 99);
            this.pOldEditControl.TabIndex = 31;
            // 
            // ParseEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 525);
            this.Controls.Add(this.pOldEditControl);
            this.Controls.Add(this.docMap);
            this.Controls.Add(this.btnInsert);
            this.Controls.Add(this.editBox);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Font = new System.Drawing.Font("Consolas", 9F);
            this.Name = "ParseEditorForm";
            this.Text = "Parser Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ParseEditorForm_FormClosed);
            this.Load += new System.EventHandler(this.ParseEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.editBox)).EndInit();
            this.pmEdit.ResumeLayout(false);
            this.pOldEditControl.ResumeLayout(false);
            this.pOldEditControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Label lPosInfo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tComment;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tName;
        private System.Windows.Forms.Label lPos;
        private System.Windows.Forms.TextBox tPos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbLookup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFieldType;
        private FastColoredTextBoxNS.FastColoredTextBox editBox;
        private FastColoredTextBoxNS.DocumentMap docMap;
        private System.Windows.Forms.ContextMenuStrip pmEdit;
        private System.Windows.Forms.ToolStripSeparator miBlank1;
        private System.Windows.Forms.ToolStripMenuItem miInsert;
        private System.Windows.Forms.Panel pOldEditControl;
    }
}