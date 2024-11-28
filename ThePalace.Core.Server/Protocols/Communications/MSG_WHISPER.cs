using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("whis")]
    public class MSG_WHISPER : IProtocolReceive, IProtocolSend
    {
        public UInt32 targetID;
        public string text;

        public void Deserialize(Packet packet, params object[] args)
        {
            targetID = packet.ReadUInt32();
            text = packet.ReadCString();
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteCString(text);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                targetID = jsonResponse.targetID;
                text = jsonResponse.text;
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                text = text,
            });
        }
    }
}
