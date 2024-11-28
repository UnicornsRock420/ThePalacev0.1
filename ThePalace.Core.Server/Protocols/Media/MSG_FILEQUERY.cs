using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("qFil")]
    public class MSG_FILEQUERY : IProtocolReceive
    {
        public string fileName;
        public void Deserialize(Packet packet, params object[] args)
        {
            fileName = packet.ReadPString(64, 1);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }
    }
}
