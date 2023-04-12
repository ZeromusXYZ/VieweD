using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VieweD.Forms
{
    public partial class InputBoxDialog : Form
    {
        private string revertString { get; set; } = string.Empty;

        public InputBoxDialog()
        {
            InitializeComponent();
        }

        public static string InputTextBox(string defaultText, string title = "", string inputPrompt = "")
        {
            var res = defaultText;
            using var dlg = new InputBoxDialog();

            dlg.revertString = defaultText;

            if (!string.IsNullOrWhiteSpace(title))
                dlg.Text = title;

            if (!string.IsNullOrWhiteSpace(inputPrompt))
                dlg.PromptLabel.Text = inputPrompt;

            dlg.InputText.Text = defaultText;
            dlg.InputText.SelectAll();

            if (dlg.ShowDialog() == DialogResult.OK)
                res = dlg.InputText.Text;

            return res;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnRevert_Click(object sender, EventArgs e)
        {
            InputText.Text = revertString;
            InputText.SelectAll();
            InputText.Focus();
        }
    }
}
