using System;

namespace ThePalace.Core.Client.Core.Models.Threads
{
    public sealed class Cmd : IDisposable
    {
        public CmdFnc CmdFnc { get; set; } = null;
        public object[] Values { get; set; } = null;

        public void Dispose()
        {
            CmdFnc = null;
            Values = null;
        }
    }
}
