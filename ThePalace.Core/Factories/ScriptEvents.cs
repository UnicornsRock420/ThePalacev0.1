using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;

namespace ThePalace.Core.Factories
{
    public class ScriptEvents : Disposable
    {
        private static readonly IReadOnlyList<IptEventTypes> _eventTypes = Enum.GetValues<IptEventTypes>().ToList();

        private static readonly Lazy<ScriptEvents> _current = new();
        public static ScriptEvents Current => _current.Value;

        private ConcurrentDictionary<IptEventTypes, List<EventHandler>> _events = new();

        public ScriptEvents()
        {
            foreach (var type in _eventTypes)
                this._events[type] = new List<EventHandler>();
        }
        ~ScriptEvents() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            foreach (var @event in this._events.Values)
                try { @event?.Clear(); } catch { }
            this._events.Clear();
            this._events = null;
        }

        public void Invoke(IptEventTypes eventType, ISessionState sessionState, IProtocolRec packet, object scriptState = null)
        {
            var scriptEvent = new ScriptEvent
            {
                EventType = eventType,
                Packet = packet,
                ScriptState = scriptState,
            };

            foreach (var handler in this._events[eventType])
            {
                try
                {
                    handler(sessionState, scriptEvent);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif

                    if (eventType != IptEventTypes.UnhandledError)
                        this.Invoke(IptEventTypes.UnhandledError, sessionState, packet, sessionState.ScriptState);
                }
            }
        }

        public void RegisterEvent(IptEventTypes eventType, EventHandler handler)
        {
            if (handler != null)
                this._events[eventType].Add(handler);
        }

        public void UnregisterEvent(IptEventTypes eventType, EventHandler handler)
        {
            this._events[eventType].Remove(handler);
        }
    }
}
