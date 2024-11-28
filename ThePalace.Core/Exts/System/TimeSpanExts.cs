using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public static class TimeSpanExts
    {
        public static class Types
        {
            public static readonly Type TimeSpan = typeof(TimeSpan);
            public static readonly Type TimeSpanArray = typeof(TimeSpan[]);
            public static readonly Type TimeSpanList = typeof(List<TimeSpan>);
        }

        //static TimeSpanExts() { }

        public static Task GetDelay(this TimeSpan timespan, CancellationToken cancellationToken) =>
            Task.Delay(timespan, cancellationToken);
        public static DateTimeOffset ToOffset(this TimeSpan timespan) =>
            (DateTimeOffset)DateTime.UtcNow.Add(timespan);
    }
}
