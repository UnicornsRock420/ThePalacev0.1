using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("opSd")]
    public sealed class MSG_SPOTDEL : IProtocolReceive, IProtocolSend
    {
        public short spotID;

        public void Deserialize(Packet packet, params object[] values)
        {
            spotID = packet.ReadSInt16();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                spotID = jsonResponse.spotID;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(spotID);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                spotID,
            });
        }
    }
}
