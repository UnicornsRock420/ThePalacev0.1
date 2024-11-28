using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Protocols.Communications
{
    [Description("xtlk")]
    public sealed class MSG_XTALK : IProtocolCommunications, IProtocolReceive, IProtocolSend
    {
        public string text { get; set; }

        public void Deserialize(Packet packet, params object[] values)
        {
            text = packet.ReadPString(255, 2, 0, 2).GetBytes()?.DecryptString();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                text = jsonResponse.text?.DecryptString();
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16((short)(text.Length + 3));
                packet.WriteBytes(text.EncryptString());
                packet.WriteByte(0);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                text,
            });
        }
    }
}
