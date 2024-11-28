using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Network
{
    [Description("tiyr")]
    public sealed class MSG_TIYID : IProtocolReceive, IProtocolSend
    {
        public string ipAddress;

        public void Deserialize(Packet packet, params object[] values)
        {
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                ipAddress = JObject.Parse(jsonResponse.ipAddress);
            }
        }

        public byte[] Serialize(params object[] values)
        {
            return null;
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                ipAddress,
            });
        }
    }
}
