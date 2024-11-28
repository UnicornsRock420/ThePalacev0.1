using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Palace
{
    public class MapHeaderRec : IProtocolRec
    {
        public int nbrTypes;
        public int nbrAssets;
        public int lenNames;
        public int typesOffset;
        public int recsOffset;
        public int namesOffset;

        public MapHeaderRec() { }

        public MapHeaderRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(nbrTypes);
                packet.WriteInt32(nbrAssets);
                packet.WriteInt32(lenNames);
                packet.WriteInt32(typesOffset);
                packet.WriteInt32(recsOffset);
                packet.WriteInt32(namesOffset);

                return packet.GetData();
            }
        }

        public static int SizeOf => 24;
    }
}
