using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml;
using VieweD.Forms;
using VieweD.Helpers.System;

namespace VieweD.engine.common;

/// <summary>
/// Generic Action
/// </summary>
public class RulesAction
{
    protected LoopActionResult InternalLoopActionResult;

    public XmlNode Node { get; set; }

    public PacketRule ParentRule { get; set; }

    public RulesAction? ParentAction { get; set; }

    public int ActionStep { get; set; }

    public Dictionary<string, string> Attributes;
    public bool IsReversed { get; protected set; }

    public int? OverrideStartByte { get; set; }
    public int? OverrideStartBit { get; set; }

    public LoopActionResult LoopActionResult
    {
        get => InternalLoopActionResult;
        protected set
        {
            InternalLoopActionResult = value;
            // Aggregate to parents until we hit a loop node
            if ((Node.Name != "loop") && (ParentAction != null))
                ParentAction.LoopActionResult = value;
        }
    }
    public int Depth { get; set; }

    public RulesAction(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed)
    {
        ParentRule = parent;
        ParentAction = parentAction;
        Node = thisNode;
        ActionStep = thisStep;
        IsReversed = reversed;
        Attributes = XmlHelper.ReadNodeAttributes(Node);
        Depth = parentAction != null ? parentAction.Depth + 1 : 0;
        // If the field has a "pos" attribute, use it to override the starting position
        if (XmlHelper.HasAttribute(Attributes, "pos"))
        {
            OverrideStartByte = (int)XmlHelper.GetAttributeInt(Attributes, "pos");

            // Additionally, check if there is also a "bit" attribute to override that as well
            if (XmlHelper.HasAttribute(Attributes, "bit"))
                OverrideStartBit = (int)XmlHelper.GetAttributeInt(Attributes, "bit");
        }
    }

    protected void GotoStartPosition(BasePacketData packetData)
    {
        if (OverrideStartByte != null)
            packetData.Cursor = (int)OverrideStartByte;
        if (OverrideStartBit != null)
            packetData.BitCursor = (byte)OverrideStartBit;
    }

    /// <summary>
    /// Parses data by executing this action, needs to be overriden by descendants
    /// </summary>
    /// <param name="packetData"></param>
    public virtual void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var eText = Node.Name;
        foreach (var a in Attributes)
            eText += " (" + a.Key + "=" + a.Value + ")";
        packetData.AddParsedError("A" + GetActionStepName(), "Action Error", "Unsupported: " + eText, Depth);
    }

    /// <summary>
    /// Generate the full Actions Indexed name
    /// </summary>
    /// <returns></returns>
    public string GetActionStepName()
    {
        return ParentAction == null ? ActionStep.ToString() : ParentAction?.GetActionStepName() + "-" + ActionStep.ToString();
    }

    public string GetLookup(ulong id,int lookupIndex = 0, bool showArrow = true)
    {
        var lookupVal = string.Empty;
        var lookupAttribute = "lookup";

        if (lookupIndex > 0)
            lookupAttribute += lookupIndex.ToString();

        var lookupListName = XmlHelper.GetAttributeString(Attributes, lookupAttribute);
        if (lookupListName != string.Empty)
            lookupVal = ParentRule.Parent.Parent.ParentProject.DataLookup.NLU(lookupListName).GetValue(id);

        if ((lookupListName != string.Empty) && (showArrow))
            lookupVal += " <= ";

        return lookupVal;
    }
}

/// <summary>
/// Read byte
/// </summary>
public class RulesActionReadByte : RulesAction
{
    public RulesActionReadByte(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetByteAtPos(pos);
        var dataString = data + " - " + data.ToHex() + " - " + NumberHelper.ByteToBits(data) + " - '" + (char)data + "'";
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, pos+0, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read sbyte
/// </summary>
public class RulesActionReadSByte : RulesAction
{
    public RulesActionReadSByte(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var udata = packetData.GetByteAtPos(pos);
        var data = packetData.GetSByteAtPos(pos);
        var dataString = data + " - " + udata.ToHex() + " - " + NumberHelper.ByteToBits(udata) + " - '" + (char)udata + "'";
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup((byte)data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, pos + 0, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read ushort
/// </summary>
public class RulesActionReadUInt16 : RulesAction
{
    public RulesActionReadUInt16(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetUInt16AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor-1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read short
/// </summary>
public class RulesActionReadInt16 : RulesAction
{
    public RulesActionReadInt16(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        // 
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetInt16AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup((ulong)data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read uint
/// </summary>
public class RulesActionReadUInt32 : RulesAction
{
    public RulesActionReadUInt32(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetUInt32AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read int
/// </summary>
public class RulesActionReadInt32 : RulesAction
{
    public RulesActionReadInt32(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetInt32AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup((ulong)data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read float
/// </summary>
public class RulesActionReadSingle : RulesAction
{
    public RulesActionReadSingle(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetFloatAtPos(pos, IsReversed);
        var dataString = data.ToString(CultureInfo.InvariantCulture);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, dataString);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read double
/// </summary>
public class RulesActionReadDouble : RulesAction
{
    public RulesActionReadDouble(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetDoubleAtPos(pos, IsReversed);
        var dataString = data.ToString(CultureInfo.InvariantCulture);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, dataString);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read Half (float16)
/// </summary>
public class RulesActionReadHalf : RulesAction
{
    public RulesActionReadHalf(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetHalfAtPos(pos, IsReversed);
        var dataString = data.ToString(CultureInfo.InvariantCulture);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, dataString);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read ulong
/// </summary>
public class RulesActionReadUInt64 : RulesAction
{
    public RulesActionReadUInt64(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetUInt64AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read long
/// </summary>
public class RulesActionReadInt64 : RulesAction
{
    public RulesActionReadInt64(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetInt64AtPos(pos, IsReversed);
        var dataString = data + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup((ulong)data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read uint24
/// </summary>
public class RulesActionReadUInt24 : RulesAction
{
    public RulesActionReadUInt24(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var rd = packetData.GetDataBytesAtPos(pos, 3).ToList();
        if (IsReversed)
            rd.Reverse();
        rd.Add(0);
        var data = BitConverter.ToUInt32(rd.ToArray(), 0);
        var dataString = data + " - " + data.ToHex(6);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Read byte array
/// </summary>
public class RulesActionReadArray : RulesAction
{
    public RulesActionReadArray(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var sizeAttribute = XmlHelper.GetAttributeString(Attributes, "arg");
        var varName = XmlHelper.GetAttributeString(Attributes, "name");

        ParentRule.TryGetLocalInt(sizeAttribute, 0, out var size);

        if (size <= 0)
        {
            packetData.AddParsedError("A" + GetActionStepName(), varName, "NullArray", Depth);
            ParentRule.SetLocalVar(varName, string.Empty);
            return;
        }

        var data = packetData.GetDataAtPos(pos, size);
        var dataString = data;
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read unix base time stamp
/// </summary>
public class RulesActionReadUnixTimeStamp : RulesAction
{
    private static readonly DateTime UnixTime = DateTimeOffset.FromUnixTimeSeconds(0).DateTime;

    public RulesActionReadUnixTimeStamp(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetUInt64AtPos(pos, IsReversed);
        string dataString;
        try 
        {
            var d = UnixTime.AddSeconds(data);
            dataString = d + " (0x" + data.ToHex() + ")";
        }
        catch
        {
            dataString = "Invalid (0x" + data.ToHex() + ")";
        }
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read string
/// </summary>
public class RulesActionReadString : RulesAction
{
    private readonly Encoding _enc;
    private readonly bool _includesSize;

    public RulesActionReadString(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, Encoding useEncoding,bool includesSize = false) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _enc = useEncoding;
        _includesSize = includesSize;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        int size;
        var sizeFieldSize = 0;

        if (_includesSize)
        {
            size = packetData.GetUInt16AtPos(pos, IsReversed);
            sizeFieldSize = 2;
        }
        else
        {
            var sizeAttribute = XmlHelper.GetAttributeString(Attributes, "arg");
            ParentRule.TryGetLocalInt(sizeAttribute, 0, out size);
        }

        var varName = XmlHelper.GetAttributeString(Attributes, "name");

        // var untilNullChar = (size == 0);
        // when size arg is -1, keep reading everything until end of packet
        if (size == -1)
            size = packetData.ByteData.Count - pos - sizeFieldSize;

        var dBytes = packetData.GetDataBytesAtPos(pos + sizeFieldSize, size);
        var dl = new List<byte>();
        foreach (var dByte in dBytes)
        {
            if (dByte == 0)
                break;
            dl.Add(dByte);
        }
        var d = dl.ToArray();
        string stringVal;
        var hexVal = string.Empty;
        try
        {
            var isNull = true;
            foreach (var dChar in d)
            {
                if (dChar != 0)
                    isNull = false;
            }

            if ((size > 0) && (!isNull))
            {
                stringVal = _enc.GetString(d);

                if (Properties.Settings.Default.ShowStringHexData)
                {
                    foreach (var c in d)
                    {
                        if (hexVal != string.Empty)
                            hexVal += " ";
                        hexVal += (c).ToString("X2");
                    }
                    hexVal = " (" + hexVal + ")";
                }
                else
                {
                    hexVal = string.Empty;
                }
            }
            else
            {
                stringVal = "NULL";
                hexVal = string.Empty;
            }
        }
        catch (Exception ex)
        {
            stringVal = "Exception: " + ex.Message;
            hexVal = string.Empty;
        }

        ParentRule.SetLocalVar(varName, stringVal);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, stringVal + hexVal, Depth);
    }
}

/// <summary>
/// Comparison functions
/// </summary>
public class RulesActionCompareOperation : RulesAction
{
    public List<RulesAction> ChildActions;
    public List<RulesAction> ElseChildActions;
    private readonly string _arg1Name;
    private readonly string _arg2Name;
    private readonly string _operatorName;

    public RulesActionCompareOperation(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep,string arg1Name, string operatorName, string arg2Name) : base(parent, parentAction, thisNode, thisStep, false)
    {
        ChildActions = new List<RulesAction>();
        ElseChildActions = new List<RulesAction>();
        this._arg1Name = arg1Name;
        _operatorName = operatorName;
        this._arg2Name = arg2Name;

        var startElseBlock = false;

        ChildActions.Clear();
        for (var i = 0; i < thisNode.ChildNodes.Count; i++)
        {
            var actionNode = thisNode.ChildNodes.Item(i);
            if (actionNode == null)
                continue;
            var attributes = XmlHelper.ReadNodeAttributes(actionNode);
            var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
            if (newAction is RulesActionElse)
                startElseBlock = true;
            if (newAction != null)
            {
                if (startElseBlock)
                    ElseChildActions.Add(newAction);
                else
                    ChildActions.Add(newAction);
            }
        }
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        InternalLoopActionResult = LoopActionResult.Normal;
        var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);

        if (val1Attribute == string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg1Name + " can not be empty", Depth);
            return;
        }

        if (!ParentRule.TryGetLocalLong(val1Attribute, 0, out var val1))
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg1Name + ": " + val1Attribute, Depth);
            return;
        }

        long val2;
        var val2Attribute = "";
        
        if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
        {
            val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);

            if (val2Attribute == string.Empty)
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg2Name + " can not be empty", Depth);
                return;
            }

            if (!ParentRule.TryGetLocalLong(val2Attribute, 0, out val2))
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg2Name + ": " + val2Attribute, Depth);
                return;
            }
        }
        else
        {
            val2 = 0;
        }

        var res = false;
        switch (_operatorName)
        {
            case "==":
                res = (val1 == val2);
                break;
            case "<>":
            case "!=":
                res = (val1 != val2);
                break;
            case ">=":
                res = (val1 >= val2);
                break;
            case "<=":
                res = (val1 <= val2);
                break;
            case "<":
                res = (val1 < val2);
                break;
            case ">":
                res = (val1 > val2);
                break;
            default:
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid @@operator: " + _operatorName, Depth);
                break;
        }

        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, $"{res} <= {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2})", Depth, Color.DarkGray);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {res}");
        }

        // Do child actions
        if (res)
        {
            foreach (var child in ChildActions)
            {
                if (LoopActionResult != LoopActionResult.Normal)
                    break;

                try
                {
                    child.RunAction(packetData);
                }
                catch (Exception ex)
                {
                    packetData.AddParsedError("A" + child.GetActionStepName(), "Error@ \"" + child.Node.Name + "\"",
                        "Exception: " + ex.Message + " => " + child.Node.OuterXml, Depth);
                    break;
                }

                // Check for End of Packet
                if (packetData.Cursor <= packetData.ByteData.Count)
                    continue;

                LoopActionResult = LoopActionResult.Break;
                packetData.AddParsedError("A" + child.GetActionStepName(), Node.Name, "Reached past end of Packet Data",
                    Depth);

                if (MainForm.Instance?.ShowDebugInfo ?? false)
                    Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - EOP");

                break;
            }
        }
        else
        {
            foreach (var child in ElseChildActions)
            {
                if (LoopActionResult != LoopActionResult.Normal)
                    break;

                try
                {
                    child.RunAction(packetData);
                }
                catch (Exception ex)
                {
                    packetData.AddParsedError("A" + child.GetActionStepName(), "Error@ \"" + child.Node.Name + "\"",
                        "Exception: " + ex.Message + " => " + child.Node.OuterXml, Depth);
                    break;
                }

                // Check for End of Packet
                if (packetData.Cursor <= packetData.ByteData.Count)
                    continue;

                LoopActionResult = LoopActionResult.Break;
                packetData.AddParsedError("A" + child.GetActionStepName(), Node.Name, "Reached past end of Packet Data",
                    Depth);

                if (MainForm.Instance?.ShowDebugInfo ?? false)
                    Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - EOP");

                break;
            }
        }
    }
}

/// <summary>
/// This does not do anything on it's own, but is used to mark the start of a else part in a if block
/// </summary>
public class RulesActionElse : RulesAction
{

    public RulesActionElse(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        // Does not do anything on it's own, used by CompareOperation
    }

    public override void RunAction(BasePacketData packetData)
    {
        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, $"<else />", Depth, Color.DarkGray);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: else");
        }
    }
}


/// <summary>
/// Arithmetic functions
/// </summary>
public class RulesActionArithmeticOperation : RulesAction
{
    private readonly string _arg1Name;
    private readonly string _arg2Name;
    private readonly string _operatorName;
    private readonly string _destName;

    public RulesActionArithmeticOperation(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string arg1Name, string operatorName, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _arg1Name = arg1Name;
        _operatorName = operatorName;
        _arg2Name = arg2Name;
        _destName = destName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);

        if (val1Attribute == string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg1Name + " can not be empty", Depth);
            return;
        }

        if (!ParentRule.TryGetLocalLong(val1Attribute, 0, out var val1))
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg1Name + ": " + val1Attribute, Depth);
            return;
        }

        long val2 = 0;
        var val2Attribute = "<none>";
        if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
        {
            val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);

            if (val2Attribute == string.Empty)
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg2Name + " can not be empty", Depth);
                return;
            }

            if (!ParentRule.TryGetLocalLong(val2Attribute, 0, out val2))
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg2Name + ": " + val2Attribute, Depth);
                return;
            }
        }
        var destinationAttribute = XmlHelper.GetAttributeString(Attributes, _destName);

        long res = 0;
        switch (_operatorName)
        {
            case "+":
                res = (val1 + val2);
                break;
            case "-":
                res = (val1 - val2);
                break;
            case "x":
            case "*":
                res = (val1 * val2);
                break;
            case "/":
                if (val2 == 0)
                {
                    packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Division by Zero!", Depth);
                    break;
                }
                res = (val1 / val2);
                break;
            case "%":
                if (val2 == 0)
                {
                    packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Division by Zero!", Depth);
                    break;
                }
                res = (val1 % val2);
                break;
            case "&":
                res = (val1 & val2);
                break;
            case "|":
                res = (val1 | val2);
                break;
            case "<<":
                // By default the shift bit instructions cannot be applied to long, so we limit it to int
                res = ((int)val1 << (int)val2);
                break;
            case ">>":
                // By default the shift bit instructions cannot be applied to long, so we limit it to int
                res = ((int)val1 >> (int)val2);
                break;
            case "=":
                res = (val1);
                break;
            default:
                res = 0 ;
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid @@operator: " + _operatorName, Depth);
                break;
        }

        ParentRule.SetLocalVar(destinationAttribute, res.ToString(CultureInfo.InvariantCulture));
        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "(" + val1.ToString() + " " + _operatorName + " " + val2.ToString() + ") => " + res.ToString() + " => " + destinationAttribute, Depth);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {destinationAttribute}({res})");
        }
    }
}

/// <summary>
/// Floating-point Arithmetic functions
/// </summary>
public class RulesActionDoubleArithmeticOperation : RulesAction
{
    private readonly string _arg1Name;
    private readonly string _arg2Name;
    private readonly string _operatorName;
    private readonly string _destName;

    public RulesActionDoubleArithmeticOperation(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string arg1Name, string operatorName, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _arg1Name = arg1Name;
        _operatorName = operatorName;
        _arg2Name = arg2Name;
        _destName = destName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);

        if (val1Attribute == string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg1Name + " can not be empty", Depth);
            return;
        }

        if (!ParentRule.TryGetLocalDouble(val1Attribute, 0.0, out var val1))
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg1Name + ": " + val1Attribute, Depth);
            return;
        }


        double val2 = 0;
        var val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);

        if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
        {
            if (val2Attribute == string.Empty)
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _arg2Name + " can not be empty", Depth);
                return;
            }

            if (!ParentRule.TryGetLocalDouble(val2Attribute, 0.0, out val2))
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid " + _arg2Name + ": " + val2Attribute, Depth);
                return;
            }
        }
        var destinationAttribute = XmlHelper.GetAttributeString(Attributes, _destName);

        var res = double.NaN ;
        switch (_operatorName)
        {
            case "+":
                res = (val1 + val2);
                break;
            case "-":
                res = (val1 - val2);
                break;
            case "x":
            case "*":
                res = (val1 * val2);
                break;
            case "/":
                if (val2 == 0.0)
                {
                    packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Division by Zero!", Depth);
                    res = double.NaN;
                    break;
                }
                res = (val1 / val2);
                break;
            case "=":
                res = (val1);
                break;
            default:
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Invalid @@operator: " + _operatorName, Depth);
                break;
        }

        ParentRule.SetLocalVar(destinationAttribute, res.ToString(CultureInfo.InvariantCulture));
        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "(" + val1.ToString(CultureInfo.InvariantCulture) + " " + _operatorName + " " + val2.ToString(CultureInfo.InvariantCulture) + ") => " + res.ToString(CultureInfo.InvariantCulture) + " => " + destinationAttribute, Depth);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {destinationAttribute}({res})");
        }
    }
}

/// <summary>
/// Loop function
/// </summary>
public class RulesActionLoop : RulesAction
{
    public List<RulesAction> ChildActions;
    private readonly string _maxCountLoopName;
    public int MaxSafetyCount = 255;

    public RulesActionLoop(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string maximumLoopCount = "") : base(parent, parentAction, thisNode, thisStep, false)
    {
        ChildActions = new List<RulesAction>();
        for (var i = 0; i < thisNode.ChildNodes.Count; i++)
        {
            var actionNode = thisNode.ChildNodes.Item(i);
            if (actionNode == null)
                continue;
            var attributes = XmlHelper.ReadNodeAttributes(actionNode);

            var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
            if (newAction != null)
                ChildActions.Add(newAction);
        }

        _maxCountLoopName = maximumLoopCount;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        if (_maxCountLoopName != string.Empty)
        {
            int val1 = 255;
            var val1Attribute = XmlHelper.GetAttributeString(Attributes, _maxCountLoopName);

            if ((val1Attribute != string.Empty) && (NumberHelper.TryFieldParse(val1Attribute, out int aMaxCount)))
            {
                val1 = aMaxCount;
            }

            MaxSafetyCount = val1;
        }

        if (MaxSafetyCount < 1)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Maximum loop count must be bigger than zero", Depth);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Maximum loop count must be bigger than zero");
            return;
        }

        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, $"{this.GetType().Name} - Begin", Depth);
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Begin");
        }

        // Do child actions
        LoopActionResult = LoopActionResult.Normal;
        var safetyCounter = 0;
        while (LoopActionResult == LoopActionResult.Normal)
        {
            for (var i = 0; i < ChildActions.Count; i++)
            {
                var child = ChildActions[i];
                if (MainForm.Instance?.ShowDebugInfo ?? false)
                {
                    Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Step {i} ({child.Node.Name})");
                }
                try
                {
                    child.RunAction(packetData);
                }
                catch (Exception ex)
                {
                    packetData.AddParsedError("A" + child.GetActionStepName(), "Error@ \"" + child.Node.Name + "\"", "Exception: " + ex.Message + " => " + child.Node.OuterXml, Depth);
                    Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Step {i} ({child.Node.Name}) Exception: {ex.Message}");
                    LoopActionResult = LoopActionResult.Break ;
                }
                
                if (LoopActionResult == LoopActionResult.Continue)
                {
                    if (MainForm.Instance?.ShowDebugInfo ?? false)
                        Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Continue");

                    continue;
                }
                if (LoopActionResult == LoopActionResult.Break)
                {
                    if (MainForm.Instance?.ShowDebugInfo ?? false)
                        Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Break");

                    break;
                }

                // Check for End of PacketData
                if (packetData.Cursor > packetData.ByteData.Count)
                {
                    // Force break the loop if past end of packet
                    LoopActionResult = LoopActionResult.Break;
                    packetData.AddParsedError("A" + child.GetActionStepName(), Node.Name, "Reached past end of Packet Data", Depth);
                    if (MainForm.Instance?.ShowDebugInfo ?? false)
                        Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - EOP");

                    break;
                }
            }
            safetyCounter++;
            
            if (safetyCounter > MaxSafetyCount)
            {
                Debug.WriteLine($"{this.ParentRule.Name} {GetActionStepName()} {this.GetType().Name} - SafetyCheck, took over {MaxSafetyCount} loops");
                break;
            }

            if (LoopActionResult == LoopActionResult.Break)
                break;
        }

        LoopActionResult = LoopActionResult.Normal;
        if (MainForm.Instance?.ShowDebugInfo ?? false)
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - End");
    }
}

/// <summary>
/// Issues a Break command to the parent Loop
/// </summary>
public class RulesActionBreak : RulesAction
{
    public RulesActionBreak(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        if (ParentAction != null)
        {
            if (MainForm.Instance?.ShowDebugInfo ?? false)
            {
                packetData.AddParsedError("A" + GetActionStepName(), "Break", "", Depth);
            }
            LoopActionResult = LoopActionResult.Break;
        }
        else
        {
            packetData.AddParsedError("A" + GetActionStepName(), "Break Error", "Missing parent node", Depth);
        }
    }
}

/// <summary>
/// Issues a Continue command to the parent Loop
/// </summary>
public class RulesActionContinue : RulesAction
{
    public RulesActionContinue(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        if (ParentAction != null)
        {
            if (MainForm.Instance?.ShowDebugInfo ?? false)
            {
                packetData.AddParsedError("A" + GetActionStepName(), "Continue", "", Depth);
            }
            LoopActionResult = LoopActionResult.Continue;
        }
        else
        {
            packetData.AddParsedError("A" + GetActionStepName(), "Continue Error", "Missing parent node", Depth);
        }
    }
}

/// <summary>
/// Saves a value in the project's lookup tables
/// </summary>
public class RulesActionSaveLookup : RulesAction
{
    private readonly string _sourceValueName;
    private readonly string _sourceIdName;
    private readonly string _destName;
    private readonly string _altLookupTableName;
    private readonly string _saveLookupName;

    public RulesActionSaveLookup(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string sourceIdField, string sourceValueField, string altLookupTable, string destName, string saveLookupName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _sourceIdName = sourceIdField;
        _sourceValueName = sourceValueField;
        _destName = destName;
        _altLookupTableName = altLookupTable;
        _saveLookupName = saveLookupName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var idAttribute = XmlHelper.GetAttributeString(Attributes, _sourceIdName);
        if (idAttribute == string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _sourceIdName + " can not be empty", Depth);
            return;
        }
        long sourceId = 0;
        var sourceIdString = ParentRule.GetLocalVar(idAttribute);
        if (NumberHelper.TryFieldParse(sourceIdString, out long sourceIdParse))
        {
            sourceId = sourceIdParse;
        }
        else
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _sourceIdName + " must be a integer value field", Depth);
        }

        var valAttribute = XmlHelper.GetAttributeString(Attributes, _sourceValueName);
        if (valAttribute == string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _sourceValueName + " can not be empty", Depth);
            return;
        }

        var sourceValue = ParentRule.GetLocalVar(valAttribute);
        var saveLookup = XmlHelper.GetAttributeString(Attributes, _saveLookupName);
        
        var altLookupAttribute = XmlHelper.GetAttributeString(Attributes, _altLookupTableName);
        if (altLookupAttribute != string.Empty)
        {
            if (NumberHelper.TryFieldParse(sourceValue, out long sourceValueParsed))
            {
                var lookedUpValue = packetData.ParentProject.DataLookup.NLU(altLookupAttribute).GetValue((ulong)sourceValueParsed);
                sourceValue = "(" + altLookupAttribute + ":" + sourceValueParsed + ") " + lookedUpValue;
                if (saveLookup != string.Empty)
                {
                    // save result as var
                    ParentRule.SetLocalVar(saveLookup, lookedUpValue);
                }
            }
            else
            {
                packetData.AddParsedError("A" + GetActionStepName(), Node.Name, _sourceValueName + " must be a integer value field to use the alt lookups function", Depth);
            }
        }
        else if (saveLookup != string.Empty)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Save lookup to local var requires to also have alt lookup set", Depth);
        }

        var destListName = XmlHelper.GetAttributeString(Attributes, _destName);

        packetData.ParentProject.DataLookup.RegisterCustomLookup(destListName, (ulong)sourceId, sourceValue);
        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Save (" + sourceId + " => " + sourceValue + ") into " + destListName, Depth);
            Console.WriteLine($"{GetActionStepName()} {GetType().Name}: {sourceId}({idAttribute}) => {sourceValue}({valAttribute}) into {destListName}");
        }
    }
}

/// <summary>
/// Show a comment
/// </summary>
public class RulesActionComment : RulesAction
{
    public string Comment;

    public RulesActionComment(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        Comment = thisNode.InnerText.Trim();
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        const string varName = "#comment";
        packetData.AddParsedError(pos.ToHex(2), varName, Comment, Depth, Color.DarkGray);
    }
}

/// <summary>
/// uint as milliseconds
/// </summary>
public class RulesActionReadUInt32Ms : RulesAction
{
    public RulesActionReadUInt32Ms(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var pos = packetData.Cursor;
        var data = packetData.GetUInt32AtPos(pos, IsReversed);
        var dataString = data.AsMilliseconds() + " - " + data.ToHex();
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var lookupVal = GetLookup(data);
        ParentRule.SetLocalVar(varName, data.ToString(CultureInfo.InvariantCulture));
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, lookupVal + dataString, Depth);
    }
}

/// <summary>
/// Execute/Inject a template
/// </summary>
public class RulesActionTemplate : RulesAction
{
    public List<RulesAction> ChildActions;
    public string TemplateName;

    public RulesActionTemplate(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
    {
        ChildActions = new List<RulesAction>();
        TemplateName = XmlHelper.GetAttributeString(Attributes, name.ToLower());
        var templates = parent.Parent.Parent.Templates;
        if (!templates.TryGetValue(TemplateName, out var thisTemplate))
        {
            Debug.WriteLine("Template not found: " + TemplateName);
            return;
        }

        for (var i = 0; i < thisTemplate.ChildNodes.Count; i++)
        {
            var actionNode = thisTemplate.ChildNodes.Item(i);
            if (actionNode == null)
                continue;

            var attributes = XmlHelper.ReadNodeAttributes(actionNode);

            var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
            if (newAction != null)
                ChildActions.Add(newAction);
        }
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        if (MainForm.Instance?.ShowDebugInfo ?? false)
        {
            packetData.AddParsedError("A" + GetActionStepName(), Node.Name, "Begin Template: " + TemplateName, Depth);
            Debug.WriteLine("{1} {0} - Begin Template", this.GetType().Name, GetActionStepName());
        }

        // Do child actions
        for (var i = 0; i < ChildActions.Count; i++)
        {
            var child = ChildActions[i];
            if (MainForm.Instance?.ShowDebugInfo ?? false)
            {
                Debug.WriteLine("{1} {0} - Template Step {2} ({3})", this.GetType().Name, GetActionStepName(), i, child.Node.Name);
            }
            try
            {
                child.RunAction(packetData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{1} {0} - Template Step {2} ({3}) Exception: {4}", this.GetType().Name, GetActionStepName(), i, child.Node.Name, ex.Message);
                packetData.AddParsedError("A" + child.GetActionStepName(), "Error at \"" + child.Node.Name + "\"", "Exception: " + ex.Message + " => " + child.Node.OuterXml, Depth);
                LoopActionResult = LoopActionResult.Break;
            }

            // Check for End of Packet

            if (packetData.Cursor > packetData.ByteData.Count)
            {
                LoopActionResult = LoopActionResult.Break;
                packetData.AddParsedError("A" + child.GetActionStepName(), Node.Name, "Reached past end of Packet Data", Depth);
                if (MainForm.Instance?.ShowDebugInfo ?? false)
                    Debug.WriteLine("{1} {0} - EOP", this.GetType().Name, GetActionStepName());

                break;
            }
        }

        if (MainForm.Instance?.ShowDebugInfo ?? false)
            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - End Template");
    }
}

/// <summary>
/// Outputs a local var to the field view, can be used as a opening and closing tag to form a block
/// </summary>
public class RulesActionEcho : RulesAction
{
    public string FieldName;
    private List<RulesAction> ChildActions;

    public RulesActionEcho(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
    {
        FieldName = XmlHelper.GetAttributeString(Attributes, name.ToLower());

        ChildActions = new List<RulesAction>();
        for (var i = 0; i < thisNode.ChildNodes.Count; i++)
        {
            var actionNode = thisNode.ChildNodes.Item(i);
            if (actionNode == null)
                continue;
            var attributes = XmlHelper.ReadNodeAttributes(actionNode);
            var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
            if (newAction != null)
                ChildActions.Add(newAction);
        }
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var dataString = ParentRule.GetLocalVar(FieldName);
        var hexString = string.Empty;
        var lookupVal = string.Empty;

        // Handle output differently if float val
        if (double.TryParse(dataString, out var valDouble) && (Math.Abs(Math.Floor(valDouble) - valDouble) > double.Epsilon))
        {
            packetData.AddParsedError("A"+GetActionStepName(), FieldName, valDouble.ToString(CultureInfo.InvariantCulture), Depth);
            return;
        }

        // check if it's a natural number
        if (NumberHelper.TryFieldParse(dataString, out ulong valNumber))
        {
            hexString = " - " + valNumber.ToHex(1);
            lookupVal = GetLookup(valNumber);
        }
        else
        // is it a signed number maybe
        if (NumberHelper.TryFieldParse(dataString, out long valInt))
        {
            hexString = " - " + valInt.ToHex();
            lookupVal = GetLookup((ulong)valInt);
        }
        packetData.AddParsedError("A" + GetActionStepName(), FieldName, lookupVal + dataString + hexString, Depth);

        // Do child actions
        foreach (var child in ChildActions)
        {
            try
            {
                child.RunAction(packetData);
            }
            catch (Exception ex)
            {
                packetData.AddParsedError("A" + child.GetActionStepName(), "Error@ \"" + child.Node.Name + "\"", "Exception: " + ex.Message + " => " + child.Node.OuterXml, Depth);
                break;
            }
        }
    }
}

/// <summary>
/// Read bits
/// local vars are stored as name-bit_number as 0 or 1
/// </summary>
public class RulesActionReadBits : RulesAction
{
    private readonly string _bitCountName;
    private readonly string _styleName;

    public RulesActionReadBits(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string bitCountName, string styleName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _bitCountName = bitCountName;
        _styleName = styleName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        uint bitCount = 64;
        var style = "normal";

        if (XmlHelper.HasAttribute(Attributes, _bitCountName))
        {
            bitCount = (uint)XmlHelper.GetAttributeInt(Attributes, _bitCountName);
        }
        if (XmlHelper.HasAttribute(Attributes, _styleName))
        {
            style = XmlHelper.GetAttributeString(Attributes, _styleName);
        }

        var fieldName = XmlHelper.GetAttributeString(Attributes, "name");

        var pos = packetData.Cursor;
        var endBitPos = ((packetData.Cursor * 8) + packetData.BitCursor + (int)bitCount); // end bit
        var endPos = ((endBitPos % 8) == 0) ? (endBitPos / 8) - 1 : (endBitPos / 8); // end byte
        var posFieldString = pos.ToHex(2) + ":" + packetData.BitCursor + "~" + (packetData.BitCursor + bitCount - 1);
        
        if (style == "normal")
        {
            packetData.AddParsedField(true, pos, endPos, posFieldString, XmlHelper.GetAttributeString(Attributes, "name"), "", Depth);

            // Normal style: Each set bit generates a line
            for (ulong i = 0; i < bitCount; i++)
            {
                var byteCursor = packetData.Cursor;
                var bitVal = packetData.GetBitAtPos(packetData.Cursor, packetData.BitCursor);
                var varName = fieldName + "-" + i;
                var lookupVal = GetLookup(i);
                var dataString = "Bit " + i;
                ParentRule.SetLocalVar(varName+"-"+i, bitVal ? "1" : "0");
                if (bitVal || (bitCount == 1))
                    packetData.AddParsedField(true, byteCursor, byteCursor, pos.ToHex(2) + "-" + i, varName, lookupVal + dataString, Depth+1);
            }
        }
        else
        if (style == "full")
        {
            packetData.AddParsedField(false, pos, endPos, posFieldString, XmlHelper.GetAttributeString(Attributes, "name"), "", Depth);

            // Full style: All bits generates a line
            for (ulong i = 0; i < bitCount; i++)
            {
                var byteCursor = packetData.Cursor;
                var bitVal = packetData.GetBitAtPos(packetData.Cursor, packetData.BitCursor);
                var varName = fieldName + "-" + i;
                var lookupVal = GetLookup(i);
                var dataString = "Bit " + i + " - " + bitVal.ToString(CultureInfo.InvariantCulture);
                ParentRule.SetLocalVar(varName + "-" + i, bitVal ? "1" : "0");
                packetData.AddParsedField(true, byteCursor, byteCursor, pos.ToHex(2) + "-" + i, varName, lookupVal + dataString, Depth+1);
            }
        }
        else
        if (style == "compact")
        {
            // Compact style: Bit Ids are omitted, but replaced by their lookup values when set,
            // and shown as one line
            var dataString = string.Empty;
            for (ulong i = 0; i < bitCount; i++)
            {
                var bitVal = packetData.GetBitAtPos(packetData.Cursor, packetData.BitCursor);
                var lookupVal = GetLookup(i, 0, false);
                if (bitCount == 1)
                {
                    if (lookupVal != string.Empty)
                        dataString += lookupVal + " = " + bitVal.ToString();
                    else
                        dataString += bitVal.ToString();
                }
                else
                if (bitVal)
                {
                    if (lookupVal != string.Empty)
                        dataString += lookupVal + " ";
                    else
                        dataString += "[bit " + i + "] ";
                }
                ParentRule.SetLocalVar(fieldName + "-" + i, bitVal ? "1" : "0");
            }

            packetData.AddParsedField(true, pos, endPos, posFieldString, fieldName, dataString, Depth);
        }
        else
        {
            // Unknown style
            packetData.AddParsedError("A" + GetActionStepName(), fieldName, "Unknown style: " + style+ " (allowed styles are normal, full, compact)", Depth);
        }

    }
}

/// <summary>
/// Read bits as a uint64 (long)
/// </summary>
public class RulesActionReadBitValue : RulesAction
{
    private readonly string _bitCountName;

    public RulesActionReadBitValue(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, string bitCountName) : base(parent, parentAction, thisNode, thisStep, false)
    {
        _bitCountName = bitCountName;
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        int bitCount = 64;

        if (XmlHelper.HasAttribute(Attributes, _bitCountName))
        {
            bitCount = (int)XmlHelper.GetAttributeInt(Attributes, _bitCountName);
        }

        var fieldName = XmlHelper.GetAttributeString(Attributes, "name");

        var pos = packetData.Cursor;
        var endBitPos = ((packetData.Cursor * 8) + packetData.BitCursor + (int)bitCount); // end bit
        var endPos = ((endBitPos % 8) == 0) ? (endBitPos / 8) - 1 : (endBitPos / 8); // end byte
        var posFieldString = pos.ToHex(2) + ":" + packetData.BitCursor + "~" + (packetData.BitCursor + bitCount - 1);

        var data = packetData.GetBitsAtPos(packetData.Cursor, packetData.BitCursor, bitCount);
        var lookupVal = GetLookup((ulong)data);

        packetData.AddParsedField(true, pos, endPos, posFieldString, fieldName, lookupVal + data.ToString() + " - " + data.ToHex(1), Depth);
        ParentRule.SetLocalVar(fieldName, data.ToString());
    }
}

/// <summary>
/// Read IPv4
/// </summary>
public class RulesActionReadIp4 : RulesAction
{
    public RulesActionReadIp4(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var pos = packetData.Cursor;
        var data = packetData.GetDataBytesAtPos(pos, 4);
        if (IsReversed)
            data = data.Reverse().ToArray();
        var dataString = string.Join('.', data);
        ParentRule.SetLocalVar(varName, dataString);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}

/// <summary>
/// Read IPv6
/// </summary>
public class RulesActionReadIp6 : RulesAction
{
    public RulesActionReadIp6(PacketRule parent, RulesAction? parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
    {
        //
    }

    public override void RunAction(BasePacketData packetData)
    {
        GotoStartPosition(packetData);
        var varName = XmlHelper.GetAttributeString(Attributes, "name");
        var pos = packetData.Cursor;
        var dataString = string.Empty;
        for (var i = 0; i < 8; i++)
        {
            var data = packetData.GetUInt16AtPos(packetData.Cursor);
            if (i > 0)
                dataString += ":";
            if (data > 0)
                dataString += data.ToHex(1);
        }
        ParentRule.SetLocalVar(varName, dataString);
        packetData.AddParsedField(true, pos, packetData.Cursor - 1, pos.ToHex(2), varName, dataString, Depth);
    }
}
