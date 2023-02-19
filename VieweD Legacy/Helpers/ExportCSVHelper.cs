using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using VieweD.Engine;
using VieweD.Engine.Common;

namespace VieweD.Helpers.System
{
    static class ExportCSVHelper
    {
        public static bool ExportPacketToCSV(PacketList PL,string TargetFileName)
        {
            if (!PL.IsPreParsed)
                return false;

            List<string> FieldNames = new List<string>();

            void AddFieldName(string fName)
            {
                if (fName.StartsWith("?") || fName.StartsWith("_"))
                    return;
                if (!FieldNames.Contains(fName))
                    FieldNames.Add(fName);
            }

            int FieldIndex(string fName)
            {
                for (int i = 0; i < FieldNames.Count; i++)
                    if (FieldNames[i] == fName)
                        return i;
                return -1;
            }

            string FilteredString(string s)
            {
                string res = string.Empty;
                foreach(char c in s)
                {
                    if (((byte)c < 16) || ((byte)c > 192))
                        res += ".";
                    else
                        res += c;
                }
                return res;
            }

            var packetCount = PL.Count;

            // Generate Table-Headers
            FieldNames.Clear();
            AddFieldName("Header-Time");
            for (int x = 0; x < packetCount; x++)
                foreach (var pv in PL.GetPacket(x).PP.ParsedView)
                    AddFieldName(EngineBase.StripSpacer(pv.Var));

            if (FieldNames.Count > 100)
            {
                if (MessageBox.Show("The exported file will contain a large number of field columns (" + FieldNames.Count.ToString() + ")\r\n" +
                    "Are you sure you want to continue ?", "Export CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return false;
            }

            List<List<string>> csvString = new List<List<string>>();
            csvString.Add(FieldNames); // add header

            // Generate CSV data
            for (int x = 0; x < packetCount; x++)
            {
                var thisPacket = PL.GetPacket(x);
                var pvl = thisPacket.PP.ParsedView;
                List<string> line = new List<string>();
                foreach(var _ in FieldNames) line.Add("");
                line[0] = thisPacket.TimeStamp.ToString();
                foreach (var pv in pvl)
                {
                    var FieldName = EngineBase.StripSpacer(pv.Var);
                    var fIndex = FieldIndex(FieldName);
                    if (fIndex > 0)
                        line[fIndex] = FilteredString(pv.Data);
                }
                csvString.Add(line);
            }

            var lines = new List<string>();
            foreach(var lData in csvString)
            {
                string l = string.Empty;
                foreach(var f in lData)
                {
                    string FieldPart;
                    if (f.Contains("\"") || f.Contains(";") || f.Contains("'"))
                        FieldPart = "\"" + f.Replace("\"", "\\\"") + "\"" ;
                    else
                        FieldPart = f;
                    l += FieldPart + ";"; 
                }
                lines.Add(l);
            }
            try
            {
                File.WriteAllLines(TargetFileName, lines, Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
