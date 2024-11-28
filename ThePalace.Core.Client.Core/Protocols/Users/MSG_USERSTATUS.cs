using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("uSta")]
    public sealed class MSG_USERSTATUS : IProtocolReceive, IProtocolSend
    {
        public UserFlags flags;
        public Guid hash;

        public void Deserialize(Packet packet, params object[] values)
        {
            flags = (UserFlags)packet.ReadSInt16();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                flags = jsonResponse.flags;
                hash = Guid.Parse(jsonResponse.hash);
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16((short)flags);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                flags,
                hash = hash.ToString(),
            });
        }
    }
}
