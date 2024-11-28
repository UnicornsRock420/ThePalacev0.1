using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Palace
{
    public class AssetSpec : IProtocolRec
    {
        public int id;
        public uint crc;

        public AssetSpec()
        {
            id = 0;
            crc = 0;
        }

        public AssetSpec(int ID)
        {
            id = ID;
            crc = 0;
        }

        public AssetSpec(int ID, uint Crc)
        {
            id = ID;
            crc = Crc;
        }

        public AssetSpec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            id = packet.ReadSInt32();
            crc = packet.ReadUInt32();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(id);
                packet.WriteInt32(crc);

                return packet.GetData();
            }
        }
    }
}
