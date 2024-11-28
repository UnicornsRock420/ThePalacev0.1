namespace ThePalace.Core.Database.Core.Model
{
    public partial class Auth
    {
        public int UserId { get; set; }
        public byte AuthType { get; set; }
        public string Value { get; set; }
        public int? Ctr { get; set; }
        public int? Crc { get; set; }
    }
}
