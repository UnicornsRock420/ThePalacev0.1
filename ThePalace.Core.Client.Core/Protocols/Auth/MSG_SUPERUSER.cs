using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Protocols.Auth
{
    [Description("susr")]
    public sealed class MSG_SUPERUSER : IProtocolReceive, IProtocolSend
    {
        public string password;

        public void Deserialize(Packet packet, params object[] values)
        {
            password = packet.ReadPString(32, 1)
                .GetBytes()
                .DecryptString();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                password = jsonResponse.password;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteByte((byte)password.Length);
                packet.WriteBytes(password.EncryptString());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                password,
            });
        }
    }
}
