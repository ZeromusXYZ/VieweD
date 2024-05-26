using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using VieweD.engine.common;

namespace VieweD.Forms
{
    public partial class DecryptKeySelectDialog : Form
    {
        public string SelectedKey { get; set; } = string.Empty;
        public List<string> DetectedKeys { get; set; } = new();
        public string LoadedKey { get; set; } = string.Empty;
        public ViewedProjectTab? ParentProject { get; set; }

        public DecryptKeySelectDialog()
        {
            InitializeComponent();
        }

        public void FillForm()
        {
            SelectedKey = string.Empty;

            var defaultKeysPath = Path.Combine(Application.StartupPath, "data", ParentProject?.InputReader?.DataFolder ?? "base", "keys");
            try
            {
                var def = string.Empty;
                var lastDisplaySize = 0;
                var detectedFiles = new List<string>();
                try
                {
                    if (Directory.Exists(defaultKeysPath))
                    {
                        var defFiles = Directory.GetFiles(defaultKeysPath, "*.key");
                        if (defFiles.Length > 0)
                            detectedFiles.AddRange(defFiles);
                    }

                    if ((ParentProject != null) && (ParentProject.ProjectFolder != string.Empty) && Directory.Exists(ParentProject.ProjectFolder))
                    {
                        var localFiles = Directory.GetFiles(ParentProject.ProjectFolder, "*.key", SearchOption.AllDirectories);
                        if (localFiles.Length > 0)
                            detectedFiles.AddRange(localFiles);
                    }
                }
                catch
                {
                    DetectedKeys.Clear();
                }

                foreach (var detectedFile in detectedFiles)
                {
                    var lines = File.ReadAllLines(detectedFile);
                    foreach (var line in lines)
                    {
                        var field = line.Split(';');
                        if (field.Length >= 2)
                            DetectedKeys.Add(field[0]);
                    }
                }


                var list = new List<RuleComboBoxEntry>();
                foreach (var keyName in DetectedKeys)
                {
                    var displayText = keyName;
                    if (keyName == LoadedKey)
                        displayText = ">> " + displayText + " << (current)";

                    list.Add(new RuleComboBoxEntry(displayText, keyName));
                    if (LoadedKey.Contains(displayText))
                    {
                        if (displayText.Length > lastDisplaySize)
                        {
                            def = keyName;
                            lastDisplaySize = displayText.Length;
                        }
                    }
                }

                cbKeysList.DisplayMember = "Display";
                cbKeysList.ValueMember = "Value";
                cbKeysList.DataSource = list;
                if (def != "")
                    cbKeysList.SelectedValue = def;
                else
                    cbKeysList.SelectedIndex = cbKeysList.Items.Count - 1;
            }
            catch
            {
                // Ignored
            }
        }

        private void RulesSelectForm_Load(object sender, EventArgs e)
        {
        }

        public static string SelectDecryptionKeyName(string keyName, ViewedProjectTab project)
        {
            string res;
            using var thisForm = new DecryptKeySelectDialog();

            thisForm.LoadedKey = keyName;
            thisForm.ParentProject = project;
            thisForm.FillForm();
            if (thisForm.DetectedKeys.Count <= 0)
                res = string.Empty;
            else
            if (thisForm.DetectedKeys.Count == 1)
                res = thisForm.DetectedKeys[0];
            else
            if (thisForm.ShowDialog() == DialogResult.OK)
            {
                res = thisForm.SelectedKey;
            }
            else
                res = string.Empty;

            return res;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedKey = cbKeysList.SelectedValue?.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
