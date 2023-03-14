using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class RulesSelectDialog : Form
    {
        public string SelectedFile { get; set; } = string.Empty;
        public List<string> DetectedFiles { get; set; } = new();
        public string LoadedRuleFileName { get; set; } = string.Empty;
        public ViewedProjectTab? ParentProject { get; set; }

        public RulesSelectDialog()
        {
            InitializeComponent();
        }

        public void FillForm()
        {
            SelectedFile = string.Empty;
            var defaultRulesPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? "", "data", ParentProject?.InputReader?.DataFolder ?? "base", "rules");
            try
            {
                var def = string.Empty;
                var lastDispSize = 0;
                DetectedFiles = new List<string>();
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
                    var disp = Path.GetFileNameWithoutExtension(f);
                    var disp2 = disp;
                    if (f == LoadedRuleFileName)
                        disp2 = ">> " + disp2 + " << (current)";
                    if (!f.StartsWith(defaultRulesPath))
                        disp2 = disp + " (local)";

                    list.Add(new RuleComboBoxEntry(disp2, f));
                    if (LoadedRuleFileName.Contains(disp))
                    {
                        if (disp.Length > lastDispSize)
                        {
                            def = f;
                            lastDispSize = disp.Length;
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
        }

        public static string SelectRulesFile(string ruleFileName, ViewedProjectTab project)
        {
            string res;
            using var thisForm = new RulesSelectDialog();

            thisForm.LoadedRuleFileName = ruleFileName;
            thisForm.ParentProject = project;
            thisForm.FillForm();
            if (thisForm.DetectedFiles.Count <= 0)
                res = string.Empty;
            else
            if (thisForm.DetectedFiles.Count == 1)
                res = thisForm.DetectedFiles[0];
            else
            if (thisForm.ShowDialog() == DialogResult.OK)
            {
                res = thisForm.SelectedFile;
            }
            else
                res = string.Empty;

            return res;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedFile = cbRulesList?.SelectedValue.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
