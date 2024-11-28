using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("usrF")]
    public sealed class MSG_USERFACE : IProtocolReceive, IProtocolSend
    {
        public short faceNbr;

        public void Deserialize(Packet packet, params object[] values)
        {
            faceNbr = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(faceNbr);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                faceNbr = jsonResponse.faceNbr;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                faceNbr,
            });
        }
    }
}
