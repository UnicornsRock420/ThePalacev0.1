using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("mPrp")]
    public class MSG_PROPMOVE : IProtocolReceive, IProtocolSend
    {
        public Int32 propNum;
        public Point pos;

        public void Deserialize(Packet packet, params object[] args)
        {
            propNum = packet.ReadSInt32();
            pos = new Point(packet);
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(propNum);
                packet.WriteBytes(pos.Serialize());

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                propNum = jsonResponse.propNum;

                var h = (Int16)jsonResponse.pos.h;
                var v = (Int16)jsonResponse.pos.v;

                pos = new Point(h, v);
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                propNum = propNum,
                pos = pos,
            });
        }
    }
}
