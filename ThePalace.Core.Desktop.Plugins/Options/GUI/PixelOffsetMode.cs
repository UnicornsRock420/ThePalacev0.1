using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.GUI
{
    public sealed class PixelOffsetMode : IOption<System.Drawing.Drawing2D.PixelOffsetMode>
    {
        private static readonly Lazy<PixelOffsetMode> _current = new();
        public static PixelOffsetMode Current => _current.Value;

        public string Category => @"\GUI\General\";
        public string Name => nameof(PixelOffsetMode);
        public string Description => string.Empty;
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = Enum.Parse<System.Drawing.Drawing2D.PixelOffsetMode>(values.FirstOrDefault());

        public IReadOnlyDictionary<string, System.Drawing.Drawing2D.PixelOffsetMode> Values { get; } =
            new Dictionary<string, System.Drawing.Drawing2D.PixelOffsetMode>
            {
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.Invalid), System.Drawing.Drawing2D.PixelOffsetMode.Invalid },
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.None), System.Drawing.Drawing2D.PixelOffsetMode.None },
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.Default), System.Drawing.Drawing2D.PixelOffsetMode.Default },
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.Half), System.Drawing.Drawing2D.PixelOffsetMode.Half },
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed), System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed },
                { nameof(System.Drawing.Drawing2D.PixelOffsetMode.HighQuality), System.Drawing.Drawing2D.PixelOffsetMode.HighQuality },
            };
        public System.Drawing.Drawing2D.PixelOffsetMode Value { get; set; } = System.Drawing.Drawing2D.PixelOffsetMode.Half;
    }
}
