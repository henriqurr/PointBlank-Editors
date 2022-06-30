namespace PropertyEditor.Models
{
    public class Header
    {
        public string ResourceType { get; set; }
        public int Unk { get; set; }
        public ushort VersionMajor { get; set; }
        public ushort VersionMinor { get; set; }
        public int StringTableCount { get; set; }
        public ulong StringTableOffset { get; set; }
        public ulong StringTableSizes { get; set; }
        public int ObjectInfoCount { get; set; }
        public ulong ObjectInfoOffset { get; set; }
        public ulong ObjectInfoSize { get; set; }
    }
}
