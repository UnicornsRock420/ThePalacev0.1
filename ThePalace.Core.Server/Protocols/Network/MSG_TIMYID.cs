using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("timy")]
    public class MSG_TIMYID : IProtocolReceive, IProtocolSend
    {
        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }

        public byte[] Serialize(params object[] args) => Array.Empty<byte>();

        public string SerializeJSON(params object[] args) => string.Empty;
    }
}
