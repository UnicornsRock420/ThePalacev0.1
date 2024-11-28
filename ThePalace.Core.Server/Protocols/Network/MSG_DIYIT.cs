using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("ryit")]
    public class MSG_DIYIT : IProtocolSend
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
