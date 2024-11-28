namespace ThePalace.Core.Client.Core.Models
{
    public sealed class LocalizationNoun
    {
        public string singular { get; set; }
        public string plural { get; set; }
    }

    public sealed class LocalizationVerb
    {
        public string present { get; set; }
        public string past { get; set; }
        public string active { get; set; }
    }
}
