using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public static class DisposableDictionaryExts
    {
        public static class Types
        {
            public static readonly Type DisposableDictionaryGeneric = typeof(DisposableDictionary<,>);
        }

        //static DisposableDictionaryExts() { }

        public static IReadOnlyDictionary<TKey, TValue> IReadOnlyDictionary<TKey, TValue>(this DisposableDictionary<TKey, TValue> values)
            where TValue : IDisposable =>
                values;

        public static TValue GetValue<TKey, TValue>(this DisposableDictionary<TKey, TValue> values, TKey key)
            where TValue : IDisposable =>
                values != null && values.ContainsKey(key) ? values[key] : default;

        public static TValue GetValueLocked<TKey, TValue>(this DisposableDictionary<TKey, TValue> values, TKey key)
            where TValue : IDisposable
        {
            lock (values)
                return values != null && values.ContainsKey(key) ? values[key] : default;
        }

        public static void Remove<TKey, TValue>(this DisposableDictionary<TKey, TValue> values, TKey key)
            where TValue : IDisposable =>
                values.Remove(key, out TValue value);
    }
}
