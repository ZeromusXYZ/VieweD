using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.Helpers;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public class PacketListFilter
    {
        public FilterType FilterOutType { get; set; }
        public List<PacketFilterListEntry> FilterOutList { get; set; }
        public FilterType FilterInType { get; set; }
        public List<PacketFilterListEntry> FilterInList { get; set; }

        public PacketListFilter()
        {
            FilterOutList = new List<PacketFilterListEntry>();
            FilterInList = new List<PacketFilterListEntry>();
            Clear();
        }

        public void Clear()
        {
            FilterOutType = FilterType.Off;
            FilterOutList.Clear();
            FilterInType = FilterType.Off;
            FilterInList.Clear();
        }

        public void CopyFrom(PacketListFilter aFilter)
        {
            FilterOutType = aFilter.FilterOutType;
            FilterInType = aFilter.FilterInType;
            FilterOutList.Clear();
            FilterOutList.AddRange(aFilter.FilterOutList);
            FilterInList.Clear();
            FilterInList.AddRange(aFilter.FilterInList);
        }

        public void AddOutFilterValueToList(PacketFilterListEntry entry) => AddOutFilterValueToList(entry.Id, entry.Level, entry.StreamId);

        public void AddOutFilterValueToList(ulong packetId, byte level, byte streamId)
        {
            var filter = new PacketFilterListEntry(packetId, level, streamId);
            if ((packetId > 0) && !FilterOutList.Any(x => x.Id == filter.Id && x.Level == filter.Level && x.StreamId == filter.StreamId))
                FilterOutList.Add(filter);
        }

        public void AddInFilterValueToList(PacketFilterListEntry entry) => AddInFilterValueToList(entry.Id, entry.Level, entry.StreamId);

        public void AddInFilterValueToList(ulong packetId, byte level, byte streamId)
        {
            var filter = new PacketFilterListEntry(packetId, level, streamId);
            if ((packetId > 0) && !FilterInList.Any(x => x.Id == filter.Id && x.Level == filter.Level && x.StreamId == filter.StreamId))
                FilterInList.Add(filter);
        }

        public bool LoadFromFile(string filename)
        {
            try
            {
                var sl = File.ReadAllLines(filename).ToList();

                Clear();
                foreach (string line in sl)
                {
                    var fields = line.Split(';');
                    if (fields.Length <= 1)
                        continue;

                    var f0 = fields[0].ToLower();
                    var f1 = fields[1].ToLower();

                    switch (f0)
                    {
                        case "outtype":
                            switch (f1)
                            {
                                case "off":
                                    FilterOutType = FilterType.Off;
                                    break;
                                case "show":
                                    FilterOutType = FilterType.ShowPackets;
                                    break;
                                case "hide":
                                    FilterOutType = FilterType.HidePackets;
                                    break;
                                case "none":
                                    FilterOutType = FilterType.AllowNone;
                                    break;
                            }

                            break;
                        case "intype":
                            switch (f1)
                            {
                                case "off":
                                    FilterInType = FilterType.Off;
                                    break;
                                case "show":
                                    FilterInType = FilterType.ShowPackets;
                                    break;
                                case "hide":
                                    FilterInType = FilterType.HidePackets;
                                    break;
                                case "none":
                                    FilterInType = FilterType.AllowNone;
                                    break;
                            }

                            break;
                        case "out":
                            var outSplit = f1.Split('-');

                            if (outSplit.Length == 1)
                            {
                                if (NumberHelper.TryFieldParse(f1, out long nOut))
                                    AddOutFilterValueToList((ulong)nOut,0,0);
                            }
                            else
                            {
                                if (NumberHelper.TryFieldParse(outSplit[0], out long nOutId) &&
                                    NumberHelper.TryFieldParse(outSplit[1], out long nOutLv) &&
                                    NumberHelper.TryFieldParse(outSplit[2], out long nOutStream))
                                    AddOutFilterValueToList((ulong)nOutId, (byte)nOutLv, (byte)nOutStream);
                            }

                            break;
                        case "in":
                            var inSplit = f1.Split('-');

                            if (inSplit.Length == 1)
                            {
                                if (NumberHelper.TryFieldParse(f1, out long nIn))
                                    AddInFilterValueToList((ulong)nIn, 0, 0);
                            }
                            else
                            {
                                if (NumberHelper.TryFieldParse(inSplit[0], out long nInId) &&
                                    NumberHelper.TryFieldParse(inSplit[1], out long nInLv) &&
                                    NumberHelper.TryFieldParse(inSplit[2], out long nInStream))
                                    AddInFilterValueToList((ulong)nInId, (byte)nInLv, (byte)nInStream);
                            }

                            break;
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Failed to load " + filename + "\r\nException: " + ex.Message, @"Load Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool SaveToFile(string filename,EngineBase engine)
        {
            var sl = new List<string>();
            sl.Add("rem;original-file;" + Path.GetFileName(filename));
            switch(FilterOutType)
            {
                case FilterType.Off:
                    sl.Add("outtype;off");
                    break;
                case FilterType.ShowPackets:
                    sl.Add("outtype;show");
                    break;
                case FilterType.HidePackets:
                    sl.Add("outtype;hide");
                    break;
                case FilterType.AllowNone:
                    sl.Add("outtype;none");
                    break;
            }
            foreach(var i in FilterOutList)
            {
                var fVal = "0x" + i.Id.ToString("X3");
                if ((i.Level > 0) || (i.StreamId > 0))
                {
                    fVal += "-0x" + i.Level.ToString("X2");
                    fVal += "-0x" + i.StreamId.ToString("X2");
                }
                sl.Add("out;"+ fVal + ";" + engine.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(i.AsMergedId()));
            }

            switch (FilterInType)
            {
                case FilterType.Off:
                    sl.Add("intype;off");
                    break;
                case FilterType.ShowPackets:
                    sl.Add("intype;show");
                    break;
                case FilterType.HidePackets:
                    sl.Add("intype;hide");
                    break;
                case FilterType.AllowNone:
                    sl.Add("intype;none");
                    break;
            }
            foreach (var i in FilterInList)
            {
                var fVal = "0x" + i.Id.ToString("X3");
                if ((i.Level > 0) || (i.StreamId > 0))
                {
                    fVal += "-0x" + i.Level.ToString("X2");
                    fVal += "-0x" + i.StreamId.ToString("X2");
                }
                sl.Add("in;" + fVal + ";" + engine.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(i.AsMergedId()));
            }

            try
            {
                File.WriteAllLines(filename, sl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Failed to save " + filename + "\r\nException: " + ex.Message, @"Save Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}