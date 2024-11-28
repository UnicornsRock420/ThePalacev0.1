using System;
using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sFil")]
    public class MSG_FILESEND : IProtocolSend
    {
        public byte[] Serialize(params object[] args) => Array.Empty<byte>();

        public string SerializeJSON(params object[] args) => string.Empty;
    }
}
