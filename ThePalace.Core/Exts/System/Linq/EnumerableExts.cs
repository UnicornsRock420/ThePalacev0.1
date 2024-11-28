using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class EnumerableExts
    {
        public static class Types
        {
            public static readonly Type Enumerable = typeof(Enumerable);
        }

        //static EnumerableExts() { }

        public static T Coalesce<T>(this Array input) =>
            input.Cast<T>()
                .FirstOrDefault(v => v != null);

        public static T Coalesce<T>(this List<T> input) =>
            input.FirstOrDefault(v => v != null);

        public static T Coalesce<T>(this IList<T> input) =>
            input.FirstOrDefault(v => v != null);

        public static T Coalesce<T>(this IEnumerable<T> input) =>
            input.FirstOrDefault(v => v != null);
    }
}
