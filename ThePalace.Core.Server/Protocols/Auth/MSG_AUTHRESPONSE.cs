using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("autr")]
    public class MSG_AUTHRESPONSE : IProtocolReceive
    {
        public string userName;
        public string password;

        public void Deserialize(Packet packet, params object[] args)
        {
            var nameAndPassword = packet.ReadPString(128).GetBytes().DecryptString().Split(':');

            userName = nameAndPassword[0];
            password = nameAndPassword[1];
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                userName = jsonResponse.userName;
                password = jsonResponse.password;
            }
        }
    }
}
