namespace ThePalace.Core.Database.Core.Model
{
    public partial class Hotspots
    {
        public int UserId { get; set; }
        public short RoomId { get; set; }
        public short HotspotId { get; set; }
        public int Flags { get; set; }
        public short Type { get; set; }
        public string Name { get; set; }
        public short State { get; set; }
        public int? ScriptEventMask { get; set; }
        public int? SecureInfo { get; set; }
        public int? RefCon { get; set; }
        public short? LocH { get; set; }
        public short? LocV { get; set; }
        public short? Dest { get; set; }
        public string Script { get; set; }
    }
}
