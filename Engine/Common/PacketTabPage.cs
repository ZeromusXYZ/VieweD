using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms ;
using System.ComponentModel;
using System.Drawing;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class PacketTabPage: TabPage
    {
        private MainForm ownerMainForm;
        // ReSharper disable once InconsistentNaming
        public PacketList PLLoaded; // File Loaded
        // ReSharper disable once InconsistentNaming
        public PacketList PL; // Filtered File Data Displayed

        public uint CurrentSync;
        public string LoadedLogFile ;
        public string LoadedRulesFile;
        public VideoLinkForm VideoLink ;
        public string ProjectFolder;
        public string ProjectFile;
        public string ProjectTags;
        public string LinkVideoFileName;
        public string LinkYoutubeUrl;
        public string LinkPacketsDownloadUrl;
        public TimeSpan LinkVideoOffset;
        public string DecryptVersion = "_None_";

        public FlickerFreeListBox LbPackets;
        // Popup Menu Controls
        private ContextMenuStrip pmPl;
        private ToolStripMenuItem pmPlShowPacketName;
        private ToolStripSeparator pmPls1;
        private ToolStripMenuItem pmPlShowOnly;
        private ToolStripMenuItem pmPlHideThis;
        private ToolStripSeparator pmPls2;
        private ToolStripMenuItem pmPlShowOutOnly;
        private ToolStripMenuItem pmPlShowInOnly;
        private ToolStripSeparator pmPls3;
        private ToolStripMenuItem pmPlResetFilters;
        private ToolStripSeparator pmPls4;
        private ToolStripMenuItem pmPlEditParser;
        private ToolStripMenuItem pmPlExportPacket;
        // Engine Handler
        public EngineBase Engine;

        public PacketTabPage(MainForm mainForm)
        {
            ownerMainForm = mainForm;

            Engine = new EngineBase(this); // Default NULL Handler

            // Create base controls
            PLLoaded = new PacketList(this);
            PL = new PacketList(this);
            LbPackets = new FlickerFreeListBox();
            VideoLink = null;
            ProjectFolder = string.Empty;
            LinkVideoFileName = string.Empty;
            LinkYoutubeUrl = string.Empty;
            LinkPacketsDownloadUrl = string.Empty;
            ProjectFile = string.Empty;

            // Set ListBox Position
            LbPackets.Parent = this;
            LbPackets.Location = new System.Drawing.Point(0, 0);
            LbPackets.Size = new System.Drawing.Size(this.Width, this.Height);
            LbPackets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LbPackets.Dock = DockStyle.Fill;
            LbPackets.Font = Properties.Settings.Default.PacketListFont ; // Add fixed sized font (to override the tab page itself)
            LbPackets.ItemHeight = (int)Math.Ceiling(LbPackets.Font.GetHeight());
            //lbPackets.Font = new Font("Consolas", 9); // Add fixed sized font (to override the tab page itself)
            LbPackets.DrawMode = DrawMode.OwnerDrawFixed;

            LbPackets.DrawItem += lbPackets_DrawItem;
            // Add the SelectedIndexChanged for this from MainForm/process creating it, as it's too complex to handle internally
            // lbPackets.SelectedIndexChanged += new System.EventHandler(this.lbPackets_SelectedIndexChanged); 

            // Title to use on main program as "Filename"
            LoadedLogFile = "?Packets";
            ProjectTags = string.Empty;

            // Create Popup Menu
            pmPl = new ContextMenuStrip();
            pmPl.Opening += PmPL_Opening;

            LbPackets.ContextMenuStrip = pmPl;

            pmPlShowPacketName = new ToolStripMenuItem("Show packet name");
            pmPl.Items.Add(pmPlShowPacketName);

            pmPls1 = new ToolStripSeparator();
            pmPl.Items.Add(pmPls1);

            pmPlShowOnly = new ToolStripMenuItem("Show this type only");
            pmPlShowOnly.Click += PmPLShowOnly_Click;
            pmPl.Items.Add(pmPlShowOnly);

            pmPlHideThis = new ToolStripMenuItem("Hide this type");
            pmPlHideThis.Click += PmPLHideThis_Click;
            pmPl.Items.Add(pmPlHideThis);

            pmPls2 = new ToolStripSeparator();
            pmPl.Items.Add(pmPls2);

            pmPlShowOutOnly = new ToolStripMenuItem("Show only Outgoing");
            pmPlShowOutOnly.Click += PmPLShowOutgoingOnly_Click;
            pmPl.Items.Add(pmPlShowOutOnly);

            pmPlShowInOnly = new ToolStripMenuItem("Show only Incoming");
            pmPlShowInOnly.Click += PmPLShowIncomingOnly_Click;
            pmPl.Items.Add(pmPlShowInOnly);

            pmPls3 = new ToolStripSeparator();
            pmPl.Items.Add(pmPls3);

            pmPlResetFilters = new ToolStripMenuItem("Reset all filters");
            pmPlResetFilters.Click += PmPLResetFilter_Click;
            pmPl.Items.Add(pmPlResetFilters);

            pmPls4 = new ToolStripSeparator();
            pmPl.Items.Add(pmPls4);

            pmPlEditParser = new ToolStripMenuItem("Edit this parser");
            pmPlEditParser.Click += PmPLEditParser_Click;
            pmPl.Items.Add(pmPlEditParser);

            pmPlExportPacket = new ToolStripMenuItem("Export Packet");
            pmPlExportPacket.Click += PmPLExport_Click;
            pmPl.Items.Add(pmPlExportPacket);

            // Init misc stuff
            CurrentSync = 0xFFFFFFFF;
        }

        public PacketData GetSelectedPacket()
        {
            if ((LbPackets.SelectedIndex < 0) || (LbPackets.SelectedIndex >= PL.Count))
                return null;
            return PL.GetPacket(LbPackets.SelectedIndex);
        }

        private void lbPackets_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!(sender is ListBox lb))
                return;
            
            PacketData pd;
            if ((lb.Parent is PacketTabPage tp) && (e.Index >= 0) && (e.Index < tp.PL.Count))
            {
                pd = tp.PL.GetPacket(e.Index);
            }
            else
            {
                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                return;
            }

            var barOn = (tp.CurrentSync == pd.PacketSync);
            var isSelected = (e.Index == lb.SelectedIndex);
            Color textCol;
            Color backCol;
            Color barCol;

            // Determine the color of the brush to draw each item based 
            // on the index of the item to draw.
            var pltForBackGround = pd.PacketLogType;
            if (pd.MarkedAsInvalid)
                pltForBackGround = PacketLogTypes.Unknown;
            switch (pltForBackGround)
            {
                case PacketLogTypes.Incoming:
                    textCol = PacketColors.ColFontIN;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectIN;
                        textCol = PacketColors.ColSelectedFontIN;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncIN;
                    else
                        backCol = PacketColors.ColBackIN;
                    barCol = PacketColors.ColBarIN;
                    break;
                case PacketLogTypes.Outgoing:
                    textCol = PacketColors.ColFontOUT;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectOUT;
                        textCol = PacketColors.ColSelectedFontOUT;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncOUT;
                    else
                        backCol = PacketColors.ColBackOUT;
                    barCol = PacketColors.ColBarOUT;
                    break;
                default:
                    textCol = PacketColors.ColFontUNK;
                    if (isSelected)
                    {
                        backCol = PacketColors.ColSelectUNK;
                        textCol = PacketColors.ColSelectedFontUNK;
                    }
                    else
                    if (barOn)
                        backCol = PacketColors.ColSyncUNK;
                    else
                        backCol = PacketColors.ColBackUNK;
                    barCol = PacketColors.ColBarUNK;
                    break;
            }

            if (pd.MarkedAsDimmed)
            {
                Colorspace.RgbToHls(backCol, out _, out var l, out _);

                var isDark = l < 0.5 ;

                if (isDark)
                    backCol = Color.FromArgb(backCol.A, backCol.R / 2, backCol.G / 2, backCol.B / 2);
                else
                    backCol = Color.FromArgb(
                        backCol.A,
                        backCol.R > 128 ? 255 : backCol.R * 2,
                        backCol.G > 128 ? 255 : backCol.G * 2,
                        backCol.B > 128 ? 255 : backCol.B * 2);

                textCol = Color.FromArgb(textCol.A, (backCol.R + textCol.R) / 2, (backCol.G + textCol.G) / 2, (backCol.B + textCol.B) / 2);
                //barCol = Color.FromArgb(barCol.A, barCol.R / 4, barCol.G / 4, barCol.B / 4);
            }

            // Define the colors of our brushes.
            Brush textBrush = new SolidBrush(textCol);
            Brush backBrush = new SolidBrush(backCol);
            Brush barBrush = new SolidBrush(barCol);

            // Draw the background of the ListBox control for each item.
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            
            // header text
            var s = lb.Items[e.Index].ToString();
            //s = pd.VirtualTimeStamp.ToString() + "." + pd.VirtualTimeStamp.Millisecond.ToString("0000");

            var icon1 = new Rectangle(e.Bounds.Left, e.Bounds.Top + ((e.Bounds.Height - Properties.Resources.mini_unk_icon.Height) / 2), Properties.Resources.mini_unk_icon.Width, Properties.Resources.mini_unk_icon.Height);
            var icon2 = new Rectangle(icon1.Left + icon1.Width, icon1.Top, icon1.Width, icon1.Height);

            Rectangle textBounds;
            if ((tp.VideoLink != null) && (tp.VideoLink.IsInTimeRange(pd.VirtualTimeStamp)))
            {
                e.Graphics.DrawImage(Properties.Resources.mini_video_icon, icon2);
            }

            if ((tp.LinkVideoFileName != string.Empty) || (tp.LinkYoutubeUrl != string.Empty))
            { 
                textBounds = new Rectangle(e.Bounds.Left + (icon1.Width * 2), e.Bounds.Top, e.Bounds.Width - (icon1.Width * 2), e.Bounds.Height);
            }
            else
            {
                textBounds = new Rectangle(e.Bounds.Left + (icon1.Width), e.Bounds.Top, e.Bounds.Width - (icon1.Width), e.Bounds.Height);
            }

            switch (PacketColors.PacketListStyle)
            {
                case 1:
                    // Colored arrows
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_in_icon, icon1);
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_out_icon, icon1);
                    }
                    else
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_unk_icon, icon1);
                    }
                    break;
                case 2:
                    // transparent arrows
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_in_ticon, icon1);
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_out_ticon, icon1);
                    }
                    else
                    {
                        e.Graphics.DrawImage(Properties.Resources.mini_unk_ticon, icon1);
                    }
                    break;
                case 0:
                default:
                    textBounds = e.Bounds ;
                    // No icons, just text
                    if (pd.PacketLogType == PacketLogTypes.Incoming)
                    {
                        s = "<= " + s;
                    }
                    else
                    if (pd.PacketLogType == PacketLogTypes.Outgoing)
                    {
                        s = "=> " + s;
                    }
                    else
                    {
                        s = "?? " + s;
                    }
                    break;
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(s, e.Font, textBrush, textBounds, StringFormat.GenericDefault);

            if (barOn)
            {
                var barSize = 8;
                if (isSelected)
                    barSize = 16;
                e.Graphics.FillRectangle(barBrush, new Rectangle(e.Bounds.Right - barSize, e.Bounds.Top, barSize, e.Bounds.Height));
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        public void CenterListBox()
        {
            // Move to center
            var iHeight = LbPackets.ItemHeight;
            if (iHeight <= 0)
                iHeight = 8;
            var iCount = LbPackets.Size.Height / iHeight;
            var tPos = LbPackets.SelectedIndex - (iCount / 2);
            if (tPos < 0)
                tPos = 0;
            LbPackets.TopIndex = tPos;
        }

        public void FillListBox(uint gotToLastSync = 0)
        {
            var gotoIndex = -1;
            Application.UseWaitCursor = true;
            using (LoadingForm loadForm = new LoadingForm(ownerMainForm))
            {
                try
                {
                    var c = loadForm.BackColor;
                    Engine.GetLoadListBoxFlavor(out var t, ref c);
                    loadForm.Text = t ;
                    loadForm.BackColor = c;

                    loadForm.Show();
                    loadForm.pb.Minimum = 0;
                    loadForm.pb.Maximum = PL.Count;
                    LbPackets.BeginUpdate();
                    LbPackets.Items.Clear();
                    for (int i = 0; i < PL.Count; i++)
                    {
                        PacketData pd = PL.GetPacket(i);
                        LbPackets.Items.Add(pd.HeaderText);
                        if ((gotoIndex < 0) && (gotToLastSync > 0) && (pd.PacketSync >= gotToLastSync))
                        {
                            gotoIndex = LbPackets.Items.Count - 1 ;
                        }
                        loadForm.pb.Value = i;
                        if ((i % 50) == 0)
                        {
                            loadForm.pb.Refresh();
                            loadForm.BringToFront();
                            Application.DoEvents();
                        }
                    }
                    LbPackets.EndUpdate();
                    if (gotoIndex >= 0)
                    {
                        LbPackets.SelectedIndex = gotoIndex;
                    }
                    loadForm.Hide();
                }
                catch
                {
                    // ignored
                }
            }
            Application.UseWaitCursor = false;
        }


        private void PmPL_Opening(object sender, CancelEventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
            {
                e.Cancel = true;
                return;
            }
            if (pd.Parent._parentTab.Engine.HasRulesFile)
            {
                var r = pd.Parent.Rules.GetPacketRule(pd.PacketLogType, pd.StreamId, pd.PacketLevel, pd.PacketId);
                if (r != null)
                {
                    var lookupKey = r.LookupKey + (((ulong)pd.PacketLogType) * 0x0100000000);
                    pmPlShowPacketName.Text = lookupKey.ToString("X8");
                    pmPlEditParser.Tag = r ;
                    if (pd.PacketLogType != PacketLogTypes.Unknown)
                    {
                        pmPlEditParser.Text = @"Edit " + lookupKey.ToString("X8") + @" => " + r.Name;
                        pmPlEditParser.Visible = true;
                    }
                    else
                    {
                        pmPlEditParser.Text = @"Unknown direction, can't edit";
                        pmPlEditParser.Visible = false;
                    }
                }
                else
                {
                    pmPlShowPacketName.Text = @"No rule assigned";
                    pmPlEditParser.Tag = null;
                    pmPlEditParser.Text = @"Nothing to edit";
                    pmPlEditParser.Visible = false;
                }

            }
            else
            {
                pmPlShowPacketName.Text = pd.PacketLogType.ToString() + @" - 0x" + pd.PacketId.ToString("X3");
                string parserFileName;
                switch (pd.PacketLogType)
                {
                    case PacketLogTypes.Incoming:
                        parserFileName = Path.Combine("data", pd.Parent._parentTab.Engine.EngineId, "parse", "in-0x" + pd.PacketId.ToString("X3") + ".txt");
                        pmPlEditParser.Text = @"Edit " + parserFileName;
                        pmPlEditParser.Visible = true;
                        break;
                    case PacketLogTypes.Outgoing:
                        parserFileName = Path.Combine("data", pd.Parent._parentTab.Engine.EngineId, "parse", "out-0x" + pd.PacketId.ToString("X3") + ".txt");
                        pmPlEditParser.Text = @"Edit " + parserFileName;
                        pmPlEditParser.Visible = true;
                        break;
                    default:
                        parserFileName = "";
                        pmPlEditParser.Text = "";
                        pmPlEditParser.Visible = false;
                        break;
                }
                pmPlEditParser.Tag = parserFileName;
            }
            pmPlShowOnly.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);
            pmPlHideThis.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);

        }

        private void PmPLEditParser_Click(object sender, EventArgs e)
        {
            if (pmPlEditParser.Tag == null)
                return;
            if (pmPlEditParser.Tag is string fName)
                ownerMainForm.OpenBasicParseEditor(fName);
            if (pmPlEditParser.Tag is PacketRule pr)
                ownerMainForm.OpenXmlRulesParseEditor(pr);
        }

        private void PmPLShowOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            // ulong packetKey = (ulong)(pd.PacketID + (pd.StreamId * 0x01000000));
            ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            switch (pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    PL.Filter.Clear();
                    PL.Filter.FilterInType = FilterType.ShowPackets;
                    PL.Filter.FilterInList.Add(packetKey);
                    PL.Filter.FilterOutType = FilterType.AllowNone;
                    break;
                case PacketLogTypes.Outgoing:
                    PL.Filter.Clear();
                    PL.Filter.FilterOutType = FilterType.ShowPackets;
                    PL.Filter.FilterOutList.Add(packetKey);
                    PL.Filter.FilterInType = FilterType.AllowNone;
                    break;
                default:
                    return;
            }
            var lastSync = CurrentSync;
            if (hasShift)
                PL.HightlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLHideThis_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            //ulong packetKey = (ulong)(pd.PacketID + (pd.StreamId * 0x01000000));
            ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            switch (pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    if (PL.Filter.FilterInType != FilterType.HidePackets)
                    {
                        PL.Filter.FilterInType = FilterType.HidePackets;
                        PL.Filter.FilterInList.Clear();
                    }

                    PL.Filter.FilterInList.Add(packetKey);
                    break;
                case PacketLogTypes.Outgoing:
                    if (PL.Filter.FilterOutType != FilterType.HidePackets)
                    {
                        PL.Filter.FilterOutType = FilterType.HidePackets;
                        PL.Filter.FilterOutList.Clear();
                    }
                    PL.Filter.FilterOutList.Add(packetKey);
                    break;
                default:
                    return;
            }
            var lastSync = CurrentSync;
            if (hasShift)
                PL.HightlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowIncomingOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterInType == FilterType.AllowNone) || (PL.Filter.FilterInType == FilterType.HidePackets))
            {
                PL.Filter.FilterInType = FilterType.Off;
            }
            PL.Filter.FilterOutType = FilterType.AllowNone;

            var lastSync = CurrentSync;
            if (hasShift)
                PL.HightlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowOutgoingOnly_Click(object sender, EventArgs e)
        {
            var hasShift = Control.ModifierKeys.HasFlag(Keys.Shift);
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterOutType == FilterType.AllowNone) || (PL.Filter.FilterOutType == FilterType.HidePackets))
            {
                PL.Filter.FilterOutType = FilterType.Off;
            }
            PL.Filter.FilterInType = FilterType.AllowNone;

            var lastSync = CurrentSync;
            if (hasShift)
                PL.HightlightFilterFrom(PLLoaded);
            else
                PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLResetFilter_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            PL.Filter.Clear();
            var lastSync = CurrentSync;
            PL.CopyFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLExport_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            string exportName = "";
            switch(pd.PacketLogType)
            {
                case PacketLogTypes.Incoming:
                    exportName += "i";
                    break;
                case PacketLogTypes.Outgoing:
                    exportName += "o";
                    break;
                default:
                    exportName += "u";
                    break;
            }
            exportName += pd.PacketLevel.ToString() + "-" + pd.PacketId.ToString("X4");

            using (var saveDlg = new SaveFileDialog())
            {
                saveDlg.FileName = exportName;
                saveDlg.CheckPathExists = true;
                if (saveDlg.ShowDialog() != DialogResult.OK)
                    return;
                exportName = saveDlg.FileName;
            }

            try
            {
                File.WriteAllBytes(exportName,pd.RawBytes.ToArray());
            }
            catch (Exception x)
            {
                MessageBox.Show(@"Error saving raw packet " + exportName + "\r\nException: " + x.Message);
            }

        }

        public void ClearProjectFile()
        {
            LoadedLogFile = string.Empty;
            LinkVideoFileName = string.Empty;
            LinkVideoOffset = TimeSpan.Zero;
            LinkYoutubeUrl = string.Empty;
            LinkPacketsDownloadUrl = string.Empty;
            LoadedRulesFile = string.Empty;
        }

        public bool LoadProjectFile(Stream projectStream)
        {
            ClearProjectFile();
            try
            {
                var sl = new List<string>();
                using (var ps = new StreamReader(projectStream))
                {
                    while(!ps.EndOfStream)
                    {
                        sl.Add(ps.ReadLine());
                    }
                }
                    

                foreach (string s in sl)
                {
                    var fields = s.Split(';');
                    if (fields.Length < 2)
                        continue;
                    var fieldType = fields[0].ToLower();
                    var fieldVal = fields[1];
                    if (fieldType == "packetlog")
                    {
                        LoadedLogFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "rules")
                    {
                        LoadedRulesFile = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "video")
                    {
                        LinkVideoFileName = Helper.TryMakeFullPath(ProjectFolder, fieldVal);
                    }
                    else
                    if (fieldType == "youtube")
                    {
                        LinkYoutubeUrl = fieldVal;
                    }
                    else
                    if (fieldType == "offset")
                    {
                        if (DataLookups.TryFieldParse(fieldVal, out int n))
                            LinkVideoOffset = TimeSpan.FromMilliseconds(n);
                    }
                    else
                    if (fieldType == "packedsource")
                    {
                        LinkPacketsDownloadUrl = fieldVal;
                    }
                    else
                    if (fieldType == "tags")
                    {
                        ProjectTags = fieldVal;
                    }
                    else
                    if (fieldType == "decrypt")
                    {
                        DecryptVersion = fieldVal;
                    }
                    /*
                    else
                    if (fieldType == "pin")
                    {
                        // not used for reading inside this program
                    }
                    else
                    if (fieldType == "pout")
                    {
                        // not used for reading inside this program
                    }
                    else
                    {
                        continue;
                    }
                    */

                }

            }
            catch
            {
                ClearProjectFile();
                return false;
            }
            return true;
        }

        public bool LoadProjectFile(string aProjectFile)
        {
            ClearProjectFile();
            ProjectFolder = Path.GetDirectoryName(aProjectFile)?.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var res = false;
            try
            {
                if (File.Exists(aProjectFile))
                {
                    using (var projectStream = new FileStream(aProjectFile, FileMode.Open, FileAccess.Read))
                    {
                        res = LoadProjectFile(projectStream);
                    }
                }
            }
            catch
            {
                ClearProjectFile();
                res = false;
            }
            if (res)
                ProjectFile = aProjectFile;
            return res ;
        }

        public bool LoadProjectFileFromLogFile(string fromLogFile)
        {
            if (!string.IsNullOrEmpty(fromLogFile))
                ProjectFolder = Helper.MakeProjectDirectoryFromLogFileName(fromLogFile);

            var aProjectFile = ProjectFolder + Path.GetFileName(ProjectFolder.TrimEnd('\\')) + ".pvd";

            return LoadProjectFile(aProjectFile);
        }

        public bool SaveProjectFile()
        {
            if ((ProjectFolder == null) || (ProjectFolder == string.Empty))
                return false;

            // Generate Filename if needed
            if ((ProjectFile == null) || (ProjectFile == string.Empty))
            {
                var partialProjectFileName = Path.GetFileName(ProjectFolder.TrimEnd('\\'));
                // don't create in a drive root directory
                if (partialProjectFileName == string.Empty)
                    return false;
                ProjectFile = ProjectFolder + partialProjectFileName + ".pvd";
            }

            if ( (Properties.Settings.Default.AskCreateNewProjectFile) && (!File.Exists(ProjectFile)) )
            {
                if (MessageBox.Show("Do you want to save project settings as a new project file ?\r\n" + ProjectFile, @"Create Project File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    ProjectFile = string.Empty;
                    return false;
                }
            }


            string relVideo = string.Empty;
            if (!string.IsNullOrEmpty(LinkVideoFileName))
                relVideo = Helper.MakeRelative(ProjectFolder, LinkVideoFileName);

            string relLogFile = string.Empty;
            if (!string.IsNullOrEmpty(LoadedLogFile))
                relLogFile = Helper.MakeRelative(ProjectFolder, LoadedLogFile);

            string relRuleFile = string.Empty;
            if (!string.IsNullOrEmpty(LoadedRulesFile))
                relRuleFile = Helper.MakeRelative(ProjectFolder, LoadedRulesFile);

            try
            {
                string pin = string.Empty;
                foreach(UInt16 n in PLLoaded.ContainsPacketsIn)
                {
                    if (pin != string.Empty)
                        pin += ",";
                    pin += n.ToString("X3");
                }
                string pout = string.Empty;
                foreach (UInt16 n in PLLoaded.ContainsPacketsOut)
                {
                    if (pout != string.Empty)
                        pout += ",";
                    pout += n.ToString("X3");
                }

                var sl = new List<string>();
                sl.Add("rem;VieweD Project File");
                sl.Add("packetlog;" + relLogFile);
                sl.Add("rules;" + relRuleFile);
                sl.Add("decrypt;" + DecryptVersion);
                sl.Add("video;" + relVideo);
                sl.Add("tags;" + ProjectTags);
                sl.Add("packedsource;" + LinkPacketsDownloadUrl);
                sl.Add("youtube;" + LinkYoutubeUrl);
                sl.Add("offset;" + LinkVideoOffset.TotalMilliseconds.ToString());
                sl.Add("pin;" + pin);
                sl.Add("pout;" + pout);
                File.WriteAllLines(ProjectFile, sl);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
