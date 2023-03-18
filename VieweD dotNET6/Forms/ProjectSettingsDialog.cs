using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Automation;
using System.Xml.Linq;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class ProjectSettingsDialog : Form
    {
        public ViewedProjectTab? ParentProject { get; set; }
        public bool RequiresReload { get; set; }
        private string DefaultTitle { get; set; } = string.Empty;
        private List<RuleComboBoxEntry> RulesSelectionList { get; set; } = new();

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

        private void FillRulesListFromParser()
        {
            RulesSelectionList.Clear();
            var currentRuleFile = ParentProject?.InputParser?.Rules?.LoadedRulesFileName ?? string.Empty;

            RulesSelectionList.Add(new RuleComboBoxEntry(">> " + Path.GetFileNameWithoutExtension(currentRuleFile) + " (current)", currentRuleFile));

            if ((ParentProject == null) || (ParentProject.InputReader == null))
                return;

            var rulesFolder = Path.Combine(Application.StartupPath, "data", ParentProject.InputReader.DataFolder, "rules");
            if (Directory.Exists(rulesFolder))
            {
                var rulesFiles = Directory.GetFiles(rulesFolder, "*.xml", SearchOption.AllDirectories);
                foreach (var rulesFile in rulesFiles)
                    RulesSelectionList.Add(
                        new RuleComboBoxEntry(Path.GetFileNameWithoutExtension(rulesFile), rulesFile));
            }

            if ((ParentProject.ProjectFolder != "") && Directory.Exists(ParentProject.ProjectFolder))
            {
                var localRulesFiles =
                    Directory.GetFiles(ParentProject.ProjectFolder, "*.xml", SearchOption.AllDirectories);
                foreach (var rulesFile in localRulesFiles)
                    RulesSelectionList.Add(
                        new RuleComboBoxEntry(Path.GetFileNameWithoutExtension(rulesFile) + " (local)", rulesFile));
            }
        }

        private void FillFieldsFromProject()
        {
            if (ParentProject == null)
                return;

            Text = DefaultTitle + @" - " + ParentProject.ProjectName;
            TextProjectFile.Text = ParentProject.ProjectFile;
            TextLogFile.Text = ParentProject.OpenedLogFile;
            TextVideoFile.Text = ParentProject.VideoSettings.VideoFile;

            CbInputReader.Text = ParentProject.InputReader?.Name ?? string.Empty;
            CbInputReader.Enabled = CbInputReader.Text == string.Empty;
            CbParser.Text = ParentProject.InputParser?.Name ?? string.Empty;
            CbParser.Enabled = CbParser.Text == string.Empty;

            // TODO: Fill in all rules files
            FillRulesListFromParser();
            CbRules.Items.Clear();
            CbRules.DataSource = RulesSelectionList;
            CbRules.DisplayMember = "Display";
            CbRules.ValueMember = "Value";

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
            CbInputReader.Items.Clear();
            CbParser.Items.Clear();
            CbRules.Items.Clear();

            foreach (var baseInputReader in EngineManager.AllInputReaders)
                CbInputReader.Items.Add(baseInputReader.Name);

            foreach (var baseParser in EngineManager.AllParsers)
                CbParser.Items.Add(baseParser.Name);

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

            var label = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = SystemColors.Highlight,
                ForeColor = SystemColors.HighlightText,
                Text = tag,
                AutoSize = true,
                Cursor = Cursors.No,
            };
            label.Click += TagLabel_Click;
            TagLayout.Controls.Add(label);
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
    }
}
