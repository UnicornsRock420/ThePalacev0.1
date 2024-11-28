using System;
using System.Collections.Generic;
using System.Diagnostics;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;

namespace ThePalace.Core.Utility
{
    public enum MessageTypes : byte
    {
        Info = 0,
        Error = 1,
        Warning = 2,
        Failure = 3,
        ShakaWhenTheWallsFell = 4,
    };

    public static class Logger
    {
        public static void Log(MessageTypes messageType, string message, params string[] stacktrace)
        {
            var st = (string)null;

            if (stacktrace != null && stacktrace.Length > 0)
            {
                st = string.Join("; ", stacktrace);
            }

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var logEntry = new Log
                {
                    MessageType = messageType.ToString(),
                    MachineName = Environment.MachineName,
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                    ProcessId = Process.GetCurrentProcess().Id,
                    CreateDate = DateTime.UtcNow,
                    Message = message,
                    StackTrace = st,
                };

                dbContext.Log.Add(logEntry);

                dbContext.SaveChanges();

                Console.WriteLine($"LOG[{logEntry.LogId}]: {message}");

                if (stacktrace != null && stacktrace.Length > 0)
                {
                    Console.WriteLine($"LOG-ST[{logEntry.LogId}]: {st}");
                }
            }
        }

        public static void Log(this Exception ex, MessageTypes? messageType = null, bool includeStackTrace = true)
        {
            var stacktrace = includeStackTrace ? string.Join("; ", ex.GetFullStackTrace()) : null;

            Log(messageType ?? MessageTypes.Error, string.Join("; ", ex.GetFullMessage()), stacktrace);
        }

        public static void ConsoleLog(string message, params string[] stacktrace)
        {
            Console.WriteLine($"CONSOLE: {message}");

            if (stacktrace != null && stacktrace.Length > 0)
            {
                Console.WriteLine($"CONSOLE-ST: {string.Join("; ", stacktrace)}");
            }
        }

        public static void ConsoleLog(this Exception ex, bool includeStackTrace = true)
        {
            var stacktrace = includeStackTrace ? string.Join("; ", ex.GetFullStackTrace()) : null;

            ConsoleLog(string.Join("; ", ex.GetFullMessage()), stacktrace);
        }

        public static void DebugLog(string message, params string[] stacktrace)
        {
            Console.WriteLine($"DEBUG: {message}");

            if (stacktrace != null && stacktrace.Length > 0)
            {
                Console.WriteLine($"DEBUG-ST: {string.Join("; ", stacktrace)}");
            }
        }

        public static void DebugLog(this Exception ex, bool includeStackTrace = true)
        {
            var stacktrace = includeStackTrace ? string.Join("; ", ex.GetFullStackTrace()) : null;

            DebugLog(string.Join("; ", ex.GetFullMessage()), stacktrace);
        }

        public static List<string> GetFullMessage(this Exception ex)
        {
            var result = new List<string>
            {
                ex.Message,
            };

            if (ex.InnerException != null)
            {
                result.AddRange(ex.InnerException.GetFullMessage());
            }

            return result;
        }

        public static List<string> GetFullStackTrace(this Exception ex)
        {
            var result = new List<string>
            {
                ex.StackTrace,
            };

            if (ex.InnerException != null)
            {
                result.AddRange(ex.InnerException.GetFullStackTrace());
            }

            return result;
        }
    }
}
