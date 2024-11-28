namespace System.Collections.Generic
{
    public static class IReadOnlyDictionaryExts
    {
        public static class Types
        {
            public static readonly Type IReadOnlyDictionaryGeneric = typeof(IReadOnlyDictionary<,>);
        }

        //static IReadOnlyDictionaryExts() { }

        public static TValue GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> values, TKey key) =>
            values != null && values.ContainsKey(key) ? values[key] : default;
    }
}
