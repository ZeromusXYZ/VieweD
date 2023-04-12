using System.Windows.Forms;

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
            ListBoxInputReaders = new ListBox();
            BtnSelect = new Button();
            TextBoxDescription = new TextBox();
            SuspendLayout();
            // 
            // ListBoxInputReaders
            // 
            resources.ApplyResources(ListBoxInputReaders, "ListBoxInputReaders");
            ListBoxInputReaders.FormattingEnabled = true;
            ListBoxInputReaders.Name = "ListBoxInputReaders";
            ListBoxInputReaders.SelectedIndexChanged += ListBoxInputReaders_SelectedIndexChanged;
            // 
            // BtnSelect
            // 
            resources.ApplyResources(BtnSelect, "BtnSelect");
            BtnSelect.Name = "BtnSelect";
            BtnSelect.UseVisualStyleBackColor = true;
            BtnSelect.Click += BtnSelect_Click;
            // 
            // TextBoxDescription
            // 
            resources.ApplyResources(TextBoxDescription, "TextBoxDescription");
            TextBoxDescription.Name = "TextBoxDescription";
            TextBoxDescription.ReadOnly = true;
            // 
            // InputReaderDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TextBoxDescription);
            Controls.Add(BtnSelect);
            Controls.Add(ListBoxInputReaders);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "InputReaderDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            Load += InputReaderDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox ListBoxInputReaders;
        private Button BtnSelect;
        private TextBox TextBoxDescription;
    }
}