using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("ofNr")]
    public sealed class MSG_ROOMINFO : IProtocolReceive, IProtocolSend
    {
        public RoomRec roomInfo;

        public void Deserialize(Packet packet, params object[] values)
        {
            roomInfo.roomID = packet.ReadSInt16();
            roomInfo.roomName = packet.ReadPString(32, 1);
            roomInfo.roomPicture = packet.ReadPString(32, 1);
            roomInfo.roomArtist = packet.ReadPString(32, 1);
            roomInfo.facesID = packet.ReadSInt32();
            roomInfo.roomFlags = packet.ReadSInt32();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomInfo.roomID = jsonResponse.roomID;
                roomInfo.roomName = jsonResponse.name;
                roomInfo.roomPicture = jsonResponse.pictName;
                roomInfo.roomArtist = jsonResponse.artistName;
                roomInfo.facesID = jsonResponse.facesID;
                roomInfo.roomFlags = jsonResponse.flags;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(roomInfo.roomID);
                packet.WritePString(roomInfo.roomName, 32, 1);
                packet.WritePString(roomInfo.roomPicture, 32, 1);
                packet.WritePString(roomInfo.roomArtist, 32, 1);
                packet.WriteInt32(roomInfo.facesID);
                packet.WriteInt32(roomInfo.roomFlags);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                roomInfo.roomID,
                name = roomInfo.roomName,
                pictName = roomInfo.roomPicture,
                artistName = roomInfo.roomArtist,
                roomInfo.facesID,
                flags = roomInfo.roomFlags,
            });
        }
    }
}
