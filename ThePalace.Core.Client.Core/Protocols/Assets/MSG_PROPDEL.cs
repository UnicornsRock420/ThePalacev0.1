using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Assets
{
    [Description("dPrp")]
    public sealed class MSG_PROPDEL : IProtocolReceive, IProtocolSend
    {
        public int propNum;

        public void Deserialize(Packet packet, params object[] values)
        {
            propNum = packet.ReadSInt32();
        }

        public byte[] Serialize(params object[] values)
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

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                propNum,
            });
        }
    }
}
