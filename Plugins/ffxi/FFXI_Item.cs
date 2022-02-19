using System;

namespace VieweD.Engine.FFXI
{
    public class FFXI_Item : IComparable
    {
        public uint Id { get; set; }
        public FFXIHelper.FFXI_ItemFlags Flags { get; set; }
        public FFXIHelper.FFXI_ItemType Type { get; set; }
        public uint StackSize { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NameSingle { get; set; }
        public string NameMultiple { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            return Id.CompareTo((obj as FFXI_Item).Id);
        }
    }
}