using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("usrF")]
    public class MSG_USERFACE : IProtocolReceive, IProtocolSend
    {
        public Int16 faceNbr;

        public void Deserialize(Packet packet, params object[] args)
        {
            faceNbr = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] args)
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

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                faceNbr = jsonResponse.faceNbr;
            }
            catch
            {
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                faceNbr = faceNbr,
            });
        }
    }
}
