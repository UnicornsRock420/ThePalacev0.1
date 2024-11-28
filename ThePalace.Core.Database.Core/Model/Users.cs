namespace ThePalace.Core.Database.Core.Model
{
    public partial class Users
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
