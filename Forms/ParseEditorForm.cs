using System;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using VieweD.Engine.Common;
using System.Drawing;
using VieweD.Helpers.System;

namespace VieweD
{
    public partial class ParseEditorForm : Form
    {
        public string LoadedFile = "";
        public PacketRule LoadedRule;
        public string OldRuleXml = "";
        public PacketTabPage CurrentTab;
        private bool TestOk;

        public ParseEditorForm(PacketTabPage parent)
        {
            CurrentTab = parent;
            InitializeComponent();
            var syntaxFile = Path.Combine(Application.StartupPath, "data", CurrentTab?.Engine?.EngineId ?? "null", "editor-syntax.xml"); 
            if (File.Exists(syntaxFile))
            {
                try
                {
                    editBox.DescriptionFile = syntaxFile;
                    editBox.Language = FastColoredTextBoxNS.Language.Custom;
                }
                catch (Exception ex)
                {
                    editBox.DescriptionFile = "";
                    editBox.Language = FastColoredTextBoxNS.Language.XML;
                    MessageBox.Show($"Error in {syntaxFile}\r\n{ex.Message}", "Syntax File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else 
            {
                editBox.DescriptionFile = "";
                editBox.Language = FastColoredTextBoxNS.Language.XML;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if ((LoadedFile == string.Empty) && (LoadedRule == null))
            {
                MessageBox.Show("Not supported, nothing loaded ?");
                return;
            }
            BtnTest_Click(null, null);

            if (TestOk == false)
            {
                MessageBox.Show("Cannot save while there are syntax errors!");
                return;
            }

            if (LoadedRule == null)
            {
                File.WriteAllText(LoadedFile, editBox.Text);
            }
            else
            {
                // LoadedRule.RootNode.InnerXml = OldRuleXml;
                LoadedRule.Build();

                PacketTabPage tp = MainForm.ThisMainForm.GetCurrentPacketTabPage();
                if ((tp != null) && (tp.PLLoaded.Rules != null))
                {
                    tp.PLLoaded.Rules.SaveRulesFile(tp.LoadedRulesFile);
                }

                LoadedRule = null;
            }

            Dispose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (LoadedRule != null)
            {
                LoadedRule.RootNode.InnerXml = OldRuleXml;
                LoadedRule.Build();
                LoadedRule = null;
            }
            Dispose();
        }

        public void LoadFromFile(string filename)
        {
            editBox.Text = File.ReadAllText(filename);
            LoadedFile = filename;
            Text = LoadedFile;
            editBox.SelectionLength = 0;
            editBox.SelectionStart = editBox.Text.Length;
            btnRename.Visible = false;
            FillTypes();
        }

        public void LoadFromRule(PacketRule rule)
        {
            LoadedRule = rule;
            OldRuleXml = LoadedRule.RootNode.InnerXml;
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
            Text = Text = PacketFilterListEntry.AsString(LoadedRule.PacketId, LoadedRule.Level, LoadedRule.StreamId) + " - " + LoadedRule.Name;
            editBox.SelectionLength = 0;
            editBox.SelectionStart = editBox.Text.Length;
            btnRename.Visible = true;
            FillTypes();
        }

        private void FillTypes()
        {
            var dataTypes = CurrentTab?.Engine?.EditorDataTypes;
            cbFieldType.Items.Clear();
            var defaultType = string.Empty;
            if ((dataTypes != null) && (dataTypes.Count > 0))
            {
                foreach (var dType in dataTypes)
                {
                    if (string.IsNullOrWhiteSpace(defaultType))
                        defaultType = dType;
                    cbFieldType.Items.Add(dType);
                }
            }
            cbFieldType.Sorted = true;

            if (!string.IsNullOrWhiteSpace(defaultType))
                cbFieldType.Text = defaultType;

            if ((dataTypes != null) && (CurrentTab.Engine.HasRulesFile))
            {
                lPos.Visible = false;
                lPosInfo.Visible = false;
                tPos.Visible = false;

                cbFieldType.Width = tName.Left - cbFieldType.Left - 8;

                // Just hide the entire old control set for now
                pOldEditControl.Visible = false;
                btnInsert.Visible = false;
                editBox.Height += pOldEditControl.Height;
                docMap.Height += pOldEditControl.Height;

                tComment.Enabled = false;
            }
            else
            {
                lPos.Visible = true;
                lPosInfo.Visible = true;
                tPos.Visible = true;

                cbFieldType.Width = cbLookup.Width;

                tComment.Enabled = true;
            }
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
            TestOk = false;
            var tp = MainForm.ThisMainForm.GetCurrentPacketTabPage();
            if (tp == null)
            {
                return;
            }

            var pd = tp.GetSelectedPacket();
            if (pd == null)
            {
                return;
            }

            var dontReloadParser = true;
            if (pd.Parent.ParentTab.Engine.HasRulesFile)
            {
                // var currentPP = MainForm.ThisMainForm.CurrentPP;
                try
                {
                    LoadedRule.RootNode.InnerXml = editBox.Text;
                    LoadedRule.Build();
                    dontReloadParser = false; // need a reload when adding a new one
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message, "Error in rule", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadedRule.RootNode.InnerXml = OldRuleXml;
                    LoadedRule.Build();
                    return;
                }
                //MainForm.thisMainForm.CurrentPP.ParsedView.Clear();
                //MainForm.thisMainForm.UpdatePacketDetails(tp, pd, "-", true);
            }
            else
            {
                var currentPP = MainForm.ThisMainForm.CurrentPP;
                currentPP.RawParseData.Clear();
                currentPP.RawParseData.AddRange(editBox.Lines);
            }
            MainForm.ThisMainForm.CurrentPP.ParsedView.Clear();
            MainForm.ThisMainForm.UpdatePacketDetails(tp, pd, "-", dontReloadParser);
            TestOk = true;
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
                LoadedRule.RootNode.InnerXml = OldRuleXml;
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

        private static DialogResult ShowInputDialogBox(ref string input, string prompt, string title = "Title", int width = 300, int height = 200, Form parentForm = null)
        {
            //This function creates the custom input dialog box by individually creating the different window elements and adding them to the dialog box

            //Specify the size of the window using the parameters passed
            Size size = new Size(width, height);
            //Create a new form using a System.Windows Form
            var inputBox = new Form();
            if (parentForm != null)
                inputBox.Location = new Point(
                    parentForm.Left + (parentForm.Width / 2) - 175,
                    parentForm.Top + (parentForm.Height / 2) - 50);

            inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            //Set the window title using the parameter passed
            inputBox.Text = title;

            //Create a new label to hold the prompt
            var label = new Label();
            label.Text = prompt;
            label.Location = new Point(5, 5);
            label.Width = size.Width - 10;
            inputBox.Controls.Add(label);

            //Create a textbox to accept the user's input
            var textBox = new TextBox();
            textBox.Size = new Size(size.Width - 10, 23);
            textBox.Location = new Point(5, label.Location.Y + 20);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            //Create an OK Button 
            var okButton = new Button();
            okButton.DialogResult = DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new Point(size.Width - 80 - 80, size.Height - 30);
            inputBox.Controls.Add(okButton);

            //Create a Cancel Button
            var cancelButton = new Button();
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new Point(size.Width - 80, size.Height - 30);
            inputBox.Controls.Add(cancelButton);

            //Set the input box's buttons to the created OK and Cancel Buttons respectively so the window appropriately behaves with the button clicks
            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            //Show the window dialog box 
            var result = inputBox.ShowDialog();
            input = textBox.Text;

            //After input has been submitted, return the input value
            return result;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (LoadedRule == null)
            {
                MessageBox.Show("Rename packet is only available for Rules files");
                return;
            }

            var s = LoadedRule.Name;
            if (ShowInputDialogBox(ref s, "Name:", "Renamed Packet", 350, 100) == DialogResult.OK)
            {
                if (LoadedRule.Name != s)
                {
                    LoadedRule.Name = s;
                    XmlHelper.SetAttribute(LoadedRule.RootNode, "desc", s);
                    Text = PacketFilterListEntry.AsString(LoadedRule.PacketId, LoadedRule.Level, LoadedRule.StreamId) + " - " + LoadedRule.Name;
                    MessageBox.Show("Note that packet names in the list are not updates until the file is reloaded!","Rename Packet", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }
    }
}
