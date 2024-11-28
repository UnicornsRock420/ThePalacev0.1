using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("kill")]
    public class MSG_KILLUSER : IProtocolReceive
    {
        public UInt32 targetID;

        public void Deserialize(Packet packet, params object[] args)
        {
            targetID = packet.ReadUInt32();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                targetID = jsonResponse.targetID;
            }
            catch
            {
            }
        }
    }
}
