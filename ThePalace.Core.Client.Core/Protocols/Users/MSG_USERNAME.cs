using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("usrN")]
    public sealed class MSG_USERNAME : IProtocolReceive, IProtocolSend
    {
        public string name;

        public void Deserialize(Packet packet, params object[] values)
        {
            name = packet.ReadPString(32, 1);
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WritePString(name, 32, 1);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                name = jsonResponse.name;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                name,
            });
        }
    }
}
