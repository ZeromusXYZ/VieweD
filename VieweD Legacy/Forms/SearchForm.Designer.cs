namespace VieweD
{
    partial class SearchForm
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
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbPacketType = new System.Windows.Forms.GroupBox();
            this.rbOutgoing = new System.Windows.Forms.RadioButton();
            this.rbIncoming = new System.Windows.Forms.RadioButton();
            this.rbAny = new System.Windows.Forms.RadioButton();
            this.btnAsNewTab = new System.Windows.Forms.Button();
            this.gbSearchPacket = new System.Windows.Forms.GroupBox();
            this.lPacketLevel = new System.Windows.Forms.Label();
            this.ePacketLevel = new System.Windows.Forms.TextBox();
            this.lPacketSync = new System.Windows.Forms.Label();
            this.lPacketId = new System.Windows.Forms.Label();
            this.eSync = new System.Windows.Forms.TextBox();
            this.ePacketID = new System.Windows.Forms.TextBox();
            this.rbUInt32 = new System.Windows.Forms.RadioButton();
            this.rbUInt16 = new System.Windows.Forms.RadioButton();
            this.rbByte = new System.Windows.Forms.RadioButton();
            this.lRawValue = new System.Windows.Forms.Label();
            this.eValue = new System.Windows.Forms.TextBox();
            this.gbRawValues = new System.Windows.Forms.GroupBox();
            this.rbUInt24 = new System.Windows.Forms.RadioButton();
            this.gbSearchByField = new System.Windows.Forms.GroupBox();
            this.cbFieldNames = new System.Windows.Forms.ComboBox();
            this.lParsedNote = new System.Windows.Forms.Label();
            this.lFieldValue = new System.Windows.Forms.Label();
            this.lFieldName = new System.Windows.Forms.Label();
            this.eFieldValue = new System.Windows.Forms.TextBox();
            this.gbPacketType.SuspendLayout();
            this.gbSearchPacket.SuspendLayout();
            this.gbRawValues.SuspendLayout();
            this.gbSearchByField.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFindNext
            // 
            this.btnFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindNext.Enabled = false;
            this.btnFindNext.Location = new System.Drawing.Point(405, 12);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(75, 23);
            this.btnFindNext.TabIndex = 0;
            this.btnFindNext.Text = "Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.BtnFindNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(405, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // gbPacketType
            // 
            this.gbPacketType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPacketType.Controls.Add(this.rbOutgoing);
            this.gbPacketType.Controls.Add(this.rbIncoming);
            this.gbPacketType.Controls.Add(this.rbAny);
            this.gbPacketType.Location = new System.Drawing.Point(12, 12);
            this.gbPacketType.Name = "gbPacketType";
            this.gbPacketType.Size = new System.Drawing.Size(375, 52);
            this.gbPacketType.TabIndex = 3;
            this.gbPacketType.TabStop = false;
            this.gbPacketType.Text = "Packet Types to search in";
            // 
            // rbOutgoing
            // 
            this.rbOutgoing.AutoSize = true;
            this.rbOutgoing.Location = new System.Drawing.Point(210, 19);
            this.rbOutgoing.Name = "rbOutgoing";
            this.rbOutgoing.Size = new System.Drawing.Size(68, 17);
            this.rbOutgoing.TabIndex = 2;
            this.rbOutgoing.Text = "Outgoing";
            this.rbOutgoing.UseVisualStyleBackColor = true;
            this.rbOutgoing.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // rbIncoming
            // 
            this.rbIncoming.AutoSize = true;
            this.rbIncoming.Location = new System.Drawing.Point(98, 19);
            this.rbIncoming.Name = "rbIncoming";
            this.rbIncoming.Size = new System.Drawing.Size(68, 17);
            this.rbIncoming.TabIndex = 1;
            this.rbIncoming.Text = "Incoming";
            this.rbIncoming.UseVisualStyleBackColor = true;
            this.rbIncoming.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // rbAny
            // 
            this.rbAny.AutoSize = true;
            this.rbAny.Checked = true;
            this.rbAny.Location = new System.Drawing.Point(6, 19);
            this.rbAny.Name = "rbAny";
            this.rbAny.Size = new System.Drawing.Size(43, 17);
            this.rbAny.TabIndex = 0;
            this.rbAny.TabStop = true;
            this.rbAny.Text = "Any";
            this.rbAny.UseVisualStyleBackColor = true;
            this.rbAny.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // btnAsNewTab
            // 
            this.btnAsNewTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAsNewTab.Enabled = false;
            this.btnAsNewTab.Location = new System.Drawing.Point(405, 89);
            this.btnAsNewTab.Name = "btnAsNewTab";
            this.btnAsNewTab.Size = new System.Drawing.Size(75, 23);
            this.btnAsNewTab.TabIndex = 2;
            this.btnAsNewTab.Text = "As New Tab";
            this.btnAsNewTab.UseVisualStyleBackColor = true;
            this.btnAsNewTab.Click += new System.EventHandler(this.BtnAsNewTab_Click);
            // 
            // gbSearchPacket
            // 
            this.gbSearchPacket.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbSearchPacket.Controls.Add(this.lPacketLevel);
            this.gbSearchPacket.Controls.Add(this.ePacketLevel);
            this.gbSearchPacket.Controls.Add(this.lPacketSync);
            this.gbSearchPacket.Controls.Add(this.lPacketId);
            this.gbSearchPacket.Controls.Add(this.eSync);
            this.gbSearchPacket.Controls.Add(this.ePacketID);
            this.gbSearchPacket.Location = new System.Drawing.Point(12, 70);
            this.gbSearchPacket.Name = "gbSearchPacket";
            this.gbSearchPacket.Size = new System.Drawing.Size(218, 102);
            this.gbSearchPacket.TabIndex = 4;
            this.gbSearchPacket.TabStop = false;
            this.gbSearchPacket.Text = "Search for packet";
            // 
            // lPacketLevel
            // 
            this.lPacketLevel.AutoSize = true;
            this.lPacketLevel.Location = new System.Drawing.Point(6, 45);
            this.lPacketLevel.Name = "lPacketLevel";
            this.lPacketLevel.Size = new System.Drawing.Size(70, 13);
            this.lPacketLevel.TabIndex = 2;
            this.lPacketLevel.Text = "Packet Level";
            // 
            // ePacketLevel
            // 
            this.ePacketLevel.Location = new System.Drawing.Point(136, 42);
            this.ePacketLevel.Name = "ePacketLevel";
            this.ePacketLevel.Size = new System.Drawing.Size(68, 20);
            this.ePacketLevel.TabIndex = 3;
            this.ePacketLevel.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // lPacketSync
            // 
            this.lPacketSync.AutoSize = true;
            this.lPacketSync.Location = new System.Drawing.Point(6, 71);
            this.lPacketSync.Name = "lPacketSync";
            this.lPacketSync.Size = new System.Drawing.Size(71, 13);
            this.lPacketSync.TabIndex = 4;
            this.lPacketSync.Text = "Sync Number";
            // 
            // lPacketId
            // 
            this.lPacketId.AutoSize = true;
            this.lPacketId.Location = new System.Drawing.Point(6, 19);
            this.lPacketId.Name = "lPacketId";
            this.lPacketId.Size = new System.Drawing.Size(88, 13);
            this.lPacketId.TabIndex = 0;
            this.lPacketId.Text = "Packet ID (Type)";
            // 
            // eSync
            // 
            this.eSync.Location = new System.Drawing.Point(136, 68);
            this.eSync.Name = "eSync";
            this.eSync.Size = new System.Drawing.Size(68, 20);
            this.eSync.TabIndex = 5;
            this.eSync.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // ePacketID
            // 
            this.ePacketID.Location = new System.Drawing.Point(136, 16);
            this.ePacketID.Name = "ePacketID";
            this.ePacketID.Size = new System.Drawing.Size(68, 20);
            this.ePacketID.TabIndex = 1;
            this.ePacketID.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // rbUInt32
            // 
            this.rbUInt32.AutoSize = true;
            this.rbUInt32.Location = new System.Drawing.Point(389, 22);
            this.rbUInt32.Name = "rbUInt32";
            this.rbUInt32.Size = new System.Drawing.Size(54, 17);
            this.rbUInt32.TabIndex = 5;
            this.rbUInt32.Text = "uint32";
            this.rbUInt32.UseVisualStyleBackColor = true;
            this.rbUInt32.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // rbUInt16
            // 
            this.rbUInt16.AutoSize = true;
            this.rbUInt16.Location = new System.Drawing.Point(269, 22);
            this.rbUInt16.Name = "rbUInt16";
            this.rbUInt16.Size = new System.Drawing.Size(54, 17);
            this.rbUInt16.TabIndex = 3;
            this.rbUInt16.Text = "uint16";
            this.rbUInt16.UseVisualStyleBackColor = true;
            this.rbUInt16.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // rbByte
            // 
            this.rbByte.AutoSize = true;
            this.rbByte.Checked = true;
            this.rbByte.Location = new System.Drawing.Point(218, 22);
            this.rbByte.Name = "rbByte";
            this.rbByte.Size = new System.Drawing.Size(45, 17);
            this.rbByte.TabIndex = 2;
            this.rbByte.TabStop = true;
            this.rbByte.Text = "byte";
            this.rbByte.UseVisualStyleBackColor = true;
            this.rbByte.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // lRawValue
            // 
            this.lRawValue.AutoSize = true;
            this.lRawValue.Location = new System.Drawing.Point(6, 24);
            this.lRawValue.Name = "lRawValue";
            this.lRawValue.Size = new System.Drawing.Size(59, 13);
            this.lRawValue.TabIndex = 0;
            this.lRawValue.Text = "Raw Value";
            // 
            // eValue
            // 
            this.eValue.Location = new System.Drawing.Point(85, 21);
            this.eValue.Name = "eValue";
            this.eValue.Size = new System.Drawing.Size(119, 20);
            this.eValue.TabIndex = 1;
            this.eValue.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // gbRawValues
            // 
            this.gbRawValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRawValues.Controls.Add(this.rbUInt24);
            this.gbRawValues.Controls.Add(this.rbUInt32);
            this.gbRawValues.Controls.Add(this.lRawValue);
            this.gbRawValues.Controls.Add(this.rbUInt16);
            this.gbRawValues.Controls.Add(this.eValue);
            this.gbRawValues.Controls.Add(this.rbByte);
            this.gbRawValues.Location = new System.Drawing.Point(12, 178);
            this.gbRawValues.Name = "gbRawValues";
            this.gbRawValues.Size = new System.Drawing.Size(468, 62);
            this.gbRawValues.TabIndex = 5;
            this.gbRawValues.TabStop = false;
            this.gbRawValues.Text = "Search for RAW value";
            // 
            // rbUInt24
            // 
            this.rbUInt24.AutoSize = true;
            this.rbUInt24.Location = new System.Drawing.Point(329, 22);
            this.rbUInt24.Name = "rbUInt24";
            this.rbUInt24.Size = new System.Drawing.Size(54, 17);
            this.rbUInt24.TabIndex = 4;
            this.rbUInt24.Text = "uint24";
            this.rbUInt24.UseVisualStyleBackColor = true;
            this.rbUInt24.CheckedChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // gbSearchByField
            // 
            this.gbSearchByField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSearchByField.Controls.Add(this.cbFieldNames);
            this.gbSearchByField.Controls.Add(this.lParsedNote);
            this.gbSearchByField.Controls.Add(this.lFieldValue);
            this.gbSearchByField.Controls.Add(this.lFieldName);
            this.gbSearchByField.Controls.Add(this.eFieldValue);
            this.gbSearchByField.Location = new System.Drawing.Point(12, 246);
            this.gbSearchByField.Name = "gbSearchByField";
            this.gbSearchByField.Size = new System.Drawing.Size(468, 78);
            this.gbSearchByField.TabIndex = 6;
            this.gbSearchByField.TabStop = false;
            this.gbSearchByField.Text = "Search for PARSED value";
            // 
            // cbFieldNames
            // 
            this.cbFieldNames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbFieldNames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbFieldNames.FormattingEnabled = true;
            this.cbFieldNames.Location = new System.Drawing.Point(85, 19);
            this.cbFieldNames.Name = "cbFieldNames";
            this.cbFieldNames.Size = new System.Drawing.Size(119, 21);
            this.cbFieldNames.TabIndex = 1;
            this.cbFieldNames.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // lParsedNote
            // 
            this.lParsedNote.AutoSize = true;
            this.lParsedNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lParsedNote.Location = new System.Drawing.Point(6, 47);
            this.lParsedNote.Name = "lParsedNote";
            this.lParsedNote.Size = new System.Drawing.Size(451, 13);
            this.lParsedNote.TabIndex = 4;
            this.lParsedNote.Text = "Note: parsed values are always treated as strings, you can leave field name blank" +
    " to search all";
            // 
            // lFieldValue
            // 
            this.lFieldValue.AutoSize = true;
            this.lFieldValue.Location = new System.Drawing.Point(210, 24);
            this.lFieldValue.Name = "lFieldValue";
            this.lFieldValue.Size = new System.Drawing.Size(59, 13);
            this.lFieldValue.TabIndex = 2;
            this.lFieldValue.Text = "Field Value";
            // 
            // lFieldName
            // 
            this.lFieldName.AutoSize = true;
            this.lFieldName.Location = new System.Drawing.Point(6, 24);
            this.lFieldName.Name = "lFieldName";
            this.lFieldName.Size = new System.Drawing.Size(60, 13);
            this.lFieldName.TabIndex = 0;
            this.lFieldName.Text = "Field Name";
            // 
            // eFieldValue
            // 
            this.eFieldValue.Location = new System.Drawing.Point(269, 21);
            this.eFieldValue.Name = "eFieldValue";
            this.eFieldValue.Size = new System.Drawing.Size(174, 20);
            this.eFieldValue.TabIndex = 3;
            this.eFieldValue.TextChanged += new System.EventHandler(this.SearchFieldsChanged);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.btnFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(489, 335);
            this.Controls.Add(this.gbSearchByField);
            this.Controls.Add(this.gbRawValues);
            this.Controls.Add(this.gbSearchPacket);
            this.Controls.Add(this.btnAsNewTab);
            this.Controls.Add(this.gbPacketType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFindNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search";
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.Shown += new System.EventHandler(this.SearchForm_Shown);
            this.gbPacketType.ResumeLayout(false);
            this.gbPacketType.PerformLayout();
            this.gbSearchPacket.ResumeLayout(false);
            this.gbSearchPacket.PerformLayout();
            this.gbRawValues.ResumeLayout(false);
            this.gbRawValues.PerformLayout();
            this.gbSearchByField.ResumeLayout(false);
            this.gbSearchByField.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.GroupBox gbPacketType;
        private System.Windows.Forms.RadioButton rbOutgoing;
        private System.Windows.Forms.RadioButton rbIncoming;
        private System.Windows.Forms.RadioButton rbAny;
        public System.Windows.Forms.Button btnAsNewTab;
        private System.Windows.Forms.GroupBox gbSearchPacket;
        private System.Windows.Forms.RadioButton rbUInt32;
        private System.Windows.Forms.RadioButton rbUInt16;
        private System.Windows.Forms.RadioButton rbByte;
        private System.Windows.Forms.Label lRawValue;
        private System.Windows.Forms.Label lPacketSync;
        private System.Windows.Forms.Label lPacketId;
        private System.Windows.Forms.TextBox eValue;
        private System.Windows.Forms.TextBox eSync;
        private System.Windows.Forms.TextBox ePacketID;
        private System.Windows.Forms.GroupBox gbRawValues;
        private System.Windows.Forms.ComboBox cbFieldNames;
        private System.Windows.Forms.Label lParsedNote;
        private System.Windows.Forms.Label lFieldValue;
        private System.Windows.Forms.Label lFieldName;
        private System.Windows.Forms.TextBox eFieldValue;
        public System.Windows.Forms.GroupBox gbSearchByField;
        private System.Windows.Forms.Label lPacketLevel;
        private System.Windows.Forms.TextBox ePacketLevel;
        private System.Windows.Forms.RadioButton rbUInt24;
    }
}