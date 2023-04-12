using System;
using System.Collections.Generic;
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
        return dataType switch
        {
            "dir" => new RulesActionFfxiReadDirectionByte(this, parentAction, actionNode, step),
            "pos" => new RulesActionFfxiReadPosition(this, parentAction, actionNode, step),
            "combatskill" => new RulesActionFfxiReadCombatSkill(this, parentAction, actionNode, step),
            "craftskill" => new RulesActionFfxiReadCraftSkill(this, parentAction, actionNode, step),
            "jobpoints" => new RulesActionFfxiReadJobPoints(this, parentAction, actionNode, step),
            "buffs" => new RulesActionFfxiReadBuffs(this, parentAction, actionNode, step, "arg"),
            "vanatime" => new RulesActionFfxiReadVanaTime(this, parentAction, actionNode, step),
            "roequest" => new RulesActionFfxiReadRoEQuest(this, parentAction, actionNode, step),
            "linkshell" => new RulesActionFfxiReadLinkShellName(this, parentAction, actionNode, step),
            "inscription" => new RulesActionFfxiReadItemInscription(this, parentAction, actionNode, step),
            _ => base.BuildFallbackDataAction(parentAction, actionNode, attributes, step, dataType, isReversed)
        };
    }
}

/// <summary>
/// Read Byte as Direction
/// </summary>
public class RulesActionFfxiReadDirectionByte : RulesAction
{
    private static readonly string[] CompassDirectionNames = new []
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

public class RulesActionFfxiReadCombatSkill : RulesAction
{
    public RulesActionFfxiReadCombatSkill(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var val = packetData.GetUInt16AtPos(packetData.Cursor);
        var cappedString = ((val & 0x8000) != 0) ? " (Capped)" : string.Empty;
        var skillLevel = (val & 0x7FFF);

        var dataString = skillLevel + cappedString + " - " + val.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

public class RulesActionFfxiReadCraftSkill : RulesAction
{
    public RulesActionFfxiReadCraftSkill(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var val = packetData.GetUInt16AtPos(packetData.Cursor);
        var cappedString = ((val & 0x8000) != 0) ? " (Capped)" : string.Empty;
        var craftLevel = ((val >> 5) & 0x03FF);
        var craftRank = (val & 0x001F);

        var dataString = "Level: " + craftLevel + cappedString + " Rank:" +  craftRank + " - " + val.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

public class RulesActionFfxiReadJobPoints : RulesAction
{
    public RulesActionFfxiReadJobPoints(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var cpVal = packetData.GetUInt16AtPos(packetData.Cursor);
        var jpVal = packetData.GetUInt16AtPos(packetData.Cursor);
        var spentVal = packetData.GetUInt16AtPos(packetData.Cursor);
        var data = $"{cpVal} CP  {jpVal} JP  {spentVal} spent JP";

        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, data, Depth);
    }
}

public class RulesActionFfxiReadBuffs : RulesAction
{
    private readonly string _countName;

    public RulesActionFfxiReadBuffs(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string countName) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        _countName = countName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupName = XmlHelper.GetAttributeString(Attributes, "lookup");
        if (string.IsNullOrWhiteSpace(lookupName))
            lookupName = "buffs";
        var sizeAttribute = (int)XmlHelper.GetAttributeInt(Attributes, _countName);
        if (sizeAttribute < 1)
            sizeAttribute = 1;

        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        for (int c = 0; c < sizeAttribute; c++)
        {
            var buffPos = pos + (c * 2);
            var timerPos = pos + (sizeAttribute * 2) + (c * 4);
            var iconVal = packetData.GetUInt16AtPos(buffPos);
            var timerVal = packetData.GetInt32AtPos(timerPos);
            var iconVarName = varName + "-" + c + "-icon";
            var timerVarName = varName + "-" + c + "-time";


            var iconStr = iconVal.ToHex() + " => " + packetData.ParentProject.DataLookup.NLU(lookupName).GetValue(iconVal);

            string timerStr;
            if (timerVal == 0)
                timerStr = timerVal.ToHex() + " => Not defined";
            else
            if (timerVal == int.MaxValue)
                timerStr = timerVal.ToHex() + " => Always";
            else
                timerStr = timerVal.ToHex() + " => " + ((uint)timerVal).AsMilliseconds();

            if (iconVal == 0xFF)
            {
                iconStr = "None";
                timerStr = string.Empty;
            }
            ParentRule.SetLocalVar(iconVarName, iconVal.ToString());
            ParentRule.SetLocalVar(timerVarName, timerVal.ToString());
            packetData.AddParsedField(true, buffPos, buffPos+1, buffPos.ToHex(2), iconVarName, iconStr, Depth);
            packetData.AddParsedField(true, timerPos, timerPos + 3, timerPos.ToHex(2), timerVarName, timerStr, Depth);
        }
    }
}

public class RulesActionFfxiReadVanaTime : RulesAction
{
    public RulesActionFfxiReadVanaTime(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var pos = packetData.Cursor;
        var vanaTime = packetData.GetUInt32AtPos(packetData.Cursor);
        var vt = new VanadielTime();
        vt.FromVanadielIntTime((int)vanaTime);

        var data = vt.LocalEarthTime.ToString("yyyy-MM-dd HH:mm:ss") + " (" + vanaTime + ") => " + vt;
        ParentRule.SetLocalVar(varName, vanaTime.ToString());
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, data, Depth);
    }
}

public class RulesActionFfxiReadRoEQuest : RulesAction
{
    public RulesActionFfxiReadRoEQuest(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupName = XmlHelper.GetAttributeString(Attributes, "lookup");
        var pos = packetData.Cursor;

        var idVal = packetData.GetBitsAtPos(packetData.Cursor, packetData.BitCursor, 12);
        var progressVal = packetData.GetBitsAtPos(packetData.Cursor, packetData.BitCursor, 20);
        var lookupTable = packetData.ParentProject.DataLookup.NLU(lookupName);
        var maxVal = lookupTable.GetExtra((ulong)idVal);
        if (maxVal == "")
            maxVal = "???";
        var data = string.Empty;
        data += "ID: " + idVal.ToHex(3) + " => " + lookupTable.GetValue((ulong)idVal) + " - ";
        data += progressVal + " / " + maxVal;

        ParentRule.SetLocalVar(varName + "-id", idVal.ToString());
        ParentRule.SetLocalVar(varName + "-progress", progressVal.ToString());
        ParentRule.SetLocalVar(varName + "-max", maxVal);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, data, Depth);
    }
}

public class RulesActionFfxiReadLinkShellName : RulesAction
{
    public RulesActionFfxiReadLinkShellName(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var pos = packetData.Cursor;

        var data = FfxiStrings.GetPackedString16AtPos(packetData, pos, FfxiStrings.LinkShellEncoding);
        var dataRaw = packetData.GetDataBytesAtPos(pos, 16);

        ParentRule.SetLocalVar(varName+"-raw", NumberHelper.BytesToHexString(dataRaw));
        ParentRule.SetLocalVar(varName + "-decode", data);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, data, Depth);
    }
}

public class RulesActionFfxiReadItemInscription : RulesAction
{
    public RulesActionFfxiReadItemInscription(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) :
        base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var pos = packetData.Cursor;

        var data = FfxiStrings.GetPackedString16AtPos(packetData, pos, FfxiStrings.ItemEncoding);
        var dataRaw = packetData.GetDataBytesAtPos(pos, 16);

        ParentRule.SetLocalVar(varName + "-raw", NumberHelper.BytesToHexString(dataRaw));
        ParentRule.SetLocalVar(varName + "-decode", data);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, data, Depth);
    }
}
