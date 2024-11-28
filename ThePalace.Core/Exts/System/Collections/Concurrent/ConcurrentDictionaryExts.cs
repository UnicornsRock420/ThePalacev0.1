using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryExts
    {
        public static class Types
        {
            public static readonly Type ConcurrentDictionaryGeneric = typeof(ConcurrentDictionary<,>);
        }

        //static ConcurrentDictionaryExts() { }

        public static IReadOnlyDictionary<TKey, TValue> IReadOnlyDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> values) =>
            values;

        public static TValue GetValue<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> values, TKey key) =>
            values != null && values.ContainsKey(key) ? values[key] : default;

        public static TValue GetValueLocked<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> values, TKey key)
        {
            lock (values)
                return values != null && values.ContainsKey(key) ? values[key] : default;
        }

        public static void Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> values, TKey key) =>
            values.Remove(key, out TValue value);
    }
}
