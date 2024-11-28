using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    #region Aliases
    using c = Convert;
    using db = Double;
    using i = Int32;
    using l = Int64;
    using o = Object;
    using s = String;
    using tc = TypeCodeEnum;
    using ts = TimeSpan;
    #endregion

    public static class StringExts
    {
        private const string STRING_KEY_CANNOT_BE_NULL = "Key cannot be null";
        private const char CHAR_UNDERSCORE = '_';
        private const char CHAR_PERIOD = '.';

        #region TryParse<T> Constants
        private delegate o d(s value);
        private static readonly char[] CHARARRAY_TIMESPAN_UNITS = null;
        private static readonly IReadOnlyDictionary<i, d[]> IREADONLYDICTIONARY_CONVERT_DELEGATES = null;
        private static readonly Dictionary<Type, Dictionary<s, o>> DICTIONARY_ENUM_TYPE_CONVERSION_CACHE = null;
        #endregion

        public static class Types
        {
            public static readonly Type String = typeof(string);
            public static readonly Type StringArray = typeof(string[]);
            public static readonly Type StringList = typeof(List<string>);
        }

        static StringExts()
        {
            #region TryParse<T> Static Constructor
            DICTIONARY_ENUM_TYPE_CONVERSION_CACHE = new Dictionary<Type, Dictionary<s, o>>();
            CHARARRAY_TIMESPAN_UNITS = "dhmstw".ToCharArray();
            IREADONLYDICTIONARY_CONVERT_DELEGATES =
                new Dictionary<i, d[]>
                {
                    {(i)tc.Char,new d[]{v=>char.Parse(v),v=>c.ToChar(v)}},
                    {(i)tc.Byte,new d[]{v=>byte.Parse(v),v=>c.ToByte(v)}},
                    {(i)tc.SByte,new d[]{v=>sbyte.Parse(v),v=>c.ToSByte(v)}},
                    {(i)tc.Int16,new d[]{v=>short.Parse(v),v=>c.ToInt16(v)}},
                    {(i)tc.UInt16,new d[]{v=>ushort.Parse(v),v=>c.ToUInt16(v)}},
                    {(i)tc.Int32,new d[]{v=>i.Parse(v),v=>c.ToInt32(v)}},
                    {(i)tc.UInt32,new d[]{v=>uint.Parse(v),v=>c.ToUInt32(v)}},
                    {(i)tc.Int64,new d[]{v=>l.Parse(v),v=>c.ToInt64(v)}},
                    {(i)tc.UInt64,new d[]{v=>ulong.Parse(v),v=>c.ToUInt64(v)}},
                    {(i)tc.Single,new d[]{v=>float.Parse(v),v=>c.ToSingle(v)}},
                    {(i)tc.Double,new d[]{v=>db.Parse(v),v=>c.ToDouble(v)}},
                    {(i)tc.Decimal,new d[]{v=>decimal.Parse(v),v=>c.ToDecimal(v)}},
                    {(i)tc.Boolean,new d[]{v=>bool.Parse(v),v=>c.ToBoolean(v)}},
                    {(i)tc.DateTime,new d[]{v=>DateTime.Parse(v),v=>c.ToDateTime(v)}},
                    {(i)tc.Guid,new d[]{v=>Guid.Parse(v)}},
                    {(i)tc.TimeSpan,new d[]{v=>ts.Parse(v)}},
                }.IReadOnlyDictionary();
            #endregion
        }

        #region TryParse<T> Static Methods
        public static T TryParse<T>(this s value, T defaultValue = default(T), s format = null)
        {
            if (s.IsNullOrWhiteSpace(value)) return defaultValue;
            var type = typeof(T);
            var typeID = type.TypeID();
            switch (typeID)
            {
                case (i)tc.String: return value.CastType<T>();
                case (i)tc.Enum:
                    if (Enum.TryParse(type, value, true, out var enumValue)) return enumValue.CastType<T>();
                    else if (!DICTIONARY_ENUM_TYPE_CONVERSION_CACHE.ContainsKey(type))
                    {
                        var enumValues = Enum
                            .GetValues(type)
                            .Cast<T>()
                            .Where(v => v.ToString() != v.GetDescription())
                            .Distinct()
                            .ToDictionary(
                                v => v.GetDescription(),
                                v => (o)v);
                        if (enumValues.Count < 1) return defaultValue;
                        else DICTIONARY_ENUM_TYPE_CONVERSION_CACHE.Add(type, enumValues);
                    }
                    if (!DICTIONARY_ENUM_TYPE_CONVERSION_CACHE[type].ContainsKey(value)) return defaultValue;
                    else return DICTIONARY_ENUM_TYPE_CONVERSION_CACHE[type][value].CastType<T>();
                case (i)tc.DateTime:
                    if (!s.IsNullOrWhiteSpace(format) &&
                        DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var datetimeValue)) return datetimeValue.CastType<T>();
                    else goto default;
                case (i)tc.TimeSpan:
                    var chars = value.Trim().ToLowerInvariant().ToCharArray();
                    var cLength = chars.Length;
                    if (cLength < 2) goto default;
                    var units = chars[(cLength - 2)..].Where(c => CHARARRAY_TIMESPAN_UNITS.Contains(c)).GetString();
                    var uLength = units.Length;
                    if (uLength < 1) goto default;
                    var units0 = units[0];
                    value = chars[0..(cLength - 1)].TakeWhile(c => (units0 != 't' && c == '.') || char.IsDigit(c)).GetString();
                    if (units0 == 't' && l.TryParse(value, out var longValue)) return ts.FromTicks(longValue).CastType<T>();
                    if (db.TryParse(value, out var doubleValue))
                        switch (units0)
                        {
                            case 'd': return ts.FromDays(doubleValue).CastType<T>();
                            case 'h': return ts.FromHours(doubleValue).CastType<T>();
                            case 'm':
                                if (uLength > 1 && units[1] == 's') return ts.FromMilliseconds(doubleValue).CastType<T>();
                                else return ts.FromMinutes(doubleValue).CastType<T>();
                            case 's': return ts.FromSeconds(doubleValue).CastType<T>();
                            case 'w': return ts.FromDays(doubleValue * 7).CastType<T>();
                        }
                    goto default;
                default:
                    if (IREADONLYDICTIONARY_CONVERT_DELEGATES.ContainsKey(typeID)) foreach (var func in IREADONLYDICTIONARY_CONVERT_DELEGATES[typeID]) try { return func(value).CastType<T>(); } catch { }
                    try { return c.ChangeType(value, type, CultureInfo.InvariantCulture).CastType<T>(); } catch { }
                    return defaultValue;
            }
        }
        #endregion

        public static string Format(this string format, params object[] args) => string.Format(format, args);
        public static string Join(this string[] values, char delimiter) => string.Join(delimiter, values);
        public static string Join(this string[] values, string delimiter = "") => string.Join(delimiter, values);

        public static string SanitizeKey(this string key, bool toLower = false, bool toUpper = false)
        {
            key = key?.Trim();

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), STRING_KEY_CANNOT_BE_NULL);

            key = string.Concat(key
                .ToCharArray()
                .Where(c => char.IsLetterOrDigit(c) ||
                    c == CHAR_UNDERSCORE ||
                    c == CHAR_PERIOD)
                .ToArray());

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), STRING_KEY_CANNOT_BE_NULL);

            if (toLower) key = key.ToLowerInvariant();
            else if (toUpper) key = key.ToUpperInvariant();

            return key;
        }

        public static byte[] GZCompress(this string value)
        {
            var buffer = value.GetBytes();

            using (var memOutput = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memOutput, CompressionMode.Compress, true))
                    gZipStream.Write(buffer, 0, buffer.Length);

                memOutput.Position = 0;

                //var compressedData = new byte[memOutput.Length];
                //memOutput.Read(compressedData, 0, compressedData.Length);
                //return compressedData;

                return memOutput.ToArray();
            }
        }

        public static byte[] GetBytes(this string value, int limit = 0, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(value)) return new byte[0];
            else if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit), nameof(limit) + " cannot be less than 0");
            else if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), nameof(offset) + " cannot be less than 0");
            else if ((offset + limit) > value.Length) throw new ArgumentOutOfRangeException(nameof(offset), nameof(offset) + " is out of bounds of the " + nameof(value));

            if (limit < 1 || limit > value.Length)
                limit = value.Length;

            return value
                .ToCharArray()
                .Skip(offset)
                .Take(limit)
                .Select(c => (byte)c)
                .ToArray();
        }
        //public static byte[] GetBytes(this string source, int limit = 0, int offset = 0)
        //{
        //    if (limit < 1)
        //    {
        //        limit = source.Length;
        //    }

        //    var result = new byte[source.Length];

        //    for (var j = offset; j < limit; j++)
        //    {
        //        result[j] = (byte)source[j];
        //    }

        //    return result;
        //}

        public static string GetEmbeddedResource(this string name)
        {
            var assembly = Assembly.GetCallingAssembly();
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public static string ToHex(this string input)
        {
            var bytes = input.GetBytes();
            var max = bytes.Length;
            var result = new StringBuilder(max * 2);
            for (var j = 0; j < max; j++)
                result.AppendFormat("{0:X2}", bytes[j]);
            return result.ToString();
        }

        public static string ToBase64(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            return input.GetBytes()
                .ToBase64();
        }

        public static T FromBase64<T>(this string input)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var type = typeof(T);
            var bytes = c.FromBase64String(input);
            if (type == ByteExts.Types.ByteArray) return (T)(object)bytes;
            else if (type == Types.String) return (T)(object)bytes.GetString();
            else return null;
        }

        public static string Substr(this string input, int startIndex, int endIndex)
        {
            return input.Substring(startIndex, endIndex - startIndex);
        }

        public static string ComputeMd5(this string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);

            return inputBytes.ComputeMd5();
        }
    }
}
