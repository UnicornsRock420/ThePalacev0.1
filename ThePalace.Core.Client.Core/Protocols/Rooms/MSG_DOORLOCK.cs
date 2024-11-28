using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("lock")]
    public sealed class MSG_DOORLOCK : IProtocolReceive, IProtocolSend
    {
        public short roomID;
        public short spotID;

        public void Deserialize(Packet packet, params object[] values)
        {
            roomID = packet.ReadSInt16();
            spotID = packet.ReadSInt16();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomID = jsonResponse.roomID;
                spotID = jsonResponse.spotID;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(roomID);
                packet.WriteInt16(spotID);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                roomID,
                spotID,
            });
        }
    }
}
