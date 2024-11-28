using System.ComponentModel;

namespace System
{
    public enum DateTimeFormatsEnum : int
    {
        Void = unchecked(-1),
        [Description("yyyy-MM-dd'T'HH:mm:ss.fffzzz")]
        Rfc3339Iso8601,
        [Description("MM/dd/yyyy")]
        LogDateFormat,
        [Description("HH:mm:ss")]
        LogTimeFormat,
        [Description("MMM-dd-yyyy")]
        FilenameDateFormat,
        [Description("HH-mm-ss")]
        FilenameTimeFormat,
        Max
    }
}
