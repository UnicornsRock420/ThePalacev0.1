using System;
using System.Collections.Generic;

namespace ThePalace.Core.Client.Core.Models
{
#if WINDOWS10_0_17763_0_OR_GREATER
    public sealed class ToastCfg
    {
        public DateTimeOffset ExpirationTime { get; set; }
        public IReadOnlyDictionary<string, object> Args { get; set; }
        public IReadOnlyList<string> Text { get; set; } = null;
    }
#endif
}
