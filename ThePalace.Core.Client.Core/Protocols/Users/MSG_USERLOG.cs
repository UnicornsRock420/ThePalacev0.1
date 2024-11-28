using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("log ")]
    public sealed class MSG_USERLOG : IProtocolReceive, IProtocolSend
    {
        public uint nbrUsers;

        public void Deserialize(Packet packet, params object[] values)
        {
            nbrUsers = packet.ReadUInt32();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                nbrUsers = jsonResponse.nbrUsers;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(nbrUsers);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                nbrUsers,
            });
        }
    }
}
