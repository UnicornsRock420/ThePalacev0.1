namespace ThePalace.Core.Database.Core.Model
{
    public partial class LooseProps
    {
        public int TemplateId { get; set; }
        public int OrderId { get; set; }
        public int AssetId { get; set; }
        public int AssetCrc { get; set; }
        public int Flags { get; set; }
        public int? RefCon { get; set; }
        public short LocH { get; set; }
        public short LocV { get; set; }
    }
}
