namespace ThePalace.Core.Database.Core.Model
{
    public partial class DrawCmds
    {
        public int UserId { get; set; }
        public short RoomId { get; set; }
        public int DrawCmdId { get; set; }
        public short DrawCmdType { get; set; }
    }
}
