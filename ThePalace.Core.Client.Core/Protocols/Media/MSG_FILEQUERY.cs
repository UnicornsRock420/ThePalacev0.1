using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Media
{
    [Description("qFil")]
    public sealed class MSG_FILEQUERY : IProtocolReceive, IProtocolSend
    {
        public string fileName;

        public void Deserialize(Packet packet, params object[] values)
        {
            fileName = packet.ReadPString(packet.Length - 1, 1);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                fileName = jsonResponse.fileName;
            }
        }

        public byte[] Serialize(params object[] values) => Array.Empty<byte>();

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
