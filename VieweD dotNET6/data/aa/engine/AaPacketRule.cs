using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Linq;
using VieweD.engine.common;
using VieweD.Helpers.System;
using System.Numerics;

namespace VieweD.data.aa.engine;

public class AaPacketRule : PacketRule
{
    public AaPacketRule(RulesGroup parent, byte streamId, byte level, ushort packetId, string description, XmlNode node) : base(parent, streamId, level, packetId, description, node)
    {
        // No extra data needed
    }

    // Custom Data Types for ArcheAge
    public override RulesAction BuildFallbackDataAction(RulesAction? parentAction, XmlNode actionNode, Dictionary<string, string> attributes, int step, string dataType, bool isReversed)
    {
        return dataType switch
        {
            "pish" => new RulesActionReadPish(this, parentAction, actionNode, step, "count"),
            "bcx" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.0002f, 0.5f, 128f, 0f, isReversed),
            "bcy" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.0002f, 0.5f, 128f, 0f, isReversed),
            "rbcx" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.0002f, 0.5f, 128f, 0f, isReversed),
            "rbcy" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.0002f, 0.5f, 128f, 0f, isReversed),
            "bcz" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.001f, 0.2561f, 65.5625f, -100f, isReversed),
            "rbcz" => new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, 0.001f, 0.2561f, 65.5625f, -100f, isReversed),
            "qx" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f), 
            "qy" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f),
            "qz" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f), 
            "rqx" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f),
            "rqy" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f),
            "rqz" => new RulesActionReadUInt64ShiftFraction(this, parentAction, actionNode, step, isReversed, 32, 4096f),
            _ => base.BuildFallbackDataAction(parentAction, actionNode, attributes, step, dataType, isReversed)
        };
    }

    public override RulesAction BuildFallbackAction(RulesAction? parentAction, XmlNode actionNode, Dictionary<string, string> attributes, int step)
    {
        return actionNode.Name switch
        {
            "doodadrotationxyztorpy" => new RulesActionConvertDoodadAngles(this, parentAction, actionNode, step, "x", "y", "z", "roll", "pitch", "yaw"),
            _ => base.BuildFallbackAction(parentAction, actionNode, attributes, step)
        };
    }
}

public class RulesActionReadPish : RulesAction
{
    public string CountArg;

    public RulesActionReadPish(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string countArgName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        CountArg = XmlHelper.GetAttributeString(Attributes, countArgName);
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);

        var pos = packetData.Cursor;
        var varName = XmlHelper.GetAttributeString(Attributes, "name");

        #region readpisc
        long count = 0;
        if (CountArg.StartsWith("#"))
        {
            if (NumberHelper.TryFieldParse(ParentRule.GetLocalVar(CountArg.TrimStart('#')), out long aVal))
            {
                count = aVal;
            }
        }
        else
        if (NumberHelper.TryFieldParse(CountArg, out long aVal))
        {
            count = aVal;
        }


        // First Byte
        var pish = packetData.GetByteAtPos(packetData.Cursor);
        ParentRule.SetLocalVar(@"pish", pish.ToString(CultureInfo.InvariantCulture));
        ParentRule.SetLocalVar(varName+@"-pish", pish.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, pos, pos.ToHex(2), varName, pish.ToHex() + " (" + pish.ToBinary() + ")", Depth);

        var bits = new BitArray(BitConverter.GetBytes(pish));
        var lookupNumber = 0;
        for (var pIndex = 0; pIndex < count * 2; pIndex += 2)
        {
            var pishCursorPos = packetData.Cursor;
            lookupNumber++;
            if (bits.Get(pIndex) && bits.Get(pIndex + 1))
            {
                // 0101 = 5 uint
                var uintVal = packetData.GetUInt32AtPos(packetData.Cursor);
                var lookup = GetLookup(uintVal, lookupNumber);
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString() + "-uint32", uintVal.ToString(CultureInfo.InvariantCulture));
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString(), uintVal.ToString(CultureInfo.InvariantCulture));
                packetData.AddParsedField(true, pishCursorPos, packetData.Cursor-1, pishCursorPos.ToHex(2), varName + @"-" + lookupNumber, lookup + uintVal + " - "+uintVal.ToHex(), Depth+1);
            }
            else if (bits.Get(pIndex + 1))
            {
                // 0100 = 4 uint24 (bc)
                var bcVal = packetData.GetUInt24AtPos(packetData.Cursor);
                var lookup = GetLookup(bcVal, lookupNumber);
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString() + "-uint24", bcVal.ToString(CultureInfo.InvariantCulture));
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString(), bcVal.ToString(CultureInfo.InvariantCulture));
                packetData.AddParsedField(true, pishCursorPos, packetData.Cursor - 1, pishCursorPos.ToHex(2), varName + @"-" + lookupNumber, lookup + bcVal + " - " + bcVal.ToHex(6), Depth + 1);
            }
            else if (bits.Get(pIndex))
            {
                // 0001 = 1 ushort
                var ushortVal = packetData.GetUInt16AtPos(packetData.Cursor);
                var lookup = GetLookup(ushortVal, lookupNumber);
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString() + "-uint16", ushortVal.ToString(CultureInfo.InvariantCulture));
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString(), ushortVal.ToString(CultureInfo.InvariantCulture));
                packetData.AddParsedField(true, pishCursorPos, packetData.Cursor - 1, pishCursorPos.ToHex(2), varName + @"-" + lookupNumber, lookup + ushortVal + " - " + ushortVal.ToHex(), Depth + 1);
            }
            else
            {
                // 00 = 0 byte
                var byteVal = packetData.GetByteAtPos(packetData.Cursor);
                var lookup = GetLookup(byteVal, lookupNumber);
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString() + "-byte", byteVal.ToString(CultureInfo.InvariantCulture));
                ParentRule.SetLocalVar(varName + "-" + lookupNumber.ToString(), byteVal.ToString(CultureInfo.InvariantCulture));
                packetData.AddParsedField(true, pishCursorPos, packetData.Cursor - 1, pishCursorPos.ToHex(2), varName + @"-" + lookupNumber, lookup + byteVal + " - " + byteVal.ToHex(), Depth + 1);
            }
        }
        #endregion
    }
}

public class RulesActionReadUInt64ShiftFraction : RulesAction
{
    public int ShiftValue;
    public float FractionValue;

    public RulesActionReadUInt64ShiftFraction(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed, int shiftValue, float fractionValue) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        ShiftValue = shiftValue;
        FractionValue = fractionValue;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var d = packetData.GetUInt64AtPos(pos);
        if (IsReversed)
            d = BitConverter.ToUInt64(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
        var f = (d >> ShiftValue) / FractionValue;
        var dataString = f + " (" + d.ToHex() + ")";
        var varName = XmlHelper.GetAttributeString(Attributes, "name");

        ParentRule.SetLocalVar(varName, f.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

public class RulesActionRead3ByteFloat : RulesAction
{
    private readonly double _byte1Factor;
    private readonly double _byte2Factor;
    private readonly double _byte3Factor;
    private readonly double _valueOffset;

    public RulesActionRead3ByteFloat(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, double byte1Factor, double byte2Factor, double byte3Factor, double valueOffset, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        _byte1Factor = byte1Factor;
        _byte2Factor = byte2Factor;
        _byte3Factor = byte3Factor;
        _valueOffset = valueOffset;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var varName = XmlHelper.GetAttributeString(Attributes, "name");

        var rd = packetData.GetDataBytesAtPos(pos, 3).ToList();
        if (IsReversed)
            rd.Reverse();
        var d = Math.Round((rd[0] * _byte1Factor) + (rd[1] * _byte2Factor) + (rd[2] * _byte3Factor) + _valueOffset, 4, MidpointRounding.ToEven);
        rd.Add(0);

        var dataString = d.ToString(CultureInfo.InvariantCulture) + " (0x" + rd[0].ToString("X2") + rd[1].ToString("X2") + rd[2].ToString("X2") + ")";
        ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}


public class RulesActionConvertDoodadAngles : RulesAction
{
    private string SourceXName { get; set; }
    private string SourceYName { get; set; }
    private string SourceZName { get; set; }
    private string OutputRollName { get; set; }
    private string OutputPitchName { get; set; }
    private string OutputYawName { get; set; }

    public RulesActionConvertDoodadAngles(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string sourceXName, string sourceYName, string sourceZName, string outputRollName, string outputPitchName, string outputYawName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        SourceXName = sourceXName;
        SourceYName = sourceYName;
        SourceZName = sourceZName;
        OutputRollName = outputRollName;
        OutputPitchName = outputPitchName;
        OutputYawName = outputYawName;
    }

    private long GetSource(BasePacketData data, string varName)
    {
        long valX = 0;
        var valXAttrib = XmlHelper.GetAttributeString(Attributes, varName);
        if (valXAttrib == string.Empty)
        {
            data.AddParsedError("A" + GetActionStepName(),Node.Name, varName + " can not be empty", Depth);
            return 0;
        }
        if (valXAttrib.StartsWith("#"))
        {
            var argXName = ParentRule.GetLocalVar(valXAttrib.TrimStart('#'));
            if (NumberHelper.TryFieldParse(argXName, out long aVal))
            {
                valX = aVal;
            }
            else
            {
                data.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + argXName + ": " + valXAttrib, Depth);
            }
        }
        else
        if (NumberHelper.TryFieldParse(valXAttrib, out long aVal))
        {
            valX = aVal;
        }
        return valX;
    }

    public static double CopySignOld(double x, double y)
    {
        return SoftwareFallback(x, y);

        double SoftwareFallback(double xx, double yy)
        {
            const long signMask = 1L << 63;

            // This method is required to work for all inputs,
            // including NaN, so we operate on the raw bits.
            long xbits = BitConverter.DoubleToInt64Bits(xx);
            long ybits = BitConverter.DoubleToInt64Bits(yy);

            // Remove the sign from x, and remove everything but the sign from y
            xbits &= ~signMask;
            ybits &= signMask;

            // Simply OR them to get the correct sign
            return BitConverter.Int64BitsToDouble(xbits | ybits);
        }
    }

    // Copied from .NET source
    public static double CopySign(double x, double y)
    {
        long int64Bits1 = BitConverter.DoubleToInt64Bits(x);
        long int64Bits2 = BitConverter.DoubleToInt64Bits(y);
        return (int64Bits1 ^ int64Bits2) < 0 ? BitConverter.DoubleToInt64Bits(int64Bits1 ^ long.MinValue) : x;
    }

    private static Vector3 FromQuaternion(float x, float y, float z, float w)
    {
        Vector3 angles;

        // roll (x-axis rotation)
        var sinRCosP = 2f * (w * x + y * z);
        var cosRCosP = 1f - 2f * (x * x + y * y);
        angles.X = (float)Math.Atan2(sinRCosP, cosRCosP);

        // pitch (y-axis rotation)
        var sinP = 2f * (w * y - z * x);
        // Note: CopySign doesn't exist in non core .NET frameworks
        angles.Y = (float)(Math.Abs(sinP) >= 1f ? CopySign(Math.PI / 2f, sinP) : Math.Asin(sinP));

        // yaw (z-axis rotation)
        var sinYCosP = 2f * (w * z + x * y);
        var cosYCosP = 1f - 2f * (y * y + z * z);
        angles.Z = (float)Math.Atan2(sinYCosP, cosYCosP);

        return angles;
    }

    public override void RunAction(BasePacketData pp)
    {
        // Read truncated Quaternion
        var sourceY = (float)Math.Round(GetSource(pp, SourceXName) / 32767f, 4, MidpointRounding.ToEven);
        var sourceX = (float)Math.Round(GetSource(pp, SourceYName) / 32767f, 4, MidpointRounding.ToEven);
        var sourceZ = (float)Math.Round(GetSource(pp, SourceZName) / 32767f, 4, MidpointRounding.ToEven);

        // Calculate W
        var ww = 1.0 - ((sourceX * sourceX) + (sourceY * sourceY) + (sourceZ * sourceZ));
        var w = (float)Math.Sqrt(ww);

        // Create Vector3 from Quaternion
        var v = FromQuaternion(sourceX, sourceY, sourceZ, w);

        // Save the results
        ParentRule.SetLocalVar(OutputRollName, (v.X * -1f).RadToDeg().ToString(CultureInfo.InvariantCulture));
        ParentRule.SetLocalVar(OutputPitchName, (v.Y * -1f).RadToDeg().ToString(CultureInfo.InvariantCulture));
        ParentRule.SetLocalVar(OutputYawName, v.Z.RadToDeg().ToString(CultureInfo.InvariantCulture));
    }
}
