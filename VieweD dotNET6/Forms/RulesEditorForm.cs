using System.Data;
using System.Xml;
using System.Xml.Linq;
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
            editor.BuildInsertMenu();
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

            var nextIndentCount = 0;
            foreach (var theLine in lines)
            {
                var indentCount = nextIndentCount;
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

        public ToolStripMenuItem? AddMenuItem(ToolStripItemCollection toolStripItemCollection, string title, string command)
        {
            var reader = Rule?.Parent?.Parent;
            if (reader == null)
                return null;

            if ((title == "-") || (title == ""))
            {
                var si = new ToolStripSeparator();
                toolStripItemCollection.Add(si);
                return null;
            }

            var ni = new ToolStripMenuItem();

            ni.Text = title;
            ni.Tag = command;
            ni.Click += miCustomInsert_Click;
            toolStripItemCollection.Add(ni);
            return ni;
        }

        private void miCustomInsert_Click(object? sender, EventArgs? e)
        {
            if (sender is not ToolStripMenuItem { Tag: string insertText }) 
                return;

            var currentCursorStart = RuleEdit.SelectionStart;
            var hasCursorMarker = insertText.IndexOf('|') >= 0;
            var lineCount = insertText.Split('\n').Length;

            if (lineCount > 1)
            {
                RuleEdit.SelectedText = insertText;
                RuleEdit.SelectionStart = currentCursorStart;
                RuleEdit.SelectionLength = 0;
                return;
            }

            if (!hasCursorMarker)
            {
                RuleEdit.SelectedText = insertText;
                RuleEdit.SelectionStart = currentCursorStart;
                RuleEdit.SelectionLength = insertText.Length;
                return;
            }

            var insertSplit = insertText.Split('|');
            var mergedInsertText = string.Concat(insertSplit);
            if (insertSplit.Length == 2)
            {
                // only one marker
                RuleEdit.SelectedText = mergedInsertText;
                RuleEdit.SelectionStart = currentCursorStart + insertSplit[0].Length;
                RuleEdit.SelectionLength = 0;
            }
            else
            if (insertSplit.Length == 3)
            {
                // two markers (select range)
                RuleEdit.SelectedText = mergedInsertText;
                RuleEdit.SelectionStart = currentCursorStart + insertSplit[0].Length;
                RuleEdit.SelectionLength = insertSplit[1].Length;
            }
            else
            {
                // more than 2 markers, just insert the entire thing as normal
                RuleEdit.SelectedText = insertText;
                RuleEdit.SelectionStart = currentCursorStart;
                RuleEdit.SelectionLength = insertText.Length;
            }
        }

        private void BuildInsertMenu()
        {
            if (RuleEdit.SelectedText.Length > 0)
                MiInsert.Text = "Replace";
            else
                MiInsert.Text = "Insert";
            MiInsert.Items.Clear();

            Rule?.Parent?.Parent.BuildEditorPopupMenu(MiInsert, this);
        }

        private void MiInsert_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void RulesEditorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
