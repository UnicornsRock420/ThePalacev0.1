using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("qAst")]
    public class MSG_ASSETQUERY : IProtocolReceive, IProtocolSend
    {
        public LegacyAssetTypes assetType;
        public AssetSpec assetSpec;

        public void Deserialize(Packet packet, params object[] args)
        {
            assetType = (LegacyAssetTypes)packet.ReadSInt32();
            assetSpec = new AssetSpec(packet);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                assetType = (LegacyAssetTypes)jsonResponse.assetType;
                assetSpec = new AssetSpec(jsonResponse.assetSpec.id, jsonResponse.assetSpec.crc);
            }
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32((int)assetType);
                packet.WriteBytes(assetSpec.Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                assetType = assetType.ToString(),
                assetSpec = assetSpec,
            });
        }
    }
}
