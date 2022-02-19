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
        public PacketList PLLoaded; // File Loaded
        public PacketList PL; // Filtered File Data Displayed
        // public PacketParser PP;
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

        public FlickerFreeListBox lbPackets;
        // Popup Menu Controls
        public ContextMenuStrip pmPL;
        public ToolStripMenuItem pmPLShowPacketName;
        public ToolStripSeparator pmPLS1;
        public ToolStripMenuItem pmPLShowOnly;
        public ToolStripMenuItem pmPLHideThis;
        public ToolStripSeparator pmPLS2;
        public ToolStripMenuItem pmPLShowOutOnly;
        public ToolStripMenuItem pmPLShowInOnly;
        public ToolStripSeparator pmPLS3;
        public ToolStripMenuItem pmPLResetFilters;
        public ToolStripSeparator pmPLS4;
        public ToolStripMenuItem pmPLEditParser;
        public ToolStripMenuItem pmPLExportPacket;
        // Engine Handler
        public EngineBase Engine;

        public PacketTabPage(MainForm mainForm)
        {
            ownerMainForm = mainForm;

            Engine = new EngineBase(this); // Default NULL Handler

            // Create base controls
            PLLoaded = new PacketList(this);
            PL = new PacketList(this);
            lbPackets = new FlickerFreeListBox();
            VideoLink = null;
            ProjectFolder = string.Empty;
            LinkVideoFileName = string.Empty;
            LinkYoutubeUrl = string.Empty;
            LinkPacketsDownloadUrl = string.Empty;
            ProjectFile = string.Empty;

            // Set ListBox Position
            lbPackets.Parent = this;
            lbPackets.Location = new System.Drawing.Point(0, 0);
            lbPackets.Size = new System.Drawing.Size(this.Width, this.Height);
            lbPackets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbPackets.Font = new Font("Consolas", 9); // Add fixedsized font (to override the tab page itself)
            lbPackets.DrawMode = DrawMode.OwnerDrawFixed;
            lbPackets.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbPackets_DrawItem);
            // Add the SelectedIndexChanged for this from MainForm/process creating it, as it's too complex to handle internally
            // lbPackets.SelectedIndexChanged += new System.EventHandler(this.lbPackets_SelectedIndexChanged); 

            // Title to use on main program as "Filename"
            LoadedLogFile = "?Packets";
            ProjectTags = string.Empty;

            // Create Popup Menu
            pmPL = new ContextMenuStrip();
            pmPL.Opening += new CancelEventHandler(PmPL_Opening);

            lbPackets.ContextMenuStrip = pmPL;

            pmPLShowPacketName = new ToolStripMenuItem("Show packet name");
            pmPL.Items.Add(pmPLShowPacketName);

            pmPLS1 = new ToolStripSeparator();
            pmPL.Items.Add(pmPLS1);

            pmPLShowOnly = new ToolStripMenuItem("Show this type only");
            pmPLShowOnly.Click += new EventHandler(PmPLShowOnly_Click);
            pmPL.Items.Add(pmPLShowOnly);

            pmPLHideThis = new ToolStripMenuItem("Hide this type");
            pmPLHideThis.Click += new EventHandler(PmPLHideThis_Click);
            pmPL.Items.Add(pmPLHideThis);

            pmPLS2 = new ToolStripSeparator();
            pmPL.Items.Add(pmPLS2);

            pmPLShowOutOnly = new ToolStripMenuItem("Show only Outgoing");
            pmPLShowOutOnly.Click += new EventHandler(PmPLShowOutgoingOnly_Click);
            pmPL.Items.Add(pmPLShowOutOnly);

            pmPLShowInOnly = new ToolStripMenuItem("Show only Incoming");
            pmPLShowInOnly.Click += new EventHandler(PmPLShowIncomingOnly_Click);
            pmPL.Items.Add(pmPLShowInOnly);

            pmPLS3 = new ToolStripSeparator();
            pmPL.Items.Add(pmPLS3);

            pmPLResetFilters = new ToolStripMenuItem("Reset all filters");
            pmPLResetFilters.Click += new EventHandler(PmPLResetFilter_Click);
            pmPL.Items.Add(pmPLResetFilters);

            pmPLS4 = new ToolStripSeparator();
            pmPL.Items.Add(pmPLS4);

            pmPLEditParser = new ToolStripMenuItem("Edit this parser");
            pmPLEditParser.Click += new EventHandler(PmPLEditParser_Click);
            pmPL.Items.Add(pmPLEditParser);

            pmPLExportPacket = new ToolStripMenuItem("Export Packet");
            pmPLExportPacket.Click += new EventHandler(PmPLExport_Click);
            pmPL.Items.Add(pmPLExportPacket);

            // Init misc stuff
            CurrentSync = 0xFFFFFFFF;
        }

        public PacketData GetSelectedPacket()
        {
            if ((lbPackets.SelectedIndex < 0) || (lbPackets.SelectedIndex >= PL.Count))
                return null;
            return PL.GetPacket(lbPackets.SelectedIndex);
        }

        public void lbPackets_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lb = (sender as ListBox);
            if (!(lb.Parent is PacketTabPage))
                return;
            PacketTabPage tp = (lb.Parent as PacketTabPage);
            PacketData pd = null;
            if ((e.Index >= 0) && (e.Index < tp.PL.Count))
            {
                pd = tp.PL.GetPacket(e.Index);
            }
            else
            {
                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                return;
            }

            bool barOn = (tp.CurrentSync == pd.PacketSync);
            bool isSelected = (e.Index == lb.SelectedIndex);
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

            // Define the colors of our brushes.
            Brush textBrush = new SolidBrush(textCol);
            Brush backBrush = new SolidBrush(backCol);
            Brush barBrush = new SolidBrush(barCol);

            // Draw the background of the ListBox control for each item.
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            // header text
            var s = lb.Items[e.Index].ToString();
            //s = pd.VirtualTimeStamp.ToString() + "." + pd.VirtualTimeStamp.Millisecond.ToString("0000");

            Rectangle icon1 = new Rectangle(e.Bounds.Left, e.Bounds.Top + ((e.Bounds.Height - Properties.Resources.mini_unk_icon.Height) / 2), Properties.Resources.mini_unk_icon.Width, Properties.Resources.mini_unk_icon.Height);
            Rectangle icon2 = new Rectangle(icon1.Left + icon1.Width, icon1.Top, icon1.Width, icon1.Height);

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
            e.Graphics.DrawString(s,
                e.Font, textBrush, textBounds, StringFormat.GenericDefault);

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
            var iHeight = lbPackets.ItemHeight;
            if (iHeight <= 0)
                iHeight = 8;
            var iCount = lbPackets.Size.Height / iHeight;
            var tPos = lbPackets.SelectedIndex - (iCount / 2);
            if (tPos < 0)
                tPos = 0;
            lbPackets.TopIndex = tPos;
        }

        public void FillListBox(uint gotTolastSync = 0)
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
                    lbPackets.BeginUpdate();
                    lbPackets.Items.Clear();
                    for (int i = 0; i < PL.Count; i++)
                    {
                        PacketData pd = PL.GetPacket(i);
                        lbPackets.Items.Add(pd.HeaderText);
                        if ((gotoIndex < 0) && (gotTolastSync > 0) && (pd.PacketSync >= gotTolastSync))
                        {
                            gotoIndex = lbPackets.Items.Count - 1 ;
                        }
                        loadForm.pb.Value = i;
                        if ((i % 50) == 0)
                            loadForm.pb.Refresh();
                    }
                    lbPackets.EndUpdate();
                    if (gotoIndex >= 0)
                    {
                        lbPackets.SelectedIndex = gotoIndex;
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
                var r = pd?.Parent?.Rules.GetPacketRule(pd.PacketLogType, pd.StreamId, pd.PacketLevel, pd.PacketId);
                if (r != null)
                {
                    var lookupKey = r.LookupKey + (((ulong)pd.PacketLogType) * 0x0100000000);
                    pmPLShowPacketName.Text = lookupKey.ToString("X8");
                    pmPLEditParser.Tag = r ;
                    if (pd.PacketLogType != PacketLogTypes.Unknown)
                    {
                        pmPLEditParser.Text = @"Edit " + lookupKey.ToString("X8") + @" => " + r.Name;
                        pmPLEditParser.Visible = true;
                    }
                    else
                    {
                        pmPLEditParser.Text = @"Unknown direction, can't edit";
                        pmPLEditParser.Visible = false;
                    }
                }
                else
                {
                    pmPLShowPacketName.Text = @"No rule assigned";
                    pmPLEditParser.Tag = null;
                    pmPLEditParser.Text = @"Nothing to edit";
                    pmPLEditParser.Visible = false;
                }

            }
            else
            {
                pmPLShowPacketName.Text = pd.PacketLogType.ToString() + @" - 0x" + pd.PacketId.ToString("X3");
                string parserFileName;
                switch (pd.PacketLogType)
                {
                    case PacketLogTypes.Incoming:
                        parserFileName = Path.Combine("data", pd.Parent._parentTab.Engine.EngineId, "parse", "in-0x" + pd.PacketId.ToString("X3") + ".txt");
                        pmPLEditParser.Text = @"Edit " + parserFileName;
                        pmPLEditParser.Visible = true;
                        break;
                    case PacketLogTypes.Outgoing:
                        parserFileName = Path.Combine("data", pd.Parent._parentTab.Engine.EngineId, "parse", "out-0x" + pd.PacketId.ToString("X3") + ".txt");
                        pmPLEditParser.Text = @"Edit " + parserFileName;
                        pmPLEditParser.Visible = true;
                        break;
                    default:
                        parserFileName = "";
                        pmPLEditParser.Text = "";
                        pmPLEditParser.Visible = false;
                        break;
                }
                pmPLEditParser.Tag = parserFileName;
            }
            pmPLShowOnly.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);
            pmPLHideThis.Enabled = (pd.PacketLogType != PacketLogTypes.Unknown);

        }

        private void PmPLEditParser_Click(object sender, EventArgs e)
        {
            if (pmPLEditParser.Tag == null)
                return;
            if (pmPLEditParser.Tag is string fName)
                ownerMainForm.OpenBasicParseEditor(fName);
            if (pmPLEditParser.Tag is PacketRule pr)
                ownerMainForm.OpenXmlRulesParseEditor(pr);
        }

        private void PmPLShowOnly_Click(object sender, EventArgs e)
        {
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
            PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLHideThis_Click(object sender, EventArgs e)
        {
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
            PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowIncomingOnly_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterInType == FilterType.AllowNone) || (PL.Filter.FilterInType == FilterType.HidePackets))
            {
                PL.Filter.FilterInType = FilterType.Off;
            }
            PL.Filter.FilterOutType = FilterType.AllowNone;

            var lastSync = CurrentSync;
            PL.FilterFrom(PLLoaded);
            FillListBox(lastSync);
            CenterListBox();
        }

        private void PmPLShowOutgoingOnly_Click(object sender, EventArgs e)
        {
            var pd = GetSelectedPacket();
            if (pd == null)
                return;

            if ((PL.Filter.FilterOutType == FilterType.AllowNone) || (PL.Filter.FilterOutType == FilterType.HidePackets))
            {
                PL.Filter.FilterOutType = FilterType.Off;
            }
            PL.Filter.FilterInType = FilterType.AllowNone;

            var lastSync = CurrentSync;
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
            PL.FilterFrom(PLLoaded);
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
                MessageBox.Show("Error saving raw packet " + exportName + "\r\nException: " + x.Message);
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
                else
                {
                    res = false;
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

            if ( (Properties.Settings.Default.AskCreateNewProjectFile == true) && (!File.Exists(ProjectFile)) )
            {
                if (MessageBox.Show("Do you want to save project settings as a new project file ?\r\n" + ProjectFile, "Create Project File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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

                List<string> sl = new List<string>();
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
