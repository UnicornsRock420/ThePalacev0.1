using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Factories;

namespace ThePalace.Core.Server.Protocols
{
    [Description("room")]
    public class MSG_ROOMDESC : IProtocolSend
    {
        public RoomBuilder room;

        public byte[] Serialize(params object[] args)
        {
            return room.Serialize();
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
