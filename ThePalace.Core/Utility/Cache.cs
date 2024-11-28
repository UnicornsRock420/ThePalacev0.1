using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;

namespace ThePalace.Core.Utility
{
    public static class Cache
    {
        private static readonly ConcurrentDictionary<string, object> _memory;
        private static readonly ObjectCache _cache;

        public static class Types
        {
            public const string ClassName = nameof(Cache);
            public static readonly Type Cache = typeof(Cache);
        }

        static Cache()
        {
            _memory = new ConcurrentDictionary<string, object>();
            _cache = MemoryCache.Default;
        }

        public static void Dispose() =>
            _memory?.Clear();

        public static bool ContainsKey(string key) =>
            _memory.ContainsKey(key);

        public static T GetCache<T>(string key)
        {
            if (!string.IsNullOrWhiteSpace(key) &&
                _memory.ContainsKey(key))
                return (T)_memory[key];
            else
                return default(T);
        }

        public static void SetCache<T>(string key, T value)
        {
            if (!string.IsNullOrWhiteSpace(key) &&
                value != null)
                if (_memory.ContainsKey(key))
                    _memory[key] = value;
                else
                    _memory.TryAdd(key, value);
        }

        public static T GetCache<T>(string key, Func<T> callbackGenerateValue, TimeSpan? expiresIn = null, CacheItemPriority priority = CacheItemPriority.Default, TimeSpan? ifUnusedRemoveIn = null, string regionName = null)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), nameof(key) + " cannot be null");
            else if (callbackGenerateValue == null) throw new ArgumentNullException(nameof(callbackGenerateValue), "Callback cannot be null");

            T value = null;

            if (value != null) return value;

            value = callbackGenerateValue();

            if (value != null)
                TCF._Try(() =>
                {
                    var policy = new CacheItemPolicy
                    {
                        Priority = priority,
                    };

                    if (expiresIn.HasValue)
                        policy.AbsoluteExpiration = expiresIn.Value.ToOffset();

                    if (ifUnusedRemoveIn.HasValue)
                        policy.SlidingExpiration = ifUnusedRemoveIn.Value;

                    _cache.Set(new CacheItem(key, value, regionName), policy);
                }).Execute();

            return value;
        }

        public static void RemoveKey(string key, string regionName = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), nameof(key) + " cannot be null");
        }
    }
}
