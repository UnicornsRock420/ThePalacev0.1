using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Server
{
    [Description("down")]
    public sealed class MSG_SERVERDOWN : IProtocolReceive, IProtocolSend
    {
        public string whyMessage;

        public void Deserialize(Packet packet, params object[] values)
        {
            //whyMessage = packet.ReadCString();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                whyMessage = jsonResponse.whyMessage;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            //using (var packet = new Packet())
            //{
            //    if (!string.IsNullOrWhiteSpace(whyMessage))
            //    {
            //        packet.WriteCString(whyMessage);
            //    }

            //    return packet.GetData();
            //}

            return Array.Empty<byte>();
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                whyMessage,
            });
        }
    }
}
