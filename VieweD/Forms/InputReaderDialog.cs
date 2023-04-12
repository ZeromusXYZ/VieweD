using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class InputReaderDialog : Form
    {
        public BaseInputReader? InputReader { get; set; }

        public InputReaderDialog()
        {
            InitializeComponent();
        }

        private void InputReaderDialog_Load(object sender, EventArgs e)
        {
            ListBoxInputReaders.Items.Clear();
            foreach (var baseInputReader in EngineManager.AllInputReaders)
            {
                if (baseInputReader.Description == "")
                    continue;
                ListBoxInputReaders.Items.Add(baseInputReader);
                if ((InputReader != null) && (InputReader.Name == baseInputReader.Name))
                    ListBoxInputReaders.SelectedItem = baseInputReader;
            }
        }

        public static BaseInputReader? SelectInputReader(ViewedProjectTab project, string fileName)
        {
            var res = new List<BaseInputReader>();
            foreach (var baseInputReader in EngineManager.AllInputReaders)
            {
                if (baseInputReader.CanHandleSource(fileName))
                    res.Add(baseInputReader);
            }
            if (res.Count == 1)
                return res[0];

            using var selectForm = new InputReaderDialog();
            selectForm.InputReader = project.InputReader;
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                EngineManager.Instance.GetInputReaderByName(selectForm.InputReader?.Name ?? "", project);
                return selectForm.InputReader?.CreateNew(project);
            }
            return project.InputReader;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            InputReader = ListBoxInputReaders.SelectedItem as BaseInputReader;
            DialogResult = DialogResult.OK;
        }

        private void ListBoxInputReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBoxInputReaders.SelectedItem is BaseInputReader reader)
            {
                TextBoxDescription.Text = reader.Description;
            }
            else
            {
                TextBoxDescription.Text = string.Empty;
            }
        }
    }
}
