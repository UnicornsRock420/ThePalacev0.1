using System.ComponentModel;

namespace System.Drawing
{
    /// <summary>Enum representing the available chat name color presets.</summary>
    /// <see cref="CheerColorEnum"
    ///      cref="KnownColor"
    ///      cref="Color"/>
    public enum ColorEnum
    {
        ActiveBorder = KnownColor.ActiveBorder, //1
        ActiveCaption = KnownColor.ActiveCaption, //2
        ActiveCaptionText = KnownColor.ActiveCaptionText, //3
        AppWorkspace = KnownColor.AppWorkspace, //4
        Control = KnownColor.Control, //5
        ControlDark = KnownColor.ControlDark, //6
        ControlDarkDark = KnownColor.ControlDarkDark, //7
        ControlLight = KnownColor.ControlLight, //8
        ControlLightLight = KnownColor.ControlLightLight, //9
        ControlText = KnownColor.ControlText, //10
        Desktop = KnownColor.Desktop, //11
        GrayText = KnownColor.GrayText, //12
        Highlight = KnownColor.Highlight, //13
        HighlightText = KnownColor.HighlightText, //14
        HotTrack = KnownColor.HotTrack, //15
        InactiveBorder = KnownColor.InactiveBorder, //16
        InactiveCaption = KnownColor.InactiveCaption, //17
        InactiveCaptionText = KnownColor.InactiveCaptionText, //18
        Info = KnownColor.Info, //19
        InfoText = KnownColor.InfoText, //20
        Menu = KnownColor.Menu, //21
        MenuText = KnownColor.MenuText, //22
        ScrollBar = KnownColor.ScrollBar, //23
        Window = KnownColor.Window, //24
        WindowFrame = KnownColor.WindowFrame, //25
        WindowText = KnownColor.WindowText, //26
        Transparent = KnownColor.Transparent, //27

        /// <see cref="KnownColor.AliceBlue"/>
        /// <seealso cref="Color.AliceBlue"/>
        [Description("#F0F8FF")]
        AliceBlue = KnownColor.AliceBlue, //28
        /// <see cref="KnownColor.AntiqueWhite"/>
        /// <seealso cref="Color.AntiqueWhite"/>
        [Description("#FAEBD7")]
        AntiqueWhite = KnownColor.AntiqueWhite, //29
        /// <see cref="KnownColor.Aqua"/>
        /// <seealso cref="Color.Aqua"/>
        [Description("#00FFFF")]
        Aqua = KnownColor.Aqua, //30
        /// <see cref="KnownColor.Aquamarine"/>
        /// <seealso cref="Color.Aquamarine"/>
        [Description("#7FFFD4")]
        Aquamarine = KnownColor.Aquamarine, //31
        /// <see cref="KnownColor.Azure"/>
        /// <seealso cref="Color.Azure"/>
        [Description("#F0FFFF")]
        Azure = KnownColor.Azure, //32
        /// <see cref="KnownColor.Beige"/>
        /// <seealso cref="Color.Beige"/>
        [Description("#F5F5DC")]
        Beige = KnownColor.Beige, //33
        /// <see cref="KnownColor.Bisque"/>
        /// <seealso cref="Color.Bisque"/>
        [Description("#FFE4C4")]
        Bisque = KnownColor.Bisque, //34
        /// <see cref="KnownColor.Black"/>
        /// <seealso cref="Color.Black"/>
        [Description("#000000")]
        Black = KnownColor.Black, //35
        /// <see cref="KnownColor.BlanchedAlmond"/>
        /// <seealso cref="Color.BlanchedAlmond"/>
        [Description("#FFEBCD")]
        BlanchedAlmond = KnownColor.BlanchedAlmond, //36
        /// <see cref="KnownColor.Blue"/>
        /// <seealso cref="Color.Blue"/>
        [Description("#0000FF")]
        Blue = KnownColor.Blue, //37
        /// <see cref="KnownColor.BlueViolet"/>
        /// <seealso cref="Color.BlueViolet"/>
        [Description("#8A2BE2")]
        BlueViolet = KnownColor.BlueViolet, //38
        /// <see cref="KnownColor.Brown"/>
        /// <seealso cref="Color.Brown"/>
        [Description("#A52A2A")]
        Brown = KnownColor.Brown, //39
        /// <see cref="KnownColor.BurlyWood"/>
        /// <seealso cref="Color.BurlyWood"/>
        [Description("#DEB887")]
        BurlyWood = KnownColor.BurlyWood, //40
        /// <see cref="KnownColor.CadetBlue"/>
        /// <seealso cref="Color.CadetBlue"/>
        [Description("#5F9EA0")]
        CadetBlue = KnownColor.CadetBlue, //41
        /// <see cref="KnownColor.Chartreuse"/>
        /// <seealso cref="Color.Chartreuse"/>
        [Description("#7FFF00")]
        Chartreuse = KnownColor.Chartreuse, //42
        /// <see cref="KnownColor.Chocolate"/>
        /// <seealso cref="Color.Chocolate"/>
        [Description("#D2691E")]
        Chocolate = KnownColor.Chocolate, //43
        /// <see cref="KnownColor.Coral"/>
        /// <seealso cref="Color.Coral"/>
        [Description("#FF7F50")]
        Coral = KnownColor.Coral, //44
        /// <see cref="KnownColor.CornflowerBlue"/>
        /// <seealso cref="Color.CornflowerBlue"/>
        [Description("#6495ED")]
        CornflowerBlue = KnownColor.CornflowerBlue, //45
        /// <see cref="KnownColor.Cornsilk"/>
        /// <seealso cref="Color.Cornsilk"/>
        [Description("#FFF8DC")]
        Cornsilk = KnownColor.Cornsilk, //46
        /// <see cref="KnownColor.Crimson"/>
        /// <seealso cref="Color.Crimson"/>
        [Description("#DC143C")]
        Crimson = KnownColor.Crimson, //47
        /// <see cref="KnownColor.Cyan"/>
        /// <seealso cref="Color.Cyan"/>
        [Description("#00FFFF")]
        Cyan = KnownColor.Cyan, //48
        /// <see cref="KnownColor.DarkBlue"/>
        /// <seealso cref="Color.DarkBlue"/>
        [Description("#00008B")]
        DarkBlue = KnownColor.DarkBlue, //49
        /// <see cref="KnownColor.DarkCyan"/>
        /// <seealso cref="Color.DarkCyan"/>
        [Description("#008B8B")]
        DarkCyan = KnownColor.DarkCyan, //50
        /// <see cref="KnownColor.DarkGoldenrod"/>
        /// <seealso cref="Color.DarkGoldenrod"/>
        [Description("#B8860B")]
        DarkGoldenrod = KnownColor.DarkGoldenrod, //51
        /// <see cref="KnownColor.DarkGray"/>
        /// <seealso cref="Color.DarkGray"/>
        [Description("#A9A9A9")]
        DarkGray = KnownColor.DarkGray, //52
        /// <see cref="KnownColor.DarkGreen"/>
        /// <seealso cref="Color.DarkGreen"/>
        [Description("#006400")]
        DarkGreen = KnownColor.DarkGreen, //53
        /// <see cref="KnownColor.DarkKhaki"/>
        /// <seealso cref="Color.DarkKhaki"/>
        [Description("#BDB76B")]
        DarkKhaki = KnownColor.DarkKhaki, //54
        /// <see cref="KnownColor.DarkMagenta"/>
        /// <seealso cref="Color.DarkMagenta"/>
        [Description("#8B008B")]
        DarkMagenta = KnownColor.DarkMagenta, //55
        /// <see cref="KnownColor.DarkOliveGreen"/>
        /// <seealso cref="Color.DarkOliveGreen"/>
        [Description("#556B2F")]
        DarkOliveGreen = KnownColor.DarkOliveGreen, //56
        /// <see cref="KnownColor.DarkOrange"/>
        /// <seealso cref="Color.DarkOrange"/>
        [Description("#FF8C00")]
        DarkOrange = KnownColor.DarkOrange, //57
        /// <see cref="KnownColor.DarkOrchid"/>
        /// <seealso cref="Color.DarkOrchid"/>
        [Description("#9932CC")]
        DarkOrchid = KnownColor.DarkOrchid, //58
        /// <see cref="KnownColor.DarkRed"/>
        /// <seealso cref="Color.DarkRed"/>
        [Description("#8B0000")]
        DarkRed = KnownColor.DarkRed, //59
        /// <see cref="KnownColor.DarkSalmon"/>
        /// <seealso cref="Color.DarkSalmon"/>
        [Description("#E9967A")]
        DarkSalmon = KnownColor.DarkSalmon, //60
        /// <see cref="KnownColor.DarkSeaGreen"/>
        /// <seealso cref="Color.DarkSeaGreen"/>
        [Description("#8FBC8F")]
        DarkSeaGreen = KnownColor.DarkSeaGreen, //61
        /// <see cref="KnownColor.DarkSlateBlue"/>
        /// <seealso cref="Color.DarkSlateBlue"/>
        [Description("#483D8B")]
        DarkSlateBlue = KnownColor.DarkSlateBlue, //62
        /// <see cref="KnownColor.DarkSlateGray"/>
        /// <seealso cref="Color.DarkSlateGray"/>
        [Description("#2F4F4F")]
        DarkSlateGray = KnownColor.DarkSlateGray, //63
        /// <see cref="KnownColor.DarkTurquoise"/>
        /// <seealso cref="Color.DarkTurquoise"/>
        [Description("#00CED1")]
        DarkTurquoise = KnownColor.DarkTurquoise, //64
        /// <see cref="KnownColor.DarkViolet"/>
        /// <seealso cref="Color.DarkViolet"/>
        [Description("#9400D3")]
        DarkViolet = KnownColor.DarkViolet, //65
        /// <see cref="KnownColor.DeepPink"/>
        /// <seealso cref="Color.DeepPink"/>
        [Description("#FF1493")]
        DeepPink = KnownColor.DeepPink, //66
        /// <see cref="KnownColor.DeepSkyBlue"/>
        /// <seealso cref="Color.DeepSkyBlue"/>
        [Description("#00BFFF")]
        DeepSkyBlue = KnownColor.DeepSkyBlue, //67
        /// <see cref="KnownColor.DimGray"/>
        /// <seealso cref="Color.DimGray"/>
        [Description("#696969")]
        DimGray = KnownColor.DimGray, //68
        /// <see cref="KnownColor.DodgerBlue"/>
        /// <seealso cref="Color.DodgerBlue"/>
        [Description("#1E90FF")]
        DodgerBlue = KnownColor.DodgerBlue, //69
        /// <see cref="KnownColor.Firebrick"/>
        /// <seealso cref="Color.Firebrick"/>
        [Description("#B22222")]
        Firebrick = KnownColor.Firebrick, //70
        /// <see cref="KnownColor.FloralWhite"/>
        /// <seealso cref="Color.FloralWhite"/>
        [Description("#FFFAF0")]
        FloralWhite = KnownColor.FloralWhite, //71
        /// <see cref="KnownColor.ForestGreen"/>
        /// <seealso cref="Color.ForestGreen"/>
        [Description("#228B22")]
        ForestGreen = KnownColor.ForestGreen, //72
        /// <see cref="KnownColor.Fuchsia"/>
        /// <seealso cref="Color.Fuchsia"/>
        [Description("#FF00FF")]
        Fuchsia = KnownColor.Fuchsia, //73
        /// <see cref="KnownColor.Gainsboro"/>
        /// <seealso cref="Color.Gainsboro"/>
        [Description("#DCDCDC")]
        Gainsboro = KnownColor.Gainsboro, //74
        /// <see cref="KnownColor.GhostWhite"/>
        /// <seealso cref="Color.GhostWhite"/>
        [Description("#F8F8FF")]
        GhostWhite = KnownColor.GhostWhite, //75
        /// <see cref="KnownColor.Gold"/>
        /// <seealso cref="Color.Gold"/>
        [Description("#FFD700")]
        Gold = KnownColor.Gold, //76
        /// <see cref="KnownColor.Goldenrod"/>
        /// <seealso cref="Color.Goldenrod"/>
        [Description("#DAA520")]
        Goldenrod = KnownColor.Goldenrod, //77
        /// <see cref="KnownColor.Gray"/>
        /// <seealso cref="Color.Gray"/>
        [Description("#808080")]
        Gray = KnownColor.Gray, //78
        /// <see cref="KnownColor.Green"/>
        /// <seealso cref="Color.Green"/>
        [Description("#008000")]
        Green = KnownColor.Green, //79
        /// <see cref="KnownColor.GreenYellow"/>
        /// <seealso cref="Color.GreenYellow"/>
        [Description("#ADFF2F")]
        GreenYellow = KnownColor.GreenYellow, //80
        /// <see cref="KnownColor.Honeydew"/>
        /// <seealso cref="Color.Honeydew"/>
        [Description("#F0FFF0")]
        Honeydew = KnownColor.Honeydew, //81
        /// <see cref="KnownColor.HotPink"/>
        /// <seealso cref="Color.HotPink"/>
        [Description("#FF69B4")]
        HotPink = KnownColor.HotPink, //82
        /// <see cref="KnownColor.IndianRed"/>
        /// <seealso cref="Color.IndianRed"/>
        [Description("#CD5C5C")]
        IndianRed = KnownColor.IndianRed, //83
        /// <see cref="KnownColor.Indigo"/>
        /// <seealso cref="Color.Indigo"/>
        [Description("#4B0082")]
        Indigo = KnownColor.Indigo, //84
        /// <see cref="KnownColor.Ivory"/>
        /// <seealso cref="Color.Ivory"/>
        [Description("#FFFFF0")]
        Ivory = KnownColor.Ivory, //85
        /// <see cref="KnownColor.Khaki"/>
        /// <seealso cref="Color.Khaki"/>
        [Description("#F0E68C")]
        Khaki = KnownColor.Khaki, //86
        /// <see cref="KnownColor.Lavender"/>
        /// <seealso cref="Color.Lavender"/>
        [Description("#E6E6FA")]
        Lavender = KnownColor.Lavender, //87
        /// <see cref="KnownColor.LavenderBlush"/>
        /// <seealso cref="Color.LavenderBlush"/>
        [Description("#FFF0F5")]
        LavenderBlush = KnownColor.LavenderBlush, //88
        /// <see cref="KnownColor.LawnGreen"/>
        /// <seealso cref="Color.LawnGreen"/>
        [Description("#7CFC00")]
        LawnGreen = KnownColor.LawnGreen, //89
        /// <see cref="KnownColor.LemonChiffon"/>
        /// <seealso cref="Color.LemonChiffon"/>
        [Description("#FFFACD")]
        LemonChiffon = KnownColor.LemonChiffon, //90
        /// <see cref="KnownColor.LightBlue"/>
        /// <seealso cref="Color.LightBlue"/>
        [Description("#ADD8E6")]
        LightBlue = KnownColor.LightBlue, //91
        /// <see cref="KnownColor.LightCoral"/>
        /// <seealso cref="Color.LightCoral"/>
        [Description("#F08080")]
        LightCoral = KnownColor.LightCoral, //92
        /// <see cref="KnownColor.LightCyan"/>
        /// <seealso cref="Color.LightCyan"/>
        [Description("#E0FFFF")]
        LightCyan = KnownColor.LightCyan, //93
        /// <see cref="KnownColor.LightGoldenrodYellow"/>
        /// <seealso cref="Color.LightGoldenrodYellow"/>
        [Description("#FAFAD2")]
        LightGoldenrodYellow = KnownColor.LightGoldenrodYellow, //94
        /// <see cref="KnownColor.LightGray"/>
        /// <seealso cref="Color.LightGray"/>
        [Description("#D3D3D3")]
        LightGray = KnownColor.LightGray, //95
        /// <see cref="KnownColor.LightGreen"/>
        /// <seealso cref="Color.LightGreen"/>
        [Description("#90EE90")]
        LightGreen = KnownColor.LightGreen, //96
        /// <see cref="KnownColor.LightPink"/>
        /// <seealso cref="Color.LightPink"/>
        [Description("#FFB6C1")]
        LightPink = KnownColor.LightPink, //97
        /// <see cref="KnownColor.LightSalmon"/>
        /// <seealso cref="Color.LightSalmon"/>
        [Description("#FFA07A")]
        LightSalmon = KnownColor.LightSalmon, //98
        /// <see cref="KnownColor.LightSeaGreen"/>
        /// <seealso cref="Color.LightSeaGreen"/>
        [Description("#20B2AA")]
        LightSeaGreen = KnownColor.LightSeaGreen, //99
        /// <see cref="KnownColor.LightSkyBlue"/>
        /// <seealso cref="Color.LightSkyBlue"/>
        [Description("#87CEFA")]
        LightSkyBlue = KnownColor.LightSkyBlue, //100
        /// <see cref="KnownColor.LightSlateGray"/>
        /// <seealso cref="Color.LightSlateGray"/>
        [Description("#778899")]
        LightSlateGray = KnownColor.LightSlateGray, //101
        /// <see cref="KnownColor.LightSteelBlue"/>
        /// <seealso cref="Color.LightSteelBlue"/>
        [Description("#B0C4DE")]
        LightSteelBlue = KnownColor.LightSteelBlue, //102
        /// <see cref="KnownColor.LightYellow"/>
        /// <seealso cref="Color.LightYellow"/>
        [Description("#FFFFE0")]
        LightYellow = KnownColor.LightYellow, //103
        /// <see cref="KnownColor.Lime"/>
        /// <seealso cref="Color.Lime"/>
        [Description("#00FF00")]
        Lime = KnownColor.Lime, //104
        /// <see cref="KnownColor.LimeGreen"/>
        /// <seealso cref="Color.LimeGreen"/>
        [Description("#32CD32")]
        LimeGreen = KnownColor.LimeGreen, //105
        /// <see cref="KnownColor.Linen"/>
        /// <seealso cref="Color.Linen"/>
        [Description("#FAF0E6")]
        Linen = KnownColor.Linen, //106
        /// <see cref="KnownColor.Magenta"/>
        /// <seealso cref="Color.Magenta"/>
        [Description("#FF00FF")]
        Magenta = KnownColor.Magenta, //107
        /// <see cref="KnownColor.Maroon"/>
        /// <seealso cref="Color.Maroon"/>
        [Description("#800000")]
        Maroon = KnownColor.Maroon, //108
        /// <see cref="KnownColor.MediumAquamarine"/>
        /// <seealso cref="Color.MediumAquamarine"/>
        [Description("#66CDAA")]
        MediumAquamarine = KnownColor.MediumAquamarine, //109
        /// <see cref="KnownColor.MediumBlue"/>
        /// <seealso cref="Color.MediumBlue"/>
        [Description("#0000CD")]
        MediumBlue = KnownColor.MediumBlue, //110
        /// <see cref="KnownColor.MediumOrchid"/>
        /// <seealso cref="Color.MediumOrchid"/>
        [Description("#BA55D3")]
        MediumOrchid = KnownColor.MediumOrchid, //111
        /// <see cref="KnownColor.MediumPurple"/>
        /// <seealso cref="Color.MediumPurple"/>
        [Description("#9370DB")]
        MediumPurple = KnownColor.MediumPurple, //112
        /// <see cref="KnownColor.MediumSeaGreen"/>
        /// <seealso cref="Color.MediumSeaGreen"/>
        [Description("#3CB371")]
        MediumSeaGreen = KnownColor.MediumSeaGreen, //113
        /// <see cref="KnownColor.MediumSlateBlue"/>
        /// <seealso cref="Color.MediumSlateBlue"/>
        [Description("#7B68EE")]
        MediumSlateBlue = KnownColor.MediumSlateBlue, //114
        /// <see cref="KnownColor.MediumSpringGreen"/>
        /// <seealso cref="Color.MediumSpringGreen"/>
        [Description("#00FA9A")]
        MediumSpringGreen = KnownColor.MediumSpringGreen, //115
        /// <see cref="KnownColor.MediumTurquoise"/>
        /// <seealso cref="Color.MediumTurquoise"/>
        [Description("#48D1CC")]
        MediumTurquoise = KnownColor.MediumTurquoise, //116
        /// <see cref="KnownColor.MediumVioletRed"/>
        /// <seealso cref="Color.MediumVioletRed"/>
        [Description("#C71585")]
        MediumVioletRed = KnownColor.MediumVioletRed, //117
        /// <see cref="KnownColor.MidnightBlue"/>
        /// <seealso cref="Color.MidnightBlue"/>
        [Description("#191970")]
        MidnightBlue = KnownColor.MidnightBlue, //118
        /// <see cref="KnownColor.MintCream"/>
        /// <seealso cref="Color.MintCream"/>
        [Description("#F5FFFA")]
        MintCream = KnownColor.MintCream, //119
        /// <see cref="KnownColor.MistyRose"/>
        /// <seealso cref="Color.MistyRose"/>
        [Description("#FFE4E1")]
        MistyRose = KnownColor.MistyRose, //120
        /// <see cref="KnownColor.Moccasin"/>
        /// <seealso cref="Color.Moccasin"/>
        [Description("#FFE4B5")]
        Moccasin = KnownColor.Moccasin, //121
        /// <see cref="KnownColor.NavajoWhite"/>
        /// <seealso cref="Color.NavajoWhite"/>
        [Description("#FFDEAD")]
        NavajoWhite = KnownColor.NavajoWhite, //122
        /// <see cref="KnownColor.Navy"/>
        /// <seealso cref="Color.Navy"/>
        [Description("#000080")]
        Navy = KnownColor.Navy, //123
        /// <see cref="KnownColor.OldLace"/>
        /// <seealso cref="Color.OldLace"/>
        [Description("#FDF5E6")]
        OldLace = KnownColor.OldLace, //124
        /// <see cref="KnownColor.Olive"/>
        /// <seealso cref="Color.Olive"/>
        [Description("#808000")]
        Olive = KnownColor.Olive, //125
        /// <see cref="KnownColor.OliveDrab"/>
        /// <seealso cref="Color.OliveDrab"/>
        [Description("#6B8E23")]
        OliveDrab = KnownColor.OliveDrab, //126
        /// <see cref="KnownColor.Orange"/>
        /// <seealso cref="Color.Orange"/>
        [Description("#FFA500")]
        Orange = KnownColor.Orange, //127
        /// <see cref="KnownColor.OrangeRed"/>
        /// <seealso cref="Color.OrangeRed"/>
        [Description("#FF4500")]
        OrangeRed = KnownColor.OrangeRed, //128
        /// <see cref="KnownColor.Orchid"/>
        /// <seealso cref="Color.Orchid"/>
        [Description("#DA70D6")]
        Orchid = KnownColor.Orchid, //129
        /// <see cref="KnownColor.PaleGoldenrod"/>
        /// <seealso cref="Color.PaleGoldenrod"/>
        [Description("#EEE8AA")]
        PaleGoldenrod = KnownColor.PaleGoldenrod, //130
        /// <see cref="KnownColor.PaleGreen"/>
        /// <seealso cref="Color.PaleGreen"/>
        [Description("#98FB98")]
        PaleGreen = KnownColor.PaleGreen, //131
        /// <see cref="KnownColor.PaleTurquoise"/>
        /// <seealso cref="Color.PaleTurquoise"/>
        [Description("#AFEEEE")]
        PaleTurquoise = KnownColor.PaleTurquoise, //132
        /// <see cref="KnownColor.PaleVioletRed"/>
        /// <seealso cref="Color.PaleVioletRed"/>
        [Description("#DB7093")]
        PaleVioletRed = KnownColor.PaleVioletRed, //133
        /// <see cref="KnownColor.PapayaWhip"/>
        /// <seealso cref="Color.PapayaWhip"/>
        [Description("#FFEFD5")]
        PapayaWhip = KnownColor.PapayaWhip, //134
        /// <see cref="KnownColor.PeachPuff"/>
        /// <seealso cref="Color.PeachPuff"/>
        [Description("#FFDAB9")]
        PeachPuff = KnownColor.PeachPuff, //135
        /// <see cref="KnownColor.Peru"/>
        /// <seealso cref="Color.Peru"/>
        [Description("#CD853F")]
        Peru = KnownColor.Peru, //136
        /// <see cref="KnownColor.Pink"/>
        /// <seealso cref="Color.Pink"/>
        [Description("#FFC0CB")]
        Pink = KnownColor.Pink, //137
        /// <see cref="KnownColor.Plum"/>
        /// <seealso cref="Color.Plum"/>
        [Description("#DDA0DD")]
        Plum = KnownColor.Plum, //138
        /// <see cref="KnownColor.PowderBlue"/>
        /// <seealso cref="Color.PowderBlue"/>
        [Description("#B0E0E6")]
        PowderBlue = KnownColor.PowderBlue, //139
        /// <see cref="KnownColor.Purple"/>
        /// <seealso cref="Color.Purple"/>
        [Description("#800080")]
        Purple = KnownColor.Purple, //140
        /// <see cref="KnownColor.Red"/>
        /// <seealso cref="Color.Red"/>
        [Description("#FF0000")]
        Red = KnownColor.Red, //141
        /// <see cref="KnownColor.RosyBrown"/>
        /// <seealso cref="Color.RosyBrown"/>
        [Description("#BC8F8F")]
        RosyBrown = KnownColor.RosyBrown, //142
        /// <see cref="KnownColor.RoyalBlue"/>
        /// <seealso cref="Color.RoyalBlue"/>
        [Description("#4169E1")]
        RoyalBlue = KnownColor.RoyalBlue, //143
        /// <see cref="KnownColor.SaddleBrown"/>
        /// <seealso cref="Color.SaddleBrown"/>
        [Description("#8B4513")]
        SaddleBrown = KnownColor.SaddleBrown, //144
        /// <see cref="KnownColor.Salmon"/>
        /// <seealso cref="Color.Salmon"/>
        [Description("#FA8072")]
        Salmon = KnownColor.Salmon, //145
        /// <see cref="KnownColor.SandyBrown"/>
        /// <seealso cref="Color.SandyBrown"/>
        [Description("#F4A460")]
        SandyBrown = KnownColor.SandyBrown, //146
        /// <see cref="KnownColor.SeaGreen"/>
        /// <seealso cref="Color.SeaGreen"/>
        [Description("#2E8B57")]
        SeaGreen = KnownColor.SeaGreen, //147
        /// <see cref="KnownColor.SeaShell"/>
        /// <seealso cref="Color.SeaShell"/>
        [Description("#FFF5EE")]
        SeaShell = KnownColor.SeaShell, //148
        /// <see cref="KnownColor.Sienna"/>
        /// <seealso cref="Color.Sienna"/>
        [Description("#A0522D")]
        Sienna = KnownColor.Sienna, //149
        /// <see cref="KnownColor.Silver"/>
        /// <seealso cref="Color.Silver"/>
        [Description("#C0C0C0")]
        Silver = KnownColor.Silver, //150
        /// <see cref="KnownColor.SkyBlue"/>
        /// <seealso cref="Color.SkyBlue"/>
        [Description("#87CEEB")]
        SkyBlue = KnownColor.SkyBlue, //151
        /// <see cref="KnownColor.SlateBlue"/>
        /// <seealso cref="Color.SlateBlue"/>
        [Description("#6A5ACD")]
        SlateBlue = KnownColor.SlateBlue, //152
        /// <see cref="KnownColor.SlateGray"/>
        /// <seealso cref="Color.SlateGray"/>
        [Description("#708090")]
        SlateGray = KnownColor.SlateGray, //153
        /// <see cref="KnownColor.Snow"/>
        /// <seealso cref="Color.Snow"/>
        [Description("#FFFAFA")]
        Snow = KnownColor.Snow, //154
        /// <see cref="KnownColor.SpringGreen"/>
        /// <seealso cref="Color.SpringGreen"/>
        [Description("#00FF7F")]
        SpringGreen = KnownColor.SpringGreen, //155
        /// <see cref="KnownColor.SteelBlue"/>
        /// <seealso cref="Color.SteelBlue"/>
        [Description("#4682B4")]
        SteelBlue = KnownColor.SteelBlue, //156
        /// <see cref="KnownColor.Tan"/>
        /// <seealso cref="Color.Tan"/>
        [Description("#D2B48C")]
        Tan = KnownColor.Tan, //157
        /// <see cref="KnownColor.Teal"/>
        /// <seealso cref="Color.Teal"/>
        [Description("#008080")]
        Teal = KnownColor.Teal, //158
        /// <see cref="KnownColor.Thistle"/>
        /// <seealso cref="Color.Thistle"/>
        [Description("#D8BFD8")]
        Thistle = KnownColor.Thistle, //159
        /// <see cref="KnownColor.Tomato"/>
        /// <seealso cref="Color.Tomato"/>
        [Description("#FF6347")]
        Tomato = KnownColor.Tomato, //160
        /// <see cref="KnownColor.Turquoise"/>
        /// <seealso cref="Color.Turquoise"/>
        [Description("#40E0D0")]
        Turquoise = KnownColor.Turquoise, //161
        /// <see cref="KnownColor.Violet"/>
        /// <seealso cref="Color.Violet"/>
        [Description("#EE82EE")]
        Violet = KnownColor.Violet, //162
        /// <see cref="KnownColor.Wheat"/>
        /// <seealso cref="Color.Wheat"/>
        [Description("#F5DEB3")]
        Wheat = KnownColor.Wheat, //163
        /// <see cref="KnownColor.White"/>
        /// <seealso cref="Color.White"/>
        [Description("#FFFFFF")]
        White = KnownColor.White, //164
        /// <see cref="KnownColor.WhiteSmoke"/>
        /// <seealso cref="Color.WhiteSmoke"/>
        [Description("#F5F5F5")]
        WhiteSmoke = KnownColor.WhiteSmoke, //165
        /// <see cref="KnownColor.Yellow"/>
        /// <seealso cref="Color.Yellow"/>
        [Description("#FFFF00")]
        Yellow = KnownColor.Yellow, //166
        /// <see cref="KnownColor.YellowGreen"/>
        /// <seealso cref="Color.YellowGreen"/>
        [Description("#9ACD32")]
        YellowGreen = KnownColor.YellowGreen, //167

        // System (UI) Colors
        ButtonFace = KnownColor.ButtonFace, //168
        ButtonHighlight = KnownColor.ButtonHighlight, //169
        ButtonShadow = KnownColor.ButtonShadow, //170
        GradientActiveCaption = KnownColor.GradientActiveCaption, //171
        GradientInactiveCaption = KnownColor.GradientInactiveCaption, //172
        MenuBar = KnownColor.MenuBar, //173
        MenuHighlight = KnownColor.MenuHighlight //174
    }
}
