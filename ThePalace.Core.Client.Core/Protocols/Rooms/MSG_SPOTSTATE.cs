using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("sSta")]
    public sealed class MSG_SPOTSTATE : IProtocolReceive, IProtocolSend
    {
        public short roomID;
        public short spotID;
        public short state;

        public void Deserialize(Packet packet, params object[] values)
        {
            roomID = packet.ReadSInt16();
            spotID = packet.ReadSInt16();
            state = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(roomID);
                packet.WriteInt16(spotID);
                packet.WriteInt16(state);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomID = jsonResponse.roomID;
                spotID = jsonResponse.spotID;
                state = jsonResponse.state;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                roomID,
                spotID,
                state,
            });
        }
    }
}
