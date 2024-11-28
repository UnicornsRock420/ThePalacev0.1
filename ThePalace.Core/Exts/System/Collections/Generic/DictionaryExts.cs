namespace System.Collections.Generic
{
    public static class DictionaryExts
    {
        public static class Types
        {
            public static readonly Type DictionaryGeneric = typeof(Dictionary<,>);
        }

        //static DictionaryExts() { }

        public static IReadOnlyDictionary<TKey, TValue> IReadOnlyDictionary<TKey, TValue>(this Dictionary<TKey, TValue> values) =>
            values;

        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> values, TKey key) =>
            values.ContainsKey(key) ? values[key] : default;

        public static TValue GetValueLocked<TKey, TValue>(this Dictionary<TKey, TValue> values, TKey key)
        {
            lock (values)
                return values.ContainsKey(key) ? values[key] : default;
        }
    }
}
