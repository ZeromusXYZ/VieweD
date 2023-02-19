using VieweD.Helpers.System;

namespace VieweD.engine.common;

public class DataLookups
{
    // Default lookup table names for packet in and packet out names
    public static string LuPacketOut = "out";
    public static string LuPacketIn = "in";

    /// <summary>
    /// Empty LookupList to undefined searches
    /// </summary>
    public DataLookupList NullList { get; } = new ();

    /// <summary>
    /// Empty LookupEntry for undefined lookup keys
    /// </summary>
    public DataLookupEntry NullEntry { get; set; }

    /// <summary>
    /// A string list containing all added Values, used for auto-completion fields
    /// </summary>
    public List<string> AllValues { get; set; } = new ();

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
                    if (NumberHelper.TryFieldParse(fields[0], out int newId))
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
            catch (Exception ex)
            {
                AllLoadErrors += $"\n\r\n\rException loading {fileName} at line {lineNumber} :\n\r{ex.Message}\r\n=> {line}";
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
    public bool LoadLookups(string engineId, bool initialLoading)
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
    public DataLookupList NLU(string lookupName, string lookupOffsetString = "")
    {
        if (LookupLists.TryGetValue(lookupName, out var res))
            return res;
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
    /// <param name="packetDataDirection">Direction of the packet</param>
    /// <param name="packetId">The ID to lookup</param>
    /// <returns>PacketName or "??? unknown"</returns>
    public string PacketTypeToString(PacketDataDirection packetDataDirection, uint packetId)
    {
        var res = packetDataDirection switch
        {
            PacketDataDirection.Outgoing => NLU(LuPacketOut).GetValue(packetId),
            PacketDataDirection.Incoming => NLU(LuPacketIn).GetValue(packetId),
            _ => ""
        };

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

        // Handle §playerid special case
        if (customListName.StartsWith("§"))
        {
            /*
             // TODO: Implement Game Data Viewer

            if ((customListName == @"§playerid") && (GameViewForm.GV != null))
            {
                GameViewForm.GV.gbPlayer.Text = @"Player 0x" + customId.ToHex();
                GameViewForm.GV.lPlayerName.Text = customValue;
            }
            */
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