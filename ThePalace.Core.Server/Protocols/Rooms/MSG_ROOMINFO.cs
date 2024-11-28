using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Server.Protocols
{
    [Description("ofNr")]
    public class MSG_ROOMINFO : IProtocolReceive
    {
        public RoomRec room;

        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                room = new RoomRec();
                room.roomID = jsonResponse.roomID;
                room.roomName = jsonResponse.name;
                room.roomPicture = jsonResponse.pictName;
                room.roomArtist = jsonResponse.artistName;
                room.facesID = jsonResponse.facesID;
                room.roomFlags = jsonResponse.flags;
            }
        }
    }
}
