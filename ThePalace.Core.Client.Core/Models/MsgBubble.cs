using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Utility;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Models
{
    public sealed class MsgBubble : IDisposable
    {
        private const int maxWidth = 150;
        private const int minWidth = 20;

        public Bitmap Image { get; private set; } = null;

        public BubbleTypes Type { get; private set; } = BubbleTypes.Normal;
        public DateTime Created { get; private set; } = DateTime.Now;
        private DateTime? _accessed = null;
        public DateTime Accessed
        {
            get
            {
                if (!this._accessed.HasValue)
                    this._accessed = DateTime.Now;

                return this._accessed.Value;
            }
        }
        public bool Visible { get; private set; } = true;
        public Color Colour { get; private set; } = Color.White;
        public Point Origin { get; private set; } = new(0, 0);
        public Point Location { get; set; } = new(0, 0);

        public int Duration { get; private set; } = 0;
        public string OriginalText { get; private set; } = null;
        public string[] Text { get; private set; } = null;
        public string[] MediaFilenames { get; private set; } = null;
        public Size TextSize = new();

        public MsgBubble(string text) =>
            Parse(text);
        public MsgBubble(string text, int duration) =>
            Parse(text, duration);
        public MsgBubble(short colorNbr, string text)
        {
            Colour = UIConstants.NbrToColor(colorNbr);
            Parse(text);
        }
        public MsgBubble(short colorNbr, string text, int duration)
        {
            Colour = UIConstants.NbrToColor(colorNbr);
            Parse(text, duration);
        }
        public MsgBubble(Color colour, string text)
        {
            Colour = colour;
            Parse(text);
        }
        public MsgBubble(Color colour, string text, int duration)
        {
            Colour = colour;
            Parse(text, duration);
        }

        public void Dispose()
        {
            try { this.Image?.Dispose(); } catch { }

            Text = null;
            MediaFilenames = null;
        }

        private void Parse(string text, int duration = 0)
        {
            OriginalText = text;

            #region Regex Matching & Formatting

            if (UIConstants.REGEX_VISIBLE.IsMatch(text))
            {
                Visible = false;

                return;
            }
            if (UIConstants.REGEX_TYPE.IsMatch(text))
            {
                var match = UIConstants.REGEX_TYPE.Match(text);
                text = match.Groups[2].Value;

                if (Type == BubbleTypes.Normal)
                    switch (match.Groups[1].Value[0])
                    {
                        case '!':
                            Type = BubbleTypes.Shout;
                            break;
                        case ':':
                            Type = BubbleTypes.Thought;
                            break;
                        case '^':
                            Type = BubbleTypes.Sticky;
                            break;
                    }
            }
            if (UIConstants.REGEX_LOCATION.IsMatch(text))
            {
                var match = UIConstants.REGEX_LOCATION.Match(text);
                var x = Convert.ToInt32(match.Groups[1].Value);
                var y = Convert.ToInt32(match.Groups[2].Value);
                text = match.Groups[3].Value;

                Origin = new Point(x, y);
            }
            if (UIConstants.REGEX_TYPE.IsMatch(text))
            {
                var match = UIConstants.REGEX_TYPE.Match(text);
                text = match.Groups[2].Value;

                if (Type == BubbleTypes.Normal)
                    switch (match.Groups[1].Value[0])
                    {
                        case '!':
                            Type = BubbleTypes.Shout;
                            break;
                        case ':':
                            Type = BubbleTypes.Thought;
                            break;
                        case '^':
                            Type = BubbleTypes.Sticky;
                            break;
                    }
            }
            if (UIConstants.REGEX_SOUND.IsMatch(text))
            {
                var mediaFilenames = new List<string>();
                while (UIConstants.REGEX_SOUND.IsMatch(text))
                {
                    var match = UIConstants.REGEX_SOUND.Match(text);
                    text = match.Groups[2].Value;

                    mediaFilenames.Add(match.Groups[1].Value);
                }
                MediaFilenames = mediaFilenames.ToArray();
            }
            if (UIConstants.REGEX_TYPE.IsMatch(text))
            {
                var match = UIConstants.REGEX_TYPE.Match(text);
                text = match.Groups[2].Value;

                if (Type == BubbleTypes.Normal)
                    switch (match.Groups[1].Value[0])
                    {
                        case '!':
                            Type = BubbleTypes.Shout;
                            break;
                        case ':':
                            Type = BubbleTypes.Thought;
                            break;
                        case '^':
                            Type = BubbleTypes.Sticky;
                            break;
                    }
            }

            #endregion
            #region Word(s) & Line Formatting

            var words = ClientConstants.REGEX_WHITESPACE.Split(text).ToList();
            var lines = new List<string>();
            var line = new List<string>();

            while (words.Count > 0)
            {
                var newLine = line.Join(" ", words[0]);
                var newLineSize = TextRenderer.MeasureText(newLine, UIConstants.FONT_DEFAULT);

                if (newLineSize.Width < maxWidth)
                    line.Enqueue(words.Dequeue());

                if (newLineSize.Width >= maxWidth ||
                    words.Count < 1)
                {
                    newLine = line.Join(" ");
                    newLineSize = TextRenderer.MeasureText(newLine, UIConstants.FONT_DEFAULT);

                    if (newLineSize.Width > TextSize.Width)
                        TextSize.Width = newLineSize.Width;

                    lines.Add(newLine);
                    line.Clear();
                }
            }

            Text = lines.ToArray();
            Duration = duration > 0 ? duration : Text.Join(" ").ToCharArray().Length * 500;

            //TextSize.Width += -28; // 16;
            TextSize.Height = (Text.Length * (UIConstants.FONT_DEFAULT.Height - 5)) + 2;

            #endregion
        }

        public Bitmap Render()
        {
            if (Text.Length < 1) return null;

            //var px = (short)this.Origin;
            //var py = (short)this.Origin;

            var offsetX = (short)11;
            var offsetY = (short)11;

            var w = new int[] { minWidth, TextSize.Width }.Max();
            var h = TextSize.Height;

            var x = (short)(this.Location.X + offsetX);
            var y = (short)(this.Location.Y + offsetY);

            //if (sessionState.ScreenWidth < r)
            //{
            //    offsetX = -11;
            //    x -= (short)(w * 1.3);
            //    r = (short)(x + w);
            //}

            //if (sessionState.ScreenHeight < b)
            //{
            //    offsetY = -11;
            //    y -= (short)(h * 1.3);
            //    b = (short)(y + h);
            //}

            var imagePadding = 50;
            this.Image = new Bitmap(TextSize.Width + imagePadding, TextSize.Height + imagePadding);

            using (var g = Graphics.FromImage(this.Image))
            using (var colourBrush = new SolidBrush(Colour))
            using (var colourPen = new Pen(colourBrush, 2))
            {
                var helper = new GraphicsHelper(g, UIConstants.FONT_DEFAULT, colourPen, colourBrush);

                switch (this.Type)
                {
                    case BubbleTypes.Normal: Render_Normal(helper, w, h, x, y); break;
                    case BubbleTypes.Shout: Render_Shout(helper, w, h, x, y); break;
                    case BubbleTypes.Sticky: Render_Sticky(helper, w, h, x, y); break;
                    case BubbleTypes.Thought: Render_Thought2(helper, w, h, x, y); break;
                }

                helper.SetBrush(Brushes.Black);
                helper.BeginPath();

                for (var k = 0; k < Text.Length; k++)
                    helper.DrawText(Text[k], offsetX + x + 6, Math.Abs(offsetY > 0 ? offsetY : 11) + y - 6 + k * 16); //8, 5

                helper.Fill();
                helper.Stroke();

                return this.Image;
            }
        }

        private void Render_Normal(GraphicsHelper helper, int w, int h, short x, short y)
        {
            var px = (short)this.Origin.X;
            var py = (short)this.Origin.Y;
            var r = (short)(x + w);
            var b = (short)(y + h);
            var con1 = (short)0;
            var con2 = (short)0;
            var radius = 18;

            if (py < y || py > y + h)
            {
                con1 = (short)Math.Min(Math.Max(x + radius, px - 10), r - radius - 20);
                con2 = (short)Math.Min(Math.Max(x + radius + 20, px + 10), r - radius);
            }
            else
            {
                con1 = (short)Math.Min(Math.Max(y + radius, py - 10), b - radius - 20);
                con2 = (short)Math.Min(Math.Max(y + radius + 20, py + 10), b - radius);
            }

            var dir = (short)0;
            if (py < y) dir = 2;
            if (py > y) dir = 3;
            if (px < x && py >= y && py <= b) dir = 0;
            if (px > x && py >= y && py <= b) dir = 1;
            if (px >= x && px <= r && py >= y && py <= b) dir = -1;

            px = 0;
            py = 0;

            helper.BeginPath();

            helper.MoveTo(x + radius, y);

            if (dir == 2)
            {
                helper.LineTo(con1, y);
                helper.LineTo(px, py);
                helper.LineTo(con2, y);
            }

            helper.LineTo(r - radius, y);
            helper.QuadraticCurveTo(r, y, r, y + radius);

            if (dir == 1)
            {
                helper.LineTo(r, con1);
                helper.LineTo(px, py);
                helper.LineTo(r, con2);
            }

            helper.LineTo(r, b - radius);
            helper.QuadraticCurveTo(r, b, r - radius, b);

            if (dir == 3)
            {
                helper.LineTo(con2, b);
                helper.LineTo(px, py);
                helper.LineTo(con1, b);
            }

            helper.LineTo(x + radius, b);
            helper.QuadraticCurveTo(x, b, x, b - radius);

            if (dir == 0)
            {
                helper.LineTo(x, con2);
                helper.LineTo(px, py);
                helper.LineTo(x, con1);
            }

            helper.LineTo(x, y + radius);
            helper.QuadraticCurveTo(x, y, x + radius, y);

            helper.Fill();
            helper.Stroke();
        }
        private void Render_Shout(GraphicsHelper helper, int w, int h, short x, short y)
        {
            var SpikeDisplace = (short)20;
            var SpikeHeight = (short)16;
            var SpikeWidth = (short)12;
            var r = (short)(x + w);
            var b = (short)(y + h);

            var rect =
                new Rectangle(
                    x, y,
                    r, b);

            var hp = (rect.Width - rect.X) / SpikeWidth;
            if (hp < 2) hp = 2;
            var vp = (rect.Height - rect.Y) / SpikeHeight;
            if (vp < 2) vp = 2;
            var hw = (rect.Width - rect.X) / hp;
            var vw = (rect.Height - rect.Y) / vp;

            var px = (short)0;
            var py = (short)0;
            var _px = (short)0;
            var _py = (short)0;

            helper.BeginPath();

            helper.MoveTo((short)rect.X, (short)rect.Y);

            for (var tx = 0; tx < hp; ++tx)
            {
                px = (short)(rect.X + tx * hw + hw / 2 + tx * SpikeDisplace / (hp - 1) - SpikeDisplace / 2);
                py = (short)(rect.Y - SpikeHeight);

                _px = (short)(rect.X + tx * hw + hw);
                _py = (short)rect.Y;

                helper.LineTo(px, py);
                helper.LineTo(_px, _py);
            }

            for (var ty = 0; ty < vp; ++ty)
            {
                px = (short)(rect.Width + SpikeHeight);
                py = (short)(rect.Y + ty * vw + vw / 2 + ty * SpikeDisplace / (vp - 1) - SpikeDisplace / 2);

                _px = (short)rect.Width;
                _py = (short)(rect.Y + ty * vw + vw);

                helper.LineTo(px, py);
                helper.LineTo(_px, _py);
            }

            for (var tx = 0; tx < hp; ++tx)
            {
                px = (short)(rect.Width - tx * hw - hw / 2 + (hp - 1 - tx) * SpikeDisplace / (hp - 1) - SpikeDisplace / 2);
                py = (short)(rect.Height + SpikeHeight);

                _px = (short)(rect.Width - tx * hw - hw);
                _py = (short)rect.Height;

                helper.LineTo(px, py);
                helper.LineTo(_px, _py);
            }

            for (var ty = 0; ty < vp; ++ty)
            {
                px = (short)(rect.X - SpikeHeight);
                py = (short)(rect.Height - ty * vw - vw / 2 + (vp - 1 - ty) * SpikeDisplace / (vp - 1) - SpikeDisplace / 2);

                _px = (short)rect.X;
                _py = (short)(rect.Height - ty * vw - vw);

                helper.LineTo(px, py);
                helper.LineTo(_px, _py);
            }

            helper.LineTo((short)rect.X, (short)rect.Y);

            helper.Fill();
            helper.Stroke();
        }
        private void Render_Sticky(GraphicsHelper helper, int w, int h, short x, short y)
        {
            var r = (short)(x + w);
            var b = (short)(y + h);
            var radius = 1;

            helper.BeginPath();

            helper.MoveTo(x + radius, y);

            helper.LineTo(r - radius, y);
            helper.QuadraticCurveTo(r, y, r, y + radius);
            helper.LineTo(r, y + h - radius);
            helper.QuadraticCurveTo(r, b, r - radius, b);
            helper.LineTo(x + radius, b);
            helper.QuadraticCurveTo(x, b, x, b - radius);
            helper.LineTo(x, y + radius);
            helper.QuadraticCurveTo(x, y, x + radius, y);

            helper.Fill();
            helper.Stroke();
        }
        private void Render_Thought(GraphicsHelper helper, int w, int h, short x, short y)
        {
            var r = (short)(x + w);
            var b = (short)(y + h);

            var px = (short)0;
            var py = (short)0;
            var radius = 4;

            var dir = (short)0;
            if (py < y) dir = 2;
            if (py > y) dir = 3;
            if (px < x && py >= y && py <= b) dir = 0;
            if (px > x && py >= y && py <= b) dir = 1;
            if (px >= x && px <= r && py >= y && py <= b) dir = -1;

            dir = (short)RndGenerator.Next(0, 4);
            switch (dir)
            {
                case 2:
                    radius = 4;
                    px = (short)(x - radius * 2.5);
                    py = (short)(y - radius);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    radius = 8;
                    px = (short)(x - radius);
                    py = y;

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    break;
                case 1:
                    radius = 4;
                    px = (short)(x - 4);
                    py = (short)(b + radius * 2.5);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    radius = 8;
                    px = x;
                    py = (short)(b + 2);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    break;
                case 3:
                    radius = 4;
                    px = (short)(r + 4);
                    py = (short)(b + radius * 2.5);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    radius = 8;
                    px = (short)(r - 4);
                    py = (short)(b + 2);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    break;
                case 0:
                    radius = 4;
                    px = (short)(r + (radius * 2.5));
                    py = (short)(y - radius);

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    radius = 8;
                    px = r;
                    py = y;

                    helper.BeginPath();
                    helper.DrawEllipse(
                        new Rectangle(
                            px, py,
                            radius,
                            radius));
                    helper.Fill();
                    helper.Stroke();

                    break;
            }

            radius = 24;
            helper.BeginPath();

            helper.MoveTo(x + radius, y);

            helper.LineTo(r - radius, y);
            helper.QuadraticCurveTo(r, y, r, y + radius);
            helper.LineTo(r, y + h - radius);
            helper.QuadraticCurveTo(r, b, r - radius, b);
            helper.LineTo(x + radius, b);
            helper.QuadraticCurveTo(x, b, x, b - radius);
            helper.LineTo(x, y + radius);
            helper.QuadraticCurveTo(x, y, x + radius, y);

            helper.Fill();
            helper.Stroke();
        }
        private void Render_Thought2(GraphicsHelper helper, int w, int h, short x, short y)
        {
            var r = (short)(x + w);
            var b = (short)(y + h);
            //var px = (short)0;
            //var py = (short)0;

            var xMax = TextSize.Width / 14;
            var yMax = TextSize.Height / 13;
            var radius = (short)0;
            var rMin = 12;
            var rMax = 15;

            helper.BeginPath();

            for (var j = 0; j <= yMax; j++)
            {
                radius = (short)RndGenerator.Next(rMin, rMax);

                var ofst = rMax - radius + 1;

                helper.DrawArc(
                    new Rectangle(
                        x - ofst,
                        y,
                        radius,
                        radius),
                        90F,
                        180F);

                if (j < yMax)
                    y += radius;
            }

            helper.DrawArc(
                new Rectangle(
                    x,
                    y,
                    radius,
                    radius),
                    0F,
                    180F);

            for (var j = 0; j <= xMax; j++)
            {
                radius = (short)RndGenerator.Next(rMin, rMax);

                var ofst = rMax - radius + 1;

                helper.DrawArc(
                    new Rectangle(
                        x,
                        y + ofst,
                        radius,
                        radius),
                        0F,
                        180F);

                if (j < xMax)
                    x += radius;
            }

            helper.DrawArc(
                new Rectangle(
                    x,
                    y,
                    radius,
                    radius),
                    270F,
                    180F);

            for (var j = 0; j <= yMax; j++)
            {
                radius = (short)RndGenerator.Next(rMin, rMax);

                var ofst = rMax - radius + 1;

                helper.DrawArc(
                    new Rectangle(
                        x + ofst,
                        y,
                        radius,
                        radius),
                        270F,
                        180F);

                if (j < yMax)
                    y -= radius;
            }

            helper.DrawArc(
                new Rectangle(
                    x,
                    y,
                    radius,
                    radius),
                    180F,
                    180F);

            for (var j = 0; j <= xMax; j++)
            {
                radius = (short)RndGenerator.Next(rMin, rMax);

                var ofst = rMax - radius + 1;

                helper.DrawArc(
                    new Rectangle(
                        x,
                        y - ofst,
                        radius,
                        radius),
                        180F,
                        180F);

                if (j < xMax)
                    x -= radius;
            }

            //helper.DrawArc(
            //    new Rectangle(
            //        x,
            //        y,
            //        radius,
            //        radius),
            //        90F,
            //        180F);

            helper.Fill();
            helper.Stroke();
        }
    }
}
