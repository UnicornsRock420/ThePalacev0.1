namespace ThePalace.Core.Desktop.Core.Models
{
    public sealed class ComboboxItem
    {
        public string Text { get; set; } = null;
        public object Value { get; set; } = null;

        public override string ToString() =>
            this.Value.ToString();
    }
}
