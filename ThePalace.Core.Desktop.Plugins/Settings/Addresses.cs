using ThePalace.Core.Desktop.Plugins.Forms;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Settings
{
    public sealed class Addresses : ISettingList<string>
    {
        private static readonly Lazy<Addresses> _current = new();
        public static Addresses Current => _current.Value;

        public string Category => @"\GUI\" + nameof(Connection) + @"\";
        public string Name => nameof(Addresses);
        public string Description => string.Empty;
        public string[] Text =>
            this.Values
                .Select(v => v.ToString())
                .ToArray();

        public List<string> Values { get; set; } = new();

        public void Load(params string[] values) =>
            this.Values = new List<string>(values);
    }
}
