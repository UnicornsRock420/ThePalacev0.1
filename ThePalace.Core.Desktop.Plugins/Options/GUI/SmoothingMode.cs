using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.GUI
{
    public sealed class SmoothingMode : IOption<System.Drawing.Drawing2D.SmoothingMode>
    {
        private static readonly Lazy<SmoothingMode> _current = new();
        public static SmoothingMode Current => _current.Value;

        public string Category => @"\GUI\General\";
        public string Name => nameof(SmoothingMode);
        public string Description => string.Empty;
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = Enum.Parse<System.Drawing.Drawing2D.SmoothingMode>(values.FirstOrDefault());

        public IReadOnlyDictionary<string, System.Drawing.Drawing2D.SmoothingMode> Values { get; } =
            new Dictionary<string, System.Drawing.Drawing2D.SmoothingMode>
            {
                { nameof(System.Drawing.Drawing2D.SmoothingMode.Invalid), System.Drawing.Drawing2D.SmoothingMode.Invalid },
                { nameof(System.Drawing.Drawing2D.SmoothingMode.None), System.Drawing.Drawing2D.SmoothingMode.None },
                { nameof(System.Drawing.Drawing2D.SmoothingMode.Default), System.Drawing.Drawing2D.SmoothingMode.Default },
                { nameof(System.Drawing.Drawing2D.SmoothingMode.AntiAlias), System.Drawing.Drawing2D.SmoothingMode.AntiAlias },
                { nameof(System.Drawing.Drawing2D.SmoothingMode.HighSpeed), System.Drawing.Drawing2D.SmoothingMode.HighSpeed },
                { nameof(System.Drawing.Drawing2D.SmoothingMode.HighQuality), System.Drawing.Drawing2D.SmoothingMode.HighQuality },
            };
        public System.Drawing.Drawing2D.SmoothingMode Value { get; set; } = System.Drawing.Drawing2D.SmoothingMode.None;
    }
}
