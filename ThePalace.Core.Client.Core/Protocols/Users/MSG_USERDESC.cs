using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("usrD")]
    public sealed class MSG_USERDESC : IProtocolReceive, IProtocolSend
    {
        public short faceNbr;
        public short colorNbr;
        public int nbrProps;
        public List<AssetSpec> assetSpec;

        public void Deserialize(Packet packet, params object[] values)
        {
            faceNbr = packet.ReadSInt16();
            colorNbr = packet.ReadSInt16();
            nbrProps = packet.ReadSInt32();

            assetSpec = new();

            if (nbrProps > 0)
                for (var j = 0; j < nbrProps; j++)
                    assetSpec.Add(new AssetSpec(packet));
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                faceNbr = jsonResponse.faceNbr;
                colorNbr = jsonResponse.colorNbr;

                assetSpec = new();

                if (nbrProps > 0)
                    for (var j = 0; j < nbrProps; j++)
                    {
                        var id = (int)jsonResponse.propSpec[j].id;
                        var crc = (uint)jsonResponse.propSpec[j].crc;

                        assetSpec[j] = new AssetSpec(id, crc);
                    }
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(faceNbr);
                packet.WriteInt16(colorNbr);
                packet.WriteInt32(assetSpec?.Count ?? 0);

                for (var j = 0; j < nbrProps; j++)
                    packet.WriteBytes(assetSpec[j].Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                faceNbr = faceNbr,
                colorNbr = colorNbr,
                propSpec = assetSpec,
            });
        }
    }
}
