using ICSharpCode.SharpZipLib.GZip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class ByteExts
    {
        public static class Types
        {
            public static readonly Type Byte = typeof(Byte);
            public static readonly Type ByteArray = typeof(Byte[]);
            public static readonly Type ByteList = typeof(List<Byte>);
        }

        //static ByteExts() { }

        public static byte[] EnsureBigEndian(this byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(input);
            return input;
        }

        public static string ToHex(this byte[] input)
        {
            var max = input.Length;
            var result = new StringBuilder(max * 2);
            for (var j = 0; j < max; j++)
                result.AppendFormat("{0:X2}", input[j]);
            return result.ToString();
        }

        public static string ToBase64(this byte[] input) =>
            (input?.Length ?? -1) < 1 ? null : Convert.ToBase64String(input);

        public static T FromBase64<T>(this byte[] input)
            where T : class
        {
            if ((input?.Length ?? -1) < 1) return null;

            return input.GetString()
                .FromBase64<T>();
        }

        public static string GZUncompress(this byte[] value)
        {
            using (var memInput = new MemoryStream(value))
            using (var zipInput = new GZipInputStream(memInput))
            using (var memOutput = new MemoryStream())
            {
                zipInput.CopyTo(memOutput);

                return memOutput.ToArray().GetString();
            }
        }

        public static uint FromInt31(this byte[] input, int startIndex = 0)
        {
            if ((input?.Length ?? -1) < sizeof(uint)) return 0;

            input[3] = input[3].ClearBit(7);
            return BitConverter.ToUInt32(input, startIndex);
        }

        public static string GetString(this IEnumerable<byte> input, int limit = 0, int offset = 0) =>
            input.ToArray()
                .GetString(limit, offset);
        public static string GetString(this byte[] input, int limit = 0, int offset = 0)
        {
            if ((input?.Length ?? -1) < 1) return string.Empty;

            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), nameof(limit) + " cannot be less than 0");
            else if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), nameof(offset) + " cannot be less than 0");
            else if ((offset + limit) > input.Length) throw new IndexOutOfRangeException(nameof(offset) + " is out of bounds of the source");

            if (limit < 1 || limit > input.Length)
                limit = input.Length;

            return string.Concat(input
                .Skip(offset)
                .Take(limit)
                .Select(b => (char)b));
        }
        //public static string GetString(this byte[] source, int limit = 0, int offset = 0)
        //{
        //    if (limit < 1 || limit > source.Length)
        //    {
        //        limit = source.Length;
        //    }

        //    var sb = new StringBuilder();

        //    for (var j = offset; j < limit; j++)
        //    {
        //        sb.Append((char)source[j]);
        //    }

        //    return sb.ToString();
        //}

        public static void AddRange(this List<byte> dest, byte[] source, int max = 0, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var range = new List<byte>();

            if (max > 0)
            {
                range = source
                    .Skip(offset)
                    .Take(max)
                    .ToList();
            }
            else
            {
                range = source
                    .Skip(offset)
                    .ToList();
            }

            dest.AddRange(range);
        }

        public static string ComputeMd5(this byte[] source)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(source);

                var sb = new StringBuilder();
                for (var j = 0; j < hashBytes.Length; j++)
                    sb.Append(hashBytes[j].ToString("X2"));

                return sb.ToString();
            }
        }
    }
}
