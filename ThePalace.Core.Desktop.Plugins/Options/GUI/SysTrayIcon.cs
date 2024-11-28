using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.GUI
{
    public sealed class SysTrayIcon : IOption<bool>
    {
        private static readonly Lazy<SysTrayIcon> _current = new();
        public static SysTrayIcon Current => _current.Value;

        public string Category => @"\GUI\General\";
        public string Name => nameof(SysTrayIcon);
        public string Description => @"Toggle System Tray Icon";
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = bool.Parse(values.FirstOrDefault());

        public IReadOnlyDictionary<string, bool> Values { get; } = null;
        public bool Value { get; set; } = true;
    }
}
