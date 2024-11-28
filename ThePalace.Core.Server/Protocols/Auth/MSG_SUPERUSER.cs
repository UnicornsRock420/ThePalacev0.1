using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("susr")]
    public class MSG_SUPERUSER : IProtocolReceive
    {
        public string password;

        public void Deserialize(Packet packet, params object[] args)
        {
            password = packet.ReadPString(64).GetBytes().DecryptString();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                password = jsonResponse.password;
            }
        }
    }
}
