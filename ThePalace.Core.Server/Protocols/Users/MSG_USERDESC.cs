using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("usrD")]
    public class MSG_USERDESC : IProtocolReceive, IProtocolSend
    {
        public Int16 faceNbr;
        public Int16 colorNbr;
        public Int32 nbrProps;
        public List<AssetSpec> assetSpec;

        public void Deserialize(Packet packet, params object[] args)
        {
            faceNbr = packet.ReadSInt16();
            colorNbr = packet.ReadSInt16();
            nbrProps = packet.ReadSInt32() % 10;

            assetSpec = new();

            for (var j = 0; j < nbrProps; j++)
                assetSpec.Add(new AssetSpec(packet));
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(faceNbr);
                packet.WriteInt16(colorNbr);
                packet.WriteInt32(assetSpec?.Count ?? 0);

                for (var j = 0; j < nbrProps; j++)
                {
                    packet.WriteBytes(assetSpec[j].Serialize());
                }

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
                colorNbr = jsonResponse.colorNbr;

                nbrProps = jsonResponse.propSpec.Count;
            }
            catch
            {
                nbrProps = 0;
            }

            assetSpec = new();

            for (var j = 0; j < nbrProps; j++)
            {
                var id = (Int32)jsonResponse.propSpec[j].id;
                var crc = (UInt32)jsonResponse.propSpec[j].crc;

                assetSpec[j] = new AssetSpec(id, crc);
            }
        }

        public string SerializeJSON(params object[] args)
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
