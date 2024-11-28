namespace ThePalace.Core.Database.Core.Model
{
    public partial class Log
    {
        public int LogId { get; set; }
        public string MessageType { get; set; }
        public string MachineName { get; set; }
        public string ApplicationName { get; set; }
        public long ProcessId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
