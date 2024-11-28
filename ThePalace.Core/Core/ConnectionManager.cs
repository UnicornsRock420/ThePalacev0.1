using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core
{
    public sealed class ConnectionManager : Disposable
    {
        private static readonly Lazy<ConnectionManager> _current = new();
        public static ConnectionManager Current => _current.Value;

        private DisposableDictionary<string, IConnectionState> _connections = new();
        public IReadOnlyDictionary<string, IConnectionState> Connections => _connections;

        public ConnectionManager()
        {
            this._managedResources.Add(this._connections);
        }
        ~ConnectionManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            _connections = null;
        }

        public T CreateConnection<T>()
            where T : class, IConnectionState, new()
        {
            if (this.IsDisposed) return default;

            var connectionState = new T() as IConnectionState;
            if (connectionState == null)
                throw new Exception("ConnectionState Type doesn't implement the IConnectionState interface...");

            _connections.TryAdd(connectionState.ConnectionID, connectionState);
            return connectionState as T;
        }

        public object CreateConnection(Type type)
        {
            if (this.IsDisposed) return null;

            var connectionState = type.GetInstance() as IConnectionState;
            if (connectionState == null)
                throw new Exception("ConnectionState Type doesn't implement the IConnectionState interface...");

            _connections.TryAdd(connectionState.ConnectionID, connectionState);
            return connectionState;
        }

        public void RemoveConnection(IConnectionState connectionState)
        {
            if (this.IsDisposed) return;

            _connections.TryRemove(connectionState.ConnectionID, out var _);
            connectionState?.Dispose();
        }
    }
}
