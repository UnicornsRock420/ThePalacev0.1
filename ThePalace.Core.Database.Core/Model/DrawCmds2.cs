namespace ThePalace.Core.Database.Core.Model
{
    public partial class DrawCmds2
    {
        public short RoomId { get; set; }
        public int DrawCmdId { get; set; }
        public short DrawCmdType { get; set; }
        public byte[] Data { get; set; }
    }
}
