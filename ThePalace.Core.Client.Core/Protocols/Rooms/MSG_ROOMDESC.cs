using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("room")]
    public sealed class MSG_ROOMDESC : RoomRec, IProtocolReceive, IProtocolSend
    {
        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                roomID = jsonResponse.roomID;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                roomID,
            });
        }
    }
}
