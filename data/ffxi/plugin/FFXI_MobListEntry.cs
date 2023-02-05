namespace VieweD.Engine.FFXI
{
    public class FFXI_MobListEntry
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public ushort ExpectedZoneId { get; set; }

        public FFXI_MobListEntry()
        {
            Id = 0;
            Name = "none";
            ExpectedZoneId = 0;
        }
    }
}