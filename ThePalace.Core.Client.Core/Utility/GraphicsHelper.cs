using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace ThePalace.Core.Client.Core.Utility
{
    public enum GHDrawCmds
    {
        DrawString,
        DrawArc,
        DrawEllipse,
        DrawLine,
        DrawPie,
        DrawPolygon,
        DrawPath,
        DrawRectangle,
        FillPolygon,
    }

    public sealed class GHDrawCmd
    {
        public GHDrawCmds Type { get; set; }
        public string Text { get; set; }
        public Font Font { get; set; }
        public Pen Pen { get; set; }
        public Brush Brush { get; set; }
        //public GraphicsPath Path { get; set; }
        public Rectangle? Rect { get; set; } = null;
        public List<float> Angles { get; set; } = new();
        public List<Point> Points { get; set; } = new();
    }

    public sealed class GraphicsHelper
    {
        private Graphics _g;

        private Point _location;
        private Brush _brush;
        private Font _font;
        private Pen _pen;

        private StringFormat _stringFormat = new StringFormat(StringFormatFlags.NoWrap);
        private GraphicsPath _graphicsPath = null;
        private List<GHDrawCmd> _drawCmds = new();
        private List<Point> _points = new();

        public GraphicsHelper(Graphics g)
        {
            _location = new Point(0, 0);
            _g = g;
        }

        public GraphicsHelper(Graphics g, Font font)
            : this(g)
        {
            _font = font;
        }

        public GraphicsHelper(Graphics g, Font font, Pen pen, Brush brush)
            : this(g, font)
        {
            _brush = brush;
            _pen = pen;
        }

        public GraphicsHelper(Graphics g, Font font, Pen pen, Brush brush, int x, int y)
            : this(g, font, pen, brush)
        {
            MoveTo(x, y);
        }

        public void SetFont(Font font)
        {
            _font = font;
        }

        public void SetPen(Pen pen)
        {
            _pen = pen;
        }

        public void SetBrush(Brush brush)
        {
            _brush = brush;
        }

        public GraphicsPath BeginPath() =>
            _graphicsPath = new GraphicsPath();

        public void MoveTo(int x, int y)
        {
            _location = new Point(x, y);

            //if (_graphicsPath != null)
            //    _points.Add(_location);
        }

        public void LineTo(int x, int y)
        {
            DrawLine(x, y);

            MoveTo(x, y);
        }

        public void QuadraticCurveTo(int cpx, int cpy, int x, int y)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawPath,
                    Points = new List<Point>
                    {
                        _location,
                        new Point(cpx, cpy),
                        new Point(cpx, cpy),
                        new Point(x, y),
                    },
                    Pen = _pen,
                });
            else
            {
                using (var path = new GraphicsPath())
                {
                    path.AddBeziers(
                        _location,
                        new Point(cpx, cpy),
                        new Point(cpx, cpy),
                        new Point(x, y));

                    _graphicsPath.AddPath(
                        path,
                        true);
                }
            }

            MoveTo(x, y);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            _pen = pen;

            DrawLine(x1, y1, x2, y2);
        }
        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawLine,
                    Pen = _pen,
                    Points = new List<Point>
                    {
                        new Point(x1, y1),
                        new Point(x2, y2),
                    },
                });
            else
                _graphicsPath.AddLine(
                    new Point(x1, y1),
                    new Point(x2, y2));
        }
        public void DrawLine(Pen pen, int x, int y)
        {
            _pen = pen;

            DrawLine(x, y);
        }
        public void DrawLine(int x, int y)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawLine,
                    Points = new List<Point>
                    {
                        _location,
                        new Point(x, y),
                    },
                    Pen = _pen,
                });
            else
            {
                //_points.Add(new Point(x, y));

                _graphicsPath.AddLine(
                    _location,
                    new Point(x, y));
            }
        }

        public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            _pen = pen;

            DrawArc(rect, startAngle, sweepAngle);
        }
        public void DrawArc(Rectangle rect, float startAngle, float sweepAngle)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawArc,
                    Rect = rect,
                    Pen = _pen,
                    Angles = new List<float>
                    {
                        startAngle,
                        sweepAngle,
                    },
                });
            else
                _graphicsPath.AddArc(
                    rect,
                    startAngle,
                    sweepAngle);
        }

        public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle)
        {
            _pen = pen;

            DrawPie(rect, startAngle, sweepAngle);
        }
        public void DrawPie(Rectangle rect, float startAngle, float sweepAngle)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawPie,
                    Rect = rect,
                    Pen = _pen,
                    Angles = new List<float>
                    {
                        startAngle,
                        sweepAngle,
                    },
                });
            else
                _graphicsPath.AddPie(
                    rect,
                    startAngle,
                    sweepAngle);
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            _pen = pen;

            DrawRectangle(rect);
        }
        public void DrawRectangle(Rectangle rect)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawRectangle,
                    Rect = rect,
                    Pen = _pen,
                });
            else
            {
                //_points.Add(new Point(x, y));

                _graphicsPath.AddRectangle(rect);
            }
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            _pen = pen;

            DrawEllipse(rect);
        }
        public void DrawEllipse(Rectangle rect)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawEllipse,
                    Rect = rect,
                    Pen = _pen,
                });
            else
                _graphicsPath.AddEllipse(rect);
        }

        public void DrawPolygon(Pen pen, params Point[] points)
        {
            _pen = pen;

            DrawPolygon(points);
        }
        public void DrawPolygon(params Point[] points)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawPolygon,
                    Points = points.ToList(),
                    Pen = _pen,
                });
            else
                _graphicsPath.AddPolygon(points);
        }

        public void DrawText(Font font, Brush brush, string text, int x, int y)
        {
            _brush = brush;
            _font = font;

            DrawText(text, x, y);
        }
        public void DrawText(string text, int x, int y)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawString,
                    Brush = _brush,
                    Font = _font,
                    Text = text,
                    Points = new List<Point>
                    {
                        new Point(x, y),
                    },
                });
            else
                _graphicsPath.AddString(
                    text,
                    new FontFamily(_font.Name),
                    (int)_font.Style,
                    _font.Size,
                    new Point(x, y),
                    _stringFormat);
        }
        public void DrawText(string text)
        {
            if (_graphicsPath == null)
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawString,
                    Brush = _brush,
                    Font = _font,
                    Text = text,
                    Points = new List<Point>
                    {
                        _location,
                    },
                });
            else
                _graphicsPath.AddString(
                    text,
                    new FontFamily(_font.Name),
                    (int)_font.Style,
                    _font.Size,
                    _location,
                    _stringFormat);
        }

        public void Fill(Brush brush)
        {
            _brush = brush;

            Fill();
        }
        public void Fill()
        {
            if (_points.Count > 0)
            {
                if (_graphicsPath != null)
                {
                    _graphicsPath.AddBeziers(_points.ToArray());
                }
                else
                    _drawCmds.Add(new GHDrawCmd
                    {
                        Type = GHDrawCmds.FillPolygon,
                        Points = _points.ToList(),
                        Brush = _brush,
                    });

                _points.Clear();
            }

            if (_graphicsPath != null)
            {
                _graphicsPath.CloseFigure();

                _g.FillPath(_brush, _graphicsPath);

                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
        }

        public void Stroke()
        {
            _graphicsPath?.CloseFigure();
            _graphicsPath?.Dispose();
            _graphicsPath = null;

            if (_points.Count > 0)
            {
                _drawCmds.Add(new GHDrawCmd
                {
                    Type = GHDrawCmds.DrawPath,
                    Points = _points.ToList(),
                    Pen = _pen,
                });

                _points.Clear();
            }

            if (_drawCmds.Count > 0)
                foreach (var drawCmd in _drawCmds)
                {
                    switch (drawCmd.Type)
                    {
                        case GHDrawCmds.DrawLine:
                            _g.DrawLine(drawCmd.Pen, drawCmd.Points[0], drawCmd.Points[1]);
                            break;
                        case GHDrawCmds.DrawArc:
                            _g.DrawArc(drawCmd.Pen, drawCmd.Rect.Value, drawCmd.Angles[0], drawCmd.Angles[1]);
                            break;
                        case GHDrawCmds.DrawPie:
                            _g.DrawPie(drawCmd.Pen, drawCmd.Rect.Value, drawCmd.Angles[0], drawCmd.Angles[1]);
                            break;
                        case GHDrawCmds.DrawPolygon:
                            _g.DrawPolygon(drawCmd.Pen, drawCmd.Points.ToArray());
                            break;
                        case GHDrawCmds.DrawString:
                            _g.DrawString(drawCmd.Text, drawCmd.Font, drawCmd.Brush, drawCmd.Points[0]);
                            break;
                        case GHDrawCmds.DrawEllipse:
                            _g.DrawEllipse(drawCmd.Pen, drawCmd.Rect.Value);
                            break;
                        case GHDrawCmds.DrawRectangle:
                            _g.DrawRectangle(drawCmd.Pen, drawCmd.Rect.Value);
                            break;
                        case GHDrawCmds.DrawPath:
                            {
                                using (var path = new GraphicsPath())
                                {
                                    path.AddBeziers(drawCmd.Points.ToArray());

                                    _g.DrawPath(drawCmd.Pen, path);
                                }
                            }
                            break;
                        case GHDrawCmds.FillPolygon:
                            _g.FillPolygon(drawCmd.Brush, drawCmd.Points.ToArray());
                            break;
                    }
                }
            _drawCmds.Clear();

            _g.Flush();
        }
    }
}
