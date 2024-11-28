using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Assets
{
    [Description("prPn")]
    public sealed class MSG_PROPNEW : IProtocolReceive, IProtocolSend
    {
        public AssetSpec assetSpec;
        public Point loc;

        public void Deserialize(Packet packet, params object[] values)
        {
            assetSpec = new AssetSpec(packet);
            loc = new Point(packet);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                var id = (int)jsonResponse.propSpec.id;
                var crc = (uint)jsonResponse.propSpec.crc;
                var v = (short)jsonResponse.loc.v;
                var h = (short)jsonResponse.loc.h;

                assetSpec = new AssetSpec(id, crc);
                loc = new Point(h, v);
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteBytes(assetSpec.Serialize());
                packet.WriteBytes(loc.Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                assetSpec,
                loc,
            });
        }
    }
}
