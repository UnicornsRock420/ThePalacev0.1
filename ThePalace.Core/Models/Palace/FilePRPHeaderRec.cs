using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Palace
{
    public class FilePRPHeaderRec : IProtocolRec
    {
        public int dataOffset;
        public int dataSize;
        public int assetMapOffset;
        public int assetMapSize;

        public FilePRPHeaderRec() { }
        public FilePRPHeaderRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            dataOffset = packet.ReadSInt32();
            dataSize = packet.ReadSInt32();
            assetMapOffset = packet.ReadSInt32();
            assetMapSize = packet.ReadSInt32();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(dataOffset);
                packet.WriteInt32(dataSize);
                packet.WriteInt32(assetMapOffset);
                packet.WriteInt32(assetMapSize);

                return packet.GetData();
            }
        }

        public static int SizeOf => 16;
    }
}
