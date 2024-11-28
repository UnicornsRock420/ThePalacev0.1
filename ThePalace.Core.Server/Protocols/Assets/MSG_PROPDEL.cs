using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("dPrp")]
    public class MSG_PROPDEL : IProtocolReceive, IProtocolSend
    {
        public Int32 propNum;

        public void Deserialize(Packet packet, params object[] args)
        {
            propNum = packet.ReadSInt32();
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(propNum);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                propNum = jsonResponse.propNum;
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                propNum = propNum,
            });
        }
    }
}
