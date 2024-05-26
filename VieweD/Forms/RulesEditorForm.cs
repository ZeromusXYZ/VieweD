using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using VieweD.engine.common;
using VieweD.Helpers.System;
using VieweD.Properties;

namespace VieweD.Forms
{
    public partial class RulesEditorForm : Form
    {
        public ViewedProjectTab? ParentProject { get; set; }
        public PacketRule? Rule { get; private set; }
        private XmlNode? EditorNode { get; set; }
        public BasePacketData? PacketData { get; private set; }
        private string? OldValue { get; set; }

        private RulesEditorForm()
        {
            InitializeComponent();
        }

        public static RulesEditorForm OpenRuleEditor(PacketRule rule, BasePacketData packetData)
        {
            var editor = new RulesEditorForm();
            editor.LoadFromRule(rule, packetData);
            // editor.FillTypes();
            editor.Show();
            MainForm.Instance?.CenterMyForm(editor);
            editor.BringToFront();
            editor.RuleEdit.Focus();
            editor.BuildInsertMenu(rule.Parent.Parent);
            return editor;
        }

        public static RulesEditorForm OpenTemplateEditor(XmlNode templateNode, ViewedProjectTab project)
        {
            var editor = new RulesEditorForm();
            editor.LoadFromTemplateNode(templateNode, project);
            // editor.FillTypes();
            editor.Show();
            MainForm.Instance?.CenterMyForm(editor);
            editor.BringToFront();
            editor.RuleEdit.Focus();
            editor.BuildInsertMenu(project.InputParser?.Rules);
            editor.BtnTest.Enabled = false;
            return editor;
        }

        private void RulesEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ParentProject != null)
                ParentProject.CurrentEditor = null;
            Dispose();
        }

        private void LoadFromRule(PacketRule rule, BasePacketData packetData)
        {
            ParentProject = rule.Parent.Parent.ParentProject;
            PacketData = packetData;
            Rule = rule;
            EditorNode = null;
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

        private void LoadFromTemplateNode(XmlNode templateNode, ViewedProjectTab project)
        {
            ParentProject = project;
            PacketData = null;
            Rule = null;
            EditorNode = templateNode;
            OldValue = FormatRuleText(templateNode.InnerXml);
            RuleEdit.Text = OldValue;

            Text = @"???";
            RuleEdit.SelectionLength = 0;
            RuleEdit.SelectionStart = RuleEdit.Text.Length;

            var attributes = XmlHelper.ReadNodeAttributes(templateNode);
            Text = @"Template: " + XmlHelper.GetAttributeString(attributes, @"name");
            DescriptionBox.Text = XmlHelper.GetAttributeString(attributes, @"desc");
            CommentBox.Text = XmlHelper.GetAttributeString(attributes, @"comment");
            CreditsBox.Text = XmlHelper.GetAttributeString(attributes, @"credits");
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Rule != null)
            {
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
                        if (!Rule.Parent.Parent.SaveRulesFile(Rule.Parent.Parent.LoadedRulesFileName))
                            MessageBox.Show(
                                string.Format(Resources.FailedToSaveRulesFile, Rule.Parent.Parent.LoadedRulesFileName),
                                Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Apply to the rest of the packets with the same rule, and reparse them
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

                        MainForm.Instance.UpdateStatusBarProgress(PacketData.ParentProject.LoadedPacketList.Count,
                            PacketData.ParentProject.LoadedPacketList.Count, Resources.ApplyChanges, null);
                    }

                    Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format(Resources.ErrorInRulesFile, exception.Message), Resources.RuleError,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Rule.RootNode.InnerXml = OldValue ?? @"<null />";
                }
            }
            else if (EditorNode != null)
            {
                try
                {
                    RuleEdit.Text = FormatRuleText(RuleEdit.Text);
                    EditorNode.InnerXml = RuleEdit.Text;

                    XmlHelper.SetAttribute(EditorNode, @"desc", DescriptionBox.Text);
                    XmlHelper.SetAttribute(EditorNode, @"comment", CommentBox.Text);
                    XmlHelper.SetAttribute(EditorNode, @"credits", CreditsBox.Text);

                    if (!string.IsNullOrWhiteSpace(ParentProject?.InputParser?.Rules?.LoadedRulesFileName))
                    {
                        if (!ParentProject.InputParser.Rules.SaveRulesFile(ParentProject.InputParser.Rules.LoadedRulesFileName))
                            MessageBox.Show(string.Format(Resources.FailedToSaveRulesFile, ParentProject.InputParser.Rules.LoadedRulesFileName), Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                    Close();
                    if (ParentProject is { InputParser: not null } && 
                        MessageBox.Show(Resources.ReParseProject, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var lastPacket = ParentProject.PacketsListBox.SelectedItem as BasePacketData;
                        ParentProject.InputParser.ParseAllData(true);
                        ParentProject.ReIndexLoadedPackets();
                        ParentProject.PopulateListBox();
                        if (lastPacket != null)
                            ParentProject.PacketsListBox.SelectedItem = lastPacket;
                            //ParentProject.GotoPacketTimeOffset(lastPacket.OffsetFromStart);
                        ParentProject.OnProjectDataChanged();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format(Resources.ErrorInRulesFile, exception.Message), Resources.RuleError,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    EditorNode.InnerXml = OldValue ?? @"<null />";
                }
            }
            else
            {
                MessageBox.Show(Resources.NoRuleData, Resources.RuleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                if ((EditorNode != null) && (OldValue != null))
                {
                    EditorNode.InnerXml = OldValue;
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
                else if (line.Trim().EndsWith("-->") && (line.Trim().StartsWith("<!--")))
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
            var reader = Rule?.Parent.Parent ?? ParentProject?.InputParser?.Rules;
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

        private void BuildInsertMenu(RulesReader? reader)
        {
            MiInsert.Text = RuleEdit.SelectedText.Length > 0 ? Resources.PopupReplace : Resources.PopupInsert;
            MiInsert.Items.Clear();

            reader?.BuildEditorPopupMenu(MiInsert, this);
            // Rule?.Parent.Parent.BuildEditorPopupMenu(MiInsert, this);
        }

        private void BtnAllowEdit_Click(object sender, EventArgs e)
        {
            DescriptionBox.ReadOnly = false;
            CommentBox.ReadOnly = false;
            CreditsBox.ReadOnly = false;
            BtnAllowEdit.Hide();
            if (string.IsNullOrWhiteSpace(CreditsBox.Text))
            {
                var currentWindowsUser = string.IsNullOrWhiteSpace(Settings.Default.CreditsName) ? Environment.UserName : Settings.Default.CreditsName;
                CreditsBox.Text = currentWindowsUser;
            }
        }
    }
}
