using System;

namespace VieweD.Engine.FFXI
{
    public class FFXI_DialogTableEntry : IComparable
    {
        public ushort zoneId { get; set; }
        public ushort Id { get; set; }
        public string Text { get; set; }

        public uint KeyIndex => ((uint)zoneId * 0x10000) + (uint)Id;

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            return KeyIndex.CompareTo((obj as FFXI_DialogTableEntry).KeyIndex);
        }

    }
}