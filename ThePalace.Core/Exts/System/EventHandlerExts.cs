using System.Collections.Generic;

namespace System
{
    public static class EventHandlerExts
    {
        public static class Types
        {
            public static readonly Type EventHandler = typeof(EventHandler);
            public static readonly Type EventHandlerArray = typeof(EventHandler[]);
            public static readonly Type EventHandlerList = typeof(List<EventHandler>);
        }

        //static EventHandlerExts() { }

        public static void Clear(this EventHandler @event)
        {
            foreach (var d in @event.GetInvocationList())
                @event -= (EventHandler)d;
        }
        public static void Clear(this IEnumerable<EventHandler> events)
        {
            foreach (var @event in events)
                @event.Clear();
        }
        public static void Clear(params EventHandler[] events)
        {
            foreach (var @event in events)
                @event.Clear();
        }
        public static void Clear<T>(this EventHandler<T> @event)
            where T : EventArgs
        {
            foreach (var d in @event.GetInvocationList())
                @event -= (EventHandler<T>)d;
        }
        public static void Clear<T>(this IEnumerable<EventHandler<T>> events)
            where T : EventArgs
        {
            foreach (var @event in events)
                @event.Clear();
        }
        public static void Clear<T>(params EventHandler<T>[] events)
            where T : EventArgs
        {
            foreach (var @event in events)
                @event.Clear();
        }
    }
}
