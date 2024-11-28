using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("uLoc")]
    public class MSG_USERMOVE : IProtocolReceive, IProtocolSend
    {
        public Point pos;

        public void Deserialize(Packet packet, params object[] args)
        {
            pos = new Point(packet);
        }

        public byte[] Serialize(params object[] args)
        {
            return pos.Serialize();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                pos = new Point((Int16)jsonResponse.pos.h, (Int16)jsonResponse.pos.v);
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                pos = pos,
            });
        }
    }
}
