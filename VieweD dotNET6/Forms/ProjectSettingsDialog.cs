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
    }
}
