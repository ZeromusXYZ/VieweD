using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using VieweD.Helpers.System;
using VieweD.Properties;

namespace VieweD.engine.common;

public class BasePacketData
{
    public ViewedProjectTab ParentProject { get; set; }
    public int ThisIndex { get; set; }
    public bool IsVisible { get; set; }
    public bool MarkedAsDimmed { get; set; }
    public bool MarkedAsInvalid { get; set; }
    public uint PacketId { get; set; }
    public int SyncId { get; set; }
    /// <summary>
    /// Compression CompressionLevel of the packet if applicable
    /// </summary>
    public byte CompressionLevel { get; set; }
    public byte StreamId => ParentProject.GetExpectedStreamIdByPort(SourcePort, 0).Item1;

    public string HeaderText { get; set; }
    public string OriginalHeaderText { get; set; }
    public PacketDataDirection PacketDataDirection { get; set; }
    public List<byte> ByteData { get; set; }
    public int PacketDataSize { get; set; }
    private int _cursor;
    private byte _bitCursor;

    public int Cursor
    {
        get => _cursor;
        set
        {
            _cursor = value;
            BitCursor = 0;
        }
    }

    public byte BitCursor
    {
        get => _bitCursor;
        set
        {
            var addBytes = (value >= 8) ? value / 8 : 0;
            if (addBytes > 0) Cursor += addBytes;
            _bitCursor = (byte)(value % 8);
        }
    }

    public List<ParsedField> ParsedData { get; set; }
    public List<string> SourceText { get; set; }
    public string SourceMac { get; set; }
    public string SourceIp { get; set; }
    public ushort SourcePort { get; set; }
    public string DestinationMac { get; set; }
    public string DestinationIp { get; set; }
    public ushort DestinationPort { get; set; }
    public DateTime TimeStamp { get; set; }
    public TimeSpan OffsetFromStart { get; set; }
    public TimeSpan VirtualOffsetFromStart { get; set; }
    public string ParsedPacketName { get; set; }

    public BasePacketData? ParentPacket { get; set; }
    public List<BasePacketData> SubPackets { get; set; }
    public bool IsTruncatedByParser { get; set; }
    public int UnParseSubPacketCount { get; set; }
    public bool DoNotParse { get; set; }

    public BasePacketData(ViewedProjectTab parentProject)
    {
        ParentProject = parentProject;
        ThisIndex = -1;
        IsVisible = true;
        MarkedAsDimmed = false;
        MarkedAsInvalid = false;
        PacketId = 0;
        ParsedPacketName = string.Empty;
        SyncId = 0;
        CompressionLevel = 0;
        HeaderText = "Header";
        OriginalHeaderText = HeaderText;
        PacketDataDirection = PacketDataDirection.Incoming;
        ByteData = new List<byte>();
        PacketDataSize = 0;
        ParsedData = new List<ParsedField>();
        SourceText = new List<string>();
        SourceMac = string.Empty;
        SourceIp = string.Empty;
        SourcePort = 0;
        DestinationMac = string.Empty;
        DestinationIp = string.Empty;
        DestinationPort = 0;
        TimeStamp = DateTime.MinValue;
        VirtualOffsetFromStart = TimeSpan.Zero;
        OffsetFromStart = TimeSpan.Zero;
        Cursor = 0;
        BitCursor = 0;
        ParentPacket = null;
        SubPackets = new List<BasePacketData>();
        IsTruncatedByParser = false;
        UnParseSubPacketCount = 0;
    }

    public ParsedField? GetParsedFieldByByteIndex(int index, bool preferSelectedFields = false)
    {
        // Capture ParsedData as Span to increase speed as a lot of time is spend here
        var parsedSpan = CollectionsMarshal.AsSpan(ParsedData);

        // If enabled, check in selected fields first
        if (preferSelectedFields)
        {
            foreach (var parsed in parsedSpan)
            {
                if ((parsed.HasValue == false) || (parsed.IsSelected == false))
                    continue;

                if ((index >= parsed.StartingByte) && (index <= parsed.EndingByte))
                    return parsed;
            }
        }

        // Do a normal first come, first served
        foreach (var parsed in parsedSpan)
        {
            if (!parsed.HasValue) 
                continue;

            if ((index >= parsed.StartingByte) && (index <= parsed.EndingByte)) 
                return parsed;
        }

        return null;
    }

    public ParsedField? GetFirstParsedFieldByName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
            return null;
        foreach (var parsed in ParsedData)
            if (parsed.FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                return parsed;

        return null;
    }

    public override string ToString()
    {
        return HeaderText;
    }

    public byte GetByteAtPos(int pos)
    {
        if ((pos > (ByteData.Count - 1)) || (pos < 0)) return 0;
        Cursor = pos + 1;
        return ByteData[pos];
    }

    public sbyte GetSByteAtPos(int pos)
    {
        if ((pos > (ByteData.Count - 1)) || (pos < 0)) return 0;
        Cursor = pos + 1;
        return unchecked((sbyte)ByteData[pos]);
    }

    public bool GetBitAtPos(int pos, int bit)
    {
        if (((pos > (ByteData.Count - 1)) || ((bit < 0) || (bit > 7))) || (pos < 0)) return false;
        var b = ByteData[pos];
        var bitmask = (byte)(0x01 << bit);
        Cursor = pos;
        BitCursor = (byte)(bit + 1);
        return ((b & bitmask) != 0);
    }

    public ushort GetUInt16AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 2)) || (pos < 0)) return 0;
        Cursor = pos + 2;
        var bytes = ByteData.GetRange(pos, 2).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToUInt16(bytes, 0);
    }

    public short GetInt16AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 2)) || (pos < 0)) return 0;
        Cursor = pos + 2;
        var bytes = ByteData.GetRange(pos, 2).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToInt16(bytes, 0);
    }

    public uint GetUInt24AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 3)) || (pos < 0)) return 0;
        Cursor = pos + 3;
        var bytes = ByteData.GetRange(pos, 3);
        if (reversed) bytes.Reverse();
        bytes.Add(0);
        return BitConverter.ToUInt32(bytes.ToArray(), 0);
    }

    public uint GetUInt32AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 4)) || (pos < 0)) return 0;
        Cursor = pos + 4;
        var bytes = ByteData.GetRange(pos, 4).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToUInt32(bytes, 0);
    }

    public int GetInt32AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 4)) || (pos < 0)) return 0;
        Cursor = pos + 4;
        var bytes = ByteData.GetRange(pos, 4).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToInt32(bytes, 0);
    }

    public ulong GetUInt64AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 8)) || (pos < 0)) return 0;
        Cursor = pos + 8;
        var bytes = ByteData.GetRange(pos, 8).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToUInt64(bytes, 0);
    }

    public long GetInt64AtPos(int pos, bool reversed = false)
    {
        if ((pos > (ByteData.Count - 8)) || (pos < 0)) return 0;
        Cursor = pos + 8;
        var bytes = ByteData.GetRange(pos, 8).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToInt64(bytes, 0);
    }

    public string GetTimeStampAtPos(int pos, bool reversed = false)
    {
        var res = Resources.TypeUnknown;
        if (pos > (ByteData.Count - 4)) return res;
        try
        {
            var dt = GetUInt32AtPos(pos, reversed);
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dt).ToLocalTime();
            res = dtDateTime.ToString("yyyy/MM(MMM)/dd - HH:mm:ss");
        }
        catch
        {
            res = "ERROR";
        }

        Cursor = pos + 4;
        return res;
    }

    public string GetStringAtPos(int pos, int maxSize = -1)
    {
        var res = string.Empty;
        var i = 0;
        while (((i + pos) < ByteData.Count) && (ByteData[pos + i] != 0) && ((maxSize == -1) || (res.Length < maxSize)))
        {
            res += (char)ByteData[pos + i];
            i++;
        }

        if (maxSize < 0) Cursor = pos + res.Length;
        else Cursor = pos + maxSize;
        return res;
    }

    public string GetDataAtPos(int pos, int size)
    {
        var res = "";
        var i = 0;
        while (((i + pos) < ByteData.Count) && (i < size) && (i < 256))
        {
            res += ByteData[i + pos].ToString("X2") + " ";
            i++;
        }

        Cursor = pos + size;
        return res;
    }

    public byte[] GetDataBytesAtPos(int pos, int size)
    {
        var res = new List<byte>();
        var i = 0;
        while (((i + pos) < ByteData.Count) && (i < size) && (i < 256))
        {
            res.Add(ByteData[i + pos]);
            i++;
        }

        Cursor = pos + size;
        return res.ToArray();
    }
    
    public long GetBitsAtPos(int pos, int bitOffset, int bitsSize)
    {
        long res = 0;
        var p = pos;
        var b = bitOffset;
        var restBits = bitsSize;
        if (restBits < 1) restBits = 1;
        Int64 mask = 1;
        while (restBits > 0)
        {
            while (b >= 8)
            {
                b -= 8;
                p++;
            }

            if (GetBitAtPos(p, b)) res += mask;
            restBits--;
            mask <<= 1;
            b++;
        }

        return res;
    }

    // ReSharper disable once UnusedMember.Global
    public long GetBitsAtBitPos(int bitOffset, int bitsSize)
    {
        return GetBitsAtPos(bitOffset / 8, bitOffset % 8, bitsSize);
    }

    public float GetFloatAtPos(int pos, bool reversed = false)
    {
        if (pos > (ByteData.Count - 4)) return 0f;
        Cursor = pos + 4;
        var bytes = ByteData.GetRange(pos, 4).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToSingle(bytes, 0);
    }

    public double GetDoubleAtPos(int pos, bool reversed = false)
    {
        if (pos > (ByteData.Count - 8)) return 0.0;
        Cursor = pos + 8;
        var bytes = ByteData.GetRange(pos, 8).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToDouble(bytes, 0);
    }

    public Half GetHalfAtPos(int pos, bool reversed = false)
    {
        if (pos > (ByteData.Count - 2)) return (Half)0f;
        Cursor = pos + 2;
        var bytes = ByteData.GetRange(pos, 2).ToArray();
        if (reversed) bytes = bytes.Reverse().ToArray();
        return BitConverter.ToHalf(bytes, 0);
    }

    public void AddParsedField(bool hasValue, int startByte, int endByte, string displayPosition, string displayName,
        string displayValue, int nestingDepth, Color? colorOverride = null)
    {
        var newParsed = new ParsedField()
        {
            HasValue = hasValue, 
            StartingByte = startByte, 
            EndingByte = endByte, 
            DisplayedByteOffset = displayPosition,
            FieldName = displayName, 
            FieldValue = displayValue, 
            NestingDepth = nestingDepth
        };
        if (colorOverride != null)
        {
            newParsed.FieldColor = (Color)colorOverride;
        }
        else
        {
            var maxCol = Math.Min(PacketColors.DataColors.Count, Settings.Default.ColFieldCount);
            var colorId = (ParsedData.Count % maxCol);
            newParsed.FieldColor = PacketColors.DataColors[colorId];
        }

        ParsedData.Add(newParsed);

        // Add to field dictionary
        if (ParentProject.AllFieldNames.Contains(displayName.ToLower()) == false)
            ParentProject.AllFieldNames.Add(displayName.ToLower());
    }

    public void AddParsedError(string errorPosition, string errorName, string errorDescription, int nestingDepth,
        Color? colorOverride = null)
    {
        var col = colorOverride ?? Color.Red;
        AddParsedField(false, -1, -1, errorPosition, errorName, errorDescription, nestingDepth, col);
    }

    public void AddUnparsedFields()
    {
        if (Settings.Default.SkipUnparsed)
            return;

        var firstAdded = false;

        void AddFirstUnparsed()
        {
            if (firstAdded) return;
            firstAdded = true;
            AddParsedError("", Resources.UnParsedFieldName, Resources.UnParsedFieldDescription, 0);
        }

        for (int i = 0; i < ByteData.Count; i++)
        {
            var bytesLeft = ByteData.Count - i;
            if (bytesLeft > 0)
            {
                var p1 = GetParsedFieldByByteIndex(i);
                if (p1 == null)
                {
                    if (bytesLeft >= 2)
                    {
                        var p2 = GetParsedFieldByByteIndex(i + 1);
                        if (p2 == null)
                        {
                            var parseShort = true;
                            if (bytesLeft >= 4)
                            {
                                var p3 = GetParsedFieldByByteIndex(i + 2);
                                var p4 = GetParsedFieldByByteIndex(i + 3);
                                if ((p3 == null) && (p4 == null))
                                {
                                    parseShort = false;
                                    AddFirstUnparsed();
                                    var fourBytes = GetUInt32AtPos(i);
                                    AddParsedField(true, i, i + 3, i.ToHex(2), "??_uint32",
                                        fourBytes.ToHex() + " - " + fourBytes, 1);
                                    i += 3;
                                }
                            }

                            if (parseShort)
                            {
                                AddFirstUnparsed();
                                var twoBytes = GetUInt16AtPos(i);
                                AddParsedField(true, i, i + 1, i.ToHex(2), "??_uint16",
                                    twoBytes.ToHex() + " - " + twoBytes, 1);
                                i += 1;
                            }
                        }
                    }
                    else
                    {
                        AddFirstUnparsed();
                        var b = GetByteAtPos(i);
                        AddParsedField(true, i, i + 0, i.ToHex(2), "??_byte",
                            b.ToHex() + " - " + b, 1);
                    }
                }
            }
        }
    }

    public virtual string GetPacketName()
    {
        return GetPacketName(PacketDataDirection, PacketId);
    }

    public virtual string GetPacketName(PacketDataDirection direction, uint id)
    {
        if (!string.IsNullOrWhiteSpace(ParsedPacketName))
            return ParsedPacketName;

        return direction switch
        {
            PacketDataDirection.Unknown => Resources.TypeUnknown,
            PacketDataDirection.Outgoing => ParentProject.DataLookup.NLU(DataLookups.LuPacketOut).GetValue(id, Resources.TypeUnknown),
            PacketDataDirection.Incoming => ParentProject.DataLookup.NLU(DataLookups.LuPacketIn).GetValue(id, Resources.TypeUnknown),
            _ => "<error>"
        };
    }

    public virtual void BuildHeaderText()
    {
        var timeString = string.Empty;
        if (TimeStamp.Ticks > 0) timeString = TimeStamp.ToString(ParentProject.TimeStampFormat, CultureInfo.InvariantCulture);
        string directionString = PacketDataDirection switch
        {
            PacketDataDirection.Outgoing => Resources.DirectionOut,
            PacketDataDirection.Incoming => Resources.DirectionIn,
            _ => Resources.DirectionUnknown
        };

        var nestText = "";
        if (ParentPacket != null)
        {
            var mySubIndex = ParentPacket.SubPackets.IndexOf(this);
            nestText = mySubIndex >= ParentPacket.SubPackets.Count - 1 ? "└─ " : "├─ ";
        }

        var streamName = string.Empty;
        var levelName = string.Empty;
        var packetIdString = PacketId.ToHex(3) + " ";

        // If the stream has no short name attached, assume it won't get parsed and just mark it by its normal name
        var streamInfoFromPort = ParentProject.GetExpectedStreamIdByPort(SourcePort, 0);
        var streamShortName = streamInfoFromPort.Item3;
        if (streamInfoFromPort.Item3 == "")
        {
            streamName = "";
            ParsedPacketName = streamInfoFromPort.Item2;
            levelName = string.Empty;
            packetIdString = string.Empty;
            DoNotParse = true;
        }
        else
        {
            if ((CompressionLevel > 0) || (ParentProject.InputParser?.PacketCompressionLevelMaximum > 0))
            {
                if (ParentProject.PortToStreamIdMapping.Count > 1)
                    levelName = streamShortName + CompressionLevel + " ";
                else
                    levelName = "L" + CompressionLevel + " ";
            }
            else
            {
                if (ParentProject.PortToStreamIdMapping.Count > 1)
                    streamName = streamShortName + " ";
            }
        }

        HeaderText = timeString + " " +
                     directionString + " " +
                     streamName +
                     levelName +
                     packetIdString +
                     nestText + 
                     GetPacketName();
    }

    public static Color GetDataColor(int fieldIndex)
    {
        if (fieldIndex < 0)
            fieldIndex = int.MaxValue + fieldIndex;

        if (PacketColors.DataColors.Count > 0)
            return PacketColors.DataColors[fieldIndex % PacketColors.DataColors.Count];

        return Color.MediumPurple;
    }

    public bool HasSelectedFields()
    {
        foreach (var parsedField in ParsedData)
        {
            if (parsedField is { HasValue: true, IsSelected: true })
                return true;
        }
        return false;
    }

    /// <summary>
    /// Check if an individual packet can be shown with a given filter set
    /// </summary>
    /// <param name="packetKey"></param>
    /// <param name="filterType"></param>
    /// <param name="filterList"></param>
    /// <returns></returns>
    private static bool DoIShowThis(PacketFilterListEntry packetKey, FilterType filterType, ICollection<PacketFilterListEntry> filterList)
    {
        return filterType switch
        {
            FilterType.AllowNone => false,
            FilterType.ShowPackets => filterList.Any(x =>
                (x.PacketId == packetKey.PacketId) && (x.CompressionLevel == packetKey.CompressionLevel) &&
                (x.StreamId == packetKey.StreamId)),
            FilterType.HidePackets => !filterList.Any(x =>
                (x.PacketId == packetKey.PacketId) && (x.CompressionLevel == packetKey.CompressionLevel) &&
                (x.StreamId == packetKey.StreamId)),
            FilterType.Off => true,
            _ => true
        };
    }

    /// <summary>
    /// Check if this packet can get shown using the given filter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private bool DoIShowThis(PacketListFilter filter)
    {
        var packetKey = new PacketFilterListEntry(PacketId, CompressionLevel, StreamId);

        if ((PacketDataDirection == PacketDataDirection.Incoming) && (filter.FilterInType != FilterType.Off))
            return DoIShowThis(packetKey, filter.FilterInType, filter.FilterInList);
        if ((PacketDataDirection == PacketDataDirection.Outgoing) && (filter.FilterOutType != FilterType.Off))
            return DoIShowThis(packetKey, filter.FilterOutType, filter.FilterOutList);
        return true;
    }

    public void ApplyFilter(PacketListFilter filter)
    {
        if (filter.MarkAsDimmed)
        {
            IsVisible = true;
            MarkedAsDimmed = !DoIShowThis(filter);
        }
        else
        {
            IsVisible = DoIShowThis(filter);
            MarkedAsDimmed = false;
        }
    }

    public bool MatchesSearch(SearchParameters p)
    {
        if ((PacketDataDirection == PacketDataDirection.Incoming) && (!p.SearchIncoming))
            return false;
        if ((PacketDataDirection == PacketDataDirection.Outgoing) && (!p.SearchOutgoing))
            return false;

        var res = true;

        if (p.SearchByPacketId)
            res = (PacketId == p.SearchPacketId);

        if ((res) && (p.SearchByPacketLevel))
            res = (CompressionLevel == p.SearchPacketLevel);

        if ((res) && (p.SearchBySync))
            res = (SyncId == p.SearchSync);

        if ((res) && (p.SearchByByte))
            res = (ByteData.IndexOf(p.SearchByte) >= 0);

        if ((res) && (p.SearchByUInt16))
        {
            res = false;
            for (var i = 0; i < ByteData.Count - 2; i++)
            {
                var n = GetUInt16AtPos(i);
                if (n != p.SearchUInt16)
                    continue;

                res = true;
                break;
            }
        }

        if ((res) && (p.SearchByUInt24))
        {
            res = false;
            for (var i = 0; i < ByteData.Count - 3; i++)
            {
                var rd = GetDataBytesAtPos(i, 3).ToList();
                rd.Add(0);
                var d = BitConverter.ToUInt32(rd.ToArray(), 0);
                if (d != p.SearchUInt24)
                    continue;

                res = true;
                break;
            }
        }

        if ((res) && (p.SearchByUInt32))
        {
            res = false;
            for (var i = 0; i < ByteData.Count - 4; i++)
            {
                var n = GetUInt32AtPos(i);
                if (n != p.SearchUInt32)
                    continue;

                res = true;
                break;
            }
        }

        if (res && (p.SearchByParsedData) && (p.SearchParsedFieldValue != string.Empty))
        {
            res = false;
            foreach (var data in ParsedData)
            {
                if (p.SearchParsedFieldName != string.Empty)
                {
                    // Field Name Specified
                    res = (data.FieldName.ToLower().Contains(p.SearchParsedFieldName) && data.FieldValue.ToLower().Contains(p.SearchParsedFieldValue));
                }
                else
                {
                    // No field name defined
                    res = data.FieldValue.ToLower().Contains(p.SearchParsedFieldValue);
                }
                if (res)
                    break;
            }
        }

        return res;
    }

}