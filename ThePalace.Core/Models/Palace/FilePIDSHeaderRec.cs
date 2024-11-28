using System;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Palace
{
    public class FilePIDSHeaderRec : IProtocolRec
    {
        public AssetSpec assetSpec;
        public int dataOffset;
        public int dataSize;

        public FilePIDSHeaderRec() { }
        public FilePIDSHeaderRec(AssetSpec assetSpec)
        {
            this.assetSpec = assetSpec;
        }
        public FilePIDSHeaderRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            assetSpec = new();
            assetSpec.id = packet
                .ReadSInt32()
                .SwapInt();
            assetSpec.crc = packet
                .ReadUInt32()
                .SwapInt();
            dataOffset = packet
                .ReadSInt32()
                .SwapInt();
            dataSize = packet
                .ReadSInt32()
                .SwapInt();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(assetSpec.id.SwapInt());
                packet.WriteInt32(assetSpec.crc.SwapInt());
                packet.WriteInt32(dataOffset.SwapInt());
                packet.WriteInt32(dataSize.SwapInt());

                return packet.GetData();
            }
        }

        public static int SizeOf => 16;
    }
}
