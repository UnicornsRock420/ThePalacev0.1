namespace System.Collections.Generic
{
    public static class IEnumerableExts
    {
        public static class Types
        {
            public static readonly Type IEnumerable = typeof(IEnumerable);
            public static readonly Type IEnumerableArray = typeof(IEnumerable[]);
            public static readonly Type IEnumerableGeneric = typeof(IEnumerable<>);
        }

        //static IEnumerableExts() { }

        public static void CopyTo<T>(this IEnumerable<T> source, List<T> destination) =>
            destination.AddRange(source);
        public static bool Contains<T>(this IEnumerable<T> list, object value, string propertyName = null) =>
            IndexOf(list, value, propertyName) > -1;
        public static int IndexOf<T>(this IEnumerable<T> list, object value, string propertyName = null)
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(propertyName);
            var index = 0;

            foreach (var item in list)
            {
                var _value = propertyInfo?.GetValue(item);
                if (_value != null && value == _value) return index;
                else if ((object)item == value) return index;

                index++;
            }

            return -1;
        }
    }
}
