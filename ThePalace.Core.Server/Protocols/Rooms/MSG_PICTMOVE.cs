using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("pLoc")]
    public class MSG_PICTMOVE : IProtocolReceive, IProtocolSend
    {
        Int16 roomID;
        Int16 spotID;
        Point pos;

        public void Deserialize(Packet packet, params object[] args)
        {
            roomID = packet.ReadSInt16();
            spotID = packet.ReadSInt16();
            pos = new Point(packet);
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(roomID);
                packet.WriteInt16(spotID);
                packet.WriteBytes(pos.Serialize());

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
                pos = jsonResponse.pos;
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
                pos,
            });
        }
    }
}
