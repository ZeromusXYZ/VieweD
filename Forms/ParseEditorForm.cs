using System;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using VieweD.Engine.Common;

namespace VieweD
{
    public partial class ParseEditorForm : Form
    {
        public string LoadedFile = "";
        public PacketRule LoadedRule;
        public string OldRuleXml = "";
        public PacketTabPage CurrentTab;

        public ParseEditorForm(PacketTabPage parent)
        {
            CurrentTab = parent;
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if ((LoadedFile == string.Empty) && (LoadedRule == null))
            {
                MessageBox.Show("Not supported, nothing loaded ?");
                return;
            }
            BtnTest_Click(null, null);
            if (LoadedRule == null)
            {
                File.WriteAllText(LoadedFile, editBox.Text);
            }
            else
            {
                // LoadedRule._rootNode.InnerXml = OldRuleXml;
                LoadedRule.Build();

                PacketTabPage tp = MainForm.ThisMainForm.GetCurrentPacketTabPage();
                if ((tp != null) && (tp.PLLoaded.Rules != null))
                    tp.PLLoaded.Rules.SaveRulesFile(tp.LoadedRulesFile);

                LoadedRule = null;
            }

            Dispose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (LoadedRule != null)
            {
                LoadedRule._rootNode.InnerXml = OldRuleXml;
                LoadedRule.Build();
                LoadedRule = null;
            }
            Dispose();
        }

        public void LoadFromFile(string filename)
        {
            editBox.Text = File.ReadAllText(filename);
            LoadedFile = filename;
            Text += " - " + LoadedFile;
            editBox.SelectionLength = 0;
            editBox.SelectionStart = editBox.Text.Length;
            FillFFXITypes();
        }

        public void LoadFromRule(PacketRule rule)
        {
            LoadedRule = rule;
            OldRuleXml = LoadedRule._rootNode.InnerXml;
            var sList = OldRuleXml.Replace("><", ">\n<").Split('\n').ToList();
            var indentCount = 0 ;
            string s = string.Empty;
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
            editBox.Text = s;
            LoadedFile = string.Empty;
            Text += " - " + LoadedRule.Name;
            editBox.SelectionLength = 0;
            editBox.SelectionStart = editBox.Text.Length;
            FillAATypes();
        }

        private void FillFFXITypes()
        {
            cbFieldType.Items.Clear();

            cbFieldType.Items.Add("uint32");
            cbFieldType.Items.Add("int32");
            cbFieldType.Items.Add("uint16");
            cbFieldType.Items.Add("int16");
            cbFieldType.Items.Add("byte");
            cbFieldType.Items.Add("float");
            cbFieldType.Items.Add("pos");
            cbFieldType.Items.Add("dir");
            cbFieldType.Items.Add("switchblock");
            cbFieldType.Items.Add("showblock");
            cbFieldType.Items.Add("info");
            cbFieldType.Items.Add("bit");
            cbFieldType.Items.Add("bits");
            cbFieldType.Items.Add("string");
            cbFieldType.Items.Add("data");
            cbFieldType.Items.Add("ms");
            cbFieldType.Items.Add("frames");
            cbFieldType.Items.Add("vanatime");
            cbFieldType.Items.Add("ip4");
            cbFieldType.Items.Add("linkshellstring");
            cbFieldType.Items.Add("inscribestring");
            cbFieldType.Items.Add("bitflaglist");
            cbFieldType.Items.Add("bitflaglist2");
            cbFieldType.Items.Add("combatskill");
            cbFieldType.Items.Add("craftskill");
            cbFieldType.Items.Add("equipsetitem");
            cbFieldType.Items.Add("equipsetitemlist");
            cbFieldType.Items.Add("abilityrecastlist");
            cbFieldType.Items.Add("blacklistentry");
            cbFieldType.Items.Add("meritentries");
            cbFieldType.Items.Add("playercheckitems");
            cbFieldType.Items.Add("bufficons");
            cbFieldType.Items.Add("bufftimers");
            cbFieldType.Items.Add("buffs");
            cbFieldType.Items.Add("jobpointentries");
            cbFieldType.Items.Add("shopitems");
            cbFieldType.Items.Add("guildshopitems");
            cbFieldType.Items.Add("jobpoints");
            cbFieldType.Items.Add("roequest");
            cbFieldType.Items.Add("packet-in-0x028");

            cbFieldType.Sorted = true;
            cbFieldType.Text = "uint16";

            lPos.Visible = true;
            lPosInfo.Visible = true;
            tPos.Visible = true;

            cbFieldType.Width = cbLookup.Width;

            tComment.Enabled = true;
        }

        private void FillAATypes()
        {
            cbFieldType.Items.Clear();
            cbFieldType.Items.Add("chunk type=\"pi\"");
            cbFieldType.Items.Add("chunk type=\"pish\"");
            cbFieldType.Items.Add("chunk type=\"b\"");
            cbFieldType.Items.Add("chunk type=\"w\"");
            cbFieldType.Items.Add("chunk type=\"rw\"");
            cbFieldType.Items.Add("chunk type=\"h\"");
            cbFieldType.Items.Add("chunk type=\"rh\"");
            cbFieldType.Items.Add("chunk type=\"d\"");
            cbFieldType.Items.Add("chunk type=\"rd\"");
            cbFieldType.Items.Add("chunk type=\"i\"");
            cbFieldType.Items.Add("chunk type=\"ri\"");
            cbFieldType.Items.Add("chunk type=\"f\"");
            cbFieldType.Items.Add("chunk type=\"rf\"");
            cbFieldType.Items.Add("chunk type=\"half\"");
            cbFieldType.Items.Add("chunk type=\"q\"");
            cbFieldType.Items.Add("chunk type=\"rq\"");
            cbFieldType.Items.Add("chunk type=\"int64\""); // not used
            cbFieldType.Items.Add("chunk type=\"a\"");
            cbFieldType.Items.Add("chunk type=\"ts\"");
            cbFieldType.Items.Add("chunk type=\"rts\"");
            cbFieldType.Items.Add("chunk type=\"s\"");
            cbFieldType.Items.Add("chunk type=\"u\"");
            cbFieldType.Items.Add("chunk type=\"t\"");
            cbFieldType.Items.Add("chunk type=\"zs\"");
            cbFieldType.Items.Add("chunk type=\"zu\"");
            cbFieldType.Items.Add("chunk type=\"zu8\"");
            cbFieldType.Items.Add("chunk type=\"bc\"");
            cbFieldType.Items.Add("chunk type=\"bcx\"");
            cbFieldType.Items.Add("chunk type=\"bcy\"");
            cbFieldType.Items.Add("chunk type=\"bcz\"");
            cbFieldType.Items.Add("chunk type=\"qx\"");
            cbFieldType.Items.Add("chunk type=\"qy\"");
            cbFieldType.Items.Add("chunk type=\"qz\"");
            cbFieldType.Items.Add("chunk type=\"rqx\"");
            cbFieldType.Items.Add("chunk type=\"rqy\"");
            cbFieldType.Items.Add("chunk type=\"rqz\"");

            cbFieldType.Items.Add("ifeq arg1=\"1\" arg2=\"2\"");
            cbFieldType.Items.Add("ifneq arg1=\"1\" arg2=\"2\"");
            cbFieldType.Items.Add("iflt arg1=\"1\" arg2=\"2\"");
            cbFieldType.Items.Add("ifgt arg1=\"1\" arg2=\"2\"");
            cbFieldType.Items.Add("ifz arg=\"1\"");
            cbFieldType.Items.Add("ifnz arg=\"1\"");
            cbFieldType.Items.Add("add arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("sub arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("mul arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("shl arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("shr arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("and arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("or arg1=\"1\" arg2=\"2\" dst=\"dst\"");
            cbFieldType.Items.Add("mov val=\"1\" dst=\"dst\"");
            cbFieldType.Items.Add("loop");
            cbFieldType.Items.Add("break");
            cbFieldType.Items.Add("continue");
            cbFieldType.Items.Add("lookup save=\"custom\" source=\"s\" val=\"v\"");

            cbFieldType.Sorted = true;
            cbFieldType.Text = "chunk type=\"w\"";

            lPos.Visible = false;
            lPosInfo.Visible = false;
            tPos.Visible = false;

            cbFieldType.Width = tName.Left - cbFieldType.Left - 8 ;

            // Just hide the entire old control set for now
            pOldEditControl.Visible = false;
            btnInsert.Visible = false;
            editBox.Height += pOldEditControl.Height;
            docMap.Height += pOldEditControl.Height;

            tComment.Enabled = false;
        }


        private void ParseEditorForm_Load(object sender, EventArgs e)
        {
            cbLookup.Items.Clear();
            cbLookup.Items.Add("");
            foreach (var item in CurrentTab.Engine.DataLookups.LookupLists)
            {
                cbLookup.Items.Add(item.Key);
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            PacketTabPage tp = MainForm.ThisMainForm.GetCurrentPacketTabPage();
            if (tp == null)
            {
                return;
            }

            PacketData pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                return;
            }

            if (pd.Parent._parentTab.Engine.HasRulesFile)
            {
                var aa = MainForm.ThisMainForm.CurrentPP;
                try
                {
                    LoadedRule._rootNode.InnerXml = editBox.Text;
                    LoadedRule.Build();
                }
                catch (Exception x)
                {
                    MessageBox.Show("Exception: " + x.Message, "Error in rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadedRule._rootNode.InnerXml = OldRuleXml;
                    LoadedRule.Build();
                    return;
                }
                //MainForm.thisMainForm.CurrentPP.ParsedView.Clear();
                //MainForm.thisMainForm.UpdatePacketDetails(tp, pd, "-", true);
            }
            else
            {
                var ffxi = MainForm.ThisMainForm.CurrentPP;
                ffxi.RawParseData.Clear();
                ffxi.RawParseData.AddRange(editBox.Lines);
            }
            MainForm.ThisMainForm.CurrentPP.ParsedView.Clear();
            MainForm.ThisMainForm.UpdatePacketDetails(tp, pd, "-", true);
        }


        public ToolStripMenuItem AddMenuItem(ToolStripItemCollection tsic, string title, string command, string DefaultNameField = "")
        {
            if ((title == "-") || (title == ""))
            {
                var si = new ToolStripSeparator();
                tsic.Add(si);
                return null;
            }

            var ni = new ToolStripMenuItem();
            var c = command;
            var nameField = tName.Text;
            if (nameField.ToLower() == "myfield")
                nameField = DefaultNameField;

            c = CurrentTab?.Engine?.EditorReplaceString(c, tPos.Text, nameField, cbLookup.Text, tComment.Text);

            ni.Text = title;
            ni.Tag = c;
            ni.Click += miCustomInsert_Click;
            tsic.Add(ni);
            return ni;
        }

        private void BuildInsertMenu()
        {
            if (editBox.SelectedText.Length > 0)
                miInsert.Text = "Replace";
            else
                miInsert.Text = "Insert";
            miInsert.DropDownItems.Clear();

            CurrentTab?.Engine?.BuildEditorPopupMenu(miInsert, this);
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            //pmEdit.Show(new Point(btnInsert.Location.X + this.Location.X + btnInsert.Width, btnInsert.Location.Y + this.Location.Y));
            //return;

            string s = "";
            if (LoadedRule == null)
            {
                s += cbFieldType.Text;
                if (cbLookup.Text != string.Empty)
                    s += ":" + cbLookup.Text;
                s += ";" + tPos.Text;
                s += ";" + tName.Text;
                if (tComment.Text != string.Empty)
                    s += ";" + tComment.Text;
                s += "\r\n";
            }
            else
            {
                var fieldSplit = cbFieldType.Text.ToLower().Split(' ');
                if ((fieldSplit.Length > 1) && (fieldSplit[0] == "chunk"))
                {
                    s += "<" + cbFieldType.Text + " name=\"" + tName.Text + "\" ";
                    if (cbLookup.Text != string.Empty)
                        s += "lookup=\""+cbLookup.Text+"\" ";
                    s += "/>";
                }
                else
                {
                    s += "<" + cbFieldType.Text + " />";
                }

            }

            var p = editBox.SelectionStart;
            editBox.SelectedText = s;
            editBox.SelectionStart = p;
            editBox.SelectionLength = s.Length;
        }

        private void ParseEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (LoadedRule != null)
            {
                LoadedRule._rootNode.InnerXml = OldRuleXml;
                LoadedRule.Build();
                LoadedRule = null;
            }
        }

        private void pmEdit_Opening(object sender, CancelEventArgs e)
        {
            BuildInsertMenu();
        }

        private void miCustomInsert_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem tsmi)
            {
                if (tsmi.Tag != null)
                {
                    var s = tsmi.Tag.ToString();
                    var p = editBox.SelectionStart;
                    editBox.SelectedText = s;
                    editBox.SelectionStart = p;
                    editBox.SelectionLength = s.Length;
                    // MessageBox.Show(tsmi.Tag.ToString());
                }
            }

        }
    }
}
