using System;
using System.Threading;

namespace ThePalace.Core.Factories
{
    public sealed class LockContext : IDisposable
    {
        protected object _LockObj;
        protected bool _HasLock;

        public bool HasLock => _HasLock;

        public LockContext(object obj = null)
        {
            _LockObj = obj ?? new();
            _HasLock = false;
        }

        public bool Lock()
        {
            if (_LockObj == null) return false;
            else if (_HasLock) return true;

            Monitor.Enter(_LockObj, ref _HasLock);

            return _HasLock;
        }

        public bool TryLock(int millisecondsTimeout = 0)
        {
            if (_LockObj == null) return false;
            else if (_HasLock) return true;

            else if (millisecondsTimeout > 0)
            {
                Monitor.TryEnter(_LockObj, millisecondsTimeout, ref _HasLock);
            }
            else
            {
                Monitor.TryEnter(_LockObj, ref _HasLock);
            }

            return _HasLock;
        }

        public bool Unlock()
        {
            if (_LockObj != null &&
                _HasLock)
            {
                Monitor.Exit(_LockObj);

                _HasLock = false;
            }

            return _HasLock;
        }

        public void Dispose()
        {
            Unlock();

            _LockObj = null;
        }
    }
}
