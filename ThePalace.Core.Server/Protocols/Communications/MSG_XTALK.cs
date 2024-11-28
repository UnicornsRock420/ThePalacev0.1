using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("xtlk")]
    public class MSG_XTALK : IProtocolReceive, IProtocolSend
    {
        public string text;

        public void Deserialize(Packet packet, params object[] args)
        {
            text = packet.ReadPString(255, 0, 2, 2).GetBytes().DecryptString();
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16((Int16)(text.Length + 3));
                packet.WriteBytes(text.EncryptString());
                packet.WriteByte(0);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
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
