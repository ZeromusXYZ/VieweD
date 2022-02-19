using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VieweD.Engine.Common
{
    public class PacketListFilter
    {
        public FilterType FilterOutType { get; set; }
        public List<ulong> FilterOutList { get; set; }
        public FilterType FilterInType { get; set; }
        public List<ulong> FilterInList { get; set; }

        public PacketListFilter()
        {
            FilterOutList = new List<ulong>();
            FilterInList = new List<ulong>();
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

        public void AddOutFilterValueToList(ulong value)
        {
            if ((value > 0) && (FilterOutList.IndexOf(value) < 0))
                FilterOutList.Add(value);
        }

        public void AddInFilterValueToList(ulong value)
        {
            if ((value > 0) && (FilterInList.IndexOf(value) < 0))
                FilterInList.Add(value);
        }

        public bool LoadFromFile(string filename)
        {
            try
            {
                List<string> sl = File.ReadAllLines(filename).ToList();

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
                            switch(f1)
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
                            if (DataLookups.TryFieldParse(f1, out long nout))
                                AddOutFilterValueToList((ulong)nout);
                            break;
                        case "in":
                            if (DataLookups.TryFieldParse(f1, out long nin))
                                AddInFilterValueToList((ulong)nin);
                            break;
                    }


                }

            }
            catch (Exception x)
            {
                MessageBox.Show("Failed to load " + filename + "\r\nException: " + x.Message, "Load Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool SaveToFile(string filename,EngineBase engine)
        {
            List<string> sl = new List<string>();
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
                sl.Add("out;0x"+ i.ToString("X3") + ";" + engine.DataLookups.NLU(DataLookups.LU_PacketOut).GetValue(i));
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
                sl.Add("in;0x" + i.ToString("X3") + ";" + engine.DataLookups.NLU(DataLookups.LU_PacketIn).GetValue(i));
            }

            try
            {
                File.WriteAllLines(filename, sl);
            }
            catch (Exception x)
            {
                MessageBox.Show("Failed to save " + filename + "\r\nException: " + x.Message, "Save Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}