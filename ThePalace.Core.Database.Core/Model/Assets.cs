namespace ThePalace.Core.Database.Core.Model
{
    public partial class Assets
    {
        public int AssetId { get; set; }
        public int AssetCrc { get; set; }
        public string Name { get; set; }
        public int Flags { get; set; }
        public DateTime LastUsed { get; set; }
        public byte[] Data { get; set; }
    }
}
