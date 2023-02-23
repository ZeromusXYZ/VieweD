using System.Globalization;
using VieweD.Helpers.System;

namespace VieweD.engine.common;

public class PacketFilterListEntry
{
    public uint PacketId { get; set; }
    public byte CompressionLevel { get; set; }
    public byte StreamId { get; set; }

    // key = 0xssll 0000 pppp pppp
    // ss = stream
    // ll = compression level
    // pp = packet id
    private const int StreamBitOffset = 48 ;
    private const int CompressionBitOffset = 56;
    public ulong FilterKey => (PacketId + ((ulong)StreamId << StreamBitOffset) + ((ulong)CompressionLevel << CompressionBitOffset));

    public PacketFilterListEntry(ulong packetId, byte compressionLevel, byte streamId)
    {
        PacketId = (uint)packetId;
        CompressionLevel = compressionLevel;
        StreamId = streamId;

        // Ensure compatibility with older versions
        if ((packetId > uint.MaxValue) && (compressionLevel == 0) && (streamId == 0))
        {
            StreamId = (byte)((packetId >> StreamBitOffset) & 0xFF);
            CompressionLevel = (byte)((packetId >> CompressionBitOffset) & 0xFF);
            PacketId = (uint)(packetId & uint.MaxValue);
        }
    }

    public PacketFilterListEntry(ulong fullKey)
    {
        StreamId = (byte)((fullKey >> StreamBitOffset) & 0xFF);
        CompressionLevel = (byte)((fullKey >> CompressionBitOffset) & 0xFF);
        PacketId = (uint)(fullKey & uint.MaxValue);
    }

    public PacketFilterListEntry(string value)
    {
        var dashPos = value.IndexOf('-');
        if (dashPos > 0)
            value = value[..dashPos];

        var splitValue = value.ToUpper().Split(' ');
        foreach (var s in splitValue)
        {
            if (s.StartsWith("0X"))
            {
                var i = s[2..];
                if (uint.TryParse(i, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var idVal))
                    PacketId = idVal;
            }
            else
            if (s.StartsWith("$") || s.StartsWith("H"))
            {
                var i = s[1..];
                if (uint.TryParse(i, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var idVal))
                    PacketId = idVal;
            }
            else if (s.StartsWith("L"))
            {
                var i = s[1..];
                if (byte.TryParse(i, out var lVal))
                    CompressionLevel = lVal;
            }
            else if (s.StartsWith("S"))
            {
                var i = s[1..];
                if (byte.TryParse(i, out var sVal))
                    StreamId = sVal;
            }
        }
    }

    public override string ToString()
    {
        return PacketId.ToHex(3) + (CompressionLevel > 0 ? " L" + CompressionLevel : "") + (StreamId > 0 ? " S" + StreamId : "");
    }

    public static string AsString(uint id, byte level, byte streamId)
    {
        return id.ToHex(3) + (level > 0 ? " L" + level : "") + (streamId > 0 ? " S" + streamId : "");
    }
}