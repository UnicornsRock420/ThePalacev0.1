using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class CharExts
    {
        public static class Types
        {
            public static readonly Type Char = typeof(Char);
            public static readonly Type CharArray = typeof(Char[]);
            public static readonly Type CharList = typeof(List<Char>);
        }

        //static CharExts() { }

        public static string GetString(this IEnumerable<char> data, int limit = 0, int offset = 0) =>
            data.ToArray().GetString(limit, offset);
        public static string GetString(this char[] data, int limit = 0, int offset = 0)
        {
            if ((data?.Length ?? -1) < 1) return string.Empty;

            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), nameof(limit) + " cannot be less than 0");
            else if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), nameof(offset) + " cannot be less than 0");
            else if ((offset + limit) > data.Length) throw new IndexOutOfRangeException(nameof(offset) + " is out of bounds of the source");

            if (limit < 1 || limit > data.Length)
                limit = data.Length;

            return string.Concat(data
                .Skip(offset)
                .Take(limit));
        }
    }
}
