using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("usrC")]
    public sealed class MSG_USERCOLOR : IProtocolReceive, IProtocolSend
    {
        public short colorNbr;

        public void Deserialize(Packet packet, params object[] values)
        {
            colorNbr = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(colorNbr);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                colorNbr = jsonResponse.colorNbr;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                colorNbr,
            });
        }
    }
}
