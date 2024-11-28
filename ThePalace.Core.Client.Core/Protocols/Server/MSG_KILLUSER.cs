using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Server
{
    [Description("kill")]
    public sealed class MSG_KILLUSER : IProtocolReceive, IProtocolSend
    {
        public uint targetID;

        public void Deserialize(Packet packet, params object[] values)
        {
            targetID = packet.ReadUInt32();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                targetID = jsonResponse.targetID;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(targetID);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                targetID,
            });
        }
    }
}
