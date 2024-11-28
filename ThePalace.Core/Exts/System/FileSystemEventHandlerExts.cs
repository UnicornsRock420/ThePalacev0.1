using System.Collections.Generic;
using System.IO;

namespace System
{
    public static class FileSystemEventHandlerExts
    {
        public static class Types
        {
            public static readonly Type FileSystemEventHandler = typeof(FileSystemEventHandler);
            public static readonly Type FileSystemEventHandlerArray = typeof(FileSystemEventHandler[]);
            public static readonly Type FileSystemEventHandlerList = typeof(List<FileSystemEventHandler>);
        }

        //static FileSystemEventHandlerExts() { }

        public static void Clear(this FileSystemEventHandler @event)
        {
            foreach (var d in @event.GetInvocationList())
                @event -= (FileSystemEventHandler)d;
        }
        public static void Clear(this IEnumerable<FileSystemEventHandler> events)
        {
            foreach (var @event in events)
                @event.Clear();
        }
        public static void Clear(params FileSystemEventHandler[] events)
        {
            foreach (var @event in events)
                @event.Clear();
        }
    }
}
