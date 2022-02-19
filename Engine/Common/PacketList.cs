using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.Forms;

namespace VieweD.Engine.Common
{
    public class PacketList
    {
        private bool _isPreParsed;
        public PacketTabPage _parentTab;

        public List<PacketData> PacketDataList { get; set; }
        public List<UInt16> ContainsPacketsIn { get; set; }
        public List<UInt16> ContainsPacketsOut { get; set; }
        public bool IsPreParsed { 
            get => _isPreParsed; 
            set => _isPreParsed = value; 
        }
        public string LoadedLogFileFormat { get; set; }
        public PacketListFilter Filter ;
        public DateTime firstPacketTime;
        public UInt16 currentParseZone = 0;
        public UInt32 currentParsePlayerID = 0;
        public string currentParsePlayerName = "";
        public RulesReader Rules;
        public UInt32 XORKey;
        public byte[] AESKey;
        public uint _numPckC = 0xFFFFFFFF;
        public byte[] IV = new byte[16];

        public PacketList(PacketTabPage parent)
        {
            _parentTab = parent;
            PacketDataList = new List<PacketData>();
            ContainsPacketsIn = new List<UInt16>();
            ContainsPacketsOut = new List<UInt16>();
            Filter = new PacketListFilter();
            firstPacketTime = new DateTime(0);
            // Rules = new RulesReader();
        }

        ~PacketList()
        {
            Filter.Clear();
            Clear();
        }

        public void Clear()
        {
            PacketDataList.Clear();
            firstPacketTime = new DateTime(0);
        }

        public bool LoadFromFile(string fileName, PacketTabPage parentTab)
        {
            if (!File.Exists(fileName))
                return false;

            var fn = fileName.ToLower();

            // Try file type depending on it's extension
            var thisEngine = Engines.CreateEngineByFileExtension(Path.GetExtension(fn));
            if (thisEngine != null)
            {
                parentTab.Engine = thisEngine;
                thisEngine.ParentTab = parentTab;
                thisEngine.Init();
            }
            else
            {
                MessageBox.Show("Could not find a handler to open this file type", "LoadFromFile", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // Do Rules selector here if needed
                var rulesFile = parentTab.LoadedRulesFile;
                if ((parentTab.Engine.HasRulesFile) && ((rulesFile == string.Empty) || (!File.Exists(rulesFile))))
                    rulesFile = RulesSelectForm.SelectRulesFile(fileName, parentTab.Engine);

                bool res;
                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    res = parentTab.Engine.LoadFromStream(this, fileStream, fileName, rulesFile, parentTab.DecryptVersion);
                }

                return res;
            }
            catch (Exception x)
            {
                if (x is PathTooLongException)
                {
                    MessageBox.Show(
                        "This program does not support file paths that are longer than MAX_PATH (260 characters by default)\r\nPlease consider shortening your directory or file names, and try again.",
                        "Name too long", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    MessageBox.Show("Exception:\r\n" + x.Message, parentTab?.Engine?.EngineId + ".LoadFromFile()",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                return false;
            }
        }

        public int GetLastPosForTime(DateTime searchTime)
        {
            for(int i = PacketDataList.Count-1; i >= 0;i--)
            {
                if (PacketDataList[i].TimeStamp <= searchTime)
                    return i ;
            }
            return PacketDataList.Count - 1;
        }

        public List<string> RawBytesToString(List<byte> rawBytes)
        {
            var res = new List<string>();
            var rawtext = string.Empty;
            for (var x = 0; x < rawBytes.Count; x++)
            {
                byte b = rawBytes[x];
                rawtext += b.ToString("X2");
                if ((x % 0x10) == 0xF)
                {
                    res.Add(rawtext);
                    rawtext = string.Empty;
                }
                else
                    rawtext += " ";
            }
            if (rawtext.Trim(' ') != string.Empty)
                res.Add(rawtext);
            return res;
        }

        public int Count => PacketDataList.Count;

        public PacketData GetPacket(int index)
        {
            if ((index >= 0) && (index < PacketDataList.Count))
                return PacketDataList[index];
            if (PacketDataList.Count > 0)
                return PacketDataList[0];
            else
                return null;
        }

        public int CopyFrom(PacketList original)
        {
            var c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XORKey = original.XORKey;
            AESKey = original.AESKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach(var pd in original.PacketDataList)
            {
                PacketDataList.Add(pd);
                c++;
            }
            if (PacketDataList.Count > 0)
                firstPacketTime = PacketDataList[0].TimeStamp;
            return c;
        }

        private static bool DoIShowThis(ulong packetKey, FilterType filterType, ICollection<ulong> filterList)
        {
            switch (filterType)
            { 
                case FilterType.AllowNone:
                    return false;
                case FilterType.ShowPackets:
                    return filterList.Contains(packetKey);
                case FilterType.HidePackets:
                    return !filterList.Contains(packetKey);
                case FilterType.Off:
                default:
                    return true;
            }
        }

        private bool DoIShowThis(PacketData pd)
        {
            // On D files, also check vs packet level
            // TODO: Get rid of this as packets should not be filtered by level now that we know how they work
            /*
            if (pd._parent.LoadedLogFileFormat == "PDEC")
            {
                if (!Filter.FilterShowLevels.Contains(pd.OriginalPacketLevel))
                    return false;
            }
            */
            // ulong packetKey = (ulong)(pd.PacketID + (pd.StreamId * 0x01000000));
            ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            if ((pd.PacketLogType == PacketLogTypes.Incoming) && (Filter.FilterInType != FilterType.Off))
                return DoIShowThis(packetKey, Filter.FilterInType, Filter.FilterInList);
            if ((pd.PacketLogType == PacketLogTypes.Outgoing) && (Filter.FilterOutType != FilterType.Off))
                return DoIShowThis(packetKey, Filter.FilterOutType, Filter.FilterOutList);
            return true;
        }

        public int FilterFrom(PacketList original)
        {
            int c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XORKey = original.XORKey;
            AESKey = original.AESKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach (var pd in original.PacketDataList.Where(pd => DoIShowThis(pd)))
            {
                PacketDataList.Add(pd);
                c++;
            }
            if (PacketDataList.Count > 0)
                firstPacketTime = PacketDataList[0].TimeStamp;
            return c;
        }

        public int SearchFrom(PacketList original, SearchParameters p)
        {
            int c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XORKey = original.XORKey;
            AESKey = original.AESKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach (var pd in original.PacketDataList.Where(pd => pd.MatchesSearch(p)))
            {
                PacketDataList.Add(pd);
                c++;
            }
            if (PacketDataList.Count > 0)
                firstPacketTime = PacketDataList[0].TimeStamp;
            return c;
        }

        public int FindPacketIndexByDateTime(DateTime dt,int searchStartLocation = 0)
        {
            if (PacketDataList.Count <= 0)
                return -1;
            var i = searchStartLocation ;
            if ((i < 0) || (i >= PacketDataList.Count))
                i = 0;
            for(var c = 0;c < PacketDataList.Count; c++)
            {
                // Next
                var lastCheckTime = PacketDataList[i].VirtualTimeStamp;
                i++;
                if (i >= PacketDataList.Count)
                    i = 0;

                if ((lastCheckTime <= dt) && (dt < PacketDataList[i].VirtualTimeStamp))
                    return i;

            }
            return -1;
        }

        public void BuildVirtualTimeStamps()
        {
            // Need a minimum of 3 packets to be able to have effect
            if (PacketDataList.Count <= 1)
                return;

            int i = 0;
            int divider = 0 ;
            var firstOfGroupTime = GetPacket(0).TimeStamp;
            int firstOfGroupIndex = 0;
            var lastTimeStamp = firstOfGroupTime;

            while (i < PacketDataList.Count)
            {
                PacketData thisPacket = GetPacket(i);
                thisPacket.VirtualTimeStamp = thisPacket.TimeStamp;
                // For D files, we don't need virtual timestamps
                if (thisPacket.Parent.LoadedLogFileFormat == "PDEC")
                {
                    i++;
                    continue;
                }
                // For FFXI logs, create virtual timestamps based on packets with the same time
                if (thisPacket.TimeStamp == lastTimeStamp)
                {
                    // Same packet Group
                    divider++;
                }
                if ( ((thisPacket.TimeStamp != lastTimeStamp) || (i >= PacketDataList.Count)) )
                {
                    // Last packet of the group
                    var oneStepTime = TimeSpan.Zero;
                    if (divider > 0)
                        oneStepTime = TimeSpan.FromMilliseconds(1000 / divider);
                    var stepTime = TimeSpan.Zero;
                    for (int n = 0; n <= divider; n++)
                    {
                        GetPacket(firstOfGroupIndex + n).VirtualTimeStamp = firstOfGroupTime + stepTime;
                        stepTime += oneStepTime ;
                    }

                    if (i < (PacketDataList.Count - 1))
                    {
                        // If not the last one
                        firstOfGroupIndex = i + 1;
                        firstOfGroupTime = GetPacket(i + 1).TimeStamp;
                        divider = 0;
                    }
                }
                lastTimeStamp = thisPacket.TimeStamp;
                i++;
            }

        }

    }
}