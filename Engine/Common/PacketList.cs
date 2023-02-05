using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.Forms;

namespace VieweD.Engine.Common
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    public class PacketList
    {
        /// <summary>
        /// Reference to owning Tab page
        /// </summary>
        public PacketTabPage ParentTab { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<PacketData> PacketDataList { get; set; }
        public List<UInt16> ContainsPacketsIn { get; set; }
        public List<UInt16> ContainsPacketsOut { get; set; }
        public bool IsPreParsed { get; set; }

        public string LoadedLogFileFormat { get; set; }
        public PacketListFilter Filter { get; set; }
        public DateTime FirstPacketTime { get; set; }
        public UInt16 CurrentParseZone { get; set; }
        public UInt32 CurrentParsePlayerId { get; set; }
        public string CurrentParsePlayerName { get; set; } = string.Empty;
        public RulesReader Rules { get; set; }
        public UInt32 XorKey { get; set; }
        public byte[] AesKey { get; set; }
        public uint NumberPacketCounter { get; set; }
        public byte[] Iv { get; set; } = new byte[16];

        public PacketList(PacketTabPage parent)
        {
            ParentTab = parent;
            PacketDataList = new List<PacketData>();
            ContainsPacketsIn = new List<UInt16>();
            ContainsPacketsOut = new List<UInt16>();
            Filter = new PacketListFilter();
            FirstPacketTime = new DateTime(0);
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
            FirstPacketTime = new DateTime(0);
        }

        public bool LoadFromFile(string fileName, PacketTabPage parentTab)
        {
            if (!File.Exists(fileName))
                return false;

            var fn = fileName.ToLower();
            NumberPacketCounter = 0;

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
                MessageBox.Show(@"Could not find a handler to open this file type", @"LoadFromFile", MessageBoxButtons.OK,
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
            catch (Exception ex)
            {
                if (ex is PathTooLongException)
                {
                    MessageBox.Show($"This program does not support file paths that are longer than MAX_PATH (260 characters by default)\r\nPlease consider shortening your directory or file names, and try again.",
                        @"Name too long", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    MessageBox.Show("Exception:\r\n" + ex.Message, parentTab?.Engine?.EngineId + @".LoadFromFile()",
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
            var rawText = string.Empty;
            for (var x = 0; x < rawBytes.Count; x++)
            {
                byte b = rawBytes[x];
                rawText += b.ToString("X2");
                if ((x % 0x10) == 0xF)
                {
                    res.Add(rawText);
                    rawText = string.Empty;
                }
                else
                    rawText += " ";
            }
            if (rawText.Trim(' ') != string.Empty)
                res.Add(rawText);
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
            XorKey = original.XorKey;
            AesKey = original.AesKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach(var pd in original.PacketDataList)
            {
                pd.MarkedAsDimmed = false;
                PacketDataList.Add(pd);
                c++;
            }

            if (PacketDataList.Count > 0)
                FirstPacketTime = PacketDataList[0].TimeStamp;

            return c;
        }

        private static bool DoIShowThis(PacketFilterListEntry packetKey, FilterType filterType, ICollection<PacketFilterListEntry> filterList)
        {
            switch (filterType)
            { 
                case FilterType.AllowNone:
                    return false;
                case FilterType.ShowPackets:
                    return filterList.Any(x => (x.Id == packetKey.Id) && (x.Level == packetKey.Level) && (x.StreamId == packetKey.StreamId));
                case FilterType.HidePackets:
                    return !filterList.Any(x => (x.Id == packetKey.Id) && (x.Level == packetKey.Level) && (x.StreamId == packetKey.StreamId));
                case FilterType.Off:
                default:
                    return true;
            }
        }

        private bool DoIShowThis(PacketData pd)
        {
            // On D files, also check vs packet level

            // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
            var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);

            if ((pd.PacketLogType == PacketLogTypes.Incoming) && (Filter.FilterInType != FilterType.Off))
                return DoIShowThis(packetKey, Filter.FilterInType, Filter.FilterInList);
            if ((pd.PacketLogType == PacketLogTypes.Outgoing) && (Filter.FilterOutType != FilterType.Off))
                return DoIShowThis(packetKey, Filter.FilterOutType, Filter.FilterOutList);
            return true;
        }

        public int FilterFrom(PacketList original)
        {
            var c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XorKey = original.XorKey;
            AesKey = original.AesKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach (var pd in original.PacketDataList.Where(DoIShowThis))
            {
                pd.MarkedAsDimmed = false;
                PacketDataList.Add(pd);
                c++;
            }

            if (PacketDataList.Count > 0)
                FirstPacketTime = PacketDataList[0].TimeStamp;

            return c;
        }

        public int HighlightFilterFrom(PacketList original)
        {
            var c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XorKey = original.XorKey;
            AesKey = original.AesKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach (var pd in original.PacketDataList)
            {
                pd.MarkedAsDimmed = !DoIShowThis(pd);
                PacketDataList.Add(pd);
                c++;
            }
            if (PacketDataList.Count > 0)
                FirstPacketTime = PacketDataList[0].TimeStamp;
            return c;
        }


        public int SearchFrom(PacketList original, SearchParameters p)
        {
            int c = 0;
            Clear();
            IsPreParsed = original.IsPreParsed;
            Rules = original.Rules;
            XorKey = original.XorKey;
            AesKey = original.AesKey;
            LoadedLogFileFormat = original.LoadedLogFileFormat;
            foreach (var pd in original.PacketDataList.Where(pd => pd.MatchesSearch(p)))
            {
                PacketDataList.Add(pd);
                c++;
            }
            if (PacketDataList.Count > 0)
                FirstPacketTime = PacketDataList[0].TimeStamp;
            return c;
        }

        public int FindPacketIndexByDateTime(DateTime dt,int searchStartLocation = 0)
        {
            if (PacketDataList.Count <= 0)
                return -1;

            var i = searchStartLocation ;

            if ((i < 0) || (i >= PacketDataList.Count))
                i = 0;

            for (var c = 0; c < PacketDataList.Count; c++)
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

            var i = 0;
            var divider = 0 ;
            var firstOfGroupTime = GetPacket(0).TimeStamp;
            var firstOfGroupIndex = 0;
            var lastTimeStamp = firstOfGroupTime;

            while (i < PacketDataList.Count)
            {
                var thisPacket = GetPacket(i);
                thisPacket.VirtualTimeStamp = thisPacket.TimeStamp;
                
                /*
                // For D files, we don't need virtual timestamps
                if (thisPacket.Parent.LoadedLogFileFormat == "PDEC")
                {
                    i++;
                    continue;
                }
                */

                // For FFXI logs, create virtual timestamps based on packets with the same time
                if (thisPacket.TimeStamp == lastTimeStamp)
                {
                    // Same packet Group
                    divider++;
                }

                if ( ((thisPacket.TimeStamp != lastTimeStamp) || (i >= PacketDataList.Count)) )
                {
                    // Last packet of the group (or last packet of the list)
                    var oneStepTime = TimeSpan.Zero;

                    // Get time difference to next time step
                    var maxDeltaTime = thisPacket.TimeStamp - lastTimeStamp;
                    // If there is no difference, treat it as a 1 second group
                    if (maxDeltaTime.Milliseconds <= 0)
                        maxDeltaTime = TimeSpan.FromSeconds(1);

                    if (divider > 0)
                        oneStepTime = TimeSpan.FromMilliseconds(maxDeltaTime.TotalMilliseconds / divider);

                    var stepTime = TimeSpan.Zero;
                    for (var n = 0; n <= divider; n++)
                    {
                        GetPacket(firstOfGroupIndex + n).VirtualTimeStamp = firstOfGroupTime + stepTime;
                        stepTime += oneStepTime;
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