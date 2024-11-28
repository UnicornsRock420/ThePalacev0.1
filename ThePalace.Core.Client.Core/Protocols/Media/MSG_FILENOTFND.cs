using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Media
{
    [Description("fnfe")]
    public sealed class MSG_FILENOTFND : IProtocolReceive, IProtocolSend
    {
        public string fileName;

        public void Deserialize(Packet packet, params object[] values)
        {
            fileName = packet.ReadPString(64, 1);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                fileName = jsonResponse.fileName;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WritePString(fileName, 64, 1);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
