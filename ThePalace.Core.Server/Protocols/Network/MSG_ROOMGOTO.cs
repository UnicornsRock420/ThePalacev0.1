using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("navR")]
    public class MSG_ROOMGOTO : IProtocolReceive
    {
        public Int16 dest;

        public void Deserialize(Packet packet, params object[] args)
        {
            dest = packet.ReadSInt16();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                dest = jsonResponse.dest;
            }
            catch
            {
            }
        }
    }
}
