using System;
using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("auth")]
    public class MSG_AUTHENTICATE : IProtocolSend
    {
        public byte[] Serialize(params object[] args) => Array.Empty<byte>();

        public string SerializeJSON(params object[] args) => string.Empty;
    }
}
