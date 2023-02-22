using System.Xml;
using VieweD.engine.common;
using VieweD.Helpers.System;
using VieweD.Properties;

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
            MainForm.Instance?.CenterMyForm(editor);
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

            Text = PacketFilterListEntry.AsString(rule.PacketId, rule.Level, rule.StreamId) + @" - " + rule.Name;
            RuleEdit.SelectionLength = 0;
            RuleEdit.SelectionStart = RuleEdit.Text.Length;

            var attributes = XmlHelper.ReadNodeAttributes(Rule.RootNode);
            DescriptionBox.Text = XmlHelper.GetAttributeString(attributes, @"desc");
            CommentBox.Text = XmlHelper.GetAttributeString(attributes, @"comment");
            CreditsBox.Text = XmlHelper.GetAttributeString(attributes, @"credits");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Rule == null)
            {
                MessageBox.Show(Resources.NoRuleData, Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                MainForm.Instance!.ShowDebugInfo = false;
                RuleEdit.Text = FormatRuleText(RuleEdit.Text);
                Rule.RootNode.InnerXml = RuleEdit.Text;
                Rule.Build();
                if (PacketData != null)
                {
                    Rule.RunRule(PacketData);
                    PacketData.AddUnparsedFields();
                    MainForm.Instance.ShowPacketData(PacketData);
                }

                XmlHelper.SetAttribute(Rule.RootNode, @"desc", DescriptionBox.Text);
                XmlHelper.SetAttribute(Rule.RootNode, @"comment", CommentBox.Text);
                XmlHelper.SetAttribute(Rule.RootNode, @"credits", CreditsBox.Text);

                if (!string.IsNullOrWhiteSpace(Rule.Parent.Parent.LoadedRulesFileName))
                {
                    if (Rule.Parent.Parent.SaveRulesFile(Rule.Parent.Parent.LoadedRulesFileName))
                        Close();
                    else
                        MessageBox.Show(
                            string.Format(Resources.FailedToSaveRulesFile, Rule.Parent.Parent.LoadedRulesFileName),
                            Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Apply to the rest of the packets with the same rule, and re-parse them
                if (PacketData != null)
                {
                    MainForm.Instance.ShowDebugInfo = false;
                    MainForm.Instance.UpdateStatusBarProgress(
                        0, PacketData.ParentProject.LoadedPacketList.Count,
                        Resources.ApplyChanges, null);
                    for (var i = 0; i < PacketData.ParentProject.LoadedPacketList.Count; i++)
                    {
                        var data = PacketData.ParentProject.LoadedPacketList[i];
                        if ((data.PacketDataDirection != PacketData.PacketDataDirection) ||
                            (data.PacketId != PacketData.PacketId) ||
                            (data.CompressionLevel != PacketData.CompressionLevel) ||
                            (data.StreamId != PacketData.StreamId))
                            continue;
                        Rule.RunRule(data);
                        data.AddUnparsedFields();
                        if ((i % 50) == 0)
                        {
                            MainForm.Instance.UpdateStatusBarProgress(
                                i, PacketData.ParentProject.LoadedPacketList.Count,
                                Resources.ApplyChanges, null);
                        }
                    }
                    MainForm.Instance.UpdateStatusBarProgress(PacketData.ParentProject.LoadedPacketList.Count, PacketData.ParentProject.LoadedPacketList.Count, Resources.ApplyChanges, null);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format(Resources.ErrorInRulesFile, exception.Message), Resources.RuleError,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Rule.RootNode.InnerXml = OldValue ?? @"<null />";
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

        private static string FormatRuleText(string ruleText)
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
                MessageBox.Show(Resources.NoRuleData, Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                MainForm.Instance!.ShowDebugInfo = true;
                RuleEdit.Text = FormatRuleText(RuleEdit.Text);
                Rule.RootNode.InnerXml = RuleEdit.Text;
                Rule.Build();
                if (PacketData != null)
                {
                    Rule.RunRule(PacketData);
                    PacketData.AddUnparsedFields();
                    MainForm.Instance.ShowPacketData(PacketData);
                }
                MainForm.Instance.ShowDebugInfo = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format(Resources.ErrorInRulesFile, exception.Message), Resources.RuleError, 
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
            var reader = Rule?.Parent.Parent;
            if (reader == null)
                return null;

            if (title is "-" or "")
            {
                var si = new ToolStripSeparator();
                toolStripItemCollection.Add(si);
                return null;
            }

#pragma warning disable IDE0017 // Simplify object initialization
            var ni = new ToolStripMenuItem();
#pragma warning restore IDE0017 // Simplify object initialization
            ni.Text = title;
            ni.Tag = command;
            ni.Click += MiCustomInsert_Click;
            toolStripItemCollection.Add(ni);
            return ni;
        }

        private void MiCustomInsert_Click(object? sender, EventArgs? e)
        {
            if (sender is not ToolStripMenuItem { Tag: string insertText }) 
                return;

            var currentCursorStart = RuleEdit.SelectionStart;
            var hasCursorMarker = insertText.Contains('|');
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
            MiInsert.Text = RuleEdit.SelectedText.Length > 0 ? Resources.PopupReplace : Resources.PopupInsert;
            MiInsert.Items.Clear();

            Rule?.Parent.Parent.BuildEditorPopupMenu(MiInsert, this);
        }

        private void BtnAllowEdit_Click(object sender, EventArgs e)
        {
            DescriptionBox.ReadOnly = false;
            CommentBox.ReadOnly = false;
            CreditsBox.ReadOnly = false;
            BtnAllowEdit.Hide();
        }
    }
}
