using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExts
    {
        public static Dictionary<string, string> ToDictionaryFirstValue(this NameValueCollection collection) =>
            collection?.AllKeys?.ToDictionary(k => k, k => collection?.GetValues(k)?.Take(1)?.FirstOrDefault());
        public static Dictionary<string, string> ToDictionaryLastValue(this NameValueCollection collection) =>
            collection?.AllKeys?.ToDictionary(k => k, k => collection?.GetValues(k)?.LastOrDefault());

        public static List<KeyValuePair<string, string>> ToListFirstValue(this NameValueCollection collection) =>
            collection?.AllKeys?.Select(k => KeyValuePair.Create(k, collection?.GetValues(k)?.Take(1)?.FirstOrDefault()))?.ToList();
        public static List<KeyValuePair<string, string>> ToListLastValue(this NameValueCollection collection) =>
            collection?.AllKeys?.Select(k => KeyValuePair.Create(k, collection?.GetValues(k)?.LastOrDefault()))?.ToList();
        public static List<KeyValuePair<string, string>> ToListAllValues(this NameValueCollection collection) =>
            collection?.AllKeys?.SelectMany(k => collection?.GetValues(k)?.Select(v => KeyValuePair.Create(k, v)))?.ToList();

        public static NameValueCollection FirstValues(this NameValueCollection collection)
        {
            var newCollection = new NameValueCollection();
            collection.AllKeys.ToList().ForEach(k => newCollection.Add(k, collection.GetValues(k).FirstOrDefault()));
            return newCollection;
        }
        public static NameValueCollection LastValues(this NameValueCollection collection)
        {
            var newCollection = new NameValueCollection();
            collection.AllKeys.ToList().ForEach(k => newCollection.Add(k, collection.GetValues(k).LastOrDefault()));
            return newCollection;
        }

        public static void AddRangeFirstValue(this NameValueCollection collection1, NameValueCollection collection2) =>
            collection2?.AllKeys?.ToList()?.ForEach(k => collection1?.Add(k, collection2?.GetValues(k)?.Take(1)?.FirstOrDefault()));
        public static void AddRangeLastValue(this NameValueCollection collection1, NameValueCollection collection2) =>
            collection2?.AllKeys?.ToList()?.ForEach(k => collection1?.Add(k, collection2?.GetValues(k)?.LastOrDefault()));
        public static void AddRangeAllValues(this NameValueCollection collection1, NameValueCollection collection2) =>
            collection2?.AllKeys?.SelectMany(k => collection2?.GetValues(k)?.Select(v => KeyValuePair.Create(k, v)))?.ToList()?.ForEach(k => collection1?.Add(k.Key, k.Value));
    }
}
