using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class ProjectSettingsDialog : Form
    {
        public ViewedProjectTab? ParentProject { get; set; }
        public bool RequiresReload { get; set; }
        private string DefaultTitle { get; set; } = string.Empty;

        public ProjectSettingsDialog()
        {
            InitializeComponent();
        }

        public void AssignProject(ViewedProjectTab project)
        {
            ClearForm();
            ParentProject = project;
            FillFieldsFromProject();
        }

        private void FillFieldsFromProject()
        {
            if (ParentProject == null)
                return;

            Text = DefaultTitle + " - " + ParentProject.ProjectName;
            TextProjectFile.Text = ParentProject.ProjectFile;
            TextLogFile.Text = ParentProject.OpenedLogFile;

            CBInputReader.Text = ParentProject.InputReader?.Name ?? string.Empty;
            CBInputReader.Enabled = CBInputReader.Text == string.Empty;
            CBParser.Text = ParentProject.InputParser?.Name ?? string.Empty;
            CBParser.Enabled = CBParser.Text == string.Empty;

            // TODO: Fill in all rules files
            var loadedRule = Path.GetFileNameWithoutExtension(ParentProject.InputParser?.Rules?.LoadedRulesFileName ?? string.Empty);
            CBRules.Items.Add(loadedRule);
            CBRules.Text = loadedRule;
            // CBRules.Enabled = CBRules.Text == string.Empty;

            CreateVisualTags(ParentProject.Tags);

            RequiresReload = false;
        }

        private void ProjectSettingsDialog_Load(object sender, EventArgs e)
        {

        }

        private void ClearForm()
        {
            if (string.IsNullOrWhiteSpace(DefaultTitle))
                DefaultTitle = Text;

            TextProjectFile.Text = string.Empty;
            TextLogFile.Text = string.Empty;
            TextVideoFile.Text = string.Empty;
            CBInputReader.Items.Clear();
            CBParser.Items.Clear();
            CBRules.Items.Clear();

            foreach (var baseInputReader in EngineManager.AllInputReaders)
                CBInputReader.Items.Add(baseInputReader.Name);

            foreach (var baseParser in EngineManager.AllParsers)
                CBParser.Items.Add(baseParser.Name);

            RequiresReload = false;
        }

        private void BtnAddTag_Click(object sender, EventArgs e)
        {
            AddTag(TagTextBox.Text);
            TagTextBox.Text = string.Empty;
            TagTextBox.Focus();
        }

        private void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            var label = new Label();
            label.BorderStyle = BorderStyle.Fixed3D;
            label.BackColor = SystemColors.Highlight;
            label.ForeColor = SystemColors.HighlightText;
            TagLayout.Controls.Add(label);
            label.Text = tag;
            label.AutoSize = true;
            label.Cursor = Cursors.No;
            label.Click += TagLabel_Click;
        }

        private void TagLabel_Click(object? sender, EventArgs e)
        {
            if (sender is not Label label)
                return;

            var oldTag = label.Text;
            TagLayout.Controls.Remove(label);
            TagTextBox.Text = oldTag;
        }

        public static List<string> TagsToList(string tagString)
        {
            var res = new List<string>();
            res.AddRange(tagString.Split(',').ToList());
            return res;
        }

        private void CreateVisualTags(List<string> tags)
        {
            TagLayout.Controls.Clear();
            foreach (var t in tags)
            {
                var s = t.Trim(' ');
                if (!string.IsNullOrWhiteSpace(s))
                    AddTag(s);
            }
        }

        private string VisualTagsToString(string spacer = ",")
        {
            var res = string.Empty;

            foreach (Control c in TagLayout.Controls)
            {
                if ((c is not Label label) || (string.IsNullOrWhiteSpace(label.Text)))
                    continue;

                if (res != string.Empty)
                    res += spacer;
                res += label.Text;
            }

            return res;
        }
    }
}
