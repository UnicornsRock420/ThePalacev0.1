using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ThePalace.Core.ExtensionMethods
{
    public static class PalaceExts
    {
        public static ushort SwapInt16(this ushort source)
        {
            return (ushort)(
                (source & 0x00FF) << 8 |
                (source & 0xFF00) >> 8);
        }

        public static uint SwapInt32(this uint source)
        {
            return
                (source & 0x000000FF) << 24 |
                (source & 0x0000FF00) << 8 |
                (source & 0x00FF0000) >> 8 |
                (source & 0xFF000000) >> 24;
        }

        public static byte[] WriteInt16(this short source)
        {
            return BitConverter.GetBytes(source);
        }

        public static byte[] WriteInt32(this int source)
        {
            return BitConverter.GetBytes(source);
        }

        public static byte[] WriteInt16(this ushort source)
        {
            return BitConverter.GetBytes(source);
        }

        public static byte[] WriteInt32(this uint source)
        {
            return BitConverter.GetBytes(source);
        }

        public static short ReadSInt16(this List<byte> source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 1 > source.Count)
            {
                return 0;
            }

            return BitConverter.ToInt16(source.Skip(offset).Take(2).ToArray());
        }

        public static int ReadSInt32(this List<byte> source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 3 > source.Count)
            {
                return 0;
            }

            return BitConverter.ToInt32(source.Skip(offset).Take(4).ToArray());
        }

        public static ushort ReadUInt16(this List<byte> source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 1 > source.Count)
            {
                return 0;
            }

            return BitConverter.ToUInt16(source.Skip(offset).Take(2).ToArray());
        }

        public static uint ReadUInt32(this List<byte> source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 3 > source.Count)
            {
                return 0;
            }

            return BitConverter.ToUInt32(source.Skip(offset).Take(4).ToArray());
        }

        public static short ReadSInt16(this byte[] source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 1 > source.Length)
            {
                return 0;
            }

            return BitConverter.ToInt16(source, offset);
        }

        public static int ReadSInt32(this byte[] source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 3 > source.Length)
            {
                return 0;
            }

            return BitConverter.ToInt32(source, offset);
        }

        public static ushort ReadUInt16(this byte[] source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 1 > source.Length)
            {
                return 0;
            }

            return BitConverter.ToUInt16(source, offset);
        }

        public static uint ReadUInt32(this byte[] source, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (offset + 3 > source.Length)
            {
                return 0;
            }

            return BitConverter.ToUInt32(source, offset);
        }

        public static string ReadPString(this byte[] source, int max, int offset = 0, int size = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (size < 1)
            {
                size = 1;
            }

            var length = 0;

            switch (size)
            {
                case 4:
                    length = source.ReadSInt32(offset);

                    break;
                case 2:
                    length = source.ReadSInt16(offset);

                    break;
                default:
                    length = source[offset];
                    size = 1;

                    break;
            }

            if (length > max)
            {
                length = max;
            }

            return source
                .ToList()
                .Skip(size + offset)
                .Take(length)
                .ToArray()
                .GetString();
        }

        public static string ReadCString(this byte[] source, int offset)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var length = source
                .Skip(offset)
                .ToList()
                .IndexOf(0);

            return source
                .ToList()
                .Skip(offset)
                .Take(length - 1)
                .ToArray()
                .GetString();
        }

        public static byte[] WritePString(this string source, int max, int size, bool padding = true)
        {
            var data = new List<byte>();

            if (size < 1)
            {
                size = 1;
            }

            var length = source.Length;

            if (length >= max - size)
            {
                length = max - size;
            }

            switch (size)
            {
                case 4:
                    data.AddRange(length.WriteInt32());

                    break;
                case 2:
                    data.AddRange(((short)length).WriteInt16());

                    break;
                default:
                    data.Add((byte)length);
                    size = 1;

                    break;
            }

            data.AddRange(source.GetBytes());

            if (padding)
            {
                for (var j = size + length; j < max; j++)
                {
                    data.Add(0);
                }
            }

            return data.ToArray();
        }

        public static byte[] WriteCString(this string source)
        {
            var data = new List<byte>();

            data.AddRange(source.GetBytes());
            data.Add(0);

            return data.ToArray();
        }

        public static bool IsPointInPolygon(this PointF point, List<PointF> polygon)
        {
            return point.IsPointInPolygon(polygon.ToArray());
        }
        public static bool IsPointInPolygon(this PointF point, params PointF[] polygon)
        {
            if (polygon.Length < 3) return false;

            var i = 0;
            var j = 0;

            var result = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                if (polygon[i].Y > point.Y != polygon[j].Y > point.Y && point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                    result = !result;
            return result;
        }

        public static bool IsPointInPolygon(this Point point, List<Point> polygon)
        {
            return point.IsPointInPolygon(polygon.ToArray());
        }
        public static bool IsPointInPolygon(this Point point, params Point[] polygon)
        {
            if (polygon.Length < 3) return false;

            var i = 0;
            var j = 0;

            var result = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                if (polygon[i].Y > point.Y != polygon[j].Y > point.Y && point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                    result = !result;
            return result;
        }

        public static bool IsPointInPolygon(this Models.Palace.Point point, List<Models.Palace.Point> polygon)
        {
            return point.IsPointInPolygon(polygon.ToArray());
        }
        public static bool IsPointInPolygon(this Models.Palace.Point point, params Models.Palace.Point[] polygon)
        {
            if (polygon.Length < 3) return false;

            var i = 0;
            var j = 0;

            var inside = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                if (polygon[i].v > point.v != polygon[j].v > point.v && point.h < (polygon[j].h - polygon[i].h) * (point.v - polygon[i].v) / (polygon[j].v - polygon[i].v) + polygon[i].h)
                    inside = !inside;
            return inside;
        }

        public static Models.Palace.Point[] GetBoundingBox(this Models.Palace.Point point, int width, int height, bool centered = false)
        {
            return point.GetBoundingBox(new Size(width, height), centered);
        }
        public static Models.Palace.Point[] GetBoundingBox(this Models.Palace.Point point, Size size, bool centered = false)
        {
            var results = new List<Models.Palace.Point>();
            var w = (short)(centered ? size.Width / 2 : size.Width);
            var h = (short)(centered ? size.Height / 2 : size.Height);

            if (centered)
                results.Add(new Models.Palace.Point((short)(point.h - w), (short)(point.v - h)));
            else
                results.Add(point);

            if (centered)
                results.Add(new Models.Palace.Point((short)(point.h + w), (short)(point.v - h)));
            else
                results.Add(new Models.Palace.Point((short)(point.h + w), point.v));

            results.Add(new Models.Palace.Point((short)(point.h + w), (short)(point.v + h)));

            if (centered)
                results.Add(new Models.Palace.Point((short)(point.h - w), (short)(point.v + h)));
            else
                results.Add(new Models.Palace.Point(point.h, (short)(point.v + h)));

            return results.ToArray();
        }
    }
}
