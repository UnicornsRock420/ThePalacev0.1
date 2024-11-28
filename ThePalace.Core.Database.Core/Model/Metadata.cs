namespace ThePalace.Core.Database.Core.Model
{
    public partial class Metadata
    {
        public int AssetId { get; set; }
        public int Flags { get; set; }
        public string Format { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
        public short OffsetX { get; set; }
        public short OffsetY { get; set; }
    }
}
