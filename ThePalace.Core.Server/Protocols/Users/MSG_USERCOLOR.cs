using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("usrC")]
    public class MSG_USERCOLOR : IProtocolReceive, IProtocolSend
    {
        public Int16 colorNbr;

        public void Deserialize(Packet packet, params object[] args)
        {
            colorNbr = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(colorNbr);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                colorNbr = jsonResponse.colorNbr;
            }
            catch
            {
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                colorNbr = colorNbr,
            });
        }
    }
}
