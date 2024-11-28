using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("usrP")]
    public sealed class MSG_USERPROP : IProtocolReceive, IProtocolSend
    {
        public List<AssetSpec> assetSpec = null;

        public void Deserialize(Packet packet, params object[] values)
        {
            var nbrProps = (short)packet.ReadSInt32();

            assetSpec = new();

            for (var j = 0; j < nbrProps; j++)
                assetSpec.Add(new AssetSpec(packet));
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                var nbrProps = (short)jsonResponse.propSpec.Count;

                assetSpec = new();

                for (var j = 0; j < nbrProps; j++)
                {
                    var id = (int)jsonResponse.propSpec[j].id;
                    var crc = (uint)jsonResponse.propSpec[j].crc;

                    assetSpec.Add(new AssetSpec(id, crc));
                }
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                var count = assetSpec?.Count ?? 0;

                packet.WriteInt32(count);

                for (var j = 0; j < count; j++)
                    packet.WriteBytes(assetSpec[j].Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                assetSpec,
            });
        }
    }
}
