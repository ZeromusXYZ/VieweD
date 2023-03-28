using System.Windows.Forms;

namespace VieweD.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOutAdd = new System.Windows.Forms.Button();
            this.cbOutIDs = new System.Windows.Forms.ComboBox();
            this.LbOut = new System.Windows.Forms.ListBox();
            this.btnRemoveOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbOutNone = new System.Windows.Forms.RadioButton();
            this.rbOutShow = new System.Windows.Forms.RadioButton();
            this.rbOutHide = new System.Windows.Forms.RadioButton();
            this.rbOutOff = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnInAdd = new System.Windows.Forms.Button();
            this.cbInIDs = new System.Windows.Forms.ComboBox();
            this.LbIn = new System.Windows.Forms.ListBox();
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.btnOutAdd);
            this.groupBox1.Controls.Add(this.cbOutIDs);
            this.groupBox1.Controls.Add(this.LbOut);
            this.groupBox1.Controls.Add(this.btnRemoveOut);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbOutNone);
            this.groupBox1.Controls.Add(this.rbOutShow);
            this.groupBox1.Controls.Add(this.rbOutHide);
            this.groupBox1.Controls.Add(this.rbOutOff);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnOutAdd
            // 
            resources.ApplyResources(this.btnOutAdd, "btnOutAdd");
            this.btnOutAdd.Image = global::VieweD.Properties.Resources.add;
            this.btnOutAdd.Name = "btnOutAdd";
            this.btnOutAdd.UseVisualStyleBackColor = true;
            this.btnOutAdd.Click += new System.EventHandler(this.BtnOutAdd_Click);
            // 
            // cbOutIDs
            // 
            resources.ApplyResources(this.cbOutIDs, "cbOutIDs");
            this.cbOutIDs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbOutIDs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbOutIDs.DropDownWidth = 350;
            this.cbOutIDs.FormattingEnabled = true;
            this.cbOutIDs.Name = "cbOutIDs";
            this.cbOutIDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CbOutIDs_KeyDown);
            // 
            // LbOut
            // 
            resources.ApplyResources(this.LbOut, "LbOut");
            this.LbOut.FormattingEnabled = true;
            this.LbOut.Name = "LbOut";
            this.LbOut.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // btnRemoveOut
            // 
            resources.ApplyResources(this.btnRemoveOut, "btnRemoveOut");
            this.btnRemoveOut.Image = global::VieweD.Properties.Resources.close;
            this.btnRemoveOut.Name = "btnRemoveOut";
            this.btnRemoveOut.UseVisualStyleBackColor = true;
            this.btnRemoveOut.Click += new System.EventHandler(this.BtnRemoveOut_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // rbOutNone
            // 
            resources.ApplyResources(this.rbOutNone, "rbOutNone");
            this.rbOutNone.Name = "rbOutNone";
            this.rbOutNone.TabStop = true;
            this.rbOutNone.UseVisualStyleBackColor = true;
            // 
            // rbOutShow
            // 
            resources.ApplyResources(this.rbOutShow, "rbOutShow");
            this.rbOutShow.Name = "rbOutShow";
            this.rbOutShow.TabStop = true;
            this.rbOutShow.UseVisualStyleBackColor = true;
            // 
            // rbOutHide
            // 
            resources.ApplyResources(this.rbOutHide, "rbOutHide");
            this.rbOutHide.Name = "rbOutHide";
            this.rbOutHide.TabStop = true;
            this.rbOutHide.UseVisualStyleBackColor = true;
            // 
            // rbOutOff
            // 
            resources.ApplyResources(this.rbOutOff, "rbOutOff");
            this.rbOutOff.Name = "rbOutOff";
            this.rbOutOff.TabStop = true;
            this.rbOutOff.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Honeydew;
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.btnInAdd);
            this.groupBox2.Controls.Add(this.cbInIDs);
            this.groupBox2.Controls.Add(this.LbIn);
            this.groupBox2.Controls.Add(this.btnRemoveIn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rbInNone);
            this.groupBox2.Controls.Add(this.rbInShow);
            this.groupBox2.Controls.Add(this.rbInHide);
            this.groupBox2.Controls.Add(this.rbInOff);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnInAdd
            // 
            resources.ApplyResources(this.btnInAdd, "btnInAdd");
            this.btnInAdd.Image = global::VieweD.Properties.Resources.add;
            this.btnInAdd.Name = "btnInAdd";
            this.btnInAdd.UseVisualStyleBackColor = true;
            this.btnInAdd.Click += new System.EventHandler(this.BtnInAdd_Click);
            // 
            // cbInIDs
            // 
            resources.ApplyResources(this.cbInIDs, "cbInIDs");
            this.cbInIDs.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cbInIDs.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbInIDs.DropDownWidth = 350;
            this.cbInIDs.FormattingEnabled = true;
            this.cbInIDs.Name = "cbInIDs";
            this.cbInIDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CbInIDs_KeyDown);
            // 
            // LbIn
            // 
            resources.ApplyResources(this.LbIn, "LbIn");
            this.LbIn.FormattingEnabled = true;
            this.LbIn.Name = "LbIn";
            this.LbIn.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // btnRemoveIn
            // 
            resources.ApplyResources(this.btnRemoveIn, "btnRemoveIn");
            this.btnRemoveIn.Image = global::VieweD.Properties.Resources.close;
            this.btnRemoveIn.Name = "btnRemoveIn";
            this.btnRemoveIn.UseVisualStyleBackColor = true;
            this.btnRemoveIn.Click += new System.EventHandler(this.BtnRemoveIn_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // rbInNone
            // 
            resources.ApplyResources(this.rbInNone, "rbInNone");
            this.rbInNone.Name = "rbInNone";
            this.rbInNone.TabStop = true;
            this.rbInNone.UseVisualStyleBackColor = true;
            // 
            // rbInShow
            // 
            resources.ApplyResources(this.rbInShow, "rbInShow");
            this.rbInShow.Name = "rbInShow";
            this.rbInShow.TabStop = true;
            this.rbInShow.UseVisualStyleBackColor = true;
            // 
            // rbInHide
            // 
            resources.ApplyResources(this.rbInHide, "rbInHide");
            this.rbInHide.Name = "rbInHide";
            this.rbInHide.TabStop = true;
            this.rbInHide.UseVisualStyleBackColor = true;
            // 
            // rbInOff
            // 
            resources.ApplyResources(this.rbInOff, "rbInOff");
            this.rbInOff.Name = "rbInOff";
            this.rbInOff.TabStop = true;
            this.rbInOff.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnLoad
            // 
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // saveFileDlg
            // 
            this.saveFileDlg.DefaultExt = "pfl";
            resources.ApplyResources(this.saveFileDlg, "saveFileDlg");
            this.saveFileDlg.RestoreDirectory = true;
            // 
            // loadFileDlg
            // 
            this.loadFileDlg.DefaultExt = "pfl";
            resources.ApplyResources(this.loadFileDlg, "loadFileDlg");
            this.loadFileDlg.RestoreDirectory = true;
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnHighlight
            // 
            resources.ApplyResources(this.btnHighlight, "btnHighlight");
            this.btnHighlight.Name = "btnHighlight";
            this.btnHighlight.UseVisualStyleBackColor = true;
            this.btnHighlight.Click += new System.EventHandler(this.BtnHighlight_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VieweD.Properties.Resources.mini_out_ticon;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::VieweD.Properties.Resources.mini_in_ticon;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // FilterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
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
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FilterForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
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
        private System.Windows.Forms.ListBox LbOut;
        private System.Windows.Forms.Button btnRemoveOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnInAdd;
        private System.Windows.Forms.ComboBox cbInIDs;
        private System.Windows.Forms.ListBox LbIn;
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
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}