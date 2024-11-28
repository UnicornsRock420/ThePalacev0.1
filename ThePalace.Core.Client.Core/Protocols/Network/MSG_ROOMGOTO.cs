using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Network
{
    [Description("navR")]
    public sealed class MSG_ROOMGOTO : IProtocolReceive, IProtocolSend
    {
        public short dest;

        public void Deserialize(Packet packet, params object[] values)
        {
            dest = packet.ReadSInt16();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                dest = jsonResponse.dest;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(dest);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                dest,
            });
        }
    }
}
