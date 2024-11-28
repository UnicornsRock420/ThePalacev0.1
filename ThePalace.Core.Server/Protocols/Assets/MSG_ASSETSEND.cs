using System;
using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sAst")]
    public class MSG_ASSETSEND : IProtocolSend
    {
        public byte[] Serialize(params object[] args) => Array.Empty<byte>();

        public string SerializeJSON(params object[] args) => string.Empty;
    }
}
