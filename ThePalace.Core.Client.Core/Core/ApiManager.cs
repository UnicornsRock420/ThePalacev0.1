using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Client.Core.Models;

namespace ThePalace.Core.Client.Core
{
    public sealed class ApiManager : Disposable
    {
        private static readonly Lazy<ApiManager> _current = new();
        public static ApiManager Current => _current.Value;

        private ConcurrentDictionary<string, ApiBinding> _apiBindings = new();
        public IReadOnlyDictionary<string, ApiBinding> ApiBindings => _apiBindings;

        public ApiManager() { }
        ~ApiManager() =>
            Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            _apiBindings.Clear();
            _apiBindings = null;
        }

        public void RegisterApi(string friendlyName, EventHandler binding)
        {
            if (!_apiBindings.ContainsKey(friendlyName) &&
                binding != null)
                _apiBindings.TryAdd(friendlyName, new ApiBinding
                {
                    Binding = binding,
                });
        }

        public void UnregisterApi(string friendlyName)
        {
            if (!_apiBindings.ContainsKey(friendlyName))
                _apiBindings.TryRemove(friendlyName, out var _);
        }
    }
}
