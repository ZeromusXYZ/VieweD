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
    public partial class ParserDialog : Form
    {
        public BaseParser? Parser { get; set; }

        public ParserDialog()
        {
            InitializeComponent();
        }

        private void ParserDialog_Load(object sender, EventArgs e)
        {
            ListBoxParsers.Items.Clear();
            foreach (var parser in EngineManager.AllParsers)
            {
                if (parser.Description == "")
                    continue;
                ListBoxParsers.Items.Add(parser);
                if ((Parser != null) && (Parser.Name == parser.Name))
                    ListBoxParsers.SelectedItem = parser;
            }
        }

        public static BaseParser? SelectParser(ViewedProjectTab project, BaseInputReader? reader)
        {
            var res = new List<BaseParser>();
            foreach (var baseParser in EngineManager.AllParsers)
            {
                if (baseParser.CanHandleSource(reader))
                    res.Add(baseParser);
            }
            if (res.Count == 1)
                return res[0].CreateNew(project);

            using var selectForm = new ParserDialog();
            selectForm.Parser = project.InputParser;
            if (selectForm.ShowDialog() == DialogResult.OK)
                return selectForm.Parser?.CreateNew(project);
            return project.InputParser;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            Parser = ListBoxParsers.SelectedItem as BaseParser;
            DialogResult = DialogResult.OK;
        }

        private void ListBoxInputReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBoxParsers.SelectedItem is BaseParser parser)
            {
                TextBoxDescription.Text = parser.Description;
            }
            else
            {
                TextBoxDescription.Text = string.Empty;
            }
        }
    }
}
