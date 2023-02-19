using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using VieweD.Helpers;
using VieweD.Helpers.System;

namespace VieweD.Engine.Common
{
    public enum LoopActionResult
    {
        Normal = 0,
        Continue = 1,
        Break = -1,
    }

    public class RulesAction
    {
        protected LoopActionResult InternalLoopActionResult;

        public XmlNode Node { get; set; }

        public PacketRule ParentRule { get; set; }

        public RulesAction ParentAction { get; set; }

        public int ActionStep { get; set; }

        public Dictionary<string, string> Attributes;
        public bool IsReversed { get; protected set; }

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

        public string DepthString {  
            get 
            {
                var res = string.Empty;

                for (var i = 1; i < Depth; i++)
                    res += EngineBase.DepthSpacerVertical; // "⁞";

                if (Depth > 0)
                {
                    if (ParentAction is RulesActionLoop rulesActionLoop)
                    {
                        if ((ActionStep == 0) && (rulesActionLoop.ChildActions.Count <= 1))
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalSingle; // "── ";
                        }
                        else
                        if (ActionStep == 0)
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalTop; // "┌─ ";
                        }
                        else
                        if (rulesActionLoop.ChildActions.Count - 1 == ActionStep)
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalBottom; // "└─ ";
                        }
                        else
                        {
                            res += EngineBase.DepthSpacerHorizontalMiddle; // "├─ ";
                        }
                    }
                    else
                    if (ParentAction is RulesActionCompareOperation rulesActionCompareOperation)
                    {
                        if ((ActionStep == 0) && (rulesActionCompareOperation.ChildActions.Count <= 1))
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalSingle; // "── ";
                        }
                        else
                        if (ActionStep == 0)
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalTop; // "┌─ ";
                        }
                        else
                        if (rulesActionCompareOperation.ChildActions.Count - 1 == ActionStep)
                        {
                            // This is the last step in the loop
                            res += EngineBase.DepthSpacerHorizontalBottom; // "└─ ";
                        }
                        else
                        {
                            res += EngineBase.DepthSpacerHorizontalMiddle; // "├─ ";
                        }
                    }
                    else
                    {
                        res += EngineBase.DepthSpacerHorizontalMiddle; // "┝━ ";
                    }
                }
                return res;
            }
        }

        public RulesAction(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed)
        {
            Init(parent, parentAction, thisNode, thisStep, reversed);
        }

        private void Init(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed)
        {
            ParentRule = parent;
            ParentAction = parentAction;
            Node = thisNode;
            ActionStep = thisStep;
            IsReversed = reversed;
            Attributes = XmlHelper.ReadNodeAttributes(Node);
            Depth = parentAction != null ? parentAction.Depth + 1 : 0 ;
        }

        public virtual void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var eText = Node.Name;
            foreach (var a in Attributes)
                eText += " (" + a.Key + "=" + a.Value + ")";
            pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+"Action Error", "Unsupported: " + eText);
            fieldIndex++;
        }

        public string GetActionStepName()
        {
            var res = ActionStep.ToString();
            if (ParentAction != null)
                res = ParentAction.GetActionStepName() + "-" + res;
            return res;
        }

        public string GetLookup(ulong id,int piscIndex = 0)
        {
            var lookupVal = string.Empty;
            var lookupAttribute = "lookup";

            if (piscIndex > 0)
                lookupAttribute += piscIndex.ToString();

            var lookupList = XmlHelper.GetAttributeString(Attributes, lookupAttribute);
            if (lookupList != string.Empty)
                lookupVal = ParentRule.Parent.Parent.ParentTab.Engine.DataLookups.NLU(lookupList).GetValue(id) + " <= ";

            return lookupVal;
        }
    }

    public class RulesActionReadByte : RulesAction
    {
        public RulesActionReadByte(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 1, ref fieldIndex);
            var d = pp.PD.GetByteAtPos(pos);
            var dataString = d.ToString() + " - 0x" + d.ToString("X2") + " - " + pp.ByteToBits(d) + " - '" + (char)d + "'";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", d);
            pp.MarkParsed(pos, 1, fieldIndex);
        }
    }

    public class RulesActionReadUInt16 : RulesAction
    {
        public RulesActionReadUInt16(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 2, ref fieldIndex);
            var d = pp.PD.GetUInt16AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToUInt16(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X4") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", d);
            pp.MarkParsed(pos, 2, fieldIndex);
        }
    }

    public class RulesActionReadInt16 : RulesAction
    {
        public RulesActionReadInt16(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            // 
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 2, ref fieldIndex);
            var d = pp.PD.GetInt16AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToInt16(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X4") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", unchecked((ushort)d));
            pp.MarkParsed(pos, 2, fieldIndex);
        }
    }

    public class RulesActionReadUInt32 : RulesAction
    {
        public RulesActionReadUInt32(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref fieldIndex);
            var d = pp.PD.GetUInt32AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToUInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X8") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "",d);
            pp.MarkParsed(pos, 4, fieldIndex);
        }
    }

    public class RulesActionReadInt32 : RulesAction
    {
        public RulesActionReadInt32(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref fieldIndex);
            var d = pp.PD.GetInt32AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X8") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", unchecked((uint)d));
            pp.MarkParsed(pos, 4, fieldIndex);
        }
    }

    public class RulesActionReadSingle : RulesAction
    {
        public RulesActionReadSingle(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref fieldIndex);
            var d = pp.PD.GetFloatAtPos(pos);
            var dataString = d.ToString(CultureInfo.InvariantCulture);
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            ParentRule.SetLocalVar(varName, dataString);
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + varName, dataString, "");
            pp.MarkParsed(pos, 4, fieldIndex);
        }
    }

    public class RulesActionReadDouble : RulesAction
    {
        public RulesActionReadDouble(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref fieldIndex);
            var d = pp.PD.GetDoubleAtPos(pos);
            var dataString = d.ToString(CultureInfo.InvariantCulture);
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            ParentRule.SetLocalVar(varName, dataString);
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + varName, dataString, "");
            pp.MarkParsed(pos, 8, fieldIndex);
        }
    }

    public class RulesActionReadHalf : RulesAction
    {
        public RulesActionReadHalf(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref fieldIndex);
            var rd = pp.PD.GetUInt16AtPos(pos);
            //if (IsReversed)
            //    rd = BitConverter.ToUInt16(BitConverter.GetBytes(rd).Reverse().ToArray(), 0);
            var d = Half.ToHalf(rd);
            var dataString = d.ToString(CultureInfo.InvariantCulture);
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            ParentRule.SetLocalVar(varName, dataString);
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, dataString, "");
            pp.MarkParsed(pos, 4, fieldIndex);
        }
    }

    public class RulesActionReadUInt64 : RulesAction
    {
        public RulesActionReadUInt64(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref fieldIndex);
            var d = pp.PD.GetUInt64AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToUInt64(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X16") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", d);
            pp.MarkParsed(pos, 8, fieldIndex);
        }
    }

    public class RulesActionReadInt64 : RulesAction
    {
        public RulesActionReadInt64(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref fieldIndex);
            var d = pp.PD.GetInt64AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToInt64(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d + " (0x" + d.ToString("X16") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", unchecked((ulong)d));
            pp.MarkParsed(pos, 8, fieldIndex);
        }
    }

    public class RulesActionReadUInt24 : RulesAction
    {
        public RulesActionReadUInt24(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pos, 3, ref fieldIndex);
            var rd = pp.PD.GetDataBytesAtPos(pos, 3).ToList();
            if (IsReversed)
                rd.Reverse();
            rd.Add(0);
            var d = BitConverter.ToUInt32(rd.ToArray(), 0);
            var dataString = d + " (0x" + d.ToString("X6") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, lookupVal + dataString, "", d);
            pp.MarkParsed(pos, 3, fieldIndex);
        }
    }

    public class RulesActionReadArray : RulesAction
    {
        public RulesActionReadArray(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            var size = 0;
            var sizeAttribute = XmlHelper.GetAttributeString(Attributes, "arg");

            if (sizeAttribute.StartsWith("#"))
            {
                if (NumberHelper.TryFieldParse(ParentRule.GetLocalVar(sizeAttribute.TrimStart('#')), out int sizeVal))
                {
                    size = sizeVal;
                }
            }
            else
            if (NumberHelper.TryFieldParse(sizeAttribute ,out int sizeVal))
            {
                size = sizeVal;
            }

            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            if (size <= 0)
            {
                pp.AddParseLineToView(fieldIndex, "A" + GetActionStepName(), Color.Red, DepthString+varName, "NullArray");
                ParentRule.SetLocalVar(varName, string.Empty);
                return;
            }
            pp.AddDataFieldEx(pp.PD.Cursor, size, ref fieldIndex);
            var d = pp.PD.GetDataAtPos(pos,size);
            ParentRule.SetLocalVar(varName, d);
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+XmlHelper.GetAttributeString(Attributes, "name"), d, "");
            pp.MarkParsed(pos, size, fieldIndex);
        }
    }

    public class RulesActionReadUnixTimeStamp : RulesAction
    {
        private static DateTime _unixTime = DateTimeOffset.FromUnixTimeSeconds(0).DateTime;

        public RulesActionReadUnixTimeStamp(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref fieldIndex);
            var rd = pp.PD.GetUInt64AtPos(pos);
            if (IsReversed)
                rd = BitConverter.ToUInt64(BitConverter.GetBytes(rd).Reverse().ToArray(), 0);
            string dataString;
            try 
            {
                var d = _unixTime.AddSeconds(rd);
                dataString = d + " (0x" + rd.ToString("X16") + ")";
            }
            catch
            {
                dataString = "Invalid (0x" + rd.ToString("X16") + ")";
            }
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            ParentRule.SetLocalVar(varName, rd.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+varName, dataString, "", rd);
            pp.MarkParsed(pos, 8, fieldIndex);
        }
    }

    public class RulesActionReadString : RulesAction
    {
        private readonly Encoding _enc;
        private readonly bool _includesSize;

        public RulesActionReadString(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, Encoding useEncoding,bool includesSize = false) : base(parent, parentAction, thisNode, thisStep, false)
        {
            _enc = useEncoding;
            _includesSize = includesSize;
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            var size = 0;
            var sizeFieldSize = 0;

            if (_includesSize)
            {
                size = pp.PD.GetUInt16AtPos(pp.PD.Cursor);
                sizeFieldSize = 2;
            }
            else
            {
                var sizeAttribute = XmlHelper.GetAttributeString(Attributes, "arg");
                if (sizeAttribute.StartsWith("#"))
                {
                    if (NumberHelper.TryFieldParse(ParentRule.GetLocalVar(sizeAttribute.TrimStart('#')), out int sizeVal))
                    {
                        size = sizeVal;
                    }
                }
                else
                if (NumberHelper.TryFieldParse(sizeAttribute, out int sizeVal))
                {
                    size = sizeVal;
                }
            }

            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            pp.AddDataFieldEx(pos, size + sizeFieldSize, ref fieldIndex);
            var d = pp.PD.GetDataBytesAtPos(pos + sizeFieldSize, size);
            string stringVal;
            var hexVal = string.Empty;
            try
            {
                if (size > 0)
                {
                    stringVal = _enc.GetString(d);
                    if (Properties.Settings.Default.ShowStringHexData)
                    {
                        foreach (char c in d)
                        {
                            if (hexVal != string.Empty)
                                hexVal += " ";
                            hexVal += ((byte)c).ToString("X2");
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
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString+XmlHelper.GetAttributeString(Attributes, "name"), stringVal + hexVal, stringVal);
            pp.MarkParsed(pos, size + sizeFieldSize, fieldIndex);
        }
    }

    public class RulesActionCompareOperation : RulesAction
    {
        public List<RulesAction> ChildActions;
        private readonly string _arg1Name;
        private readonly string _arg2Name;
        private readonly string _operatorName;

        public RulesActionCompareOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep,string arg1Name, string operatorName, string arg2Name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            ChildActions = new List<RulesAction>();
            this._arg1Name = arg1Name;
            _operatorName = operatorName;
            this._arg2Name = arg2Name;

            ChildActions.Clear();
            for (var i = 0; i < thisNode.ChildNodes.Count; i++)
            {
                var actionNode = thisNode.ChildNodes.Item(i);
                var attributes = XmlHelper.ReadNodeAttributes(actionNode);
                var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            InternalLoopActionResult = LoopActionResult.Normal;
            long val1 = 0;
            var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);
            if (val1Attribute == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg1Name + " can not be empty", "");
                return;
            }
            if (val1Attribute.StartsWith("#"))
            {
                var arg1Name = ParentRule.GetLocalVar(val1Attribute.TrimStart('#'));
                if (NumberHelper.TryFieldParse(arg1Name, out long aVal))
                {
                    val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+_arg1Name+": " + val1Attribute, "");
                }
            }
            else
            if (NumberHelper.TryFieldParse(val1Attribute, out long aVal))
            {
                val1 = aVal;
            }

            long val2 = 0;
            var val2Attribute = "";
            if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
            {
                val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);
                if (val2Attribute == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg2Name + " can not be empty", "");
                    return;
                }
                if (val2Attribute.StartsWith("#"))
                {
                    var arg2Name = ParentRule.GetLocalVar(val2Attribute.TrimStart('#'));
                    if (NumberHelper.TryFieldParse(arg2Name, out long aVal))
                    {
                        val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+arg2Name+": " + val2Attribute, "");
                    }
                }
                else
                if (NumberHelper.TryFieldParse(val2Attribute, out long aVal))
                {
                    val2 = aVal;
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
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attributes, "name"), "Invalid @@operator: " + _operatorName, "");
                    break;
            }

            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), Node.Name, "(" + val1.ToString() + " " + _operatorName + " " + val2.ToString() + ") => " + res.ToString(), "");
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {res}");
            }

            if (!res) 
                return;

            // Do child actions
            foreach(var child in ChildActions)
            {
                if (LoopActionResult != LoopActionResult.Normal)
                    break;

                try
                {
                    child.RunAction(pp, ref fieldIndex);
                }
                catch (Exception ex)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Error at \"" + child?.Node.Name + "\"", "Exception: " + ex.Message + " => "+child?.Node.OuterXml, this.Node.Name);
                    break;
                }

                // Check for End of Packet
                if (pp.PD.Cursor <= pp.PD.RawBytes.Count) 
                    continue;

                LoopActionResult = LoopActionResult.Break;
                pp.AddParseLineToView(0xFFFF, "A" + child.GetActionStepName(), Color.Red, "Reached past end of Packet Data", Node.Name);

                if (Properties.Settings.Default.ShowDebugInfo)
                    Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - EOP");

                break;
            }
        }
    }

    public class RulesActionArithmeticOperation : RulesAction
    {
        private readonly string _arg1Name;
        private readonly string _arg2Name;
        private readonly string _operatorName;
        private readonly string _destName;

        public RulesActionArithmeticOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string arg1Name, string operatorName, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            _arg1Name = arg1Name;
            _operatorName = operatorName;
            _arg2Name = arg2Name;
            _destName = destName;
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            long val1 = 0;
            var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);

            if (val1Attribute == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg1Name + " can not be empty", "");
                return;
            }

            if (val1Attribute.StartsWith("#"))
            {
                var arg1Name = ParentRule.GetLocalVar(val1Attribute.TrimStart('#'));
                if (NumberHelper.TryFieldParse(arg1Name, out long aVal))
                {
                    val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+_arg1Name+": " + val1Attribute, "");
                }
            }
            else
            if (NumberHelper.TryFieldParse(val1Attribute, out long aVal))
            {
                val1 = aVal;
            }

            long val2 = 0;
            var val2Attribute = "<none>";
            if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
            {
                val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);
                if (val2Attribute == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg2Name + " can not be empty", "");
                    return;
                }

                if (val2Attribute.StartsWith("#"))
                {
                    var arg2Name = ParentRule.GetLocalVar(val2Attribute.TrimStart('#'));
                    if (NumberHelper.TryFieldParse(arg2Name, out long aVal))
                    {
                        val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+_arg2Name+": " + val2Attribute, "");
                    }
                }
                else
                if (NumberHelper.TryFieldParse(val2Attribute, out long aVal))
                {
                    val2 = aVal;
                }
            }
            var destinationAttribute = XmlHelper.GetAttributeString(Attributes, _destName);

            long res ;
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
                    res = (val1 / val2);
                    break;
                case "%":
                    res = (val1 % val2);
                    break;
                case "&":
                    res = (val1 & val2);
                    break;
                case "|":
                    res = (val1 | val2);
                    break;
                case "<<":
                    res = ((int)val1 << (int)val2);
                    break;
                case ">>":
                    res = ((int)val1 >> (int)val2);
                    break;
                case "=":
                    res = (val1);
                    break;
                default:
                    res = 0 ;
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attributes, "name"), "Invalid @@operator: " + _operatorName, "");
                    break;
            }

            ParentRule.SetLocalVar(destinationAttribute, res.ToString(CultureInfo.InvariantCulture));
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), Node.Name, "(" + val1.ToString() + " " + _operatorName + " " + val2.ToString() + ") => " + res.ToString() + " => " + destinationAttribute, "");
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {destinationAttribute}({res})");
            }
        }
    }

    public class RulesActionDoubleArithmeticOperation : RulesAction
    {
        private readonly string _arg1Name;
        private readonly string _arg2Name;
        private readonly string _operatorName;
        private readonly string _destName;

        public RulesActionDoubleArithmeticOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string arg1Name, string operatorName, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            _arg1Name = arg1Name;
            _operatorName = operatorName;
            _arg2Name = arg2Name;
            _destName = destName;
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            double val1 = 0;
            var val1Attribute = XmlHelper.GetAttributeString(Attributes, _arg1Name);
            if (val1Attribute == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg1Name + " can not be empty", "");
                return;
            }
            if (val1Attribute.StartsWith("#"))
            {
                var arg1StringVal = ParentRule.GetLocalVar(val1Attribute.TrimStart('#'));
                if (double.TryParse(arg1StringVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                {
                    val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+_arg1Name+": " + val1Attribute, "");
                }
            }
            else
            if (double.TryParse(val1Attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
            {
                val1 = aVal;
            }

            double val2 = 0;
            var val2Attribute = "<none>";
            if ((_arg2Name != string.Empty) && (_arg2Name != "0"))
            {
                val2Attribute = XmlHelper.GetAttributeString(Attributes, _arg2Name);
                if (val2Attribute == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _arg2Name + " can not be empty", "");
                    return;
                }
                if (val2Attribute.StartsWith("#"))
                {
                    var arg2StringVal = ParentRule.GetLocalVar(val2Attribute.TrimStart('#'));
                    if (double.TryParse(arg2StringVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                    {
                        val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+_arg2Name+": " + val2Attribute, "");
                    }
                }
                else
                if (double.TryParse(val2Attribute, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                {
                    val2 = aVal;
                }
            }
            var destinationAttribute = XmlHelper.GetAttributeString(Attributes, _destName);

            double res ;
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
                    res = (val1 / val2);
                    break;
                case "=":
                    res = (val1);
                    break;
                default:
                    res = 0 ;
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attributes, "name"), "Invalid @@operator: " + _operatorName, "");
                    break;
            }

            ParentRule.SetLocalVar(destinationAttribute, res.ToString(CultureInfo.InvariantCulture));
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), Node.Name, "(" + val1.ToString(CultureInfo.InvariantCulture) + " " + _operatorName + " " + val2.ToString(CultureInfo.InvariantCulture) + ") => " + res.ToString(CultureInfo.InvariantCulture) + " => " + destinationAttribute, "");
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name}: {val1Attribute}({val1}) {_operatorName} {val2Attribute}({val2}) => {destinationAttribute}({res})");
            }
        }
    }
    
    public class RulesActionLoop : RulesAction
    {
        public List<RulesAction> ChildActions;

        public RulesActionLoop(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            ChildActions = new List<RulesAction>();
            for (var i = 0; i < thisNode.ChildNodes.Count; i++)
            {
                var actionNode = thisNode.ChildNodes.Item(i);
                var attributes = XmlHelper.ReadNodeAttributes(actionNode);

                var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), Node.Name, "", "");
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Begin");
            }

            // Do child actions
            LoopActionResult = LoopActionResult.Normal;
            var safetyCounter = 0;
            var maxSafetyCount = 255;
            while (LoopActionResult == LoopActionResult.Normal)
            {
                for (var i = 0; i < ChildActions.Count; i++)
                {
                    var child = ChildActions[i];
                    if (Properties.Settings.Default.ShowDebugInfo)
                    {
                        Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Step {i} ({child.Node.Name})");
                    }
                    try
                    {
                        child.RunAction(pp, ref fieldIndex);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Step {i} ({child.Node.Name}) Exception: {ex.Message}");
                        pp.AddParseLineToView(0xFFFF, "A" + child.GetActionStepName(), Color.Red, "Error at \"" + child.Node.Name + "\"", "Exception: " + ex.Message + " => " + child.Node.OuterXml, Node.Name);
                        LoopActionResult = LoopActionResult.Break ;
                    }
                    
                    if (LoopActionResult == LoopActionResult.Continue)
                    {
                        if (Properties.Settings.Default.ShowDebugInfo)
                            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Continue");

                        continue;
                    }
                    if (LoopActionResult == LoopActionResult.Break)
                    {
                        if (Properties.Settings.Default.ShowDebugInfo)
                            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - Break");

                        break;
                    }
                    // Check for End of Packet
                    if (pp.PD.Cursor > pp.PD.RawBytes.Count)
                    {
                        LoopActionResult = LoopActionResult.Break;
                        pp.AddParseLineToView(0xFFFF, "A" + child.GetActionStepName(), Color.Red, "Reached past end of Packet Data", Node.Name);
                        if (Properties.Settings.Default.ShowDebugInfo)
                            Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - EOP");

                        break;
                    }
                }
                safetyCounter++;
                
                if (safetyCounter > maxSafetyCount)
                {
                    Debug.WriteLine($"{this.ParentRule.Name} {GetActionStepName()} {this.GetType().Name} - SafetyCheck, took over {maxSafetyCount} loops");
                    break;
                }

                if (LoopActionResult == LoopActionResult.Break)
                    break;
            }

            LoopActionResult = LoopActionResult.Normal;
            if (Properties.Settings.Default.ShowDebugInfo)
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - End");
        }
    }

    public class RulesActionBreak : RulesAction
    {
        public RulesActionBreak(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            if (ParentAction != null)
            {
                if (Properties.Settings.Default.ShowDebugInfo)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), DepthString+"Break", "");
                }
                LoopActionResult = LoopActionResult.Break;
            }
            else
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), "Break Error", "Break has no parent");
            }
        }
    }

    public class RulesActionContinue : RulesAction
    {
        public RulesActionContinue(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            if (ParentAction != null)
            {
                if (Properties.Settings.Default.ShowDebugInfo)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), DepthString+"Continue", "");
                }
                LoopActionResult = LoopActionResult.Continue;
            }
            else
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), "Continue Error", "Continue has no parent");
            }
        }
    }

    public class RulesActionSaveLookup : RulesAction
    {
        private readonly string _sourceValueName;
        private readonly string _sourceIdName;
        private readonly string _destName;
        private readonly string _altLookupTableName;
        private readonly string _saveLookupName;

        public RulesActionSaveLookup(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string sourceIdField, string sourceValueField, string altLookupTable, string destName, string saveLookupName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            _sourceIdName = sourceIdField;
            _sourceValueName = sourceValueField;
            _destName = destName;
            _altLookupTableName = altLookupTable;
            _saveLookupName = saveLookupName;
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var idAttribute = XmlHelper.GetAttributeString(Attributes, _sourceIdName);
            if (idAttribute == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _sourceIdName + " can not be empty", "");
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
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _sourceIdName + " must be a int value field", "");
            }

            var valAttribute = XmlHelper.GetAttributeString(Attributes, _sourceValueName);
            if (valAttribute == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, _sourceValueName + " can not be empty", "");
                return;
            }

            var sourceValue = ParentRule.GetLocalVar(valAttribute);
            var saveLookup = XmlHelper.GetAttributeString(Attributes, _saveLookupName);
            
            var altLookupAttribute = XmlHelper.GetAttributeString(Attributes, _altLookupTableName);
            if (altLookupAttribute != string.Empty)
            {
                if (NumberHelper.TryFieldParse(sourceValue, out long sourceValueParsed))
                {
                    var lookedUpValue = pp.PD.Parent.ParentTab.Engine.DataLookups.NLU(altLookupAttribute).GetValue((ulong)sourceValueParsed);
                    sourceValue = "(" + altLookupAttribute + ":" + sourceValueParsed + ") " + lookedUpValue;
                    if (saveLookup != string.Empty)
                    {
                        // save result as var
                        ParentRule.SetLocalVar(saveLookup, lookedUpValue);
                    }
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+Node.Name, _sourceValueName + " must be a int value field to use the alt lookups function", "");
                }
            }
            else if (saveLookup != string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+Node.Name, "Save lookup to local var requires to also have alt lookup set", "");
            }

            var destListName = XmlHelper.GetAttributeString(Attributes, _destName);

            pp.PD.Parent.ParentTab.Engine.DataLookups.RegisterCustomLookup(destListName, (ulong)sourceId, sourceValue);
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), DepthString+Node.Name, "Save (" + sourceId + " => " + sourceValue + ") into " + destListName);
                Console.WriteLine($"{GetActionStepName()} {GetType().Name}: {sourceId}({idAttribute}) => {sourceValue}({valAttribute}) into {destListName}");
            }
        }
    }

    public class RulesActionComment : RulesAction
    {
        public string Comment;

        public RulesActionComment(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            Comment = thisNode.InnerText.Trim();
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            const string varName = "#comment";
            pp.AddParseLineToView(0xFFFF, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + varName, Comment, "");
        }
    }

    public class RulesActionReadUInt32Ms : RulesAction
    {
        public RulesActionReadUInt32Ms(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
            //
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref fieldIndex);
            var d = pp.PD.GetUInt32AtPos(pos);
            if (IsReversed)
                d = BitConverter.ToUInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString =  PacketParser.MSToString(d) + " (0x" + d.ToString("X8") + ")";
            var varName = XmlHelper.GetAttributeString(Attributes, "name");
            var lookupVal = GetLookup(d);
            ParentRule.SetLocalVar(varName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + varName, lookupVal + dataString, "", d);
            pp.MarkParsed(pos, 4, fieldIndex);
        }
    }

    public class RulesActionTemplate : RulesAction
    {
        public List<RulesAction> ChildActions;
        public string TemplateName;

        public RulesActionTemplate(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            TemplateName = XmlHelper.GetAttributeString(Attributes, name.ToLower());
            var templates = parent.Parent.Parent.Templates;
            if (!templates.TryGetValue(TemplateName, out var thisTemplate))
            {
                Debug.WriteLine("Template not found: " + TemplateName);
                return;
            }

            ChildActions = new List<RulesAction>();

            for (var i = 0; i < thisTemplate.ChildNodes.Count; i++)
            {
                var actionNode = thisTemplate.ChildNodes.Item(i);
                var attributes = XmlHelper.ReadNodeAttributes(actionNode);

                var newAction = parent.BuildRuleAction(this, actionNode, attributes, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(fieldIndex), Node.Name, "Template", "");
                Debug.WriteLine("{1} {0} - Begin Template", this.GetType().Name, GetActionStepName());
            }

            // Do child actions
            for (var i = 0; i < ChildActions.Count; i++)
            {
                var child = ChildActions[i];
                if (Properties.Settings.Default.ShowDebugInfo)
                {
                    Debug.WriteLine("{1} {0} - Template Step {2} ({3})", this.GetType().Name, GetActionStepName(), i, child.Node.Name);
                }
                try
                {
                    child.RunAction(pp, ref fieldIndex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("{1} {0} - Template Step {2} ({3}) Exception: {4}", this.GetType().Name, GetActionStepName(), i, child.Node.Name, ex.Message);
                    pp.AddParseLineToView(0xFFFF, "A" + child.GetActionStepName(), Color.Red, "Error at \"" + child.Node.Name + "\"", "Exception: " + ex.Message + " => " + child.Node.OuterXml, this.Node.Name);
                    LoopActionResult = LoopActionResult.Break;
                }

                // Check for End of Packet
                if (pp.PD.Cursor > pp.PD.RawBytes.Count)
                {
                    LoopActionResult = LoopActionResult.Break;
                    pp.AddParseLineToView(0xFFFF, "A" + child.GetActionStepName(), Color.Red, "Reached past end of Packet Data", this.Node.Name);
                    if (Properties.Settings.Default.ShowDebugInfo)
                        Debug.WriteLine("{1} {0} - EOP", this.GetType().Name, GetActionStepName());

                    break;
                }
            }

            if (Properties.Settings.Default.ShowDebugInfo)
                Debug.WriteLine($"{GetActionStepName()} {this.GetType().Name} - End Template");
        }
    }

    public class RulesActionEcho : RulesAction
    {
        public string FieldName;

        public RulesActionEcho(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            FieldName = XmlHelper.GetAttributeString(Attributes, name.ToLower());
        }

        public override void RunAction(PacketParser pp, ref ushort fieldIndex)
        {
            var pos = pp.PD.Cursor;
            var dataString = ParentRule.GetLocalVar(FieldName);
            var hexString = string.Empty;

            // Handle output differently if float val
            if (double.TryParse(dataString, out var valDouble) && (Math.Abs(Math.Floor(valDouble) - valDouble) > 0.00001))
            {
                pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + FieldName, valDouble.ToString(CultureInfo.InvariantCulture), "");
            }
            else
            if (NumberHelper.TryFieldParse(dataString, out ulong valNumber))
            {
                hexString = " (0x" + valNumber.ToString("X")+")";
                var lookupVal = GetLookup(valNumber);
                pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + FieldName, lookupVal + dataString + hexString, "");
            }
            else
            {
                pp.AddParseLineToView(fieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(fieldIndex), DepthString + FieldName, dataString + hexString, "");
            }
        }
    }
}
