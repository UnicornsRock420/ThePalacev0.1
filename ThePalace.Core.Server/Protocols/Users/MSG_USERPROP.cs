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
    [Description("usrP")]
    public class MSG_USERPROP : IProtocolReceive, IProtocolSend
    {
        public Int16 nbrProps;
        public List<AssetSpec> assetSpec;

        public void Deserialize(Packet packet, params object[] args)
        {
            nbrProps = (Int16)packet.ReadSInt32();

            assetSpec = new();

            for (var j = 0; j < nbrProps; j++)
                assetSpec.Add(new AssetSpec(packet));
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(nbrProps);

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
                nbrProps = (Int16)jsonResponse.propSpec.Count;
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

                assetSpec.Add(new AssetSpec(id, crc));
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                propSpec = assetSpec,
            });
        }
    }
}
