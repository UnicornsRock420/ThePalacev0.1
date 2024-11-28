using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Protocols.Auth
{
    [Description("autr")]
    public sealed class MSG_AUTHRESPONSE : IProtocolReceive, IProtocolSend
    {
        public string userName;
        public string password;

        public void Deserialize(Packet packet, params object[] values)
        {
            var nameAndPassword = packet.ReadPString(128, 1)
                .GetBytes()
                .DecryptString()
                .Split(':');

            userName = nameAndPassword[0];
            password = nameAndPassword[1];
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                userName = jsonResponse.userName;
                password = jsonResponse.password;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                var nameAndPassword = $"{userName}:{password}";

                packet.WriteByte((byte)nameAndPassword.Length);
                packet.WriteBytes(nameAndPassword.EncryptString());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                userName,
                password,
            });
        }
    }
}
