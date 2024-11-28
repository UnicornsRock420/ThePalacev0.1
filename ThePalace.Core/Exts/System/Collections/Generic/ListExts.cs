using System.Linq;

namespace System.Collections.Generic
{
    public static class ListExts
    {
        public static class Types
        {
            public static readonly Type ListGeneric = typeof(List<>);
        }

        //static ListExts() { }

        public static T PeekF<T>(this List<T> source) =>
            source.FirstOrDefault();

        public static T PeekL<T>(this List<T> source) =>
            source.LastOrDefault();

        public static void Push<T>(this List<T> source, T item) =>
            source.Add(item);

        public static void Push<T>(this List<T> source, T[] item) =>
            source.AddRange(item);

        public static T Pop<T>(this List<T> source)
        {
            var last = source.LastOrDefault();
            if (last == null) return default;

            source.RemoveAt(source.Count - 1);
            return last;
        }

        public static void Enqueue<T>(this List<T> source, T item) =>
            source.Add(item);

        public static void Enqueue<T>(this List<T> source, T[] item) =>
            source.AddRange(item);

        public static T Dequeue<T>(this List<T> source)
        {
            var first = source.FirstOrDefault();
            if (first == null) return default;

            source.RemoveAt(0);
            return first;
        }

        public static string Join(this List<string> source, string separator, params string[] additionalItems)
        {
            var list = source.ToList();

            if (additionalItems.Length > 0)
                list.AddRange(additionalItems);

            return list.ToArray().Join(separator);
        }

        public static string Join(this List<string> source, char separator, params string[] additionalItems)
        {
            var list = source.ToList();

            if (additionalItems.Length > 0)
                list.AddRange(additionalItems);

            return list.ToArray().Join(separator);
        }
    }
}
