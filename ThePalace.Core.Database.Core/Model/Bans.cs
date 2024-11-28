namespace ThePalace.Core.Database.Core.Model
{
    public partial class Bans
    {
        public int BanId { get; set; }
        public DateTime? UntilDate { get; set; }
        public string Ipaddress { get; set; }
        public int RegCtr { get; set; }
        public int RegCrc { get; set; }
        public int Puidctr { get; set; }
        public int Puidcrc { get; set; }
        public string Note { get; set; }
    }
}
