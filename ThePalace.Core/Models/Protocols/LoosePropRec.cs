using Newtonsoft.Json;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Models.Protocols
{
    public class LoosePropRec : IProtocolRec
    {
        [JsonIgnore]
        public short nextOfst;
        //[JsonIgnore]
        //public short reserved;
        public AssetSpec assetSpec;
        public int flags;
        //[JsonIgnore]
        //public int refCon;
        public Point loc;

        public LoosePropRec() { }
        public LoosePropRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            nextOfst = packet.ReadSInt16();
            packet.DropBytes(2); //reserved
            assetSpec = new AssetSpec(packet);
            flags = packet.ReadSInt32();
            packet.DropBytes(4); //refCon
            loc = new Point(packet);
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(nextOfst);
                packet.WriteInt16(0); //reserved
                packet.WriteBytes(assetSpec.Serialize());
                packet.WriteInt32(flags);
                packet.WriteInt32(0); //refCon
                packet.WriteBytes(loc.Serialize());

                return packet.GetData();
            }
        }

        public static int SizeOf => 24;
    }
}
