using PostSharp.Extensibility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VieweD.Engine.Common;
using VieweD.Helpers.System;

namespace VieweD.Engine.pcapraw
{
    public class PCapRawPacketParser : PacketParser
    {
        public PacketRule PCapPacketRule { get; set; }

        public PCapRawPacketParser(PacketRule rule)
        {
            PCapPacketRule = rule;
            if (PCapPacketRule != null)
                PCapPacketRule.Build();
        }

        public override void ParseData(string ActiveSwitchBlock)
        {
            ushort dataFieldIndex = 0; // header is considered 0

            /*
            void AddDataField(int StartPos, int FieldByteSize)
            {
                AddDataFieldEx(StartPos, FieldByteSize, ref dataFieldIndex);
            }
            */

            ParsedView.Clear();
            SwitchBlocks.Clear();
            PD.Cursor = 0;
            // var pos = PD.Cursor;
            // var d = string.Empty;

            // Do the parsing here
            PD.Cursor = 0;
            if (PCapPacketRule == null)
                AddParseLineToView(0xFF, "L0", Color.Red, "Parser Error", "Not implemented");
            else
            {
                PD.PacketLevel = PCapPacketRule.Level;
                PCapPacketRule.RunRule(this, ref dataFieldIndex);
            }

            // The Rest
            ParseUnusedData(ref dataFieldIndex);
        }
    }
}
