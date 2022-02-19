using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD.Forms
{
    public partial class RulesSelectForm : Form
    {
        private class ComboBoxFileListValues
        {
            private string display;
            private string fileName;

            public string Display { get => display; set => display = value; }
            public string FileName { get => fileName; set => fileName = value; }
        }

        public string SelectedFile { get; set; }
        public List<string> DetectedFiles { get; set; }
        public string LoadingPacketFileName { get; set; }
        public EngineBase EngineBase { get; set; }

        public RulesSelectForm()
        {
            InitializeComponent();
        }

        public void FillForm()
        {
            SelectedFile = string.Empty;
            var defaultRulesPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data", EngineBase.EngineId , "rules");
            try
            {
                var def = string.Empty;
                var lastDispSize = 0;
                try
                {
                    if (Directory.Exists(defaultRulesPath))
                    {
                        DetectedFiles = Directory.GetFiles(defaultRulesPath, "*.xml").ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(LoadingPacketFileName) && File.Exists(LoadingPacketFileName))
                    {
                        var localPath = Path.GetDirectoryName(LoadingPacketFileName);
                        var localFiles = Directory.GetFiles(localPath, "*.xml", SearchOption.AllDirectories).ToList();
                        DetectedFiles.AddRange(localFiles);
                    }
                }
                catch
                {
                    DetectedFiles = new List<string>();
                }

                var list = new List<ComboBoxFileListValues>();
                foreach (var f in DetectedFiles)
                {
                    var disp = Path.GetFileNameWithoutExtension(f);
                    var disp2 = disp;
                    if (!f.StartsWith(defaultRulesPath))
                        disp2 = ">> " + disp;

                    list.Add(new ComboBoxFileListValues { FileName = f, Display = disp2 });
                    if (LoadingPacketFileName.Contains(disp))
                    {
                        if (disp.Length > lastDispSize)
                        {
                            def = f;
                            lastDispSize = disp.Length;
                        }
                    }
                }

                cbRulesList.DisplayMember = "Display";
                cbRulesList.ValueMember = "FileName";
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

        static public string SelectRulesFile(string packetFileName, EngineBase engine)
        {
            string res;
            using (var thisForm = new RulesSelectForm())
            {
                thisForm.LoadingPacketFileName = packetFileName;
                thisForm.EngineBase = engine;
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
            }
            return res;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedFile = cbRulesList?.SelectedValue.ToString() ?? string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
