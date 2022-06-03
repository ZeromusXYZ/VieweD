using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
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
        private XmlNode _node;
        private PacketRule parentRule;
        private int _actionStep;
        private RulesAction _parentAction;
        private bool _isReversed;
        protected LoopActionResult _loopActionResult;

        public XmlNode Node { get => _node; set => _node = value; }
        public PacketRule ParentRule { get => parentRule; set => parentRule = value; }
        public RulesAction ParentAction { get => _parentAction; set => _parentAction = value; }
        public int ActionStep { get => _actionStep; set => _actionStep = value; }
        public Dictionary<string, string> Attribs;
        public bool isReversed { get => _isReversed; protected set => _isReversed = value; }
        public LoopActionResult LoopActionResult
        {
            get => _loopActionResult;
            set
            {
                _loopActionResult = value;
                // Aggregate to parents until we hit a loop node
                if ((Node.Name != "loop") && (_parentAction != null))
                    _parentAction.LoopActionResult = value;
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
                    if (ParentAction is RulesActionLoop ral)
                    {
                        if ((ActionStep == 0) && (ral.ChildActions.Count <= 1))
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
                        if (ral.ChildActions.Count - 1 == ActionStep)
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
                    if (ParentAction is RulesActionCompareOperation raco)
                    {
                        if ((ActionStep == 0) && (raco.ChildActions.Count <= 1))
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
                        if (raco.ChildActions.Count - 1 == ActionStep)
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
            isReversed = reversed;
            Attribs = XmlHelper.ReadNodeAttributes(Node);
            Depth = parentAction != null ? parentAction.Depth + 1 : 0 ;
        }

        public virtual void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var eText = Node.Name;
            foreach (var a in Attribs)
                eText += " (" + a.Key + "=" + a.Value + ")";
            pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+"Action Error", "Unsupported: " + eText);
            FieldIndex++;
        }

        public string GetActionStepName()
        {
            var res = ActionStep.ToString();
            if (_parentAction != null)
                res = _parentAction.GetActionStepName() + "-" + res;
            return res;
        }

        public string GetLookup(ulong ID,int piscIndex = 0)
        {
            var LookupVal = string.Empty;
            var lookupAttrib = "lookup";
            if (piscIndex > 0)
                lookupAttrib += piscIndex.ToString();
            var LookupList = XmlHelper.GetAttributeString(Attribs, lookupAttrib);
            if (LookupList != string.Empty)
                LookupVal = ParentRule._parent.Parent.parentTab.Engine.DataLookups.NLU(LookupList).GetValue(ID) + " <= ";
            return LookupVal;
        }
    }

    public class RulesActionReadByte : RulesAction
    {
        public RulesActionReadByte(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 1, ref FieldIndex);
            var d = pp.PD.GetByteAtPos(pos);
            var dataString = d.ToString() + " - 0x" + d.ToString("X2") + " - " + pp.ByteToBits(d) + " - '" + (char)d + "'";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", d);
            pp.MarkParsed(pos, 1, FieldIndex);
        }

    }

    public class RulesActionReadUInt16 : RulesAction
    {
        public RulesActionReadUInt16(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 2, ref FieldIndex);
            var d = pp.PD.GetUInt16AtPos(pos);
            if (isReversed)
                d = BitConverter.ToUInt16(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X4") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", d);
            pp.MarkParsed(pos, 2, FieldIndex);
        }

    }

    public class RulesActionReadInt16 : RulesAction
    {
        public RulesActionReadInt16(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 2, ref FieldIndex);
            var d = pp.PD.GetInt16AtPos(pos);
            if (isReversed)
                d = BitConverter.ToInt16(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X4") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", unchecked((ushort)d));
            pp.MarkParsed(pos, 2, FieldIndex);
        }

    }

    public class RulesActionReadUInt32 : RulesAction
    {
        public RulesActionReadUInt32(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref FieldIndex);
            var d = pp.PD.GetUInt32AtPos(pos);
            if (isReversed)
                d = BitConverter.ToUInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X8") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "",d);
            pp.MarkParsed(pos, 4, FieldIndex);
        }

    }

    public class RulesActionReadInt32 : RulesAction
    {
        public RulesActionReadInt32(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref FieldIndex);
            var d = pp.PD.GetInt32AtPos(pos);
            if (isReversed)
                d = BitConverter.ToInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X8") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", unchecked((uint)d));
            pp.MarkParsed(pos, 4, FieldIndex);
        }

    }

    public class RulesActionReadSingle : RulesAction
    {
        public RulesActionReadSingle(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref FieldIndex);
            var d = pp.PD.GetFloatAtPos(pos);
            var dataString = d.ToString();
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + VARName, dataString, "");
            pp.MarkParsed(pos, 4, FieldIndex);
        }

    }

    public class RulesActionReadDouble : RulesAction
    {
        public RulesActionReadDouble(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref FieldIndex);
            var d = pp.PD.GetDoubleAtPos(pos);
            var dataString = d.ToString();
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + VARName, dataString, "");
            pp.MarkParsed(pos, 8, FieldIndex);
        }

    }

    public class RulesActionReadHalf : RulesAction
    {
        public RulesActionReadHalf(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref FieldIndex);
            var rd = pp.PD.GetUInt16AtPos(pos);
            //if (isReversed)
            //    rd = BitConverter.ToUInt16(BitConverter.GetBytes(rd).Reverse().ToArray(), 0);
            Half d = Half.ToHalf(rd);
            var dataString = d.ToString();
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, dataString, "");
            pp.MarkParsed(pos, 4, FieldIndex);
        }

    }

    public class RulesActionReadUInt64 : RulesAction
    {
        public RulesActionReadUInt64(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref FieldIndex);
            var d = pp.PD.GetUInt64AtPos(pos);
            if (isReversed)
                d = BitConverter.ToUInt64(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X16") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", d);
            pp.MarkParsed(pos, 8, FieldIndex);
        }

    }

    public class RulesActionReadInt64 : RulesAction
    {
        public RulesActionReadInt64(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref FieldIndex);
            var d = pp.PD.GetInt64AtPos(pos);
            if (isReversed)
                d = BitConverter.ToInt64(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X16") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup((ulong)d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", unchecked((ulong)d));
            pp.MarkParsed(pos, 8, FieldIndex);
        }

    }

    public class RulesActionReadUInt24 : RulesAction
    {
        public RulesActionReadUInt24(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pos, 3, ref FieldIndex);
            var rd = pp.PD.GetDataBytesAtPos(pos, 3).ToList();
            if (isReversed)
                rd.Reverse();
            rd.Add(0);
            UInt32 d = BitConverter.ToUInt32(rd.ToArray(), 0);
            var dataString = d.ToString() + " (0x" + d.ToString("X6") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, LookupVal + dataString, "", d);
            pp.MarkParsed(pos, 3, FieldIndex);
        }

    }

    public class RulesActionReadArray : RulesAction
    {
        public RulesActionReadArray(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            var size = 0;
            var sizeAttrib = XmlHelper.GetAttributeString(Attribs, "arg");
            if (sizeAttrib.StartsWith("#"))
            {
                if (XmlHelper.TryAttribParse(ParentRule.GetLocalVar(sizeAttrib.TrimStart('#')), out var sizeVal))
                {
                    size = (int)sizeVal;
                }
            }
            else
            if (XmlHelper.TryAttribParse(sizeAttrib ,out var sizeVal))
            {
                size = (int)sizeVal;
            }
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            if (size <= 0)
            {
                pp.AddParseLineToView(FieldIndex, "A" + GetActionStepName(), Color.Red, DepthString+VARName, "NullArray");
                ParentRule.SetLocalVar(VARName, string.Empty);
                return;
            }
            pp.AddDataFieldEx(pp.PD.Cursor, size, ref FieldIndex);
            var d = pp.PD.GetDataAtPos(pos,size);
            ParentRule.SetLocalVar(VARName, d);
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+XmlHelper.GetAttributeString(Attribs, "name"), d, "");
            pp.MarkParsed(pos, size, FieldIndex);
        }

    }

    public class RulesActionReadUnixTimeStamp : RulesAction
    {
        private static DateTime _unixTime = new DateTime(1970,1,1,0,0,0);

        public RulesActionReadUnixTimeStamp(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 8, ref FieldIndex);
            var rd = pp.PD.GetUInt64AtPos(pos);
            if (isReversed)
                rd = BitConverter.ToUInt64(BitConverter.GetBytes(rd).Reverse().ToArray(), 0);
            var dataString = string.Empty;
            try 
            {
                var d = _unixTime.AddSeconds(rd);
                dataString = d.ToString() + " (0x" + rd.ToString("X16") + ")";
            }
            catch
            {
                dataString = "Invalid (0x" + rd.ToString("X16") + ")";
            }
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            ParentRule.SetLocalVar(VARName, rd.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+VARName, dataString, "", rd);
            pp.MarkParsed(pos, 8, FieldIndex);
        }

    }

    public class RulesActionReadString : RulesAction
    {
        private Encoding enc;
        private bool IncludesSize;

        public RulesActionReadString(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, Encoding useEncoding,bool includesSize = false) : base(parent, parentAction, thisNode, thisStep, false)
        {
            enc = useEncoding;
            IncludesSize = includesSize;
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            var size = 0;
            var sizeFieldSize = 0;

            if (IncludesSize)
            {
                size = pp.PD.GetUInt16AtPos(pp.PD.Cursor);
                sizeFieldSize = 2;
            }
            else
            {
                var sizeAttrib = XmlHelper.GetAttributeString(Attribs, "arg");
                if (sizeAttrib.StartsWith("#"))
                {
                    if (XmlHelper.TryAttribParse(ParentRule.GetLocalVar(sizeAttrib.TrimStart('#')), out var sizeVal))
                    {
                        size = (int)sizeVal;
                    }
                }
                else
                if (XmlHelper.TryAttribParse(sizeAttrib, out var sizeVal))
                {
                    size = (int)sizeVal;
                }
            }

            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            pp.AddDataFieldEx(pos, size + sizeFieldSize, ref FieldIndex);
            var d = pp.PD.GetDataBytesAtPos(pos + sizeFieldSize, size);
            string StringVal = string.Empty ;
            string HexVal = string.Empty;
            try
            {
                if (size > 0)
                {
                    StringVal = enc.GetString(d);
                    if (Properties.Settings.Default.ShowStringHexData)
                    {
                        foreach (char c in d)
                        {
                            if (HexVal != string.Empty)
                                HexVal += " ";
                            HexVal += ((byte)c).ToString("X2");
                        }
                        HexVal = " (" + HexVal + ")";
                    }
                    else
                    {
                        HexVal = string.Empty;
                    }
                }
                else
                {
                    StringVal = "NULL";
                    HexVal = string.Empty;
                }
            }
            catch (Exception e)
            {
                StringVal = "Exception: " + e.Message;
                HexVal = string.Empty;
            }


            ParentRule.SetLocalVar(VARName, StringVal);
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString+XmlHelper.GetAttributeString(Attribs, "name"), StringVal + HexVal, StringVal);
            pp.MarkParsed(pos, size + sizeFieldSize, FieldIndex);
        }

    }

    public class RulesActionCompareOperation : RulesAction
    {
        public List<RulesAction> ChildActions;
        private string arg1Name;
        private string arg2Name;
        private string opperatorName;


        public RulesActionCompareOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep,string arg1Name, string opperator, string arg2Name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            ChildActions = new List<RulesAction>();
            this.arg1Name = arg1Name;
            opperatorName = opperator;
            this.arg2Name = arg2Name;

            ChildActions.Clear();
            for (int i = 0; i < thisNode.ChildNodes.Count; i++)
            {
                var actionNode = thisNode.ChildNodes.Item(i);
                var attribs = XmlHelper.ReadNodeAttributes(actionNode);
                var newAction = parent.BuildRuleAction(this, actionNode, attribs, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            _loopActionResult = LoopActionResult.Normal;
            long Val1 = 0;
            var Val1Attrib = XmlHelper.GetAttributeString(Attribs, arg1Name);
            if (Val1Attrib == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, arg1Name + " can not be empty", "");
                return;
            }
            if (Val1Attrib.StartsWith("#"))
            {
                var arg1name = ParentRule.GetLocalVar(Val1Attrib.TrimStart('#'));
                if (XmlHelper.TryAttribParse(arg1name, out var aVal))
                {
                    Val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+arg1Name+": " + Val1Attrib, "");
                }
            }
            else
            if (XmlHelper.TryAttribParse(Val1Attrib, out var aVal))
            {
                Val1 = aVal;
            }

            long Val2 = 0;
            var Val2Attrib = "";
            if ((arg2Name != string.Empty) && (arg2Name != "0"))
            {
                Val2Attrib = XmlHelper.GetAttributeString(Attribs, arg2Name);
                if (Val2Attrib == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, arg2Name + " can not be empty", "");
                    return;
                }
                if (Val2Attrib.StartsWith("#"))
                {
                    var arg2name = ParentRule.GetLocalVar(Val2Attrib.TrimStart('#'));
                    if (XmlHelper.TryAttribParse(arg2name, out var aVal))
                    {
                        Val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+arg2name+": " + Val2Attrib, "");
                    }
                }
                else
                if (XmlHelper.TryAttribParse(Val2Attrib, out var aVal))
                {
                    Val2 = aVal;
                }
            }
            else
            {
                Val2 = 0;
                // Arg2 = 0
            }

            bool res = false;
            switch (opperatorName)
            {
                case "==":
                    res = (Val1 == Val2);
                    break;
                case "<>":
                case "!=":
                    res = (Val1 != Val2);
                    break;
                case ">=":
                    res = (Val1 >= Val2);
                    break;
                case "<=":
                    res = (Val1 <= Val2);
                    break;
                case "<":
                    res = (Val1 < Val2);
                    break;
                case ">":
                    res = (Val1 > Val2);
                    break;
                default:
                    res = false;
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attribs, "name"), "Invalid opperator: " + opperatorName, "");
                    break;
            }
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), Node.Name, "(" + Val1.ToString() + " " + opperatorName + " " + Val2.ToString() + ") => " + res.ToString(), "");
                Debug.WriteLine("{7} {0}: {1}({2}) {3} {4}({5}) => {6}", this.GetType().Name, Val1Attrib, Val1, opperatorName, Val2Attrib, Val2, res, GetActionStepName());
            }
            if (res)
            {
                // Do child actions
                foreach(var child in ChildActions)
                {
                    if (LoopActionResult != LoopActionResult.Normal)
                        break;
                    try
                    {
                        child.RunAction(pp, ref FieldIndex);
                    }
                    catch (Exception x)
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Error at \"" + child?.Node.Name + "\"", "Exception: " + x.Message + " => "+child?.Node.OuterXml, this.Node.Name);
                        break;
                    }
                    // Check for End of Packet
                    if (pp.PD.Cursor > pp.PD.RawBytes.Count)
                    {
                        LoopActionResult = LoopActionResult.Break;
                        pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Reached past end of Packet Data", this.Node.Name);
                        if (Properties.Settings.Default.ShowDebugInfo)
                        {
                            Debug.WriteLine("{1} {0} - EOP", this.GetType().Name, GetActionStepName());
                        }
                        break;
                    }

                }
            }

        }

    }

    public class RulesActionArithmeticOperation : RulesAction
    {
        private string Arg1Name;
        private string Arg2Name;
        private string OpperatorName;
        private string DestName;

        public RulesActionArithmeticOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string arg1Name, string opperator, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            Arg1Name = arg1Name;
            OpperatorName = opperator;
            Arg2Name = arg2Name;
            DestName = destName;
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            long Val1 = 0;
            var Val1Attrib = XmlHelper.GetAttributeString(Attribs, Arg1Name);
            if (Val1Attrib == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, Arg1Name + " can not be empty", "");
                return;
            }
            if (Val1Attrib.StartsWith("#"))
            {
                var arg1name = ParentRule.GetLocalVar(Val1Attrib.TrimStart('#'));
                if (XmlHelper.TryAttribParse(arg1name, out var aVal))
                {
                    Val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+Arg1Name+": " + Val1Attrib, "");
                }
            }
            else
            if (XmlHelper.TryAttribParse(Val1Attrib, out var aVal))
            {
                Val1 = aVal;
            }

            long Val2 = 0;
            var Val2Attrib = "<none>";
            if ((Arg2Name != string.Empty) && (Arg2Name != "0"))
            {
                Val2Attrib = XmlHelper.GetAttributeString(Attribs, Arg2Name);
                if (Val2Attrib == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, Arg2Name + " can not be empty", "");
                    return;
                }
                if (Val2Attrib.StartsWith("#"))
                {
                    var arg2name = ParentRule.GetLocalVar(Val2Attrib.TrimStart('#'));
                    if (XmlHelper.TryAttribParse(arg2name, out var aVal))
                    {
                        Val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+Arg2Name+": " + Val2Attrib, "");
                    }
                }
                else
                if (XmlHelper.TryAttribParse(Val2Attrib, out var aVal))
                {
                    Val2 = aVal;
                }
            }
            var DestAttrib = XmlHelper.GetAttributeString(Attribs, DestName);

            long res ;
            switch (OpperatorName)
            {
                case "+":
                    res = (Val1 + Val2);
                    break;
                case "-":
                    res = (Val1 - Val2);
                    break;
                case "x":
                case "*":
                    res = (Val1 * Val2);
                    break;
                case "/":
                    res = (Val1 / Val2);
                    break;
                case "%":
                    res = (Val1 % Val2);
                    break;
                case "&":
                    res = (Val1 & Val2);
                    break;
                case "|":
                    res = (Val1 | Val2);
                    break;
                case "<<":
                    res = ((int)Val1 << (int)Val2);
                    break;
                case ">>":
                    res = ((int)Val1 >> (int)Val2);
                    break;
                case "=":
                    res = (Val1);
                    break;
                default:
                    res = 0 ;
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attribs, "name"), "Invalid opperator: " + OpperatorName, "");
                    break;
            }
            ParentRule.SetLocalVar(DestAttrib, res.ToString(CultureInfo.InvariantCulture));
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), Node.Name, "(" + Val1.ToString() + " " + OpperatorName + " " + Val2.ToString() + ") => " + res.ToString() + " => " + DestAttrib, "");
                Debug.WriteLine("{8} {0}: {1}({2}) {3} {4}({5}) => {6}({7})", this.GetType().Name, Val1Attrib, Val1, OpperatorName, Val2Attrib, Val2, DestAttrib, res, GetActionStepName());
            }
        }

    }

    public class RulesActionDoubleArithmeticOperation : RulesAction
    {
        private string Arg1Name;
        private string Arg2Name;
        private string OpperatorName;
        private string DestName;

        public RulesActionDoubleArithmeticOperation(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string arg1Name, string opperator, string arg2Name, string destName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            Arg1Name = arg1Name;
            OpperatorName = opperator;
            Arg2Name = arg2Name;
            DestName = destName;
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            double val1 = 0;
            var val1Attrib = XmlHelper.GetAttributeString(Attribs, Arg1Name);
            if (val1Attrib == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, Arg1Name + " can not be empty", "");
                return;
            }
            if (val1Attrib.StartsWith("#"))
            {
                var arg1StringVal = ParentRule.GetLocalVar(val1Attrib.TrimStart('#'));
                if (double.TryParse(arg1StringVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                {
                    val1 = aVal;
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+Arg1Name+": " + val1Attrib, "");
                }
            }
            else
            if (double.TryParse(val1Attrib, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
            {
                val1 = aVal;
            }

            double Val2 = 0;
            var val2Attrib = "<none>";
            if ((Arg2Name != string.Empty) && (Arg2Name != "0"))
            {
                val2Attrib = XmlHelper.GetAttributeString(Attribs, Arg2Name);
                if (val2Attrib == string.Empty)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, Arg2Name + " can not be empty", "");
                    return;
                }
                if (val2Attrib.StartsWith("#"))
                {
                    var arg2StringVal = ParentRule.GetLocalVar(val2Attrib.TrimStart('#'));
                    if (double.TryParse(arg2StringVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                    {
                        Val2 = aVal;
                    }
                    else
                    {
                        pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, "Invalid "+Arg2Name+": " + val2Attrib, "");
                    }
                }
                else
                if (double.TryParse(val2Attrib, NumberStyles.Float, CultureInfo.InvariantCulture, out var aVal))
                {
                    Val2 = aVal;
                }
            }
            var DestAttrib = XmlHelper.GetAttributeString(Attribs, DestName);

            double res ;
            switch (OpperatorName)
            {
                case "+":
                    res = (val1 + Val2);
                    break;
                case "-":
                    res = (val1 - Val2);
                    break;
                case "x":
                case "*":
                    res = (val1 * Val2);
                    break;
                case "/":
                    res = (val1 / Val2);
                    break;
                case "=":
                    res = (val1);
                    break;
                default:
                    res = 0 ;
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, XmlHelper.GetAttributeString(Attribs, "name"), "Invalid opperator: " + OpperatorName, "");
                    break;
            }
            ParentRule.SetLocalVar(DestAttrib, res.ToString(CultureInfo.InvariantCulture));
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), Node.Name, "(" + val1.ToString(CultureInfo.InvariantCulture) + " " + OpperatorName + " " + Val2.ToString(CultureInfo.InvariantCulture) + ") => " + res.ToString(CultureInfo.InvariantCulture) + " => " + DestAttrib, "");
                Debug.WriteLine("{8} {0}: {1}({2}) {3} {4}({5}) => {6}({7})", this.GetType().Name, val1Attrib, val1, OpperatorName, val2Attrib, Val2, DestAttrib, res, GetActionStepName());
            }
        }

    }
    
    public class RulesActionLoop : RulesAction
    {
        public List<RulesAction> ChildActions;

        public RulesActionLoop(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            ChildActions = new List<RulesAction>();
            for (int i = 0; i < thisNode.ChildNodes.Count; i++)
            {
                var actionNode = thisNode.ChildNodes.Item(i);
                var attribs = XmlHelper.ReadNodeAttributes(actionNode);

                var newAction = parent.BuildRuleAction(this, actionNode, attribs, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), Node.Name, "", "");
                Debug.WriteLine("{1} {0} - Begin", this.GetType().Name, GetActionStepName());
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
                        Debug.WriteLine("{1} {0} - Step {2} ({3})", this.GetType().Name, GetActionStepName(), i, child.Node.Name);
                    }
                    try
                    {
                        child.RunAction(pp, ref FieldIndex);
                    }
                    catch (Exception x)
                    {
                        Debug.WriteLine("{1} {0} - Step {2} ({3}) Exception: {4}", this.GetType().Name, GetActionStepName(), i, child.Node.Name,x.Message);
                        pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Error at \"" + child?.Node.Name + "\"", "Exception: " + x.Message + " => " + child?.Node.OuterXml, this.Node.Name);
                        LoopActionResult = LoopActionResult.Break ;
                    }
                    
                    if (LoopActionResult == LoopActionResult.Continue)
                    {
                        if (Properties.Settings.Default.ShowDebugInfo)
                        {
                            Debug.WriteLine("{1} {0} - Continue", this.GetType().Name, GetActionStepName());
                        }
                        continue;
                    }
                    if (LoopActionResult == LoopActionResult.Break)
                    {
                        if (Properties.Settings.Default.ShowDebugInfo)
                        {
                            Debug.WriteLine("{1} {0} - Break", this.GetType().Name, GetActionStepName());
                        }
                        break;
                    }
                    // Check for End of Packet
                    if (pp.PD.Cursor > pp.PD.RawBytes.Count)
                    {
                        LoopActionResult = LoopActionResult.Break;
                        pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Reached past end of Packet Data", this.Node.Name);
                        if (Properties.Settings.Default.ShowDebugInfo)
                        {
                            Debug.WriteLine("{1} {0} - EOP", this.GetType().Name, GetActionStepName());
                        }
                        break;
                    }
                }
                safetyCounter++;
                
                if (safetyCounter > maxSafetyCount)
                {
                    Debug.WriteLine("{2} {1} {0} - SafetyCheck, took over {3} loops", this.GetType().Name, GetActionStepName(),this.ParentRule.Name, maxSafetyCount);
                    break;
                }
                if (LoopActionResult == LoopActionResult.Break)
                    break;
            }
            LoopActionResult = LoopActionResult.Normal;
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                Debug.WriteLine("{1} {0} - End", this.GetType().Name, GetActionStepName());
            }
        }

    }

    public class RulesActionBreak : RulesAction
    {
        public RulesActionBreak(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            if (ParentAction != null)
            {
                if (Properties.Settings.Default.ShowDebugInfo)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), DepthString+"Break", "");
                }
                LoopActionResult = LoopActionResult.Break;
            }
            else
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), "Break Error", "Break has no parent");
            }
        }

    }

    public class RulesActionContinue : RulesAction
    {
        public RulesActionContinue(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            if (ParentAction != null)
            {
                if (Properties.Settings.Default.ShowDebugInfo)
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), DepthString+"Conitnue", "");
                }
                LoopActionResult = LoopActionResult.Continue;
            }
            else
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), "Conitnue Error", "Conitnue has no parent");
            }
        }

    }

    public class RulesActionSaveLookup : RulesAction
    {
        private string SourceValueName;
        private string SourceIDName;
        private string DestName;
        private string AltLookupTableName;
        private string SaveLookupName;

        public RulesActionSaveLookup(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string sourceIdField, string sourceValueField, string altLookupTable, string destName, string saveLookupName) : base(parent, parentAction, thisNode, thisStep, false)
        {
            SourceIDName = sourceIdField;
            SourceValueName = sourceValueField;
            DestName = destName;
            AltLookupTableName = altLookupTable;
            SaveLookupName = saveLookupName;
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var idAttrib = XmlHelper.GetAttributeString(Attribs, SourceIDName);
            if (idAttrib == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, SourceIDName + " can not be empty", "");
                return;
            }
            long sourceId = 0;
            var sourceIdString = ParentRule.GetLocalVar(idAttrib);
            if (XmlHelper.TryAttribParse(sourceIdString, out var sourceIdParse))
            {
                sourceId = sourceIdParse;
            }
            else
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, SourceIDName + " must be a int value field", "");
            }

            var valAttrib = XmlHelper.GetAttributeString(Attribs, SourceValueName);
            if (valAttrib == string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, Node.Name, SourceValueName + " can not be empty", "");
                return;
            }
            var sourceValue = ParentRule.GetLocalVar(valAttrib);

            var saveLookup = XmlHelper.GetAttributeString(Attribs, SaveLookupName);
            
            var altLookupAttrib = XmlHelper.GetAttributeString(Attribs, AltLookupTableName);
            if (altLookupAttrib != string.Empty)
            {
                if (XmlHelper.TryAttribParse(sourceValue, out var sourceValueParsed))
                {
                    var altLookupId = sourceValueParsed;
                    var lookedUpValue = pp.PD.Parent._parentTab.Engine.DataLookups.NLU(altLookupAttrib).GetValue((ulong)altLookupId);
                    sourceValue = "(" + altLookupAttrib + ":" + altLookupId.ToString() + ") " + lookedUpValue;
                    if (saveLookup != string.Empty)
                    {
                        // save result as var
                        ParentRule.SetLocalVar(saveLookup, lookedUpValue);
                    }
                }
                else
                {
                    pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+Node.Name, SourceValueName + " must be a int value field to use the alt lookups function", "");
                }
            }
            else if (saveLookup != string.Empty)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), Color.Red, DepthString+Node.Name, "Save lookup to local var requires to also have alt lookup set", "");
            }

            var destListName = XmlHelper.GetAttributeString(Attribs, DestName);

            pp.PD.Parent._parentTab.Engine.DataLookups.RegisterCustomLookup(destListName, (ulong)sourceId, sourceValue);
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), DepthString+Node.Name, "Save (" + sourceId.ToString() + " => " + sourceValue.ToString() + ") into " + destListName);
                Console.WriteLine("{6} {0}: {1}({2}) => {3}({4}) into {5}", this.GetType().Name, sourceId, idAttrib, sourceValue, valAttrib, destListName, GetActionStepName());
            }
        }

    }

    public class RulesActionComment : RulesAction
    {
        public string Comment = string.Empty;

        public RulesActionComment(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep) : base(parent, parentAction, thisNode, thisStep, false)
        {
            Comment = thisNode.InnerText.Trim();
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            var VARName = "#comment";
            pp.AddParseLineToView(0xFFFF, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + VARName, Comment, "");
        }

    }

    public class RulesActionReadUInt32Ms : RulesAction
    {
        public RulesActionReadUInt32Ms(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, bool reversed) : base(parent, parentAction, thisNode, thisStep, reversed)
        {
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            pp.AddDataFieldEx(pp.PD.Cursor, 4, ref FieldIndex);
            var d = pp.PD.GetUInt32AtPos(pos);
            if (isReversed)
                d = BitConverter.ToUInt32(BitConverter.GetBytes(d).Reverse().ToArray(), 0);
            var dataString =  PacketParser.MSToString(d) + " (0x" + d.ToString("X8") + ")";
            var VARName = XmlHelper.GetAttributeString(Attribs, "name");
            var LookupVal = GetLookup(d);
            ParentRule.SetLocalVar(VARName, d.ToString(CultureInfo.InvariantCulture));
            pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + VARName, LookupVal + dataString, "", d);
            pp.MarkParsed(pos, 4, FieldIndex);
        }

    }

    public class RulesActionTemplate : RulesAction
    {
        public List<RulesAction> ChildActions;
        public string TemplateName;

        public RulesActionTemplate(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            TemplateName = XmlHelper.GetAttributeString(Attribs, name.ToLower());
            var templates = parent._parent.Parent.Templates;
            if (!templates.TryGetValue(TemplateName, out var thisTemplate))
            {
                Debug.WriteLine("Template not found: " + TemplateName);
                return;
            }

            ChildActions = new List<RulesAction>();

            for (int i = 0; i < thisTemplate.ChildNodes.Count; i++)
            {
                var actionNode = thisTemplate.ChildNodes.Item(i);
                var attribs = XmlHelper.ReadNodeAttributes(actionNode);

                var newAction = parent.BuildRuleAction(this, actionNode, attribs, i);
                if (newAction != null)
                    ChildActions.Add(newAction);
            }
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            if (Properties.Settings.Default.ShowDebugInfo)
            {
                pp.AddParseLineToView(0xFFFF, "A" + GetActionStepName(), pp.GetDataColor(FieldIndex), Node.Name, "Template", "");
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
                    child.RunAction(pp, ref FieldIndex);
                }
                catch (Exception x)
                {
                    Debug.WriteLine("{1} {0} - Template Step {2} ({3}) Exception: {4}", this.GetType().Name, GetActionStepName(), i, child.Node.Name, x.Message);
                    pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Error at \"" + child?.Node.Name + "\"", "Exception: " + x.Message + " => " + child?.Node.OuterXml, this.Node.Name);
                    LoopActionResult = LoopActionResult.Break;
                }

                // Check for End of Packet
                if (pp.PD.Cursor > pp.PD.RawBytes.Count)
                {
                    LoopActionResult = LoopActionResult.Break;
                    pp.AddParseLineToView(0xFFFF, "A" + child?.GetActionStepName(), Color.Red, "Reached past end of Packet Data", this.Node.Name);
                    if (Properties.Settings.Default.ShowDebugInfo)
                    {
                        Debug.WriteLine("{1} {0} - EOP", this.GetType().Name, GetActionStepName());
                    }
                    break;
                }
            }

            if (Properties.Settings.Default.ShowDebugInfo)
            {
                Debug.WriteLine("{1} {0} - End Template", this.GetType().Name, GetActionStepName());
            }
        }

    }

    public class RulesActionEcho : RulesAction
    {
        public string FieldName;

        public RulesActionEcho(PacketRule parent, RulesAction parentAction, XmlNode thisNode, int thisStep, string name) : base(parent, parentAction, thisNode, thisStep, false)
        {
            FieldName = XmlHelper.GetAttributeString(Attribs, name.ToLower());
        }

        public override void RunAction(PacketParser pp, ref ushort FieldIndex)
        {
            var pos = pp.PD.Cursor;
            var dataString = ParentRule.GetLocalVar(FieldName);
            var hexString = string.Empty;
            // Handle output differently if float val
            if (double.TryParse(dataString, out var ValDouble) && (Math.Abs(Math.Floor(ValDouble) - ValDouble) > 0.00001))
            {
                pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + FieldName, ValDouble.ToString(CultureInfo.InvariantCulture), "");
            }
            else
            if (XmlHelper.TryAttribParse(dataString, out var ValNumber))
            {
                hexString = " (0x" + ValNumber.ToString("X")+")";
                var LookupVal = GetLookup((ulong)ValNumber);
                pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + FieldName, LookupVal + dataString + hexString, "");
            }
            else
            {
                pp.AddParseLineToView(FieldIndex, "0x" + pos.ToString("X2"), pp.GetDataColor(FieldIndex), DepthString + FieldName, dataString + hexString, "");
            }
        }

    }
    
}
