﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.Forms
{
    public partial class ProjectSettingsDialog : Form
    {
        public ViewedProjectTab? ParentProject { get; set; }
        public bool RequiresReload { get; set; }
        private string DefaultTitle { get; set; } = string.Empty;
        private List<RuleComboBoxEntry> RulesSelectionList { get; } = new();
        private const string KeyNotUsed = "<not used>";

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
            TextLogFile.Text = ParentProject.Settings.LogFile;
            TextVideoFile.Text = ParentProject.Settings.VideoSettings.VideoFile;
            TextProjectURL.Text = ParentProject.Settings.ProjectUrl;
            TextVideoURL.Text = ParentProject.Settings.VideoSettings.VideoUrl;
            TextDescription.Text = ParentProject.Settings.Description;

            DecryptionKeyNameLabel.Text = ParentProject.DecryptionKeyName != string.Empty ? ParentProject.DecryptionKeyName : KeyNotUsed;

            CbInputReader.Text = ParentProject.InputReader?.Name ?? string.Empty;
            CbInputReader.Enabled = CbInputReader.Text == string.Empty;
            CbParser.Text = ParentProject.InputParser?.Name ?? string.Empty;
            CbParser.Enabled = CbParser.Text == string.Empty;

            FillRulesListFromParser();
            CbRules.Items.Clear();
            CbRules.DataSource = RulesSelectionList;
            CbRules.DisplayMember = "Display";
            CbRules.ValueMember = "Value";

            CreateVisualTags(ParentProject.Settings.Tags);

            RequiresReload = false;
        }

        private void ProjectSettingsDialog_Load(object sender, EventArgs e)
        {
            CBIncludePacketIds.Checked = Properties.Settings.Default.CopySummaryPacketIDs;
            CBIncludePacketNames.Checked = Properties.Settings.Default.CopySummaryPacketNames;
            CBHideUrlPreviews.Checked = Properties.Settings.Default.CopySummaryNoAutoLoad;
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

        public List<string> GetTagsList()
        {
            var res = new List<string>();
            foreach (Control tagLayoutControl in TagLayout.Controls)
            {
                if (tagLayoutControl is Label label)
                {
                    var s = label.Text.Trim();
                    if (s != string.Empty)
                        res.Add(s);
                }
            }
            return res;
        }

        private void BtnCopySummary_Click(object sender, EventArgs e)
        {
            var clipText = "";
            clipText += "**Name**: " + Path.GetFileNameWithoutExtension(TextProjectFile.Text) + "\n";
            clipText += "**Description**: " + TextDescription.Text + "\n";
            // Download URL
            if (!string.IsNullOrWhiteSpace(TextProjectURL.Text))
            {
                if (CBHideUrlPreviews.Checked)
                    clipText += "> **Logs**: <" + TextProjectURL.Text + ">\n";
                else
                    clipText += "> **Logs**: " + TextProjectURL.Text + "\n";
            }

            // Video URL
            if (!string.IsNullOrWhiteSpace(TextVideoURL.Text))
            {
                if (CBHideUrlPreviews.Checked)
                    clipText += "> **Video**: <" + TextVideoURL.Text + ">\n";
                else
                    clipText += "> **Video**: " + TextVideoURL.Text + "\n";
            }

            // Tags
            var tagText = string.Join(", ", GetTagsList());
            if (tagText != string.Empty)
                clipText += "> **Tags**: *" + tagText + "*\n";

            if (CBIncludePacketIds.Checked || CBIncludePacketNames.Checked)
            {
                // Packet IDs
                // Incoming
                //var useLevels = ParentProject?.InputParser?.Rules?.UsesCompressionLevels ?? false;
                var useStreams = ParentProject?.InputParser?.Rules?.UsesMultipleStreams ?? false;

                // Dir, StreamId, CompressionLevel, PacketID
                Dictionary<PacketDataDirection, Dictionary<byte, Dictionary<byte, List<uint>>>> keyOutput = new();

                // Grab all used packet Ids
                foreach (var dir in Enum.GetValues<PacketDataDirection>())
                {
                    var packetsUsed = ParentProject?.GetAllUsedPacketsByDirection(dir);
                    if (packetsUsed == null) 
                        continue;

                    //var packets = string.Empty;
                    foreach (var key in packetsUsed)
                    {
                        var (pId, pLevel, pStream) = PacketFilterListEntry.DecodeFilterKey(key);
                        // Get Direction
                        if (!keyOutput.TryGetValue(dir, out var keyStreamLevelPacket))
                        {
                            keyStreamLevelPacket = new();
                            keyOutput.Add(dir, keyStreamLevelPacket);
                        }

                        // Get Stream
                        if (!keyStreamLevelPacket.TryGetValue(pStream, out var keyLevelPacket))
                        {
                            keyLevelPacket = new Dictionary<byte, List<uint>>();
                            keyStreamLevelPacket.Add(pStream, keyLevelPacket);
                        }

                        // Get Level
                        if (!keyLevelPacket.TryGetValue(pLevel, out var listPacketId))
                        {
                            listPacketId = new();
                            keyLevelPacket.Add(pLevel, listPacketId);
                        }

                        if (!listPacketId.Contains(pId))
                            listPacketId.Add(pId);
                    }

                }

                // Create output
                foreach (var outDirStreamLevelPacket in keyOutput)
                {
                    foreach (var outStreamLevelPacket in outDirStreamLevelPacket.Value)
                    {
                        if (ParentProject?.GetStreamIdShortName(outStreamLevelPacket.Key) == "")
                            continue;

                        clipText += "> **" + outDirStreamLevelPacket.Key + "**";
                        if (useStreams)
                            clipText += " " + ParentProject!.GetStreamIdName(outStreamLevelPacket.Key);
                        clipText += ": *";
                        var s = string.Empty;
                        foreach (var outLevelPacket in outStreamLevelPacket.Value)
                        {
                            //if (useLevels)
                            //    clipText += " L" + outLevelPacket.Key;
                            foreach (var outList in outLevelPacket.Value)
                            {
                                if (s != string.Empty)
                                    s += ", ";

                                if (CBIncludePacketIds.Checked)
                                {
                                    s += outList.ToString("X3"); // don't include the 0x here .ToHex(3);
                                }

                                if (CBIncludePacketNames.Checked)
                                {
                                    var r = ParentProject?.InputParser?.Rules?.GetPacketRule(
                                        outDirStreamLevelPacket.Key, outStreamLevelPacket.Key, outLevelPacket.Key,
                                        outList);
                                    if (CBIncludePacketIds.Checked)
                                        s += " ";
                                    if ((r != null) && (!string.IsNullOrWhiteSpace(r.Name)))
                                        s += r.Name;
                                    else
                                    {
                                        if (CBIncludePacketIds.Checked)
                                            s += "?";
                                        else
                                            s += outList.ToString("X3");
                                    }
                                }
                            }
                        }

                        clipText += s + "*\n";
                    }
                }
            }

            ClipboardHelper.SetClipboard(clipText);
        }

        private void ProjectSettingsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.CopySummaryPacketIDs = CBIncludePacketIds.Checked;
            Properties.Settings.Default.CopySummaryPacketNames = CBIncludePacketNames.Checked;
            Properties.Settings.Default.CopySummaryNoAutoLoad = CBHideUrlPreviews.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
