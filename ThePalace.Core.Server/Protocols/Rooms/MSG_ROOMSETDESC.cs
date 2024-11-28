using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Factories;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sRom")]
    public class MSG_ROOMSETDESC : IProtocolReceive, IProtocolSend
    {
        public RoomBuilder room;

        public void Deserialize(Packet packet, params object[] args)
        {
            room = new RoomBuilder();

            room.Deserialize(packet);
        }

        public byte[] Serialize(params object[] args)
        {
            return room.Serialize();
        }

        public void DeserializeJSON(string json)
        {
            // TODO: ???
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                roomID = room.ID,
            });
        }
    }
}
