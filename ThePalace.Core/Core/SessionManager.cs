using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core
{
    public sealed class SessionManager : Disposable
    {
        private static readonly Lazy<SessionManager> _current = new();
        public static SessionManager Current => _current.Value;

        private DisposableDictionary<Guid, ISessionState> _sessions = new();
        public IReadOnlyDictionary<Guid, ISessionState> Sessions => this._sessions;

        public SessionManager()
        {
            this._managedResources.Add(this._sessions);
        }
        ~SessionManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            this._sessions = null;
        }

        public T CreateSession<T>()
            where T : class, ISessionState, new()
        {
            if (this.IsDisposed) return default;

            var sessionState = new T() as ISessionState;
            if (sessionState == null)
                throw new Exception("SessionState Type doesn't implement the ISessionState interface...");

            this._sessions.TryAdd(sessionState.SessionID, sessionState);
            return sessionState as T;
        }

        public object CreateSession(Type type)
        {
            if (this.IsDisposed) return null;

            var sessionState = type.GetInstance() as ISessionState;
            if (sessionState == null)
                throw new Exception("SessionState Type doesn't implement the ISessionState interface...");

            this._sessions.TryAdd(sessionState.SessionID, sessionState);
            return sessionState;
        }

        public void RemoveSession(ISessionState sessionState)
        {
            if (this.IsDisposed) return;

            this._sessions.TryRemove(sessionState.SessionID, out var _);
            sessionState?.Dispose();
        }
    }
}
