using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Assets
{
    [Description("mPrp")]
    public sealed class MSG_PROPMOVE : IProtocolReceive, IProtocolSend
    {
        public int propNum;
        public Point pos;

        public void Deserialize(Packet packet, params object[] values)
        {
            propNum = packet.ReadSInt32();
            pos = new Point(packet);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                propNum = jsonResponse.propNum;

                var h = (short)jsonResponse.pos.h;
                var v = (short)jsonResponse.pos.v;

                pos = new Point(h, v);
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(propNum);
                packet.WriteBytes(pos.Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                propNum,
                pos,
            });
        }
    }
}
