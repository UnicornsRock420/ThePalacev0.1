using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;

namespace System.Net.Http
{
    public static class HttpClientExts
    {
        public static class Types
        {
            public static readonly Type HttpClient = typeof(HttpClient);
            public static readonly Type JObject = typeof(JObject);
            public static readonly Type ExpandoObject = typeof(ExpandoObject);
        }

        //static HttpClientExts() { }

        public static TReturn Method<TReturn>(this HttpClient httpClient, HttpMethod httpMethod, string url, HttpContent httpContent = null, params KeyValuePair<string, string>[] headers)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(url),
                Content = httpContent,
            };

            httpRequest.Headers(headers);

            return httpClient
                .SendAsync(httpRequest)
                .GetAwaiter()
                .GetResult()
                .Content.Response<TReturn>();
        }

        public static TReturn Get<TReturn>(this HttpClient httpClient, string url, params KeyValuePair<string, string>[] headers)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            };

            httpRequest.Headers(headers);

            return httpClient
                .SendAsync(httpRequest)
                .GetAwaiter()
                .GetResult()
                .Content.Response<TReturn>();
        }

        public static TReturn Post<TReturn>(this HttpClient httpClient, string url, HttpContent httpContent = null, params KeyValuePair<string, string>[] headers)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = httpContent,
            };

            httpRequest.Headers(headers);

            return httpClient
                .SendAsync(httpRequest)
                .GetAwaiter()
                .GetResult()
                .Content.Response<TReturn>();
        }

        public static TReturn Put<TReturn>(this HttpClient httpClient, string url, HttpContent httpContent = null, params KeyValuePair<string, string>[] headers)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = httpContent,
            };

            httpRequest.Headers(headers);

            return httpClient
                .SendAsync(httpRequest)
                .GetAwaiter()
                .GetResult()
                .Content.Response<TReturn>();
        }

        public static TReturn Delete<TReturn>(this HttpClient httpClient, string url, HttpContent httpContent = null, params KeyValuePair<string, string>[] headers)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url),
                Content = httpContent,
            };

            httpRequest.Headers(headers);

            return httpClient
                .SendAsync(httpRequest)
                .GetAwaiter()
                .GetResult()
                .Content.Response<TReturn>();
        }

        private static void Headers(this HttpRequestMessage httpRequest, params KeyValuePair<string, string>[] headers)
        {
            if (headers != null &&
                headers.Length > 0)
                foreach (var header in headers)
                    httpRequest.Headers.Add(header.Key, header.Value);
        }

        private static TReturn Response<TReturn>(this HttpContent httpContent)
        {
            var type = typeof(TReturn);

            if (type == ByteExts.Types.ByteArray)
                return (TReturn)(object)httpContent.ReadAsByteArrayAsync()
                    .GetAwaiter()
                    .GetResult();
            else
            {
                var result = httpContent.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();

                if (type == StringExts.Types.String) return (TReturn)(object)result;
                else if (type == Types.JObject ||
                    type == Types.ExpandoObject)
                    return (TReturn)(object)JsonConvert.DeserializeObject<JObject>(result);
            }

            return default;
        }
    }
}
