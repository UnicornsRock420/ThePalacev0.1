using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("coLs")]
    public class MSG_SPOTMOVE : IProtocolReceive, IProtocolSend
    {
        public Int16 roomID;
        public Int16 spotID;
        public Point pos;

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
                pos = new Point((Int16)jsonResponse.pos.h, (Int16)jsonResponse.pos.v);
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
