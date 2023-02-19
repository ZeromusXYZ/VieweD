using System.Globalization;
using VieweD.Helpers.System;

namespace VieweD.engine.common;

public class PacketFilterListEntry
{
    public ulong Id { get; set; }
    public byte Level { get; set; }
    public byte StreamId { get; set; }

    public PacketFilterListEntry(ulong id, byte level, byte streamId)
    {
        Id = id;
        Level = level;
        StreamId = streamId;

        // Ensure compatibility with older versions
        if ((id > 0xFFFF) && (level == 0) && (streamId == 0))
        {
            StreamId = (byte)(id / 0x1000000);
            Level = (byte)((id / 0x10000) & 0xFF);
            Id = id & 0xFFFF;
        }
    }

    public PacketFilterListEntry(ulong fullKey)
    {
        StreamId = (byte)(fullKey / 0x1000000);
        Level = (byte)((fullKey / 0x10000) & 0xFF);
        Id = fullKey & 0xFFFF;
    }

    public PacketFilterListEntry(string value)
    {
        var dashPos = value.IndexOf('-');
        if (dashPos > 0)
            value = value.Substring(0, dashPos);

        var splitValue = value.ToUpper().Split(' ');
        foreach (var s in splitValue)
        {
            if (s.StartsWith("0X"))
            {
                var i = s.Substring(2);
                if (ulong.TryParse(i, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var idVal))
                    Id = idVal;
            }
            if (s.StartsWith("$") || s.StartsWith("H"))
            {
                var i = s.Substring(1);
                if (ulong.TryParse(i, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var idVal))
                    Id = idVal;
            }
            else if (s.StartsWith("L"))
            {
                var i = s.Substring(1);
                if (byte.TryParse(i, out var lVal))
                    Level = lVal;
            }
            else if (s.StartsWith("S"))
            {
                var i = s.Substring(1);
                if (byte.TryParse(i, out var sVal))
                    StreamId = sVal;
            }
        }
    }


    public ulong AsMergedId()
    {
        // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
        // var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);
        return (Id + (ulong)(Level * 0x010000) + (ulong)(StreamId * 0x01000000));
    }

    public override string ToString()
    {
        // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
        // var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);
        return Id.ToHex(3) + (Level > 0 ? " L" + Level : "") + (StreamId > 0 ? " S" + StreamId : "");
    }

    public static string AsString(ulong id, byte level, byte streamId)
    {
        // ulong packetKey = (ulong)(pd.PacketId + (pd.PacketLevel * 0x010000) + (pd.StreamId * 0x01000000));
        // var packetKey = new PacketFilterListEntry(pd.PacketId, pd.PacketLevel, pd.StreamId);
        return id.ToHex(3) + (level > 0 ? " L" + level : "") + (streamId > 0 ? " S" + streamId : "");
    }
}