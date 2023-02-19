namespace VieweD.Forms
{
    partial class InputReaderDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputReaderDialog));
            this.ListBoxInputReaders = new System.Windows.Forms.ListBox();
            this.BtnSelect = new System.Windows.Forms.Button();
            this.TextBoxDescription = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ListBoxInputReaders
            // 
            resources.ApplyResources(this.ListBoxInputReaders, "ListBoxInputReaders");
            this.ListBoxInputReaders.FormattingEnabled = true;
            this.ListBoxInputReaders.Name = "ListBoxInputReaders";
            this.ListBoxInputReaders.SelectedIndexChanged += new System.EventHandler(this.ListBoxInputReaders_SelectedIndexChanged);
            // 
            // BtnSelect
            // 
            resources.ApplyResources(this.BtnSelect, "BtnSelect");
            this.BtnSelect.Name = "BtnSelect";
            this.BtnSelect.UseVisualStyleBackColor = true;
            this.BtnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // TextBoxDescription
            // 
            resources.ApplyResources(this.TextBoxDescription, "TextBoxDescription");
            this.TextBoxDescription.Name = "TextBoxDescription";
            this.TextBoxDescription.ReadOnly = true;
            // 
            // InputReaderDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextBoxDescription);
            this.Controls.Add(this.BtnSelect);
            this.Controls.Add(this.ListBoxInputReaders);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "InputReaderDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.InputReaderDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox ListBoxInputReaders;
        private Button BtnSelect;
        private TextBox TextBoxDescription;
    }
}