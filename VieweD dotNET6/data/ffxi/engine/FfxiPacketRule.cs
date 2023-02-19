using System.Globalization;
using System.Xml;
using VieweD.engine.common;
using VieweD.Helpers.System;

namespace VieweD.data.ffxi.engine;

public class FfxiPacketRule : PacketRule
{
    public FfxiPacketRule(RulesGroup parent, byte streamId, byte level, ushort packetId, string description,
        XmlNode node) : base(parent, streamId, level, packetId, description, node)
    {
        // No extra data needed
    }

    // Custom Data Types for FFXI
    public override RulesAction BuildFallbackDataAction(RulesAction? parentAction, XmlNode actionNode,
        Dictionary<string, string> attributes, int step, string dataType, bool isReversed)
    {
        switch (dataType)
        {
            case "dir":
                return new RulesActionFfxiReadDirectionByte(this, parentAction, actionNode, step);
            case "pos":
                return new RulesActionFfxiReadPosition(this, parentAction, actionNode, step);
            default:
                return base.BuildFallbackDataAction(parentAction, actionNode, attributes, step, dataType, isReversed);
        }
    }
}

/// <summary>
/// Read Byte as Direction
/// </summary>
public class RulesActionFfxiReadDirectionByte : RulesAction
{
    private static readonly string[] CompassDirectionNames = new string[16]
        { "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N", "NNE", "NE", "ENE" };

    public RulesActionFfxiReadDirectionByte(PacketRule parent, RulesAction? parentAction, XmlNode thisNode,
        int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetByteAtPos(pos);
        var dataString = ByteToRotation(data);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }

    protected string ByteToRotation(byte b)
    {
        var i = b * 360 / 256;
        var rads = Math.PI * 2 / 360 * i;
        return CompassDirectionNames[i / 16 % 16] + " (" + b + " - " + b.ToHex() + " ≈ " + i + "° = " +
               rads.ToString("F") + " rad)";
    }

}

/// <summary>
/// Read Byte as Direction
/// </summary>
public class RulesActionFfxiReadPosition : RulesAction
{
    public RulesActionFfxiReadPosition(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var dataX = packetData.GetFloatAtPos(packetData.Cursor);
        var dataY = packetData.GetFloatAtPos(packetData.Cursor);
        var dataZ = packetData.GetFloatAtPos(packetData.Cursor);
        var dataString = "X:" + dataX.ToString("F") + "  Y:" + dataY.ToString("F") + "  Z:" + dataZ.ToString("F");
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, dataString);
        ParentRule.SetLocalVar(varName + "-X", dataX.ToString(CultureInfo.InvariantCulture));
        ParentRule.SetLocalVar(varName + "-Y", dataY.ToString(CultureInfo.InvariantCulture));
        ParentRule.SetLocalVar(varName + "-Z", dataZ.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}