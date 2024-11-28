using System;
using System.Collections.Generic;

namespace ThePalace.Core.Models
{
    public sealed class TCFResults
    {
        internal List<Exception> IExceptions { get; set; } = new List<Exception>();
        internal List<object> IResults { get; set; } = new List<object>();
        public IReadOnlyList<Exception> Exceptions =>
            IExceptions.AsReadOnly();
        public IReadOnlyList<object> Results =>
            IResults.AsReadOnly();
    }
}
