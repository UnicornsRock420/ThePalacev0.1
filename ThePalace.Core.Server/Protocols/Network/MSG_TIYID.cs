using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("tiyr")]
    public class MSG_TIYID : IProtocolSend
    {
        public string ipAddress;

        public byte[] Serialize(params object[] args) => Array.Empty<byte>();

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                ipAddress,
            });
        }
    }
}
