using System.Windows.Forms;
using System.Drawing;
using VieweD.Properties;

namespace VieweD.Forms
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
            btnFindNext = new Button();
            btnCancel = new Button();
            gbPacketType = new GroupBox();
            rbOutgoing = new RadioButton();
            rbIncoming = new RadioButton();
            rbAny = new RadioButton();
            btnAsNewTab = new Button();
            gbSearchPacket = new GroupBox();
            lPacketLevel = new Label();
            ePacketLevel = new TextBox();
            lPacketSync = new Label();
            lPacketId = new Label();
            eSync = new TextBox();
            ePacketID = new TextBox();
            rbUInt32 = new RadioButton();
            rbUInt16 = new RadioButton();
            rbByte = new RadioButton();
            lRawValue = new Label();
            eValue = new TextBox();
            gbRawValues = new GroupBox();
            rbUInt24 = new RadioButton();
            gbSearchByField = new GroupBox();
            cbFieldNames = new ComboBox();
            lParsedNote = new Label();
            lFieldValue = new Label();
            lFieldName = new Label();
            eFieldValue = new TextBox();
            BtnClearSearch = new Button();
            gbPacketType.SuspendLayout();
            gbSearchPacket.SuspendLayout();
            gbRawValues.SuspendLayout();
            gbSearchByField.SuspendLayout();
            SuspendLayout();
            // 
            // btnFindNext
            // 
            btnFindNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFindNext.Enabled = false;
            btnFindNext.Location = new Point(472, 14);
            btnFindNext.Margin = new Padding(4, 3, 4, 3);
            btnFindNext.Name = "btnFindNext";
            btnFindNext.Size = new Size(88, 27);
            btnFindNext.TabIndex = 0;
            btnFindNext.Text = "Find Next";
            btnFindNext.UseVisualStyleBackColor = true;
            btnFindNext.Click += BtnFindNext_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(472, 47);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // gbPacketType
            // 
            gbPacketType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbPacketType.Controls.Add(rbOutgoing);
            gbPacketType.Controls.Add(rbIncoming);
            gbPacketType.Controls.Add(rbAny);
            gbPacketType.Location = new Point(14, 14);
            gbPacketType.Margin = new Padding(4, 3, 4, 3);
            gbPacketType.Name = "gbPacketType";
            gbPacketType.Padding = new Padding(4, 3, 4, 3);
            gbPacketType.Size = new Size(438, 60);
            gbPacketType.TabIndex = 3;
            gbPacketType.TabStop = false;
            gbPacketType.Text = "Packet Types to search in";
            // 
            // rbOutgoing
            // 
            rbOutgoing.AutoSize = true;
            rbOutgoing.Location = new Point(245, 22);
            rbOutgoing.Margin = new Padding(4, 3, 4, 3);
            rbOutgoing.Name = "rbOutgoing";
            rbOutgoing.Size = new Size(76, 19);
            rbOutgoing.TabIndex = 2;
            rbOutgoing.Text = "Outgoing";
            rbOutgoing.UseVisualStyleBackColor = true;
            rbOutgoing.CheckedChanged += SearchFieldsChanged;
            // 
            // rbIncoming
            // 
            rbIncoming.AutoSize = true;
            rbIncoming.Location = new Point(114, 22);
            rbIncoming.Margin = new Padding(4, 3, 4, 3);
            rbIncoming.Name = "rbIncoming";
            rbIncoming.Size = new Size(76, 19);
            rbIncoming.TabIndex = 1;
            rbIncoming.Text = "Incoming";
            rbIncoming.UseVisualStyleBackColor = true;
            rbIncoming.CheckedChanged += SearchFieldsChanged;
            // 
            // rbAny
            // 
            rbAny.AutoSize = true;
            rbAny.Checked = true;
            rbAny.Location = new Point(7, 22);
            rbAny.Margin = new Padding(4, 3, 4, 3);
            rbAny.Name = "rbAny";
            rbAny.Size = new Size(46, 19);
            rbAny.TabIndex = 0;
            rbAny.TabStop = true;
            rbAny.Text = "Any";
            rbAny.UseVisualStyleBackColor = true;
            rbAny.CheckedChanged += SearchFieldsChanged;
            // 
            // btnAsNewTab
            // 
            btnAsNewTab.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAsNewTab.Enabled = false;
            btnAsNewTab.Location = new Point(472, 103);
            btnAsNewTab.Margin = new Padding(4, 3, 4, 3);
            btnAsNewTab.Name = "btnAsNewTab";
            btnAsNewTab.Size = new Size(88, 27);
            btnAsNewTab.TabIndex = 2;
            btnAsNewTab.Text = "As New Tab";
            btnAsNewTab.UseVisualStyleBackColor = true;
            btnAsNewTab.Click += BtnAsNewTab_Click;
            // 
            // gbSearchPacket
            // 
            gbSearchPacket.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            gbSearchPacket.Controls.Add(lPacketLevel);
            gbSearchPacket.Controls.Add(ePacketLevel);
            gbSearchPacket.Controls.Add(lPacketSync);
            gbSearchPacket.Controls.Add(lPacketId);
            gbSearchPacket.Controls.Add(eSync);
            gbSearchPacket.Controls.Add(ePacketID);
            gbSearchPacket.Location = new Point(14, 81);
            gbSearchPacket.Margin = new Padding(4, 3, 4, 3);
            gbSearchPacket.Name = "gbSearchPacket";
            gbSearchPacket.Padding = new Padding(4, 3, 4, 3);
            gbSearchPacket.Size = new Size(254, 118);
            gbSearchPacket.TabIndex = 4;
            gbSearchPacket.TabStop = false;
            gbSearchPacket.Text = "Search for packet";
            // 
            // lPacketLevel
            // 
            lPacketLevel.AutoSize = true;
            lPacketLevel.Location = new Point(7, 52);
            lPacketLevel.Margin = new Padding(4, 0, 4, 0);
            lPacketLevel.Name = "lPacketLevel";
            lPacketLevel.Size = new Size(145, 15);
            lPacketLevel.TabIndex = 2;
            lPacketLevel.Text = "Packet Compression Level";
            // 
            // ePacketLevel
            // 
            ePacketLevel.Location = new Point(159, 48);
            ePacketLevel.Margin = new Padding(4, 3, 4, 3);
            ePacketLevel.Name = "ePacketLevel";
            ePacketLevel.Size = new Size(79, 23);
            ePacketLevel.TabIndex = 3;
            ePacketLevel.TextChanged += SearchFieldsChanged;
            // 
            // lPacketSync
            // 
            lPacketSync.AutoSize = true;
            lPacketSync.Location = new Point(7, 82);
            lPacketSync.Margin = new Padding(4, 0, 4, 0);
            lPacketSync.Name = "lPacketSync";
            lPacketSync.Size = new Size(79, 15);
            lPacketSync.TabIndex = 4;
            lPacketSync.Text = "Sync Number";
            // 
            // lPacketId
            // 
            lPacketId.AutoSize = true;
            lPacketId.Location = new Point(7, 22);
            lPacketId.Margin = new Padding(4, 0, 4, 0);
            lPacketId.Name = "lPacketId";
            lPacketId.Size = new Size(91, 15);
            lPacketId.TabIndex = 0;
            lPacketId.Text = "Packet ID (Type)";
            // 
            // eSync
            // 
            eSync.Location = new Point(159, 78);
            eSync.Margin = new Padding(4, 3, 4, 3);
            eSync.Name = "eSync";
            eSync.Size = new Size(79, 23);
            eSync.TabIndex = 5;
            eSync.TextChanged += SearchFieldsChanged;
            // 
            // ePacketID
            // 
            ePacketID.Location = new Point(159, 18);
            ePacketID.Margin = new Padding(4, 3, 4, 3);
            ePacketID.Name = "ePacketID";
            ePacketID.Size = new Size(79, 23);
            ePacketID.TabIndex = 1;
            ePacketID.TextChanged += SearchFieldsChanged;
            // 
            // rbUInt32
            // 
            rbUInt32.AutoSize = true;
            rbUInt32.Location = new Point(454, 25);
            rbUInt32.Margin = new Padding(4, 3, 4, 3);
            rbUInt32.Name = "rbUInt32";
            rbUInt32.Size = new Size(58, 19);
            rbUInt32.TabIndex = 5;
            rbUInt32.Text = "uint32";
            rbUInt32.UseVisualStyleBackColor = true;
            rbUInt32.CheckedChanged += SearchFieldsChanged;
            // 
            // rbUInt16
            // 
            rbUInt16.AutoSize = true;
            rbUInt16.Location = new Point(314, 25);
            rbUInt16.Margin = new Padding(4, 3, 4, 3);
            rbUInt16.Name = "rbUInt16";
            rbUInt16.Size = new Size(58, 19);
            rbUInt16.TabIndex = 3;
            rbUInt16.Text = "uint16";
            rbUInt16.UseVisualStyleBackColor = true;
            rbUInt16.CheckedChanged += SearchFieldsChanged;
            // 
            // rbByte
            // 
            rbByte.AutoSize = true;
            rbByte.Checked = true;
            rbByte.Location = new Point(254, 25);
            rbByte.Margin = new Padding(4, 3, 4, 3);
            rbByte.Name = "rbByte";
            rbByte.Size = new Size(48, 19);
            rbByte.TabIndex = 2;
            rbByte.TabStop = true;
            rbByte.Text = "byte";
            rbByte.UseVisualStyleBackColor = true;
            rbByte.CheckedChanged += SearchFieldsChanged;
            // 
            // lRawValue
            // 
            lRawValue.AutoSize = true;
            lRawValue.Location = new Point(7, 28);
            lRawValue.Margin = new Padding(4, 0, 4, 0);
            lRawValue.Name = "lRawValue";
            lRawValue.Size = new Size(60, 15);
            lRawValue.TabIndex = 0;
            lRawValue.Text = "Raw Value";
            // 
            // eValue
            // 
            eValue.Location = new Point(99, 24);
            eValue.Margin = new Padding(4, 3, 4, 3);
            eValue.Name = "eValue";
            eValue.Size = new Size(138, 23);
            eValue.TabIndex = 1;
            eValue.TextChanged += SearchFieldsChanged;
            // 
            // gbRawValues
            // 
            gbRawValues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbRawValues.Controls.Add(rbUInt24);
            gbRawValues.Controls.Add(rbUInt32);
            gbRawValues.Controls.Add(lRawValue);
            gbRawValues.Controls.Add(rbUInt16);
            gbRawValues.Controls.Add(eValue);
            gbRawValues.Controls.Add(rbByte);
            gbRawValues.Location = new Point(14, 205);
            gbRawValues.Margin = new Padding(4, 3, 4, 3);
            gbRawValues.Name = "gbRawValues";
            gbRawValues.Padding = new Padding(4, 3, 4, 3);
            gbRawValues.Size = new Size(546, 72);
            gbRawValues.TabIndex = 5;
            gbRawValues.TabStop = false;
            gbRawValues.Text = "Search for RAW value";
            // 
            // rbUInt24
            // 
            rbUInt24.AutoSize = true;
            rbUInt24.Location = new Point(384, 25);
            rbUInt24.Margin = new Padding(4, 3, 4, 3);
            rbUInt24.Name = "rbUInt24";
            rbUInt24.Size = new Size(58, 19);
            rbUInt24.TabIndex = 4;
            rbUInt24.Text = "uint24";
            rbUInt24.UseVisualStyleBackColor = true;
            rbUInt24.CheckedChanged += SearchFieldsChanged;
            // 
            // gbSearchByField
            // 
            gbSearchByField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSearchByField.Controls.Add(cbFieldNames);
            gbSearchByField.Controls.Add(lParsedNote);
            gbSearchByField.Controls.Add(lFieldValue);
            gbSearchByField.Controls.Add(lFieldName);
            gbSearchByField.Controls.Add(eFieldValue);
            gbSearchByField.Location = new Point(14, 284);
            gbSearchByField.Margin = new Padding(4, 3, 4, 3);
            gbSearchByField.Name = "gbSearchByField";
            gbSearchByField.Padding = new Padding(4, 3, 4, 3);
            gbSearchByField.Size = new Size(546, 90);
            gbSearchByField.TabIndex = 6;
            gbSearchByField.TabStop = false;
            gbSearchByField.Text = "Search for PARSED value";
            // 
            // cbFieldNames
            // 
            cbFieldNames.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbFieldNames.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbFieldNames.FormattingEnabled = true;
            cbFieldNames.Location = new Point(99, 22);
            cbFieldNames.Margin = new Padding(4, 3, 4, 3);
            cbFieldNames.Name = "cbFieldNames";
            cbFieldNames.Size = new Size(138, 23);
            cbFieldNames.TabIndex = 1;
            cbFieldNames.TextChanged += SearchFieldsChanged;
            // 
            // lParsedNote
            // 
            lParsedNote.AutoSize = true;
            lParsedNote.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lParsedNote.Location = new Point(7, 54);
            lParsedNote.Margin = new Padding(4, 0, 4, 0);
            lParsedNote.Name = "lParsedNote";
            lParsedNote.Size = new Size(451, 13);
            lParsedNote.TabIndex = 4;
            lParsedNote.Text = "Note: parsed values are always treated as strings, you can leave field name blank to search all";
            // 
            // lFieldValue
            // 
            lFieldValue.AutoSize = true;
            lFieldValue.Location = new Point(245, 28);
            lFieldValue.Margin = new Padding(4, 0, 4, 0);
            lFieldValue.Name = "lFieldValue";
            lFieldValue.Size = new Size(63, 15);
            lFieldValue.TabIndex = 2;
            lFieldValue.Text = "Field Value";
            // 
            // lFieldName
            // 
            lFieldName.AutoSize = true;
            lFieldName.Location = new Point(7, 28);
            lFieldName.Margin = new Padding(4, 0, 4, 0);
            lFieldName.Name = "lFieldName";
            lFieldName.Size = new Size(67, 15);
            lFieldName.TabIndex = 0;
            lFieldName.Text = "Field Name";
            // 
            // eFieldValue
            // 
            eFieldValue.Location = new Point(314, 24);
            eFieldValue.Margin = new Padding(4, 3, 4, 3);
            eFieldValue.Name = "eFieldValue";
            eFieldValue.Size = new Size(202, 23);
            eFieldValue.TabIndex = 3;
            eFieldValue.TextChanged += SearchFieldsChanged;
            // 
            // BtnClearSearch
            // 
            BtnClearSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnClearSearch.Location = new Point(472, 157);
            BtnClearSearch.Margin = new Padding(4, 3, 4, 3);
            BtnClearSearch.Name = "BtnClearSearch";
            BtnClearSearch.Size = new Size(88, 27);
            BtnClearSearch.TabIndex = 7;
            BtnClearSearch.Text = "Clear";
            BtnClearSearch.UseVisualStyleBackColor = true;
            BtnClearSearch.Click += BtnClearSearch_Click;
            // 
            // SearchForm
            // 
            AcceptButton = btnFindNext;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(570, 387);
            Controls.Add(BtnClearSearch);
            Controls.Add(gbSearchByField);
            Controls.Add(gbRawValues);
            Controls.Add(gbSearchPacket);
            Controls.Add(btnAsNewTab);
            Controls.Add(gbPacketType);
            Controls.Add(btnCancel);
            Controls.Add(btnFindNext);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SearchForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Search";
            Load += SearchForm_Load;
            Shown += SearchForm_Shown;
            gbPacketType.ResumeLayout(false);
            gbPacketType.PerformLayout();
            gbSearchPacket.ResumeLayout(false);
            gbSearchPacket.PerformLayout();
            gbRawValues.ResumeLayout(false);
            gbRawValues.PerformLayout();
            gbSearchByField.ResumeLayout(false);
            gbSearchByField.PerformLayout();
            ResumeLayout(false);
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
        private Button BtnClearSearch;
    }
}