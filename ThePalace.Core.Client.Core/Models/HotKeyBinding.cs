namespace ThePalace.Core.Client.Core.Models
{
    public sealed class HotKeyBinding
    {
        public ApiBinding Binding { get; set; } = null;
        public object[] Values { get; set; } = null;
    }
}
