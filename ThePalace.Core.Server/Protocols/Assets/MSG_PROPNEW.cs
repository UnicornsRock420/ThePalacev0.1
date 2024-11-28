using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Server.Protocols
{
    [Description("prPn")]
    public class MSG_PROPNEW : IProtocolReceive, IProtocolSend
    {
        public AssetSpec propSpec;
        public Point loc;

        public void Deserialize(Packet packet, params object[] args)
        {
            propSpec = new AssetSpec(packet);
            loc = new Point(packet);
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteBytes(propSpec.Serialize());
                packet.WriteBytes(loc.Serialize());

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                var id = (Int32)jsonResponse.propSpec.id;
                var crc = (UInt32)jsonResponse.propSpec.crc;
                var v = (Int16)jsonResponse.loc.v;
                var h = (Int16)jsonResponse.loc.h;

                propSpec = new AssetSpec(id, crc);
                loc = new Point(h, v);
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                propSpec = propSpec,
                loc = loc,
            });
        }
    }
}
