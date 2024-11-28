﻿using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Console.Plugins.Options.Core.Scripting.JavaScript
{
    public class Cyborgs : IOption<bool>
    {
        private static readonly Lazy<Cyborgs> _current = new();
        public static Cyborgs Current => _current.Value;

        public string Category => @"\Scripting\JavaScript\";
        public string Name => nameof(Cyborgs);
        public string Description => @"Toggle JavaScript support for Cyborgs";
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = bool.Parse(values.FirstOrDefault());

        public IReadOnlyDictionary<string, bool> Values { get; } = null;
        public bool Value { get; set; } = false;
    }
}