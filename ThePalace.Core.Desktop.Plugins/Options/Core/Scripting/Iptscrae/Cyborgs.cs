﻿using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.Core.Scripting.Iptscrae
{
    public sealed class Cyborgs : IOption<bool>
    {
        private static readonly Lazy<Cyborgs> _current = new();
        public static Cyborgs Current => _current.Value;

        public string Category => @"\Scripting\Iptscrae\";
        public string Name => nameof(Cyborgs);
        public string Description => @"Toggle Iptscrae support for Cyborgs";
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = bool.Parse(values.FirstOrDefault());

        public IReadOnlyDictionary<string, bool> Values { get; } = null;
        public bool Value { get; set; } = true;
    }
}
