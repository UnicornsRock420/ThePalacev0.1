﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("pLoc")]
    public sealed class MSG_PICTMOVE : IProtocolReceive, IProtocolSend
    {
        short roomID;
        short spotID;
        Point pos;

        public void Deserialize(Packet packet, params object[] values)
        {
            roomID = packet.ReadSInt16();
            spotID = packet.ReadSInt16();
            pos = new Point(packet);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomID = jsonResponse.roomID;
                spotID = jsonResponse.spotID;
                pos = jsonResponse.pos;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(roomID);
                packet.WriteInt16(spotID);
                packet.WriteBytes(pos.Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
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
