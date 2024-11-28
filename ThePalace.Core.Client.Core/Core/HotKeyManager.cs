using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core
{
    public delegate void HotKeyAction(ISessionState sessionState, Keys keys, object sender = null);

    public sealed class HotKeyManager : Disposable
    {
        private static readonly Lazy<HotKeyManager> _current = new();
        public static HotKeyManager Current => _current.Value;

        private ConcurrentDictionary<Keys, HotKeyBinding> _keyBindings = new();
        public IReadOnlyDictionary<Keys, HotKeyBinding> KeyBindings => _keyBindings;

        public HotKeyManager() { }
        ~HotKeyManager() =>
            Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            _keyBindings.Clear();
            _keyBindings = null;
        }

        public void RegisterHotKey(Keys keys, ApiBinding binding, params object[] values)
        {
            if (!_keyBindings.ContainsKey(keys) &&
                binding != null)
                _keyBindings.TryAdd(keys, new HotKeyBinding
                {
                    Binding = binding,
                    Values = values,
                });
        }

        public void UnregisterHotKey(Keys keys)
        {
            if (!_keyBindings.ContainsKey(keys))
                _keyBindings.TryRemove(keys, out var _);
        }

        public bool Invoke(ISessionState sessionState, Keys keys, object sender = null, params object[] values)
        {
            if (this.IsDisposed) return false;

            if (_keyBindings.ContainsKey(keys))
                try
                {
                    _keyBindings[keys].Binding.Binding(sessionState, new ApiEvent
                    {
                        Keys = keys,
                        Sender = sender,
                        HotKeyState = _keyBindings[keys].Values,
                        EventState = values,
                    });

                    return true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

            return false;
        }
    }
}
