using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("uLoc")]
    public sealed class MSG_USERMOVE : IProtocolReceive, IProtocolSend
    {
        public Point pos;

        public void Deserialize(Packet packet, params object[] values)
        {
            pos = new Point(packet);
        }

        public byte[] Serialize(params object[] values)
        {
            return pos.Serialize();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                pos = new Point((short)jsonResponse.pos.h, (short)jsonResponse.pos.v);
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                pos,
            });
        }
    }
}
