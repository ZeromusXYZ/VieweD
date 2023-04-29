using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using VieweD.Helpers.System;

namespace VieweD.engine.common;

public class PacketRule
{
    public XmlNode RootNode { get; protected set; }
    public RulesGroup Parent { get; protected set; }
    public byte StreamId { get; protected set; }
    public byte Level { get; protected set; }
    public ushort PacketId { get; protected set; }
    public string Name { get; set; }
    public List<RulesAction> Actions { get; protected set; }
    public Dictionary<string, string> LocalVars { get; protected set; }

    public virtual ulong LookupKey => PacketFilterListEntry.EncodeFilterKey(PacketId, Level, StreamId);

    public PacketRule(RulesGroup parent, byte streamId, byte level, ushort packetId, string description, XmlNode node)
    {
        Parent = parent;
        RootNode = node;
        StreamId = streamId;
        Level = level;
        PacketId = packetId;
        Name = description;
        Actions = new List<RulesAction>();
        LocalVars = new Dictionary<string, string>();
    }

    /// <summary>
    /// Custom parser needs to override this to add more Action Types / Functions
    /// </summary>
    /// <param name="parentAction"></param>
    /// <param name="actionNode"></param>
    /// <param name="attributes"></param>
    /// <param name="step"></param>
    /// <returns>Returns at least basic RulesAction</returns>
    public virtual RulesAction BuildFallbackAction(RulesAction? parentAction, XmlNode actionNode,  Dictionary<string, string> attributes, int step)
    {
        return new RulesAction(this, parentAction, actionNode, step, false);
    }

    /// <summary>
    /// Custom parser needs to override this to add more data types
    /// </summary>
    /// <param name="parentAction"></param>
    /// <param name="actionNode"></param>
    /// <param name="attributes"></param>
    /// <param name="step"></param>
    /// <param name="dataType"></param>
    /// <param name="isReversed"></param>
    /// <returns>Returns at least a basic RulesAction</returns>
    public virtual RulesAction BuildFallbackDataAction(RulesAction? parentAction, XmlNode actionNode, Dictionary<string, string> attributes, int step, string dataType, bool isReversed)
    {
        return new RulesAction(this, parentAction, actionNode, step, isReversed);
    }

    public virtual RulesAction? BuildRuleAction(RulesAction? parentAction, XmlNode actionNode, Dictionary<string, string> attributes, int step)
    {
        RulesAction? res = null;
        switch (actionNode.Name.ToLower())
        {
            case "chunk":
            case "data": 
                // Normal data
                var dataType = XmlHelper.GetAttributeString(attributes, "type").ToLower();
                var isReversed = dataType.StartsWith("r-");
                switch (dataType)
                {
                    case "b":
                    case "byte":
                        res = new RulesActionReadByte(this, parentAction, actionNode, step);
                        break;
                    case "sbyte":
                        res = new RulesActionReadSByte(this, parentAction, actionNode, step);
                        break;
                    case "w":
                    case "uint16":
                    case "r-uint16":
                    case "word":
                    case "r-word":
                    case "ushort":
                    case "r-ushort":
                        res = new RulesActionReadUInt16(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "int16":
                    case "r-int16":
                    case "h":
                    case "r-h":
                    case "short":
                    case "r-short":
                        res = new RulesActionReadInt16(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "uint32":
                    case "r-uint32":
                    case "d":
                    case "r-d":
                    case "uint":
                    case "r-uint":
                        res = new RulesActionReadUInt32(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "ms":
                    case "rms":
                        res = new RulesActionReadUInt32Ms(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "ms64":
                    case "rms64":
                        res = new RulesActionReadUInt64Ms(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "int32":
                    case "r-int32":
                    case "i":
                    case "r-i":
                    case "int":
                    case "r-int":
                        res = new RulesActionReadInt32(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "f":
                    case "r-f":
                    case "float":
                    case "r-float":
                        res = new RulesActionReadSingle(this, parentAction, actionNode, step);
                        break;
                    case "double":
                    case "r-double":
                        res = new RulesActionReadDouble(this, parentAction, actionNode, step);
                        break;
                    case "half":
                        res = new RulesActionReadHalf(this, parentAction, actionNode, step);
                        break;
                    case "q":
                    case "r-q":
                    case "ulong":
                    case "uint64":
                    case "r-uint64":
                        res = new RulesActionReadUInt64(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "long": // not used
                    case "int64": // not used
                    case "r-int64": // not used
                        res = new RulesActionReadInt64(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "a":
                        res = new RulesActionReadArray(this, parentAction, actionNode, step);
                        break;
                    case "ts":
                    case "r-ts":
                        res = new RulesActionReadUnixTimeStamp(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "s":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.ASCII);
                        break;
                    case "u":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.Unicode);
                        break;
                    case "t":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.UTF8);
                        break;
                    case "zs":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.ASCII, true);
                        break;
                    case "zu":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.Unicode, true);
                        break;
                    case "zu8":
                        res = new RulesActionReadString(this, parentAction, actionNode, step, Encoding.UTF8, true);
                        break;
                    case "uint24":
                    case "bc":
                    case "r-uint24":
                    case "r-bc":
                        res = new RulesActionReadUInt24(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "bits":
                        res = new RulesActionReadBits(this, parentAction, actionNode, step, "bits", "style");
                        break;
                    case "bitval":
                    case "bitvalue":
                    case "valuebits":
                        res = new RulesActionReadBitValue(this, parentAction, actionNode, step, "bits");
                        break;
                    case "ip4":
                    case "ipv4":
                    case "r-ip4":
                    case "r-ipv4":
                        res = new RulesActionReadIp4(this, parentAction, actionNode, step, isReversed);
                        break;
                    case "ip6":
                    case "ipv6":
                        res = new RulesActionReadIp6(this, parentAction, actionNode, step);
                        break;
                    default:
                        // Blank Command
                        res = BuildFallbackDataAction(parentAction, actionNode, attributes, step, dataType, isReversed);
                        break;
                }
                break;
            case "else":
                res = new RulesActionElse(this, parentAction, actionNode, step);
                break;
            case "ifeq":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", "==", "arg2");
                break;
            case "ifneq":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", "!=", "arg2");
                break;
            case "iflt":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", "<", "arg2");
                break;
            case "ifgt":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", ">", "arg2");
                break;
            case "iflte":
            case "ifle":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", "<=", "arg2");
                break;
            case "ifgte":
            case "ifge":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg1", ">=", "arg2");
                break;
            case "ifz":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg", "==", "0");
                break;
            case "ifnz":
                res = new RulesActionCompareOperation(this, parentAction, actionNode, step, "arg", "!=", "0");
                break;
            case "add":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "+", "arg2", "dst");
                break;
            case "sub":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "-", "arg2", "dst");
                break;
            case "mul":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "*", "arg2", "dst");
                break;
            case "div":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "/", "arg2", "dst");
                break;
            case "addf":
                res = new RulesActionDoubleArithmeticOperation(this, parentAction, actionNode, step, "arg1", "+", "arg2", "dst");
                break;
            case "subf":
                res = new RulesActionDoubleArithmeticOperation(this, parentAction, actionNode, step, "arg1", "-", "arg2", "dst");
                break;
            case "mulf":
                res = new RulesActionDoubleArithmeticOperation(this, parentAction, actionNode, step, "arg1", "*", "arg2", "dst");
                break;
            case "divf":
                res = new RulesActionDoubleArithmeticOperation(this, parentAction, actionNode, step, "arg1", "/", "arg2", "dst");
                break;
            case "shl":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "<<", "arg2", "dst");
                break;
            case "shr":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", ">>", "arg2", "dst");
                break;
            case "and":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "&", "arg2", "dst");
                break;
            case "or":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "arg1", "|", "arg2", "dst");
                break;
            case "mov":
            case "assign":
                res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "val", "=", "", "dst");
                break;
            case "movf":
            case "assignf":
                res = new RulesActionDoubleArithmeticOperation(this, parentAction, actionNode, step, "val", "=", "", "dst");
                break;
            case "loop":
                res = new RulesActionLoop(this, parentAction, actionNode, step, "max");
                break;
            case "break":
                res = new RulesActionBreak(this, parentAction, actionNode, step);
                break;
            case "continue":
                res = new RulesActionContinue(this, parentAction, actionNode, step);
                break;
            case "lookup":
                res = new RulesActionSaveLookup(this, parentAction, actionNode, step, "source", "val", "altlookup", "save", "savelookup");
                break;
            case "template":
                res = new RulesActionTemplate(this, parentAction, actionNode, step, "name");
                break;
            case "echo":
                res = new RulesActionEcho(this, parentAction, actionNode, step, "arg");
                break;
            case "export":
                res = new RulesActionToolsExport(this, parentAction, actionNode, step, "name");
                break;
            case "cur":
            case "cursor":
                res = new RulesActionCursor(this, parentAction, actionNode, step);
                break;
            case "#comment":
                res = new RulesActionComment(this, parentAction, actionNode, step);
                break;
            case "#whitespace":
                // Ignore
                break;
            default:
                // Unsupported type
                // Blank Command
                res = BuildFallbackAction(parentAction, actionNode, attributes, step);
                break;
        }
        return res;
    }

    public void Build()
    {
        LocalVars.Clear();
        Actions.Clear();
        for (var i = 0; i < RootNode.ChildNodes.Count; i++)
        {
            var actionNode = RootNode.ChildNodes.Item(i);
            if (actionNode == null)
                continue;

            var attributes = XmlHelper.ReadNodeAttributes(actionNode);
            var newAction = BuildRuleAction(null, actionNode, attributes, i);
            if (newAction != null)
                Actions.Add(newAction);
        }
    }

    public virtual void RunRule(BasePacketData packetData)
    {
        packetData.ParsedData.Clear();
        Parent.Parent.ParsePacketHeader(packetData);
        // Settings some defaults
        SetLocalVar("p_size", packetData.PacketDataSize.ToString(CultureInfo.InvariantCulture));
        SetLocalVar("p_type", packetData.PacketId.ToString(CultureInfo.InvariantCulture));
        //SetLocalVar("p_level", packetData.OriginalPacketLevel.ToString(CultureInfo.InvariantCulture));
        //SetLocalVar("pSize", packetData.PacketDataSize.ToString(CultureInfo.InvariantCulture));
        //SetLocalVar("pType", packetData.PacketId.ToString(CultureInfo.InvariantCulture));
        //SetLocalVar("pLevel", packetData.OriginalPacketLevel.ToString(CultureInfo.InvariantCulture));

        // Run all actions
        foreach (var action in Actions)
        {
            try
            {
                action.RunAction(packetData);
            }
            catch (Exception ex)
            {
                packetData.AddParsedError("A" + action.GetActionStepName(),"Error at \"" + action.Node.Name + "\"", "Exception: " + ex.Message + " => " + action.Node.OuterXml, 0);
                break;
            }
        }
    }

    public void SetLocalVar(string name, string newValue)
    {
        newValue = newValue.TrimEnd().TrimEnd('\0');
        if (LocalVars.TryGetValue(name, out _))
            LocalVars.Remove(name);
        LocalVars.Add(name, newValue);
    }

    public string GetLocalVar(string name)
    {
        // If a var name starts with a exclamation point, then treat the name itself as the result
        if (name.StartsWith("!"))
            return name[1..];

        if (LocalVars.TryGetValue(name, out var res))
        {
            return res;
        }

        SetLocalVar(name, string.Empty);
        return string.Empty;
    }

    public bool TryGetLocalInt(string unparsed, int defaultValue, out int outVal)
    {
        outVal = defaultValue;

        if (unparsed.StartsWith("#"))
            unparsed = GetLocalVar(unparsed.TrimStart('#'));

        return unparsed != string.Empty && NumberHelper.TryFieldParse(unparsed, out outVal);
    }

    public bool TryGetLocalLong(string unparsed, long defaultValue, out long outVal)
    {
        outVal = defaultValue;

        if (unparsed.StartsWith("#"))
            unparsed = GetLocalVar(unparsed.TrimStart('#'));

        return unparsed != string.Empty && NumberHelper.TryFieldParse(unparsed, out outVal);
    }

    public bool TryGetLocalLong(string unparsed, ulong defaultValue, out ulong outVal)
    {
        outVal = defaultValue;

        if (unparsed.StartsWith("#"))
            unparsed = GetLocalVar(unparsed.TrimStart('#'));

        return unparsed != string.Empty && NumberHelper.TryFieldParse(unparsed, out outVal);
    }

    public bool TryGetLocalDouble(string unparsed, double defaultValue, out double outVal)
    {
        outVal = defaultValue;

        if (unparsed.StartsWith("#"))
            unparsed = GetLocalVar(unparsed.TrimStart('#'));

        return unparsed != string.Empty && double.TryParse(unparsed, NumberStyles.Float, CultureInfo.InvariantCulture, out outVal);
    }
}