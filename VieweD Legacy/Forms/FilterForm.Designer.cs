namespace VieweD
{
    partial class FilterForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOutAdd = new System.Windows.Forms.Button();
            this.cbOutIDs = new System.Windows.Forms.ComboBox();
            this.lbOut = new System.Windows.Forms.ListBox();
            this.btnRemoveOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbOutNone = new System.Windows.Forms.RadioButton();
            this.rbOutShow = new System.Windows.Forms.RadioButton();
            this.rbOutHide = new System.Windows.Forms.RadioButton();
            this.rbOutOff = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnInAdd = new System.Windows.Forms.Button();
            this.cbInIDs = new System.Windows.Forms.ComboBox();
            this.lbIn = new System.Windows.Forms.ListBox();
            this.btnRemoveIn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rbInNone = new System.Windows.Forms.RadioButton();
            this.rbInShow = new System.Windows.Forms.RadioButton();
            this.rbInHide = new System.Windows.Forms.RadioButton();
            this.rbInOff = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.saveFileDlg = new System.Windows.Forms.SaveFileDialog();
            this.loadFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnHighlight = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.groupBox1.Controls.Add(this.btnOutAdd);
            this.groupBox1.Controls.Add(this.cbOutIDs);
            this.groupBox1.Controls.Add(this.lbOut);
            this.groupBox1.Controls.Add(this.btnRemoveOut);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbOutNone);
            this.groupBox1.Controls.Add(this.rbOutShow);
            this.groupBox1.Controls.Add(this.rbOutHide);
            this.groupBox1.Controls.Add(this.rbOutOff);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 375);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "=> Outgoing Packets";
            // 
            // btnOutAdd
            // 
            this.btnOutAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutAdd.Image = global::VieweD.Properties.Resources.add_icon;
            this.btnOutAdd.Location = new System.Drawing.Point(229, 339);
            this.btnOutAdd.Name = "btnOutAdd";
            this.btnOutAdd.Size = new System.Drawing.Size(31, 30);
            this.btnOutAdd.TabIndex = 8;
            this.btnOutAdd.Text = " ";
            this.btnOutAdd.UseVisualStyleBackColor = true;
            this.btnOutAdd.Click += new System.EventHandler(this.BtnOutAdd_Click);
            // 
            // cbOutIDs
            // 
            this.cbOutIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOutIDs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbOutIDs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOutIDs.DropDownWidth = 350;
            this.cbOutIDs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbOutIDs.FormattingEnabled = true;
            this.cbOutIDs.Location = new System.Drawing.Point(15, 345);
            this.cbOutIDs.Name = "cbOutIDs";
            this.cbOutIDs.Size = new System.Drawing.Size(208, 22);
            this.cbOutIDs.TabIndex = 7;
            this.cbOutIDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbOutIDs_KeyDown);
            // 
            // lbOut
            // 
            this.lbOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbOut.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOut.FormattingEnabled = true;
            this.lbOut.ItemHeight = 14;
            this.lbOut.Location = new System.Drawing.Point(15, 145);
            this.lbOut.Name = "lbOut";
            this.lbOut.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbOut.Size = new System.Drawing.Size(245, 186);
            this.lbOut.TabIndex = 6;
            // 
            // btnRemoveOut
            // 
            this.btnRemoveOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveOut.Image = global::VieweD.Properties.Resources.close_icon;
            this.btnRemoveOut.Location = new System.Drawing.Point(187, 116);
            this.btnRemoveOut.Name = "btnRemoveOut";
            this.btnRemoveOut.Size = new System.Drawing.Size(73, 23);
            this.btnRemoveOut.TabIndex = 5;
            this.btnRemoveOut.Text = "remove";
            this.btnRemoveOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRemoveOut.UseVisualStyleBackColor = true;
            this.btnRemoveOut.Click += new System.EventHandler(this.BtnRemoveOut_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filter ID\'s";
            // 
            // rbOutNone
            // 
            this.rbOutNone.AutoSize = true;
            this.rbOutNone.Location = new System.Drawing.Point(15, 88);
            this.rbOutNone.Name = "rbOutNone";
            this.rbOutNone.Size = new System.Drawing.Size(104, 17);
            this.rbOutNone.TabIndex = 3;
            this.rbOutNone.TabStop = true;
            this.rbOutNone.Text = "Hide all outgoing";
            this.rbOutNone.UseVisualStyleBackColor = true;
            // 
            // rbOutShow
            // 
            this.rbOutShow.AutoSize = true;
            this.rbOutShow.Location = new System.Drawing.Point(15, 65);
            this.rbOutShow.Name = "rbOutShow";
            this.rbOutShow.Size = new System.Drawing.Size(138, 17);
            this.rbOutShow.TabIndex = 2;
            this.rbOutShow.TabStop = true;
            this.rbOutShow.Text = "Show only selected ID\'s";
            this.rbOutShow.UseVisualStyleBackColor = true;
            // 
            // rbOutHide
            // 
            this.rbOutHide.AutoSize = true;
            this.rbOutHide.Location = new System.Drawing.Point(15, 42);
            this.rbOutHide.Name = "rbOutHide";
            this.rbOutHide.Size = new System.Drawing.Size(111, 17);
            this.rbOutHide.TabIndex = 1;
            this.rbOutHide.TabStop = true;
            this.rbOutHide.Text = "Hide selected ID\'s";
            this.rbOutHide.UseVisualStyleBackColor = true;
            // 
            // rbOutOff
            // 
            this.rbOutOff.AutoSize = true;
            this.rbOutOff.Location = new System.Drawing.Point(15, 19);
            this.rbOutOff.Name = "rbOutOff";
            this.rbOutOff.Size = new System.Drawing.Size(72, 17);
            this.rbOutOff.TabIndex = 0;
            this.rbOutOff.TabStop = true;
            this.rbOutOff.Text = "Don\'t filter";
            this.rbOutOff.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Honeydew;
            this.groupBox2.Controls.Add(this.btnInAdd);
            this.groupBox2.Controls.Add(this.cbInIDs);
            this.groupBox2.Controls.Add(this.lbIn);
            this.groupBox2.Controls.Add(this.btnRemoveIn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rbInNone);
            this.groupBox2.Controls.Add(this.rbInShow);
            this.groupBox2.Controls.Add(this.rbInHide);
            this.groupBox2.Controls.Add(this.rbInOff);
            this.groupBox2.Location = new System.Drawing.Point(291, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 375);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "<= Incoming Packets";
            // 
            // btnInAdd
            // 
            this.btnInAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInAdd.Image = global::VieweD.Properties.Resources.add_icon;
            this.btnInAdd.Location = new System.Drawing.Point(229, 339);
            this.btnInAdd.Name = "btnInAdd";
            this.btnInAdd.Size = new System.Drawing.Size(31, 30);
            this.btnInAdd.TabIndex = 8;
            this.btnInAdd.Text = " ";
            this.btnInAdd.UseVisualStyleBackColor = true;
            this.btnInAdd.Click += new System.EventHandler(this.BtnInAdd_Click);
            // 
            // cbInIDs
            // 
            this.cbInIDs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbInIDs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbInIDs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbInIDs.DropDownWidth = 350;
            this.cbInIDs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbInIDs.FormattingEnabled = true;
            this.cbInIDs.Location = new System.Drawing.Point(15, 345);
            this.cbInIDs.Name = "cbInIDs";
            this.cbInIDs.Size = new System.Drawing.Size(208, 22);
            this.cbInIDs.TabIndex = 7;
            this.cbInIDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbInIDs_KeyDown);
            // 
            // lbIn
            // 
            this.lbIn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbIn.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIn.FormattingEnabled = true;
            this.lbIn.ItemHeight = 14;
            this.lbIn.Location = new System.Drawing.Point(15, 145);
            this.lbIn.Name = "lbIn";
            this.lbIn.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbIn.Size = new System.Drawing.Size(245, 186);
            this.lbIn.TabIndex = 6;
            // 
            // btnRemoveIn
            // 
            this.btnRemoveIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveIn.Image = global::VieweD.Properties.Resources.close_icon;
            this.btnRemoveIn.Location = new System.Drawing.Point(187, 116);
            this.btnRemoveIn.Name = "btnRemoveIn";
            this.btnRemoveIn.Size = new System.Drawing.Size(73, 23);
            this.btnRemoveIn.TabIndex = 5;
            this.btnRemoveIn.Text = "remove";
            this.btnRemoveIn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRemoveIn.UseVisualStyleBackColor = true;
            this.btnRemoveIn.Click += new System.EventHandler(this.BtnRemoveIn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Filter ID\'s";
            // 
            // rbInNone
            // 
            this.rbInNone.AutoSize = true;
            this.rbInNone.Location = new System.Drawing.Point(15, 88);
            this.rbInNone.Name = "rbInNone";
            this.rbInNone.Size = new System.Drawing.Size(105, 17);
            this.rbInNone.TabIndex = 3;
            this.rbInNone.TabStop = true;
            this.rbInNone.Text = "Hide all incoming";
            this.rbInNone.UseVisualStyleBackColor = true;
            // 
            // rbInShow
            // 
            this.rbInShow.AutoSize = true;
            this.rbInShow.Location = new System.Drawing.Point(15, 65);
            this.rbInShow.Name = "rbInShow";
            this.rbInShow.Size = new System.Drawing.Size(138, 17);
            this.rbInShow.TabIndex = 2;
            this.rbInShow.TabStop = true;
            this.rbInShow.Text = "Show only selected ID\'s";
            this.rbInShow.UseVisualStyleBackColor = true;
            // 
            // rbInHide
            // 
            this.rbInHide.AutoSize = true;
            this.rbInHide.Location = new System.Drawing.Point(15, 42);
            this.rbInHide.Name = "rbInHide";
            this.rbInHide.Size = new System.Drawing.Size(111, 17);
            this.rbInHide.TabIndex = 1;
            this.rbInHide.TabStop = true;
            this.rbInHide.Text = "Hide selected ID\'s";
            this.rbInHide.UseVisualStyleBackColor = true;
            // 
            // rbInOff
            // 
            this.rbInOff.AutoSize = true;
            this.rbInOff.Location = new System.Drawing.Point(15, 19);
            this.rbInOff.Name = "rbInOff";
            this.rbInOff.Size = new System.Drawing.Size(72, 17);
            this.rbInOff.TabIndex = 0;
            this.rbInOff.TabStop = true;
            this.rbInOff.Text = "Don\'t filter";
            this.rbInOff.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(575, 358);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(116, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Apply";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(575, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(575, 254);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(575, 225);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(116, 23);
            this.btnLoad.TabIndex = 13;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // saveFileDlg
            // 
            this.saveFileDlg.DefaultExt = "pfl";
            this.saveFileDlg.Filter = "Filter Files|*.pfl|All Files|*.*";
            this.saveFileDlg.RestoreDirectory = true;
            this.saveFileDlg.Title = "Save Filter";
            // 
            // loadFileDlg
            // 
            this.loadFileDlg.DefaultExt = "pfl";
            this.loadFileDlg.Filter = "Filter Files|*.pfl|All Files|*.*";
            this.loadFileDlg.RestoreDirectory = true;
            this.loadFileDlg.Title = "Load Filter";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(575, 185);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(116, 23);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnHighlight
            // 
            this.btnHighlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHighlight.Location = new System.Drawing.Point(575, 329);
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.Size = new System.Drawing.Size(116, 23);
            this.btnHighlight.TabIndex = 15;
            this.btnHighlight.Text = "Highlight Only";
            this.btnHighlight.UseVisualStyleBackColor = true;
            this.btnHighlight.Click += new System.EventHandler(this.btnHighlight_Click);
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(706, 397);
            this.Controls.Add(this.btnHighlight);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filters";
            this.Load += new System.EventHandler(this.FilterForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOutNone;
        private System.Windows.Forms.RadioButton rbOutShow;
        private System.Windows.Forms.RadioButton rbOutHide;
        private System.Windows.Forms.RadioButton rbOutOff;
        private System.Windows.Forms.Button btnOutAdd;
        private System.Windows.Forms.ComboBox cbOutIDs;
        private System.Windows.Forms.ListBox lbOut;
        private System.Windows.Forms.Button btnRemoveOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnInAdd;
        private System.Windows.Forms.ComboBox cbInIDs;
        private System.Windows.Forms.ListBox lbIn;
        private System.Windows.Forms.Button btnRemoveIn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbInNone;
        private System.Windows.Forms.RadioButton rbInShow;
        private System.Windows.Forms.RadioButton rbInHide;
        private System.Windows.Forms.RadioButton rbInOff;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.SaveFileDialog saveFileDlg;
        private System.Windows.Forms.OpenFileDialog loadFileDlg;
        private System.Windows.Forms.Button btnClear;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.Button btnHighlight;
    }
}