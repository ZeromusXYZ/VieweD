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
        // Default lookup table names for packet in and packet out names
        public static string LU_PacketOut = "out";
        public static string LU_PacketIn = "in";

        /// <summary>
        /// Empty LookupList to undefined searches
        /// </summary>
        public DataLookupList NullList { get; } = new DataLookupList();

        /// <summary>
        /// Empty LookupEntry for undefined lookup keys
        /// </summary>
        public DataLookupEntry NullEntry { get; set; }

        /// <summary>
        /// Special Lookup table that can handle basic math statements
        /// </summary>
        public static DataLookupListSpecialMath MathList { get; } = new DataLookupListSpecialMath();

        /// <summary>
        /// A string list containing all added Values, used for auto-completion fields
        /// </summary>
        public List<string> AllValues { get; set; } = new List<string>();

        /// <summary>
        /// String containing all loading errors
        /// </summary>
        public string AllLoadErrors { get; set; } = string.Empty;

        /// <summary>
        /// List of all Lookup tables
        /// </summary>
        public Dictionary<string, DataLookupList> LookupLists = new Dictionary<string, DataLookupList>();

        /// <summary>
        /// Handler for lookup tables
        /// </summary>
        public DataLookups()
        {
            // Populate NullEntry
            NullEntry = new DataLookupEntry()
            {
                Id = 0,
                Val = "NULL",
                Extra = "",
            };
        }

        /// <summary>
        /// Parse a string as a int (int32) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParse(string field, out int res)
        {
            bool result;

            // Handle notation for forced positive and negative values
            var isNegative = field.StartsWith("-");

            // Remove the sign from the start of the string
            if (isNegative)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                result = int.TryParse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                result = int.TryParse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in ????h notation (classic C)
            if ( (field.EndsWith("h")) || (field.EndsWith("H")))
            {
                result = int.TryParse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            {
                result = int.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Re-apply negative sign if required
            if (isNegative)
                res *= -1;

            return result;
        }

        /// <summary>
        /// Parse a string as a long (int64) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParse(string field, out long res)
        {
            bool result;

            // Handle notation for forced positive and negative values
            var isNegative = field.StartsWith("-");

            // Remove the sign from the start of the string
            if (isNegative)
                field = field.TrimStart('-');
            if (field.StartsWith("+"))
                field = field.TrimStart('+');

            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                result = long.TryParse(field.Substring(2, field.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                result = long.TryParse(field.Substring(1, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            // Handle Hex numbers in ????h notation (classic C)
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                result = long.TryParse(field.Substring(0, field.Length - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }
            else
            {
                // Handle it as a regular long
                result = long.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Re-apply negative sign if required
            if (isNegative)
                res *= -1;

            return result;
        }

        /// <summary>
        /// Parse a string as a ulong (uint64) using various rules and notations
        /// </summary>
        /// <param name="field">string to parse</param>
        /// <param name="res">resulting value</param>
        /// <returns>Returns true if successful</returns>
        public static bool TryFieldParseUInt64(string field, out ulong res)
        {
            // Handle Hex numbers in 0x???? notation (default)
            if (field.StartsWith("0x"))
            {
                return ulong.TryParse(field.Substring(2, field.Length - 2), 
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Handle Hex numbers in $???? notation (Pascal/Delphi)
            if (field.StartsWith("$"))
            {
                return ulong.TryParse(field.Substring(1, field.Length - 1), 
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            // Handle Hex numbers in ????h notation (classic C)
            if ((field.EndsWith("h")) || (field.EndsWith("H")))
            {
                return ulong.TryParse(field.Substring(0, field.Length - 1), 
                    NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
            }

            return ulong.TryParse(field, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out res);
        }

        /// <summary>
        /// Load lookup table from file
        /// </summary>
        /// <param name="fileName">File to load</param>
        /// <returns>Returns true if successful, otherwise returns false and errors are logged in AllLoadErrors</returns>
        public bool LoadLookupFile(string fileName)
        {
            // Extract name
            var lookupName = Path.GetFileNameWithoutExtension(fileName).ToLower();

            // Remove a old list if it already exists
            if (LookupLists.TryGetValue(lookupName,out _))
                LookupLists.Remove(lookupName);

            // Create new list
            var dataLookupList = new DataLookupList();

            // Add it
            LookupLists.Add(lookupName, dataLookupList);

            // Load file
            var lines = File.ReadAllLines(fileName).ToList();

            // Parse File
            var lineNumber = 0;
            foreach(var line in lines)
            {
                lineNumber++;
                try
                {
                    var fields = line.Split(';');
                    if (fields.Length > 1)
                    {
                        if (TryFieldParse(fields[0], out int newId))
                        {
                            var dataLookupEntry = new DataLookupEntry
                            {
                                Id = (ulong)newId,
                                Val = fields[1],
                                Extra = fields.Length > 2 ? fields[2] : "",
                            };
                            dataLookupList.Data.Add((ulong)newId, dataLookupEntry);

                            // For autocomplete
                            AllValues.Add(dataLookupEntry.Val);
                        }
                    }
                }
                catch (Exception x)
                {
                    AllLoadErrors += $"\n\r\n\rException loading {fileName} at line {lineNumber} :\n\r{x.Message}\r\n=> {line}";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Constructs the default lookup location using a given EngineId
        /// </summary>
        /// <param name="engineId">EngineId name</param>
        /// <returns>Returns the default lookup location for a given Engine</returns>
        public static string DefaultLookupPath(string engineId)
        {
            return Path.Combine(Application.StartupPath, "data", engineId, "lookup");
        }

        /// <summary>
        /// Load lookup data for a given EngineId
        /// </summary>
        /// <param name="engineId">EngineId to load data for</param>
        /// <param name="initialLoading">Settings this to true will first wipe already existing data</param>
        /// <returns>Returns true if successful or empty</returns>
        public bool LoadLookups(string engineId, bool initialLoading = true)
        {
            if (initialLoading)
                LookupLists.Clear();
            AllLoadErrors = string.Empty ;
            var lookupPath = DefaultLookupPath(engineId);
            
            // If the directory doesn't exist, consider it loaded
            if (!Directory.Exists(lookupPath)) 
                return true;

            var directoryInfo = new DirectoryInfo(lookupPath);
            var hasNoErrors = true;
            foreach (var fi in directoryInfo.GetFiles())
            {
                if (!LoadLookupFile(fi.FullName))
                    hasNoErrors = false;
            }
            return hasNoErrors;
        }

        /// <summary>
        /// Main NameLookUp function
        /// </summary>
        /// <param name="lookupName">Name of the lookup list you want to return</param>
        /// <param name="lookupOffsetString">Optional Offset value or expression</param>
        /// <returns>Requested lookup list</returns>
        public DataLookupList NLU(string lookupName,string lookupOffsetString = "")
        {
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

        /// <summary>
        /// Returns a LookupList or creates a new one. Does not support special lists like @MATH
        /// </summary>
        /// <param name="lookupName">Name of the lookup table</param>
        /// <returns>Returns the (new) lookup list</returns>
        public DataLookupList NLUOrCreate(string lookupName)
        {
            if (LookupLists.TryGetValue(lookupName, out var res))
                return res;
            res = new DataLookupList();
            LookupLists.Add(lookupName, res);
            return res;
        }

        /// <summary>
        /// Returns a packet name based on it's ID from the default lookup tables
        /// </summary>
        /// <param name="packetLogType">Direction of the packet</param>
        /// <param name="packetId">The ID to lookup</param>
        /// <returns>PacketName or "??? unknown"</returns>
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

        /// <summary>
        /// Registers new custom lookup data, used for storing lookup data while parsing
        /// Overwrites previous values unless the new value is NULL
        /// Also handles some special IDs like §playerid, which are not permanently stored
        /// </summary>
        /// <param name="customListName">Lookup table name to add to</param>
        /// <param name="customId">ID to be added</param>
        /// <param name="customValue">New value to add</param>
        public void RegisterCustomLookup(string customListName, ulong customId, string customValue)
        {
            customListName = customListName.ToLower();

            // Ignore if @MATH special list
            if (customListName == "@math")
                return;

            // Handle §playerid special case
            if (customListName.StartsWith("§"))
            {
                if ((customListName == @"§playerid") && (GameViewForm.GV != null))
                {
                    GameViewForm.GV.gbPlayer.Text = @"Player 0x" + customId.ToString("X8");
                    GameViewForm.GV.lPlayerName.Text = customValue;
                }

                return;
            }

            // Prefix @ if it wasn't already
            if (!customListName.StartsWith("@"))
                customListName = "@" + customListName;

            var list = NLUOrCreate(customListName);

            var keepOldValue = false;
            if (list.Data.TryGetValue(customId, out var entry))
            {
                // Special case, don't update if this is a "null string" parsed
                if (customValue != "NULL")
                    list.Data.Remove(entry.Id);
                else
                    keepOldValue = true;
            }

            if (keepOldValue) 
                return;

            var newListValue = new DataLookupEntry
            {
                Id = customId,
                Val = customValue,
                Extra = string.Empty
            };
            list.Data.Add(customId, newListValue);
            AllValues.Add(customValue);
        }
    }
}