using System.Data;
using System.Xml;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.Forms
{
    public partial class RulesEditorForm : Form
    {
        public PacketRule? Rule { get; set; }
        public BasePacketData? PacketData { get; set; }
        public string? OldValue { get; set; }

        public RulesEditorForm()
        {
            InitializeComponent();
        }

        public static void OpenRuleEditor(PacketRule rule, BasePacketData packetData)
        {
            var editor = new RulesEditorForm();
            editor.LoadFromRule(rule, packetData);
            // editor.FillTypes();
            editor.Show();
            editor.BringToFront();
            editor.RuleEdit.Focus();
        }

        private void RulesEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        private void LoadFromRule(PacketRule rule, BasePacketData packetData)
        {
            PacketData = packetData;
            Rule = rule;
            OldValue = FormatRuleText(rule.RootNode.InnerXml);
            RuleEdit.Text = OldValue;

            /*
            var sList = OldValue.Replace("><", ">\n<").Split('\n').ToList();
            var indentCount = 0;
            string s = string.Empty;
            var isInComments = false;
            foreach (var line in sList)
            {
                var indentOffset = 0;
                var l = line.Trim();
                if (l.StartsWith("<") && l.EndsWith("/>"))
                {
                    // self-contained, nothing to indent
                }
                else
                if (l.StartsWith("<!"))
                {
                    isInComments = true;
                    // comment line, no indent
                }
                else
                if (l.StartsWith("-->"))
                {
                    isInComments = false;
                    // comment line, no indent
                }
                else
                if (l.StartsWith("</") && !l.EndsWith("/>"))
                {
                    // Ending-tag
                    indentOffset--;
                }
                else
                {
                    if (!isInComments)
                        indentOffset++;
                }

                var thisindent = indentCount;
                if (indentOffset < 0)
                    thisindent += indentOffset;
                for (var i = 0; i < thisindent; i++)
                    s += "\t";
                indentCount += indentOffset;
                s += l + "\r\n";
            }
            RuleEdit.Text = s;
            */
            Text = PacketFilterListEntry.AsString(rule.PacketId, rule.Level, rule.StreamId) + " - " + rule.Name;
            RuleEdit.SelectionLength = 0;
            RuleEdit.SelectionStart = RuleEdit.Text.Length;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Rule == null)
            {
                MessageBox.Show("No rule data", "Rule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                RuleEdit.Text = FormatRuleText(RuleEdit.Text);
                Rule.RootNode.InnerXml = RuleEdit.Text;
                Rule.Build();
                if (PacketData != null)
                {
                    Rule.RunRule(PacketData);
                    PacketData.AddUnparsedFields();
                    MainForm.Instance?.ShowPacketData(PacketData);
                }

                if (!string.IsNullOrWhiteSpace(Rule.Parent.Parent.LoadedRulesFileName))
                {
                    if (Rule.Parent.Parent.SaveRulesFile(Rule.Parent.Parent.LoadedRulesFileName))
                        Close();
                    else
                        MessageBox.Show(
                            string.Format("Failed to save rules file: {0}", Rule.Parent.Parent.LoadedRulesFileName),
                            "Rules Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Error in rules file: \n\r{0}", exception.Message), "Rule Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Rule.RootNode.InnerXml = OldValue ?? "<null />";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if ((Rule != null) && (OldValue != null))
                {
                    Rule.RootNode.InnerXml = OldValue;
                    Rule.Build();
                    if (PacketData != null)
                    {
                        Rule.RunRule(PacketData);
                        PacketData.AddUnparsedFields();
                        MainForm.Instance?.ShowPacketData(PacketData);
                    }
                }
            }
            catch
            {
                // Ignore
            }
            Close();
        }

        private string FormatRuleText(string ruleText)
        {
            var lines = ruleText.Replace("\r", "").FormatXml(true, false, "\t", ConformanceLevel.Fragment).Split("\n").ToList();
            var resLines = new List<string>();
            var indentCount = 0;
            var nextIndentCount = 0;
            foreach (var theLine in lines)
            {
                indentCount = nextIndentCount;
                var line = theLine.Replace("\r", "").Trim();

                // check single line tag
                if (line.Trim().EndsWith("/>") && (line.Trim().StartsWith("<")))
                {
                    // No change
                }
                else if (line.Trim().EndsWith(">") && (line.Trim().StartsWith("</")))
                {
                    indentCount--;
                    nextIndentCount--;
                }
                else if (line.Trim().EndsWith(">") && (line.Trim().StartsWith("<")))
                {
                    // indentCount++;
                    nextIndentCount++;
                }

                var indentString = string.Empty;
                for (var i = 0; i < indentCount; i++)
                {
                    indentString += "\t";
                }

                if (!string.IsNullOrWhiteSpace(line))
                    resLines.Add(indentString + line);
            }

            var res = string.Join('\n', resLines);
            res = res.Replace("\n", "\r\n");
            return res;
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (Rule == null)
            {
                MessageBox.Show("No rule data", "Rule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                RuleEdit.Text = FormatRuleText(RuleEdit.Text);
                Rule.RootNode.InnerXml = RuleEdit.Text;
                Rule.Build();
                if (PacketData != null)
                {
                    Rule.RunRule(PacketData);
                    PacketData.AddUnparsedFields();
                    MainForm.Instance?.ShowPacketData(PacketData);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Error in rules file: \n\r{0}", exception.Message), "Rule Error", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Rule.RootNode.InnerXml = OldValue ?? "<null />";
            }
        }

        private void RuleEdit_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            BtnSave.Enabled = true;
        }
    }
}
