using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Xml;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class PacketRule
    {
        // ReSharper disable once InconsistentNaming
        public XmlNode _rootNode { get; protected set; }
        // ReSharper disable once InconsistentNaming
        public RulesGroup _parent { get; protected set; }
        public byte StreamId { get; protected set; }
        public byte Level { get; protected set; }
        public UInt16 PacketId { get; protected set; }
        public string Name { get; protected set; }
        public List<RulesAction> Actions { get; protected set; }
        public Dictionary<string, string> LocalVars { get; protected set; }

        public virtual uint LookupKey => (uint)((StreamId * 0x1000000) + (Level * 0x10000) + PacketId);

        public PacketRule(RulesGroup parent, byte streamId, byte level, UInt16 packetId, string description, XmlNode node)
        {
            _parent = parent;
            _rootNode = node;
            StreamId = streamId;
            Level = level;
            PacketId = packetId;
            Name = description;
            Actions = new List<RulesAction>();
            LocalVars = new Dictionary<string, string>();
        }

        public virtual RulesAction BuildRuleAction(RulesAction parentAction, XmlNode actionNode, Dictionary<string, string> attribs, int step)
        {
            RulesAction res = null;
            switch (actionNode.Name.ToLower())
            {
                case "data": // Normal data
                    var dataType = XmlHelper.GetAttributeString(attribs, "type").ToLower();
                    var isReversed = dataType.StartsWith("r");
                    switch (dataType)
                    {
                        case "byte":
                            res = new RulesActionReadByte(this, parentAction, actionNode, step);
                            break;
                        case "word":
                        case "rword":
                            res = new RulesActionReadUInt16(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "h":
                        case "rh":
                            res = new RulesActionReadInt16(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "d":
                        case "rd":
                            res = new RulesActionReadUInt32(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "ms":
                        case "rms":
                            res = new RulesActionReadUInt32Ms(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "i":
                        case "ri":
                            res = new RulesActionReadInt32(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "f":
                        case "rf":
                            res = new RulesActionReadSingle(this, parentAction, actionNode, step);
                            break;
                        case "double":
                        case "rdouble":
                            res = new RulesActionReadDouble(this, parentAction, actionNode, step);
                            break;
                        case "half":
                            res = new RulesActionReadHalf(this, parentAction, actionNode, step);
                            break;
                        case "q":
                        case "rq":
                            res = new RulesActionReadUInt64(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "int64": // not used
                            res = new RulesActionReadInt64(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "a":
                            res = new RulesActionReadArray(this, parentAction, actionNode, step);
                            break;
                        case "ts":
                        case "rts":
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
                        case "bc":
                            res = new RulesActionReadUInt24(this, parentAction, actionNode, step, isReversed);
                            break;
                        case "bcx":
                        case "bcy":
                        case "bcz":
                            res = new RulesActionRead3ByteFloat(this, parentAction, actionNode, step, isReversed);
                            break;
                        default:
                            // Blank Command
                            res = new RulesAction(this, parentAction, actionNode, step, isReversed);
                            break;
                    }
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
                    res = new RulesActionArithmeticOperation(this, parentAction, actionNode, step, "val", "=", "", "dst");
                    break;
                case "loop":
                    res = new RulesActionLoop(this, parentAction, actionNode, step);
                    break;
                case "break":
                    res = new RulesActionBreak(this, parentAction, actionNode, step);
                    break;
                case "continue":
                    res = new RulesActionContinue(this, parentAction, actionNode, step);
                    break;
                case "lookup":
                    res = new RulesActionSaveLookup(this, parentAction, actionNode, step, "source", "val", "altlookup", "save");
                    break;
                case "template":
                    res = new RulesActionTemplate(this, parentAction, actionNode, step, "name");
                    break;
                case "echo":
                    res = new RulesActionEcho(this, parentAction, actionNode, step, "arg");
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
                    res = new RulesAction(this, parentAction, actionNode, step, false);
                    break;
            }
            return res;
        }

        public void Build()
        {
            Actions.Clear();
            for (int i = 0; i < _rootNode.ChildNodes.Count; i++)
            {
                var actionNode = _rootNode.ChildNodes.Item(i);
                var attribs = XmlHelper.ReadNodeAttributes(actionNode);
                var newAction = BuildRuleAction(null, actionNode, attribs, i);
                if (newAction != null)
                    Actions.Add(newAction);
            }
        }

        public virtual void RunRule(PacketParser packetParser, ref ushort dataFieldIndex)
        {
            // Settings some defaults
            SetLocalVar("p_size", packetParser.PD.PacketDataSize.ToString());
            SetLocalVar("p_type", packetParser.PD.PacketId.ToString());
            SetLocalVar("p_level", packetParser.PD.OriginalPacketLevel.ToString());
            SetLocalVar("pSize", packetParser.PD.PacketDataSize.ToString());
            SetLocalVar("pType", packetParser.PD.PacketId.ToString());
            SetLocalVar("pLevel", packetParser.PD.OriginalPacketLevel.ToString());

            // Run all actions
            foreach (var action in Actions)
            {
                try
                {
                    action.RunAction(packetParser, ref dataFieldIndex);
                }
                catch (Exception x)
                {
                    packetParser.AddParseLineToView(0xFF, "A" + action?.GetActionStepName(), Color.Red, "Error at \"" + action?.Node.Name + "\"", "Exception: " + x.Message + " => " + action?.Node.OuterXml, this.Name);
                    break;
                }
            }
        }

        public void SetLocalVar(string name, string newValue)
        {
            if (LocalVars.TryGetValue(name, out _))
                LocalVars.Remove(name);
            LocalVars.Add(name, newValue);
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                Debug.WriteLine("SetLocalVar({0},{1})", name, newValue);
            }
        }

        public string GetLocalVar(string name)
        {
            if (LocalVars.TryGetValue(name, out var res))
            {
                return res;
            }
            else
            {
                SetLocalVar(name, string.Empty);
                return string.Empty;
            }
        }
    }

}