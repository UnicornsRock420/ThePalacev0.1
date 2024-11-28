using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Communications
{
    [Description("gmsg")]
    public sealed class MSG_GMSG : IProtocolCommunications, IProtocolReceive, IProtocolSend
    {
        public string text { get; set; }

        public void Deserialize(Packet packet, params object[] values)
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

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteCString(text);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                text,
            });
        }
    }
}
