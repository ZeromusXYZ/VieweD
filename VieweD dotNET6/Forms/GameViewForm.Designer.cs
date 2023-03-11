namespace VieweD
{
    partial class GameViewForm
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
            groupBox1 = new GroupBox();
            label1 = new Label();
            eTextFilter = new TextBox();
            btnCopyID = new Button();
            btnCopyVal = new Button();
            cbHexIndex = new CheckBox();
            btnRefreshLookups = new Button();
            lbLookupValues = new ListBox();
            lbLookupGroups = new ListBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(eTextFilter);
            groupBox1.Controls.Add(btnCopyID);
            groupBox1.Controls.Add(btnCopyVal);
            groupBox1.Controls.Add(cbHexIndex);
            groupBox1.Controls.Add(btnRefreshLookups);
            groupBox1.Controls.Add(lbLookupValues);
            groupBox1.Controls.Add(lbLookupGroups);
            groupBox1.Location = new Point(15, 12);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(565, 337);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Saved Values";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(154, 55);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 7;
            label1.Text = "Filter:";
            // 
            // eTextFilter
            // 
            eTextFilter.Location = new Point(198, 52);
            eTextFilter.Margin = new Padding(4, 3, 4, 3);
            eTextFilter.Name = "eTextFilter";
            eTextFilter.Size = new Size(359, 23);
            eTextFilter.TabIndex = 6;
            eTextFilter.TextChanged += LbLookupGroups_SelectedIndexChanged;
            // 
            // btnCopyID
            // 
            btnCopyID.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyID.Location = new Point(329, 23);
            btnCopyID.Margin = new Padding(4, 3, 4, 3);
            btnCopyID.Name = "btnCopyID";
            btnCopyID.Size = new Size(111, 27);
            btnCopyID.TabIndex = 5;
            btnCopyID.Text = "Copy ID";
            btnCopyID.UseVisualStyleBackColor = true;
            btnCopyID.Click += BtnCopyID_Click;
            // 
            // btnCopyVal
            // 
            btnCopyVal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyVal.Location = new Point(447, 23);
            btnCopyVal.Margin = new Padding(4, 3, 4, 3);
            btnCopyVal.Name = "btnCopyVal";
            btnCopyVal.Size = new Size(111, 27);
            btnCopyVal.TabIndex = 4;
            btnCopyVal.Text = "Copy Value";
            btnCopyVal.UseVisualStyleBackColor = true;
            btnCopyVal.Click += BtnCopyVal_Click;
            // 
            // cbHexIndex
            // 
            cbHexIndex.AutoSize = true;
            cbHexIndex.Checked = true;
            cbHexIndex.CheckState = CheckState.Checked;
            cbHexIndex.Location = new Point(158, 27);
            cbHexIndex.Margin = new Padding(4, 3, 4, 3);
            cbHexIndex.Name = "cbHexIndex";
            cbHexIndex.Size = new Size(125, 19);
            cbHexIndex.TabIndex = 3;
            cbHexIndex.Text = "Show Index as Hex";
            cbHexIndex.UseVisualStyleBackColor = true;
            // 
            // btnRefreshLookups
            // 
            btnRefreshLookups.Location = new Point(10, 23);
            btnRefreshLookups.Margin = new Padding(4, 3, 4, 3);
            btnRefreshLookups.Name = "btnRefreshLookups";
            btnRefreshLookups.Size = new Size(140, 27);
            btnRefreshLookups.TabIndex = 2;
            btnRefreshLookups.Text = "Refresh Lookups";
            btnRefreshLookups.UseVisualStyleBackColor = true;
            btnRefreshLookups.Click += BtnRefreshLookups_Click;
            // 
            // lbLookupValues
            // 
            lbLookupValues.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbLookupValues.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lbLookupValues.FormattingEnabled = true;
            lbLookupValues.ItemHeight = 14;
            lbLookupValues.Location = new Point(158, 83);
            lbLookupValues.Margin = new Padding(4, 3, 4, 3);
            lbLookupValues.Name = "lbLookupValues";
            lbLookupValues.Size = new Size(400, 242);
            lbLookupValues.TabIndex = 1;
            // 
            // lbLookupGroups
            // 
            lbLookupGroups.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lbLookupGroups.FormattingEnabled = true;
            lbLookupGroups.ItemHeight = 15;
            lbLookupGroups.Location = new Point(10, 52);
            lbLookupGroups.Margin = new Padding(4, 3, 4, 3);
            lbLookupGroups.Name = "lbLookupGroups";
            lbLookupGroups.Size = new Size(139, 274);
            lbLookupGroups.TabIndex = 0;
            lbLookupGroups.SelectedIndexChanged += LbLookupGroups_SelectedIndexChanged;
            // 
            // GameViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(593, 361);
            Controls.Add(groupBox1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "GameViewForm";
            Text = "Game View";
            FormClosed += GameViewForm_FormClosed;
            Load += GameViewForm_Load;
            Shown += GameViewForm_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbLookupValues;
        private System.Windows.Forms.ListBox lbLookupGroups;
        private System.Windows.Forms.CheckBox cbHexIndex;
        private System.Windows.Forms.Button btnCopyID;
        private System.Windows.Forms.Button btnCopyVal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox eTextFilter;
        public System.Windows.Forms.Button btnRefreshLookups;
    }
}