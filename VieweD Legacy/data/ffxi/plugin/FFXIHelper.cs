using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Win32;

namespace VieweD.Engine.FFXI
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class FFXIHelper
    {
        public struct FFXI_FTable_Item
        {
            public uint FileId; // Just for reference
            public byte RomFolder;
            public uint SubFolder;
            public uint DatFile;
            public string FullFileName;
        }

        static public string FFXI_InstallationPath { get; private set; }
        static public string POL_InstallationPath { get; private set; }
        static public string TetraMaster_InstallationPath { get; private set; }

        static public Dictionary<uint,FFXI_FTable_Item> FFXI_FTable = new Dictionary<uint, FFXI_FTable_Item>();

        private const string GameID_POL = "1000";
        private const string GameID_FFXI = "0001";
        private const string GameID_TetraMaster = "0002";
        // public const string GameID_FFXITC = "0015";

        public enum FFXI_ItemFlags : ushort
        {
            None = 0x0000,
            // Simple Flags - mostly assumed meanings
            WallHanging = 0x0001, // Used by furnishing like paintings.
            Flag01 = 0x0002,
            MysteryBox = 0x0004,  // Can be gained from Gobbie Mystery Box
            MogGarden = 0x0008,   // Can use in Mog Garden
            CanSendPOL = 0x0010,
            Inscribable = 0x0020,
            NoAuction = 0x0040,
            Scroll = 0x0080,
            Linkshell = 0x0100,
            CanUse = 0x0200,
            CanTradeNPC = 0x0400,
            CanEquip = 0x0800,
            NoSale = 0x1000,
            NoDelivery = 0x2000,
            NoTradePC = 0x4000,
            Rare = 0x8000,
            // Combined Flags
            Ex = 0x6040, // NoAuction + NoDelivery + NoTrade
        }

        public enum FFXI_ItemType : ushort
        {
            Nothing = 0x0000,
            Item = 0x0001,
            QuestItem = 0x0002,
            Fish = 0x0003,
            Weapon = 0x0004,
            Armor = 0x0005,
            Linkshell = 0x0006,
            UsableItem = 0x0007,
            Crystal = 0x0008,
            Currency = 0x0009,
            Furnishing = 0x000A,
            Plant = 0x000B,
            Flowerpot = 0x000C,
            PuppetItem = 0x000D,
            Mannequin = 0x000E,
            Book = 0x000F,
            RacingForm = 0x0010,
            BettingSlip = 0x0011,
            SoulPlate = 0x0012,
            Reflector = 0x0013,
            ItemType20 = 0x0014,
            LotteryTicket = 0x0015,
            MazeTabula_M = 0x0016,
            MazeTabula_R = 0x0017,
            MazeVoucher = 0x0018,
            MazeRune = 0x0019,
            ItemType_26 = 0x001A,
            StorageSlip = 0x001B,
            LegionPass = 0x001C,
            MeebleBurrows = 0x001D,
            Instincts = 0x001E,
            CraftingKit = 0x001F,
            Max = CraftingKit,
        }

        [Flags]
        private enum Regions
        {
            None = 0x00,
            JP = 0x01,
            NA = 0x02,
            EU = 0x04,
        }

        public enum FFXI_ItemDatFileTypes
        {
            Unknown,
            Armor,
            Currency,
            Item,
            PuppetItem,
            UsableItem,
            Weapon,
            ItemSlip,
            Instinct,
            Monipulator
        };

        /// <summary>
        /// Read Item data from PolUtils MassExtractor xml files
        /// </summary>
        /// <param name="ItemXmlFile"></param>
        /// <returns></returns>
        static public List<FFXI_Item> ReadItemListFromXML(string ItemXmlFile)
        {
            List<FFXI_Item> res = new List<FFXI_Item>();

            XmlDocument D = new XmlDocument();
            D.Load(ItemXmlFile);

            foreach (XmlNode itemNode in D.DocumentElement.ChildNodes)
            {
                if (itemNode.Name == "thing")
                {
                    var newItem = new FFXI_Item();
                    foreach (XmlNode itemElement in itemNode.ChildNodes)
                    {
                        if (itemElement.Name == "field")
                        {
                            var fieldName = itemElement.Attributes["name"]?.Value.ToLower();
                            var fieldValueText = itemElement?.InnerText;
                            uint.TryParse(fieldValueText, out uint fieldValueUInt);
                            uint.TryParse(fieldValueText, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture , out uint fieldValueUIntHex);

                            switch (fieldName)
                            {
                                case "id":
                                    newItem.Id = fieldValueUInt;
                                    break;
                                case "flags":
                                    newItem.Flags = (FFXI_ItemFlags)fieldValueUIntHex;
                                    break;
                                case "stack-size":
                                    newItem.StackSize = fieldValueUInt;
                                    break;
                                case "type":
                                    newItem.Type = (FFXI_ItemType)fieldValueUIntHex;
                                    break;
                                case "name":
                                    newItem.Name = fieldValueText;
                                    break;
                                case "description":
                                    newItem.Description = fieldValueText;
                                    break;
                                case "log-name-singular":
                                    newItem.NameSingle = fieldValueText;
                                    break;
                                case "log-name-plural":
                                    newItem.NameMultiple = fieldValueText;
                                    break;

                            }

                        }

                    }

                    // Only add a item if it has some usefull info in it
                    if ((newItem.Id > 0) && (newItem.Name != null) && (newItem.Name != string.Empty) && (newItem.Name != "."))
                    {
                        res.Add(newItem);
                    }
                }
            }

            return res;
        }

        static private RegistryKey OpenRegistryKey(Regions Region, string KeyName)
        {
            string SubKey;
            {
                string POLKeyName = "PlayOnline";
                switch (Region)
                {
                    case Regions.EU:
                        POLKeyName += "EU";
                        break;
                    case Regions.JP:
                        break;
                    case Regions.NA:
                        POLKeyName += "US";
                        break;
                    default:
                        return null;
                }
                SubKey = Path.Combine(POLKeyName, KeyName);
            }
            try
            {
                using (RegistryKey Win64Root = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432Node"))
                {
                    if (Win64Root != null)
                    {
                        RegistryKey Win64Key = Win64Root.OpenSubKey(SubKey, false);
                        if (Win64Key != null)
                        {
                            return Win64Key;
                        }
                    }
                }
            }
            catch
            {
                // Ignore
            }
            try
            {
                return Registry.LocalMachine.OpenSubKey(Path.Combine("Software", SubKey), false);
            }
            catch
            {
                return null;
            }
        }

        static private string GetInstalledGamePath(string GameID, Regions Region)
        {
            RegistryKey POLKey = OpenRegistryKey(Region, "InstallFolder");
            if (POLKey == null)
            {
                return string.Empty;
            }
            string InstallPath = POLKey.GetValue(GameID, string.Empty) as string;
            POLKey.Close();
            if ((InstallPath != string.Empty) && (!InstallPath.EndsWith(Path.DirectorySeparatorChar.ToString())))
                InstallPath += Path.DirectorySeparatorChar;
            return InstallPath;
        }

        static private bool FFXI_LoadFileTables()
        {
            // Do we have a installation ?
            if ((FFXI_InstallationPath == null) || (FFXI_InstallationPath == string.Empty) || (!Directory.Exists(FFXI_InstallationPath)))
                return false;
            
            // Did we already load this ?
            if (FFXI_FTable.Count > 0)
                return true;

            try
            {
                byte[] FFXI_VTable = new byte[0];
                FFXI_FTable.Clear();

                for (byte RomDir = 1; RomDir < 0xF;RomDir++)
                {
                    string RomDirPath = FFXI_InstallationPath;
                    if (RomDir != 1)
                        RomDirPath = Path.Combine(FFXI_InstallationPath, "Rom" + RomDir.ToString());
                    else
                        RomDirPath = Path.Combine(FFXI_InstallationPath, "Rom");

                    string VTableFile = FFXI_InstallationPath;
                    if (RomDir != 1)
                        VTableFile = Path.Combine(VTableFile, "Rom" + RomDir.ToString(), "VTABLE" + RomDir.ToString() + ".DAT");
                    else
                        VTableFile = Path.Combine(VTableFile, "VTABLE.DAT");

                    string FTableFile = FFXI_InstallationPath;
                    if (RomDir != 1)
                        FTableFile = Path.Combine(FTableFile, "Rom" + RomDir.ToString(), "FTABLE" + RomDir.ToString() + ".DAT");
                    else
                        FTableFile = Path.Combine(FTableFile, "FTABLE.DAT");

                    if ((!File.Exists(VTableFile)) || (!File.Exists(FTableFile)))
                        continue;

                    // Open VTable and FTable files for this ROM Folder
                    FileStream VTableFS = new FileStream(VTableFile, FileMode.Open, FileAccess.Read);
                    FileStream FTableFS = new FileStream(FTableFile, FileMode.Open, FileAccess.Read);

                    // File Size check
                    if (FTableFS.Length != (VTableFS.Length * 2))
                        throw new Exception("File table size mismatch for Rom Folder " + RomDir.ToString());

                    // Change VTable size if needed
                    if (FFXI_VTable.Length < VTableFS.Length)
                        Array.Resize(ref FFXI_VTable, (int)VTableFS.Length);

                    BinaryReader VTableBR = new BinaryReader(VTableFS);
                    BinaryReader FTableBR = new BinaryReader(FTableFS);
                    for (uint i = 0; i < VTableBR.BaseStream.Length;i++)
                    {
                        byte thisRomFolder = VTableBR.ReadByte();
                        ushort thisRomFileLocation = FTableBR.ReadUInt16();

                        if (thisRomFolder > 0)
                        {
                            // Sanity Check
                            if (FFXI_VTable[i] != 0)
                                throw new Exception("File ID " + i.ToString() + " in " + VTableFile + " already has a ROM folder set");
                            FFXI_VTable[i] = thisRomFolder;

                            ushort SubDirLocation = (ushort)(thisRomFileLocation / 0x80);
                            byte FileLocation = (byte)(thisRomFileLocation % 0x80);

                            var FTableEntry = new FFXI_FTable_Item();
                            FTableEntry.FileId = i;
                            FTableEntry.RomFolder = RomDir;
                            FTableEntry.SubFolder = SubDirLocation;
                            FTableEntry.DatFile = FileLocation;
                            FTableEntry.FullFileName = Path.Combine(RomDirPath, SubDirLocation.ToString(),FileLocation.ToString()+".dat");
                            FFXI_FTable.Add(i, FTableEntry);
                        }
                    }
                    
                }
                
            }
            catch 
            {
                return false;
            }
            return true;
        }

        static public bool FindPaths()
        {
            // PlayOnline
            if (POL_InstallationPath == null)
                POL_InstallationPath = string.Empty;

            if (POL_InstallationPath == string.Empty)
                POL_InstallationPath = GetInstalledGamePath(GameID_POL, Regions.JP);

            if (POL_InstallationPath == string.Empty)
                POL_InstallationPath = GetInstalledGamePath(GameID_POL, Regions.NA);

            if (POL_InstallationPath == string.Empty)
                POL_InstallationPath = GetInstalledGamePath(GameID_POL, Regions.EU);

            // Final Fantasy XI
            if (FFXI_InstallationPath == null)
                FFXI_InstallationPath = string.Empty;

            if (FFXI_InstallationPath == string.Empty)
                FFXI_InstallationPath = GetInstalledGamePath(GameID_FFXI, Regions.JP);

            if (FFXI_InstallationPath == string.Empty)
                FFXI_InstallationPath = GetInstalledGamePath(GameID_FFXI, Regions.NA);

            if (FFXI_InstallationPath == string.Empty)
                FFXI_InstallationPath = GetInstalledGamePath(GameID_FFXI, Regions.EU);

            // Tetra Master
            if (TetraMaster_InstallationPath == null)
                TetraMaster_InstallationPath = string.Empty;

            if (TetraMaster_InstallationPath == string.Empty)
                TetraMaster_InstallationPath = GetInstalledGamePath(GameID_TetraMaster, Regions.JP);

            if (TetraMaster_InstallationPath == string.Empty)
                TetraMaster_InstallationPath = GetInstalledGamePath(GameID_TetraMaster, Regions.NA);

            if (TetraMaster_InstallationPath == string.Empty)
                TetraMaster_InstallationPath = GetInstalledGamePath(GameID_TetraMaster, Regions.EU);

            return FFXI_LoadFileTables();
        }

        static public void FFXI_LoadItemsFromDats(ref Dictionary<uint, FFXI_Item> itemList)
        {
            itemList.Clear();

            Dictionary<uint, FFXI_Item> items = new Dictionary<uint, FFXI_Item>();

            void Add(ref Dictionary<uint, FFXI_Item> iList, uint datFile)
            {
                var newItems = ReadItemListFromDatFile(datFile);
                var union = iList.Union(newItems);
                iList = union.ToDictionary(k => k.Key, v => v.Value);
            }

            Add(ref items, 73);      // General
            Add(ref items, 55671);   // General 2
            Add(ref items, 74);      // Useable
            Add(ref items, 75);      // Weapons
            Add(ref items, 76);      // Armor
            Add(ref items, 55668);   // Armor 2
            Add(ref items, 77);      // Puppet
            Add(ref items, 91);      // Currency
            Add(ref items, 55667);   // Vouchers
            Add(ref items, 55669);   // Monipulator
            Add(ref items, 55670);   // Instincts

            itemList = items.OrderBy(d => d.Value).ToDictionary(k => k.Key, v => v.Value);
        }

        static private FFXI_ItemDatFileTypes FFXI_DeduceItemType(BinaryReader BR)
        {
            FFXI_ItemDatFileTypes ItemType = FFXI_ItemDatFileTypes.Unknown;
            byte[] FirstItem = null;
            long Position = BR.BaseStream.Position;
            BR.BaseStream.Position = 0;
            try
            {
                while (BR.BaseStream.Position != BR.BaseStream.Length)
                {
                    FirstItem = BR.ReadBytes(0x4);
                    BR.BaseStream.Position += (0xc00 - 0x4);
                    FFXIEncryption.Rotate(FirstItem, 5);
                    {
                        // Type -> Based on ID
                        uint ID = 0;
                        for (int i = 0; i < 4; ++i)
                        {
                            ID <<= 8;
                            ID += FirstItem[3 - i];
                        }

                        if (ID == 0xFFFF)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Currency;
                        }
                        else if (ID < 0x1000)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Item;
                        }
                        else if (ID < 0x2000)
                        {
                            ItemType = FFXI_ItemDatFileTypes.UsableItem;
                        }
                        else if (ID < 0x2200)
                        {
                            ItemType = FFXI_ItemDatFileTypes.PuppetItem;
                        }
                        else if (ID < 0x2800)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Item;
                        }
                        else if (ID < 0x4000)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Armor;
                        }
                        else if (ID < 0x5A00)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Weapon;
                        }
                        else if (ID < 0x7000)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Armor;
                        }
                        else if (ID < 0x7400)
                        {
                            ItemType = FFXI_ItemDatFileTypes.ItemSlip;
                        }
                        else if (ID < 0x7800)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Instinct;
                        }
                        else if (ID < 0xF200)
                        {
                            ItemType = FFXI_ItemDatFileTypes.Monipulator;
                        }
                    }
                    if (ItemType != FFXI_ItemDatFileTypes.Unknown)
                    {
                        break;
                    }
                }
            }
            catch
            {
                // Ignore
            }
            BR.BaseStream.Position = Position;
            return ItemType;
        }

        static private Dictionary<uint, FFXI_Item> ReadItemListFromDatFile(uint DatFileId)
        {
            Dictionary<uint, FFXI_Item> res = new Dictionary<uint, FFXI_Item>();

            if (!FFXI_FTable.TryGetValue(DatFileId, out var FileDatInfo))
                return res;

            if (!File.Exists(FileDatInfo.FullFileName))
                return res;

            var FS = new FileStream(FileDatInfo.FullFileName, FileMode.Open, FileAccess.Read);
            // needs to be in 3072 byte block size, and a minimum of 16 entries
            if (((FS.Length % 0x0C00) != 0) || (FS.Length < 0xC000))
                return res;
            var BR = new BinaryReader(FS);
            var ExpectedItemType = FFXI_DeduceItemType(BR);

            var itemCount = BR.BaseStream.Length / 0x0C00;
            for (var i = 0; i < itemCount; i++)
            {
                BR.BaseStream.Seek(i * 0x0C00, SeekOrigin.Begin);
                byte[] ItemBytes = BR.ReadBytes(0xC00);
                FFXIEncryption.Rotate(ItemBytes, 5);
                var MemBR = new BinaryReader(new MemoryStream(ItemBytes, false));

                FFXI_Item item = new FFXI_Item();
                // Common Fields (14 bytes)
                item.Id = MemBR.ReadUInt32();
                item.Flags = (FFXI_ItemFlags)MemBR.ReadUInt16();
                item.StackSize = MemBR.ReadUInt16(); // 0xe0ff for Currency, which kinda suggests this is really 2 separate bytes
                item.Type = (FFXI_ItemType)MemBR.ReadUInt16();
                MemBR.ReadUInt16(); // item.ResourceID_ = MemBR.ReadUInt16();
                MemBR.ReadUInt16(); // item.ValidTargets_ = (ValidTarget)MemBR.ReadUInt16();
                // Extra Fields (22/30/10/6/2 bytes for Armor/Weapon/Puppet/Item/UsableItem)

                if (item.Type == FFXI_ItemType.Nothing)
                    continue;

                switch(ExpectedItemType)
                {
                    case FFXI_ItemDatFileTypes.Armor:
                    case FFXI_ItemDatFileTypes.Weapon:
                        MemBR.ReadUInt16(); // item.Level_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt16(); // item.Slots_ = (EquipmentSlot)MemBR.ReadUInt16();
                        MemBR.ReadUInt16(); // item.Races_ = (Race)MemBR.ReadUInt16();
                        MemBR.ReadUInt32(); // item.Jobs_ = (Job)MemBR.ReadUInt32();
                        MemBR.ReadUInt16(); //item.SuperiorLevel_ = MemBR.ReadUInt16();
                        if (ExpectedItemType == FFXI_ItemDatFileTypes.Armor)
                        {
                            MemBR.ReadUInt16(); // item.ShieldSize_ = MemBR.ReadUInt16();
                        }
                        else
                        {
                            // Weapon
                            MemBR.ReadUInt16(); // item.Unknown4_ = MemBR.ReadUInt16();
                            MemBR.ReadUInt16(); // item.Damage_ = MemBR.ReadUInt16();
                            MemBR.ReadInt16(); // item.Delay_ = MemBR.ReadInt16();
                            MemBR.ReadUInt16(); // item.DPS_ = MemBR.ReadUInt16();
                            MemBR.ReadByte(); // item.Skill_ = (Skill)MemBR.ReadByte();
                            MemBR.ReadByte(); // item.JugSize_ = MemBR.ReadByte();
                            MemBR.ReadUInt32(); // item.Unknown1_ = MemBR.ReadUInt32();
                        }
                        MemBR.ReadByte(); // item.MaxCharges_ = MemBR.ReadByte();
                        MemBR.ReadByte(); // item.CastingTime_ = MemBR.ReadByte();
                        MemBR.ReadUInt16(); // item.UseDelay_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt32(); // item.ReuseDelay_ = MemBR.ReadUInt32();
                        MemBR.ReadUInt16(); // item.Unknown2_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt16(); // item.iLevel_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                        break;

                    case FFXI_ItemDatFileTypes.PuppetItem:
                        MemBR.ReadUInt16(); // item.PuppetSlot_ = (PuppetSlot)MemBR.ReadUInt16();
                        MemBR.ReadUInt32(); // item.ElementCharge_ = MemBR.ReadUInt32();
                        MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                        break;

                    case FFXI_ItemDatFileTypes.Instinct:
                        MemBR.ReadUInt32();
                        MemBR.ReadUInt32();
                        MemBR.ReadUInt16();
                        MemBR.ReadUInt16(); // item.InstinctCost_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt16();
                        MemBR.ReadUInt32();
                        MemBR.ReadUInt32();
                        MemBR.ReadUInt32();
                        break;

                    case FFXI_ItemDatFileTypes.Item:
                        switch (item.Type)
                        {
                            case FFXI_ItemType.Flowerpot:
                            case FFXI_ItemType.Furnishing:
                            case FFXI_ItemType.Mannequin:
                                MemBR.ReadUInt16(); // item.Element_ = (Element)MemBR.ReadUInt16();
                                MemBR.ReadInt32(); // item.StorageSlots_ = MemBR.ReadInt32();
                                MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                                break;
                            default:
                                MemBR.ReadUInt16(); // item.Unknown2_ = MemBR.ReadUInt16();
                                MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                                MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                                break;
                        }
                        break;

                    case FFXI_ItemDatFileTypes.UsableItem:
                        MemBR.ReadUInt16(); // item.ActivationTime_ = MemBR.ReadUInt16();
                        MemBR.ReadUInt32(); // item.Unknown1_ = MemBR.ReadUInt32();
                        MemBR.ReadUInt32(); // item.Unknown3_ = MemBR.ReadUInt32();
                        MemBR.ReadUInt32(); // item.Unknown4_ = MemBR.ReadUInt32();
                        break;

                    case FFXI_ItemDatFileTypes.Currency:
                        MemBR.ReadUInt16(); // item.Unknown2_ = MemBR.ReadUInt16();
                        break;

                    case FFXI_ItemDatFileTypes.ItemSlip:
                        MemBR.ReadUInt16(); // item.Unknown1_ = MemBR.ReadUInt16();
                        for (int counter = 0; counter < 17; counter++)
                        {
                            MemBR.ReadUInt32();
                        }
                        break;
                    case FFXI_ItemDatFileTypes.Monipulator:
                        MemBR.ReadUInt16(); // item.Unknown1_ = MemBR.ReadUInt16();
                        for (int counter = 0; counter < 24; counter++)
                        {
                            MemBR.ReadInt32();
                        }
                        break;
                    default:
                        // If this is a unknown expected file type, ignore and exit
                        return res;
                }

                if (item.Type > FFXI_ItemType.Max)
                {
                    // Invalid item type ?
                    continue;
                }

                // Next Up: Strings (variable size)
                long StringBase = MemBR.BaseStream.Position;
                uint StringCount = MemBR.ReadUInt32();
                if (StringCount > 9)
                {
                    // Sanity check, for safety - 0 strings is fine for now
                    // item.Clear();
                    continue;
                }
                FFXIEncoding E = new FFXIEncoding();
                string[] Strings = new string[StringCount];
                for (byte iStrings = 0; iStrings < StringCount; iStrings++)
                {
                    long Offset = StringBase + MemBR.ReadUInt32();
                    uint Flag = MemBR.ReadUInt32();
                    if (Offset < 0 || Offset + 0x20 > 0x280 || (Flag != 0 && Flag != 1))
                    {
                        // item.Clear();
                        StringCount = 0;
                        break;
                    }
                    // Flag seems to be 1 if the offset is not actually an offset. Could just be padding to make StringCount unique per language, or it could be an indication
                    // of the pronoun to use (a/an/the/...). The latter makes sense because of the increased number of such flags for french and german.
                    if (Flag == 0)
                    {
                        MemBR.BaseStream.Position = Offset;
                        Strings[iStrings] = FFXI_ReadString(MemBR, E);
                        if (Strings[iStrings] == null)
                        {
                            // item.Clear();
                            StringCount = 0;
                            break;
                        }
                        MemBR.BaseStream.Position = StringBase + 4 + 8 * (iStrings + 1);
                    }
                }
                // Assign the strings to the proper fields
                switch (StringCount)
                {
                    case 0:
                        // Temporary hack until I can figure out what the hell is wrong with reading the strings
                        if ((Strings.Length > 1) && (Strings[0] != null))
                            item.Name = Strings[0];
                        else
                            item.Name = "<no name found>";
                        // item.Name = item.Type.ToString() + " - " + item.Id.ToString();
                        break;
                    case 1:
                        item.Name = Strings[0];
                        break;
                    case 2: // Japanese
                        item.Name = Strings[0];
                        item.Description = Strings[1];
                        break;
                    case 5: // English
                        item.Name = Strings[0];
                        // unused:              Strings[1]
                        item.NameSingle = Strings[2];
                        item.NameMultiple = Strings[3];
                        item.Description = Strings[4];
                        break;
                    case 6: // French
                        item.Name = Strings[0];
                        // unused:              Strings[1]
                        // unused:              Strings[2]
                        item.NameSingle = Strings[3];
                        item.NameMultiple = Strings[4];
                        item.Description = Strings[5];
                        break;
                    case 9: // German
                        item.Name = Strings[0];
                        // unused:              Strings[1]
                        // unused:              Strings[2]
                        // unused:              Strings[3]
                        item.NameSingle = Strings[4];
                        // unused:              Strings[5]
                        // unused:              Strings[6]
                        item.NameMultiple = Strings[7];
                        item.Description = Strings[8];
                        break;
                }
                MemBR.Close();

                if ((item.Name != null) && (item.Name != string.Empty) && (item.Name != "."))
                    res.Add(item.Id, item);
            }
            return res;
        }

        static private string FFXI_ReadString(BinaryReader ItemMemoryBR, FFXIEncoding E)
        {
            // Read past "padding"
            if (ItemMemoryBR.ReadUInt32() != 1)
            {
                return null;
            }
            for (byte i = 0; i < 6; ++i)
            {
                if (ItemMemoryBR.ReadUInt32() != 0)
                {
                    return null;
                }
            }
            List<byte> TextBytes = new List<byte>();
            while (ItemMemoryBR.BaseStream.Position < 0x280)
            {
                byte[] Next4 = ItemMemoryBR.ReadBytes(4);
                byte UsableBytes = (byte)Next4.Length;
                while (UsableBytes > 0 && Next4[UsableBytes - 1] == 0)
                {
                    --UsableBytes;
                }
                if (UsableBytes != 4)
                {
                    byte i = 0;
                    while (UsableBytes-- > 0)
                    {
                        TextBytes.Add(Next4[i++]);
                    }
                    // return E.GetString(TextBytes.ToArray()).Replace("\n", Environment.NewLine);
                    return FFXIEncoding.UTF8.GetString(TextBytes.ToArray()).Replace("\n", Environment.NewLine);
                }
                else
                {
                    TextBytes.AddRange(Next4);
                }
            }
            return null;
        }

        static private bool ReadDialogTableEntryInsideFile(BinaryReader BR, ref FFXI_DialogTableEntry DTE, uint? Index, long EntryStart, long EntryEnd)
        {
            if (Index == null)
                throw new Exception("Dialog entry is NULL");
            if (Index > ushort.MaxValue)
                throw new Exception("Dialog entry too high " + Index.ToString());
            DTE.zoneId = 0;
            DTE.Text = string.Empty;
            DTE.Id = (ushort)Index;
            try
            {
                BR.BaseStream.Seek(4 + EntryStart, SeekOrigin.Begin);
                byte[] TextBytes = BR.ReadBytes((int)(EntryEnd - EntryStart));
                for (int i = 0; i < TextBytes.Length; ++i)
                {
                    TextBytes[i] ^= 0x80; // <= Evil encryption-breaking!
                }
                DTE.Text = String.Empty;
                // FFXIEncoding E = new FFXIEncoding();
                int LastPos = 0;
                for (int i = 0; i < TextBytes.Length; ++i)
                {
                    if (TextBytes[i] == 0x07)
                    {
                        // Line Break
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += "\r\n";
                        LastPos = i + 1;
                    }
                    else if (TextBytes[i] == 0x08)
                    {
                        // Character Name (You)
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Player Name{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd);
                        LastPos = i + 1;
                    }
                    else if (TextBytes[i] == 0x09)
                    {
                        // Character Name (They)
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Speaker Name{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd);
                        LastPos = i + 1;
                    }
                    else if (TextBytes[i] == 0x0a && i + 1 < TextBytes.Length)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Numeric Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x0b)
                    {
                        // Indicates that the lines after this are in a prompt window
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Selection Dialog{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd);
                        LastPos = i + 1;
                    }
                    else if (TextBytes[i] == 0x0c && i + 1 < TextBytes.Length)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Multiple Choice (Parameter {2}){1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x19 && i + 1 < TextBytes.Length)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Item Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x1a && i + 1 < TextBytes.Length)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Key Item Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x1c && i + 1 < TextBytes.Length)
                    {
                        // Chocobo Name
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Player/Chocobo Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x1e && i + 1 < TextBytes.Length)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Set Color #{2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x7f && i + 1 < TextBytes.Length)
                    {
                        // Various stuff
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        if (TextBytes[i + 1] == 0x31 && i + 2 < TextBytes.Length)
                        {
                            // Unknown, but seems to indicate user needs to hit RET
                            if (TextBytes[i + 2] != 0)
                            {
                                DTE.Text += String.Format("{0}{2}-Second Delay + Prompt{1}", FFXIEncoding.SpecialMarkerStart,
                                    FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 2]);
                            }
                            else
                            {
                                DTE.Text += String.Format("{0}Prompt{1}", FFXIEncoding.SpecialMarkerStart,
                                    FFXIEncoding.SpecialMarkerEnd);
                            }
                            ++LastPos;
                            ++i;
                        }
                        else if (TextBytes[i + 1] == 0x85) // Multiple Choice: Player Gender
                        {
                            DTE.Text += String.Format("{0}Multiple Choice (Player Gender){1}", FFXIEncoding.SpecialMarkerStart,
                                FFXIEncoding.SpecialMarkerEnd);
                        }
                        else if (TextBytes[i + 1] == 0x8D && i + 2 < TextBytes.Length)
                        {
                            DTE.Text += String.Format("{0}Weather Event Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                                FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 2]);
                            ++LastPos;
                            ++i;
                        }
                        else if (TextBytes[i + 1] == 0x8E && i + 2 < TextBytes.Length)
                        {
                            DTE.Text += String.Format("{0}Weather Type Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                                FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 2]);
                            ++LastPos;
                            ++i;
                        }
                        else if (TextBytes[i + 1] == 0x92 && i + 2 < TextBytes.Length)
                        {
                            DTE.Text += String.Format("{0}Singular/Plural Choice (Parameter {2}){1}",
                                FFXIEncoding.SpecialMarkerStart, FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 2]);
                            ++LastPos;
                            ++i;
                        }
                        else if (TextBytes[i + 1] == 0xB1 && i + 2 < TextBytes.Length)
                        {
                            // Usually found before an item name or key item name
                            DTE.Text += String.Format("{0}Title Parameter {2}{1}", FFXIEncoding.SpecialMarkerStart,
                                FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 2]);
                            ++LastPos;
                            ++i;
                        }
                        else if (i + 2 < TextBytes.Length)
                        {
                            DTE.Text += String.Format("{0}Unknown Parameter (Type: {2:X2}) {3}{1}",
                                FFXIEncoding.SpecialMarkerStart, FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1], TextBytes[i + 2]);
                            ++LastPos;
                            ++i;
                        }
                        else
                        {
                            DTE.Text += String.Format("{0}Unknown Marker Type: {2:X2}{1}", FFXIEncoding.SpecialMarkerStart,
                                FFXIEncoding.SpecialMarkerEnd, TextBytes[i + 1]);
                        }
                        LastPos = i + 2;
                        ++i;
                    }
                    else if (TextBytes[i] == 0x7f || TextBytes[i] < 0x20)
                    {
                        if (LastPos < i)
                        {
                            DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, i - LastPos);
                        }
                        DTE.Text += String.Format("{0}Possible Special Code: {2:X2}{1}", FFXIEncoding.SpecialMarkerStart,
                            FFXIEncoding.SpecialMarkerEnd, TextBytes[i]);
                        LastPos = i + 1;
                    }
                }
                if (LastPos < TextBytes.Length)
                {
                    DTE.Text += FFXIEncoding.UTF8.GetString(TextBytes, LastPos, TextBytes.Length - LastPos);
                }
                DTE.Text = DTE.Text.TrimEnd('\0');
                return true;
            }
            catch
            {
                return false;
            }
        }

        static private Dictionary<uint, FFXI_DialogTableEntry> ReadDialogListFromDatFile(uint datFile, ushort zoneId)
        {
            var res = new Dictionary<uint, FFXI_DialogTableEntry>();

            if (!FFXI_FTable.TryGetValue(datFile, out var FileDatInfo))
                return res;

            if (!File.Exists(FileDatInfo.FullFileName))
                return res;

            var FS = new FileStream(FileDatInfo.FullFileName, FileMode.Open, FileAccess.Read);
            var BR = new BinaryReader(FS);

            if (BR.BaseStream.Length < 4)
                return res;

            uint FileSizeMaybe = BR.ReadUInt32();
            if (FileSizeMaybe != (0x10000000 + BR.BaseStream.Length - 4))
                return res;

            uint FirstTextPos = (BR.ReadUInt32() ^ 0x80808080);
            if ((FirstTextPos % 4) != 0 || FirstTextPos > BR.BaseStream.Length || FirstTextPos < 8)
                return res;

            uint EntryCount = FirstTextPos / 4;
            // The entries are usually, but not always, sequential in the file.
            // Because we need to know how long one entry is (no clear end-of-message marker), we need them in
            // sequential order.
            List<uint> Entries = new List<uint>((int)EntryCount + 1);
            Entries.Add(FirstTextPos);
            for (int i = 1; i < EntryCount; ++i)
                Entries.Add(BR.ReadUInt32() ^ 0x80808080);

            Entries.Add((uint)BR.BaseStream.Length - 4);
            Entries.Sort();
            for (uint i = 0; i < EntryCount; ++i)
            {
                if (Entries[(int)i] < 4 * EntryCount || 4 + Entries[(int)i] >= BR.BaseStream.Length)
                {
                    res.Clear();
                    break;
                }
                FFXI_DialogTableEntry DTE = new FFXI_DialogTableEntry();
                if (!ReadDialogTableEntryInsideFile(BR, ref DTE, i, Entries[(int)i], Entries[(int)i + 1]))
                {
                    res.Clear();
                    break;
                }
                DTE.zoneId = zoneId;
                res.Add(DTE.KeyIndex,DTE);
            }
            return res;

        }

        /// <summary>
        /// Loads dialog text for a specified zone into memory
        /// </summary>
        /// <param name="dialogList"></param>
        /// <param name="specifiedZone">If no zone specified, it will load all the zones (takes very long)</param>
        /// <returns></returns>
        static public bool FFXI_LoadDialogsFromDats(ref Dictionary<uint, FFXI_DialogTableEntry> dialogList,int specifiedZone = -1)
        {
            bool res = false;

            Dictionary<uint, FFXI_DialogTableEntry> dialogs = new Dictionary<uint, FFXI_DialogTableEntry>();

            bool Add(ref Dictionary<uint, FFXI_DialogTableEntry> dList, int zoneId)
            {
                int datFile ;

                // Calculate the dat file number from zone
                if ((zoneId >= 0x000) && (zoneId <= 0x0FF))
                    datFile = zoneId + 6420;
                else if ((zoneId >= 0x100) && (zoneId <= 0x1FF))
                    datFile = zoneId + 85590 - 256 ;
                else
                    datFile = -1;

                if (datFile < 0)
                    return false;
                try
                {
                    var newItems = ReadDialogListFromDatFile((uint)datFile, (ushort)zoneId);
                    var union = dList.Union(newItems);
                    dList = union.ToDictionary(k => k.Key, v => v.Value);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            if (specifiedZone >= 0)
                res = Add(ref dialogs, specifiedZone);
            else
            {
                // Load all the zones o/
                // Dialog Tables
                dialogList.Clear();
                for (ushort i = 0; i < 0x1FF; ++i)
                    res = Add(ref dialogs, i) || res ;
                // Add(ref dialogs, "50-2", (uint)57945); // Whitegate's 2nd table ?

                /*
                // first 256 zones
                for (ushort i = 0; i < 0x100; ++i)
                    Add(ref dialogs, i, (uint)6420 + i);
                Add(ref dialogs, 50, (uint)57945); // Whitegate's 2nd table ?
                // next 256 zones
                for (ushort i = 0; i < 0x100; ++i)
                    Add(ref dialogs, (ushort)(i + 0x100), (uint)85590 + i);
                */
            }

            // We don't sort this, it should already be in order
            dialogList = dialogs.OrderBy(d => d.Value).ToDictionary(k => k.Key, v => v.Value);
            return res;
        }

        /// <summary>
        /// Loads NPC names for a specified zone, special instances are not yet supported
        /// </summary>
        /// <param name="mobList"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        static public bool FFXI_LoadMobListForZone(ref Dictionary<uint, FFXI_MobListEntry> mobList,uint zoneId)
        {
            uint datFile = zoneId + 0 ;
            if ((zoneId >= 0) && (zoneId < 0x100))
            {
                datFile = zoneId + 6720;
            }
            else
            if ((zoneId >= 0x100) && (zoneId < 0x200))
            {
                datFile = zoneId + 86491 - 0x100 ;
            }
            else
            {
                datFile = zoneId + 67910; // Special instanced zones
            }

            if (!FFXI_FTable.TryGetValue(datFile, out var FileDatInfo))
                return false;

            if (!File.Exists(FileDatInfo.FullFileName))
                return false;

            var FS = new FileStream(FileDatInfo.FullFileName, FileMode.Open, FileAccess.Read);
            var BR = new BinaryReader(FS);

            if ((BR.BaseStream.Length % 0x20) != 0 || BR.BaseStream.Position != 0)
            {
                return false ;
            }
            long EntryCount = BR.BaseStream.Length / 0x20;
            try
            {
                int ZoneID = -1;
                for (int i = 0; i < EntryCount; ++i)
                {
                    FFXI_MobListEntry MLE = new FFXI_MobListEntry();

                    try
                    {
                        MLE.Name = FFXIEncoding.UTF8.GetString(BR.ReadBytes(0x1C)).TrimEnd('\0');
                        MLE.Id = BR.ReadUInt32();
                        // ID seems to be 010 + zone id + mob id (=> there's a hard max of 0xFFF (= 4095) mobs per zone, which seems plenty :))
                        // Special 'instanced' zones like MMM or Meebles use 013 + 'zone id' + mob id.
                        if ((MLE.Id != 0 && (MLE.Id & 0xFFF00000) != 0x01000000) &&
                            (MLE.Id != 0 && (MLE.Id & 0xFFF00000) != 0x01300000) &&
                            (MLE.Id != 0 && (MLE.Id & 0xFFF00000) != 0x01100000))
                        {
                            continue;
                        }
                        MLE.ExpectedZoneId = (ushort)((MLE.Id & 0x001FF000) >> 12);
                    }
                    catch
                    {
                        continue;
                    }

                    // First entry should always be "none"
                    if (i == 0 && (MLE.Id != 0 || MLE.Name != "none"))
                    {
                        break;
                    }
                    else if (i > 0)
                    {
                        // Sanity-check
                        // Entire file should be for 1 specific zone
                        if (ZoneID < 0)
                        {
                            ZoneID = MLE.ExpectedZoneId;
                        }
                        else if (ZoneID != MLE.ExpectedZoneId)
                        {
                            break;
                        }
                        mobList.Add(MLE.Id, MLE);
                    }
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
