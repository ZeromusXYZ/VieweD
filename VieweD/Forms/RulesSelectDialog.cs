using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class RulesSelectDialog : Form
    {
        private string SelectedFile { get; set; } = string.Empty;
        private List<string> DetectedFiles { get; set; } = [];
        private string LoadedRuleFileName { get; set; } = string.Empty;
        private ViewedProjectTab? ParentProject { get; set; }

        private RulesSelectDialog()
        {
            InitializeComponent();
        }

        private void FillForm()
        {
            SelectedFile = string.Empty;
            var defaultRulesPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "data", ParentProject?.InputReader?.DataFolder ?? "base", "rules");
            try
            {
                var def = string.Empty;
                var lastDisplaySize = 0;
                DetectedFiles = [];
                try
                {
                    if (Directory.Exists(defaultRulesPath))
                    {
                        var defFiles = Directory.GetFiles(defaultRulesPath, "*.xml").ToList();
                        if (defFiles.Count > 0)
                            DetectedFiles.AddRange(defFiles);
                    }

                    if (!string.IsNullOrWhiteSpace(LoadedRuleFileName) && File.Exists(LoadedRuleFileName))
                    {
                        var localPath = Path.GetDirectoryName(LoadedRuleFileName) ?? "";
                        var localFiles = Directory.GetFiles(localPath, "*.xml", SearchOption.AllDirectories).ToList();
                        if (localFiles.Count > 0)
                            DetectedFiles.AddRange(localFiles);
                    }
                }
                catch
                {
                    DetectedFiles.Clear();
                }

                var list = new List<RuleComboBoxEntry>();
                foreach (var f in DetectedFiles)
                {
                    var display = Path.GetFileNameWithoutExtension(f);
                    var displayWithSelectionInfo = display;
                    if (f == LoadedRuleFileName)
                        displayWithSelectionInfo = ">> " + displayWithSelectionInfo + " << (current)";
                    if (!f.StartsWith(defaultRulesPath))
                        displayWithSelectionInfo = display + " (local)";

                    list.Add(new RuleComboBoxEntry(displayWithSelectionInfo, f));
                    if (LoadedRuleFileName.Contains(display))
                    {
                        if (display.Length > lastDisplaySize)
                        {
                            def = f;
                            lastDisplaySize = display.Length;
                        }
                    }
                }

                cbRulesList.DisplayMember = "Display";
                cbRulesList.ValueMember = "Value";
                cbRulesList.DataSource = list;
                if (def != "")
                    cbRulesList.SelectedValue = def;
                else
                    cbRulesList.SelectedIndex = cbRulesList.Items.Count - 1;
            }
            catch
            {
                // Ignored
            }
        }

        private void RulesSelectForm_Load(object sender, EventArgs e)
        {
            //
        }

        public static string SelectRulesFile(string ruleFileName, ViewedProjectTab project)
        {
            string res;
            using var thisForm = new RulesSelectDialog();

            thisForm.LoadedRuleFileName = ruleFileName;
            thisForm.ParentProject = project;
            thisForm.FillForm();
            switch (thisForm.DetectedFiles.Count)
            {
                case <= 0:
                    res = string.Empty;
                    break;
                case 1:
                    res = thisForm.DetectedFiles[0];
                    break;
                default:
                    res = thisForm.ShowDialog() == DialogResult.OK ? thisForm.SelectedFile : string.Empty;
                    break;
            }

            return res;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedFile = cbRulesList.SelectedValue?.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
