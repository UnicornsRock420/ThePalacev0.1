using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("gmsg")]
    public class MSG_GMSG : IProtocolReceive
    {
        string text;

        public void Deserialize(Packet packet, params object[] args)
        {
            text = packet.ReadCString();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                text = jsonResponse.text;
            }
        }
    }
}
