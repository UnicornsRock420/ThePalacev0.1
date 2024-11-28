using ThePalace.Core.Desktop.Plugins.Forms;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Settings
{
    public sealed class Usernames : ISettingList<string>
    {
        private static readonly Lazy<Usernames> _current = new();
        public static Usernames Current => _current.Value;

        public string Category => @"\GUI\" + nameof(Connection) + @"\";
        public string Name => nameof(Usernames);
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
