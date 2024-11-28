using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sSta")]
    public class MSG_SPOTSTATE : IProtocolReceive, IProtocolSend
    {
        public Int16 roomID;
        public Int16 spotID;
        public Int16 state;

        public void Deserialize(Packet packet, params object[] args)
        {
            roomID = packet.ReadSInt16();
            spotID = packet.ReadSInt16();
            state = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] args)
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

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                roomID = jsonResponse.roomID;
                spotID = jsonResponse.spotID;
                state = jsonResponse.state;
            }
            catch
            {
            }
        }

        public string SerializeJSON(params object[] args)
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
