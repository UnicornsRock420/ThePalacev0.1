using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Palace
{
    public class AssetTypeRec : IProtocolRec
    {
        public LegacyAssetTypes type;
        public uint nbrAssets;
        public uint firstAsset;

        public AssetTypeRec() { }
        public AssetTypeRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            type = (LegacyAssetTypes)packet.ReadSInt32();
            nbrAssets = packet.ReadUInt32();
            firstAsset = packet.ReadUInt32();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32((int)type);
                packet.WriteInt32(nbrAssets);
                packet.WriteInt32(firstAsset);

                return packet.GetData();
            }
        }

        public static int SizeOf => 12;
    }
}
