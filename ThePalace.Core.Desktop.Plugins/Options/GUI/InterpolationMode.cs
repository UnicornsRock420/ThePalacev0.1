using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Desktop.Plugins.Options.GUI
{
    public class InterpolationMode : IOption<System.Drawing.Drawing2D.InterpolationMode>
    {
        private static readonly Lazy<InterpolationMode> _current = new();
        public static InterpolationMode Current => _current.Value;

        public string Category => @"\GUI\General\";
        public string Name => nameof(InterpolationMode);
        public string Description => string.Empty;
        public string Text => this.Value.ToString();

        public bool Enabled() => true;
        public void Load(params string[] values) =>
            this.Value = Enum.Parse<System.Drawing.Drawing2D.InterpolationMode>(values.FirstOrDefault());

        public IReadOnlyDictionary<string, System.Drawing.Drawing2D.InterpolationMode> Values { get; } =
            new Dictionary<string, System.Drawing.Drawing2D.InterpolationMode>
            {
                { nameof(System.Drawing.Drawing2D.InterpolationMode.Invalid), System.Drawing.Drawing2D.InterpolationMode.Invalid },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.Default), System.Drawing.Drawing2D.InterpolationMode.Default },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.Low), System.Drawing.Drawing2D.InterpolationMode.Low },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.High), System.Drawing.Drawing2D.InterpolationMode.High },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.Bilinear), System.Drawing.Drawing2D.InterpolationMode.Bilinear },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.Bicubic), System.Drawing.Drawing2D.InterpolationMode.Bicubic },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor), System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear), System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear },
                { nameof(System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic), System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic },
            };
        public System.Drawing.Drawing2D.InterpolationMode Value { get; set; } = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
    }
}
