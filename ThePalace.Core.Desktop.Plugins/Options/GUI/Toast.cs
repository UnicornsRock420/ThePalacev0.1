using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.GUI
{
#if WINDOWS10_0_17763_0_OR_GREATER
    public sealed class Toast : IOption<bool>
    {
        private static readonly Lazy<Toast> _current = new();
        public static Toast Current => _current.Value;

        public string Category => @"\GUI\Notifications\";
        public string Name => nameof(Toast);
        public string Description => @"Toggle Windows 10 Notifications";
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = bool.Parse(values.FirstOrDefault());

        public IReadOnlyDictionary<string, bool> Values { get; } = null;
        public bool Value { get; set; } = true;
    }
#endif
}
