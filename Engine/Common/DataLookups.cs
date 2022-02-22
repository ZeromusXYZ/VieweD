using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VieweD.Engine.Common
{
    public class DataLookups
    {
        public static string LU_PacketOut = "out";
        public static string LU_PacketIn = "in";

        public DataLookupList NullList = new DataLookupList();
        public DataLookupEntry NullEntry = new DataLookupEntry();
        public static DataLookupListSpecialMath MathList = new DataLookupListSpecialMath();
        public List<string> AllValues = new List<string>();
        public string AllLoadErrors = string.Empty ;

        // lookupname, id, lookupresult
        public Dictionary<string, DataLookupList> LookupLists = new Dictionary<string, DataLookupList>();

        public DataLookups()
        {
            NullEntry.Id = 0;
            NullEntry.Val = "NULL";
            NullEntry.Extra = "";
        }

        public static bool TryFieldParse(string field, out int res)
        {
            bool result = false ;
            bool isNegatice = field.StartsWith("-");
            if (isNegatice)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            if (field.StartsWith("0x"))
            {
                try
                {
                    res = int.Parse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if (field.StartsWith("$"))
            {
                try
                {
                    res = int.Parse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if ( (field.EndsWith("h")) || (field.EndsWith("H")))
            {
                try
                {
                    res = int.Parse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            {
                try
                {
                    res = int.Parse(field);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            if (isNegatice)
                res *= -1;
            return result;
        }

        public static bool TryFieldParse(string field, out long res)
        {
            bool result = false;
            bool isNegative = field.StartsWith("-");
            if (isNegative)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            if (field.StartsWith("0x"))
            {
                try
                {
                    res = long.Parse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if (field.StartsWith("$"))
            {
                try
                {
                    res = long.Parse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                try
                {
                    res = long.Parse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            {
                try
                {
                    res = long.Parse(field);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            if (isNegative)
                res *= -1;
            return result;
        }

        public static bool TryFieldParseUInt64(string field, out UInt64 res)
        {
            bool result = false;
            if (field.StartsWith("0x"))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if (field.StartsWith("$"))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                try
                {
                    res = UInt64.Parse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            else
            {
                try
                {
                    res = UInt64.Parse(field);
                    result = true;
                }
                catch
                {
                    res = 0;
                }
            }
            return result;
        }

        

        public bool LoadLookupFile(string fileName)
        {
            // Extract name
            var lookupName = Path.GetFileNameWithoutExtension(fileName).ToLower();

            // Remove a old list if it already exists
            if (LookupLists.TryGetValue(lookupName,out _))
                LookupLists.Remove(lookupName);

            // Create new list
            DataLookupList dll = new DataLookupList();
            // Add it
            LookupLists.Add(lookupName,dll);
            // Load file
            List<string> sl = File.ReadAllLines(fileName).ToList();
            // Parse File
            var lineNumber = 0;
            foreach(string line in sl)
            {
                lineNumber++;
                try
                {
                    string[] fields = line.Split(';');
                    if (fields.Length > 1)
                    {
                        if (TryFieldParse(fields[0], out int newId))
                        {
                            DataLookupEntry dle = new DataLookupEntry();
                            dle.Id = (UInt64)newId;
                            dle.Val = fields[1];
                            if (fields.Length > 2)
                                dle.Extra = fields[2];
                            dll.Data.Add((UInt64)newId, dle);
                            // for autocomplete
                            AllValues.Add(dle.Val);
                        }
                    }
                }
                catch (Exception x)
                {
                    AllLoadErrors += string.Format("\n\r\n\rException loading {0} at line {1} :\n\r{2}\r\n=> {3}", fileName, lineNumber,x.Message,line);
                    return false;
                }
            }
            return true;
        }

        public static string DefaultLookupPath(string engineId)
        {
            return Path.Combine(Application.StartupPath, "data", engineId, "lookup");
        }

        public bool LoadLookups(string engineId, bool initialLoading = true)
        {
            if (initialLoading)
                LookupLists.Clear();
            AllLoadErrors = string.Empty ;
            bool noErrors = true;
            var lookupPath = DefaultLookupPath(engineId);
            DirectoryInfo di = new DirectoryInfo(lookupPath);
            if (Directory.Exists(lookupPath))
            {
                foreach (var fi in di.GetFiles())
                {
                    if (!LoadLookupFile(fi.FullName))
                        noErrors = false;
                }
            }
            return noErrors;
        }

        public DataLookupList NLU(string lookupName,string lookupOffsetString = "")
        {
            /*
            if ((lookupName.ToLower() == EngineFFXI.LU_Item) && (ItemsList.items.Count > 0))
            {
                return ItemsList;
            }
            else
            if ((lookupName.ToLower() == EngineFFXI.LU_Dialog) && (DialogsList.dialogsCache.Count > 0))
            {
                return DialogsList;
            }
            else
            */
            if ((lookupOffsetString != string.Empty) && (lookupName.ToLower() == "@math"))
            {
                if (!lookupOffsetString.StartsWith("?"))
                    lookupOffsetString = "? " + lookupOffsetString;
                MathList.EvalString = lookupOffsetString;
                return MathList;
            }
            else
            {
                if (LookupLists.TryGetValue(lookupName, out var res))
                    return res;
            }
            return NullList;
        }

        public DataLookupList NLUOrCreate(string lookupName)
        {
            DataLookupList res;
            if (LookupLists.TryGetValue(lookupName, out res))
                return res;
            res = new DataLookupList();
            LookupLists.Add(lookupName, res);
            return res;
        }

        public string PacketTypeToString(PacketLogTypes packetLogType, uint packetId)
        {
            string res = "";
            if (packetLogType == PacketLogTypes.Outgoing)
                res = NLU(LU_PacketOut).GetValue(packetId);
            if (packetLogType == PacketLogTypes.Incoming)
                res = NLU(LU_PacketIn).GetValue(packetId);
            if (res == "")
                res = "??? unknown";
            return res;
        }

        public void RegisterCustomLookup(string customListName, UInt64 customId, string customValue)
        {
            customListName = customListName.ToLower();
            if (customListName == "@math")
                return;
            if (customListName.StartsWith("§"))
            {
                if ((customListName == @"§playerid") && (GameViewForm.GV != null))
                {
                    GameViewForm.GV.gbPlayer.Text = @"Player 0x" + customId.ToString("X8");
                    GameViewForm.GV.lPlayerName.Text = customValue;
                }
                return;
            }
            if (!customListName.StartsWith("@"))
                customListName = "@" + customListName;
            DataLookupList list = null;

            foreach(var ll in LookupLists)
            {
                if (ll.Key.ToLower() == customListName)
                {
                    list = ll.Value;
                    break;
                }
            }
            if (list == null)
            {
                list = new DataLookupList();
                LookupLists.Add(customListName, list);
            }
            if (list.Data.TryGetValue(customId, out var entry))
            {
                // Special case, don't update if this is a "null string" parsed
                if ((customValue != "NULL") && (entry.Val != customValue))
                    entry.Val = customValue;
                entry.Extra = string.Empty;
                return;
            }
            var newlistv = new DataLookupEntry();
            newlistv.Id = customId;
            newlistv.Val = customValue;
            newlistv.Extra = string.Empty;
            list.Data.Add(customId, newlistv);
            AllValues.Add(customValue);
        }
    }
}