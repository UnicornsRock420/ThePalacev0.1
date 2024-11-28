using System.Drawing;
using System.Text.RegularExpressions;

namespace ThePalace.Core.Client.Core.Constants
{
    public static class UIConstants
    {
        public static readonly Regex REGEX_VISIBLE = new Regex(@"^[;](.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
        public static readonly Regex REGEX_TYPE = new Regex(@"^[\s]*([!:^])[\s]*(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
        public static readonly Regex REGEX_SOUND = new Regex(@"^[\s]*[\)]([\w\d\.]+)[\s]*(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);
        public static readonly Regex REGEX_LOCATION = new Regex(@"^[\s]*[@]([0-9]+)[\s]*[,][\s]*([0-9]+)[\s]*(.*)$", RegexOptions.Multiline | RegexOptions.Compiled);

        public static readonly Font FONT_DEFAULT = new Font("Arial", 14);

        public static class AspectRatio
        {
            public static class StandardDef
            {
                public const int Multiplier = 128;
                public static readonly Size Ratio = new Size(4, 3);
                public static readonly Size Default = new Size(Ratio.Width * Multiplier, Ratio.Height * Multiplier);
            }
            public static class WidescreenDef
            {
                public const int Multiplier = 96;
                public static readonly Size Ratio = new Size(16, 9);
                public static readonly Size Default = new Size(Ratio.Width * Multiplier, Ratio.Height * Multiplier);
            }
        }

        public const int MaxNbrFaces = 13;
        public const int MaxNbrColors = 16;

        public static readonly uint[] SmileyColours = new uint[]
        {
            0xFFFF0000,
            0xFFFF5F00,
            0xFFFFBF00,
            0xFFDFFF00,
            0xFF7FFF00,
            0xFF1FFF00,
            0xFF00FF3F,
            0xFF00FF9F,
            0xFF00FFFF,
            0xFF009FFF,
            0xFF003FFF,
            0xFF1F00FF,
            0xFF7F00FF,
            0xFFDF00FF,
            0xFFFF00BF,
            0xFFFF005F,
        };

        public static Color NbrToColor(short colorNbr) =>
            Color.FromArgb((int)SmileyColours[colorNbr % MaxNbrColors]);
    }
}
