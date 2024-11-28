using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ThePalace.Core.Constants;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Models.Protocols
{
    public class AssetRec : IProtocolRec
    {
        public AssetSpec assetSpec = new();
        //public Int32 rHandle;
        public int blockOffset;
        public uint blockSize;
        public int lastUseTime;
        public int nameOffset;
        public ushort blockNbr;
        public ushort nbrBlocks;
        public int propFormat;
        public ushort assetFlags;
        public ushort propFlags;
        public uint size;
        //public LegacyAssetTypes type;
        public string name;
        public string Format;
        public byte[] data;

        public short Width;
        public short Height;
        public Palace.Point Offset;
        public Bitmap Image;
        public string md5;

        protected bool LoResIsHead => (propFlags & (int)LoResPropFlags.PF_Head) != 0;
        protected bool LoResIsGhost => (propFlags & (int)LoResPropFlags.PF_Ghost) != 0;
        protected bool LoResIsRare => (propFlags & (int)LoResPropFlags.PF_Rare) != 0;
        protected bool LoResIsAnimate => (propFlags & (int)LoResPropFlags.PF_Animate) != 0;
        protected bool LoResIsPalindrome => (propFlags & (int)LoResPropFlags.PF_Palindrome) != 0;
        protected bool LoResIsBounce => (propFlags & (int)LoResPropFlags.PF_Bounce) != 0;

        protected bool HiResIsHead => (propFlags & (int)HiResPropFlags.PF_Head) != 0;
        protected bool HiResIsGhost => (propFlags & (int)HiResPropFlags.PF_Ghost) != 0;
        protected bool HiResIsRare => false;
        protected bool HiResIsAnimate => (propFlags & (int)HiResPropFlags.PF_Animate) != 0;
        protected bool HiResIsPalindrome => false;
        protected bool HiResIsBounce => (propFlags & (int)HiResPropFlags.PF_Bounce) != 0;

        public bool IsHead
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsHead;
                else
                    return LoResIsHead;
            }
        }
        public bool IsGhost
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsGhost;
                else
                    return LoResIsGhost;
            }
        }
        public bool IsRare
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsRare;
                else
                    return LoResIsRare;
            }
        }
        public bool IsAnimate
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsAnimate;
                else
                    return LoResIsAnimate;
            }
        }
        public bool IsPalindrome
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsPalindrome;
                else
                    return LoResIsPalindrome;
            }
        }
        public bool IsBounce
        {
            get
            {
                if (IsLegacy16Bit ||
                    IsLegacy20Bit ||
                    IsLegacyS20Bit ||
                    IsLegacy32Bit)
                    return HiResIsBounce;
                else
                    return LoResIsBounce;
            }
        }

        public bool IsLegacy16Bit => (propFlags & 0xFFC1) == (int)PropFormats.PF_16Bit;
        public bool IsLegacyS20Bit => (propFormat & (int)PropFormats.PF_S20Bit) != 0;
        public bool IsLegacy20Bit => (propFormat & (int)PropFormats.PF_20Bit) != 0;
        public bool IsLegacy32Bit => (propFormat & (int)PropFormats.PF_32Bit) != 0;
        public bool IsCustom32Bit => (propFlags & (int)PropFormats.PF_Custom32Bit) != 0;

        public AssetRec() { }
        public AssetRec(int ID) =>
            this.assetSpec = new(ID);
        public AssetRec(AssetSpec assetSpec) =>
            this.assetSpec = assetSpec;
        public AssetRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose()
        {
            try { this.Image?.Dispose(); this.Image = null; } catch { }
        }

        public void Deserialize(Packet packet, params object[] values)
        {
            packet.DropBytes(4); //type = (LegacyAssetTypes)packet.ReadSInt32();
            assetSpec = new AssetSpec(packet);
            blockSize = packet.ReadUInt32();
            blockOffset = packet.ReadSInt32();
            blockNbr = packet.ReadUInt16();
            nbrBlocks = packet.ReadUInt16();

            if (blockNbr < 1)
            {
                assetFlags = packet.ReadUInt16();
                size = packet.ReadUInt32();
                name = packet.ReadPString(32, 1);

                if (blockSize > size ||
                    blockSize > AssetConstants.ASSET_MAX_BLOCK_SIZE ||
                    blockSize < 0 ||
                    size < 1 ||
                    size > AssetConstants.ASSET_MAX_BLOCK_SIZE)
                    return;

                Width = packet.ReadSInt16();
                Height = packet.ReadSInt16();
                Offset = new();
                Offset.h = packet.ReadSInt16();
                Offset.v = packet.ReadSInt16();
                packet.DropBytes(2);
                propFlags = packet.ReadUInt16();

                propFormat = propFlags & (short)PropFormats.PF_Mask;

                data = packet.GetData((int)blockSize - 12);
            }
            else
                data = packet.GetData((int)blockSize);
        }

        public void DeserializePROPS(Packet packet)
        {
            Width = packet.ReadSInt16().SwapShort();
            Height = packet.ReadSInt16().SwapShort();
            Offset = new();
            Offset.h = packet.ReadSInt16().SwapShort();
            Offset.v = packet.ReadSInt16().SwapShort();
            packet.DropBytes(2);
            propFlags = packet.ReadUInt16().SwapShort();

            propFormat = propFlags & (short)PropFormats.PF_Mask;
        }

        public void DeserializePRP(Packet packet)
        {
            assetSpec = new AssetSpec();

            assetSpec.id = packet.ReadSInt32();                     // AssetID
            packet.DropBytes(4);                                    // rHandle
            blockOffset = packet.ReadSInt32();                      // blockOffset
            blockSize = packet.ReadUInt32();                        // blockSize
            lastUseTime = packet.ReadSInt32();                      // lastUseTime
            nameOffset = packet.ReadSInt32();                       // nameOffset
            assetFlags = packet.ReadUInt16();                       // flags
            assetSpec.crc = packet.ReadUInt32();                    // crc
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(0);                               // type
                packet.WriteBytes(assetSpec.Serialize());           // propSpec
                packet.WriteInt32(blockSize);                       // blockSize
                packet.WriteInt32(blockOffset);                     // blockOffset
                packet.WriteInt32(blockNbr);                        // blockNbr
                packet.WriteInt32(nbrBlocks);                       // nbrBlocks

                if (blockNbr < 1)
                {
                    packet.WriteInt16(assetFlags);                  // flags
                    packet.WriteInt32(size);                        // size
                    packet.WritePString(name, 32, 1, true);         // name

                    packet.WriteInt16(Width);
                    packet.WriteInt16(Height);
                    packet.WriteInt16(Offset.h);
                    packet.WriteInt16(Offset.v);
                    packet.WriteInt16(0);
                    packet.WriteInt16(propFlags);
                }

                return packet.GetData((int)blockSize, blockOffset);
            }
        }

        public byte[] SerializePROPS(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(Width.SwapShort());
                packet.WriteInt16(Height.SwapShort());
                packet.WriteInt16(Offset.h.SwapShort());
                packet.WriteInt16(Offset.v.SwapShort());
                packet.WriteInt16(0);
                packet.WriteInt16(propFlags.SwapShort());

                return packet.GetData();
            }
        }

        public byte[] SerializePRP(params object[] values)
        {
            var appendName = values.Length > 0 ? (bool)values[0] : true;

            using (var packet = new Packet())
            {
                packet.WriteInt32(assetSpec.id);                    // AssetID
                packet.WriteInt32(0);                               // rHandle
                packet.WriteInt32(blockOffset);                     // blockOffset
                packet.WriteInt32(blockSize);                       // blockSize
                packet.WriteInt32(lastUseTime);                     // lastUseTime
                packet.WriteInt32(nameOffset);                      // nameOffset
                packet.WriteInt32(assetFlags);                      // flags
                packet.WriteInt32(assetSpec.crc);                   // crc

                if (appendName)
                    packet.WritePString(name, 32, 1, true);         // name

                return packet.GetData();
            }
        }

        public bool ValidateCrc() =>
            (this.data?.Length ?? 0) < 1 ? false : Cipher.ComputeCrc(this.data, 0, true) == assetSpec.crc;

        public bool ValidateCrc(UInt32 crc) =>
            Cipher.ComputeCrc(this.data, 0, true) == crc;

        public static Bitmap Render(AssetRec asset)
        {
            if (asset.IsCustom32Bit)
                return RenderCustom32bit(asset);
            else if (asset.IsLegacy32Bit)
                return RenderLegacy32bit(asset);
            else if (asset.IsLegacy16Bit)
                return RenderLegacy16bit(asset);
            else if (asset.IsLegacyS20Bit)
                return RenderLegacyS20bit(asset);
            else if (asset.IsLegacy20Bit)
                return RenderLegacy20bit(asset);
            else
                return RenderLegacy8bit(asset);
        }

        private static Bitmap RenderLegacy8bit(AssetRec asset)
        {
            var result = new Bitmap(asset.Width, asset.Height);
            if (result == null) throw new OutOfMemoryException();

            var pixelIndex = 0;
            var counter = 0;
            var ofst = 0;

            for (var y = asset.Height - 1; ofst < asset.data.Length && y >= 0; y--)
                for (var x = (int)asset.Width; ofst < asset.data.Length && x > 0;)
                {
                    var cb = asset.data[ofst++];
                    var pc = (byte)(cb & 0x0F);
                    var mc = (byte)(cb >> 4);
                    x -= mc + pc;

                    if (x < 0 ||
                        counter++ > 6000)
                        throw new Exception("Bad Prop");

                    pixelIndex += mc;

                    while (pc-- > 0 &&
                        ofst < asset.data.Length)
                    {
                        cb = asset.data[ofst++];

                        var _x = pixelIndex % asset.Width;
                        var _y = pixelIndex / asset.Height % asset.Height;
                        var colour = Color.FromArgb((Int32)AssetConstants.PalacePalette[cb]);

                        result.SetPixel(_x, _y, colour);

                        pixelIndex++;
                    }
                }

            return result;
        }
        private static Bitmap RenderLegacy16bit(AssetRec asset)
        {
            var result = new Bitmap(asset.Width, asset.Height);
            if (result == null) throw new OutOfMemoryException();

            var inflatedData = InflateData(asset.data) ?? asset.data;

            if (inflatedData == null ||
                inflatedData.Length < (1936 * 2))
                throw new Exception("Bad Prop");

            // Implementation thanks to Phalanx team
            // Translated from C++ implementation
            // Translated from ActionScript implementation (Turtle)

            var ditherS20bit = 255 / 31;
            var colour = Color.White;
            var ofst = 0;
            var x = 0;
            var y = 0;
            var a = 0;
            var r = 0;
            var g = 0;
            var b = 0;
            var C = 0;

            for (x = 0; x < 1936; x++)
            {
                ofst = x * 2;

                C = (inflatedData[ofst] << 8) | inflatedData[ofst + 1];
                r = (((inflatedData[ofst] >> 3) & 31) * ditherS20bit) & 0xFF;
                g = (((C >> 6) & 31) * ditherS20bit) & 0xFF;
                b = (((C >> 1) & 31) * ditherS20bit) & 0xFF;
                a = (C & 1) * 255 & 0xFF;

                colour = Color.FromArgb(a, r, g, b);
                var _x = y % asset.Width;
                var _y = y / asset.Height;
                result.SetPixel(_x, _y, colour);
                y++;
            }

            return result;
        }
        private static Bitmap RenderLegacy20bit(AssetRec asset)
        {
            var result = new Bitmap(asset.Width, asset.Height);
            if (result == null) throw new OutOfMemoryException();

            var inflatedData = InflateData(asset.data) ?? asset.data;

            if (inflatedData == null ||
                inflatedData.Length < (968 * 5))
                throw new Exception("Bad Prop");

            // Implementation thanks to Phalanx team
            // Translated from C++ implementation
            // Translated from ActionScript implementation (Turtle)

            var dither20bit = 255 / 63;
            var colour = Color.White;
            var ofst = 0;
            var x = 0;
            var y = 0;
            var a = 0;
            var r = 0;
            var g = 0;
            var b = 0;
            var C = 0;

            for (x = 0, y = 0; x < 968; x++)
            {
                ofst = x * 5;

                r = ((inflatedData[ofst] >> 2) & 63) * dither20bit;
                C = (inflatedData[ofst] << 8) | inflatedData[ofst + 1];
                g = ((C >> 4) & 63) * dither20bit;
                C = (inflatedData[ofst + 1] << 8) | inflatedData[ofst + 2];
                b = ((C >> 6) & 63) * dither20bit;
                a = ((C >> 4) & 3) * 85;

                colour = Color.FromArgb(a, r, g, b);
                var _x = y % asset.Width;
                var _y = y / asset.Height;
                result.SetPixel(_x, _y, colour);
                y++;

                C = (inflatedData[ofst + 2] << 8) | inflatedData[ofst + 3];
                r = ((C >> 6) & 63) * dither20bit;
                g = (C & 63) * dither20bit;
                C = inflatedData[ofst + 4];
                b = ((C >> 2) & 63) * dither20bit;
                a = (C & 3) * 85;

                colour = Color.FromArgb(a, r, g, b);
                _x = y % asset.Width;
                _y = y / asset.Height;
                result.SetPixel(_x, _y, colour);
                y++;
            }

            return result;
        }
        private static Bitmap RenderLegacyS20bit(AssetRec asset)
        {
            var result = new Bitmap(asset.Width, asset.Height);
            if (result == null) throw new OutOfMemoryException();

            var inflatedData = InflateData(asset.data) ?? asset.data;

            if (inflatedData == null ||
                inflatedData.Length < (968 * 5))
                throw new Exception("Bad Prop");

            // Implementation thanks to Phalanx team
            // Translated from C++ implementation
            // Translated from ActionScript implementation (Turtle)

            var ditherS20bit = 255 / 31;
            var colour = Color.White;
            var ofst = 0;
            var x = 0;
            var y = 0;
            var a = 0;
            var r = 0;
            var g = 0;
            var b = 0;
            var C = 0;

            for (x = 0, y = 0; x < 968; x++)
            {
                ofst = x * 5;

                r = (((inflatedData[ofst] >> 3) & 31) * ditherS20bit) & 0xFF;
                C = (inflatedData[ofst] << 8) | inflatedData[ofst + 1];
                g = ((C >> 6 & 31) * ditherS20bit) & 0xFF;
                b = ((C >> 1 & 31) * ditherS20bit) & 0xFF;
                C = (inflatedData[ofst + 1] << 8) | inflatedData[ofst + 2];
                a = ((C >> 4 & 31) * ditherS20bit) & 0xFF;

                colour = Color.FromArgb(a, r, g, b);
                var _x = y % asset.Width;
                var _y = y / asset.Height;
                result.SetPixel(_x, _y, colour);
                y++;

                C = (inflatedData[ofst + 2] << 8) | inflatedData[ofst + 3];
                r = ((C >> 7 & 31) * ditherS20bit) & 0xFF;
                g = ((C >> 2 & 31) * ditherS20bit) & 0xFF;
                C = (inflatedData[ofst + 3] << 8) | inflatedData[ofst + 4];
                b = ((C >> 5 & 31) * ditherS20bit) & 0xFF;
                a = ((C & 31) * ditherS20bit) & 0xFF;

                colour = Color.FromArgb(a, r, g, b);
                _x = y % asset.Width;
                _y = y / asset.Height;
                result.SetPixel(_x, _y, colour);
                y++;
            }

            return result;
        }
        private static Bitmap RenderLegacy32bit(AssetRec asset)
        {
            var inflatedData = InflateData(asset.data) ?? asset.data;

            var imageDataSize = asset.Width * asset.Height * 4;
            if (inflatedData == null ||
                inflatedData.Length != imageDataSize)
                throw new Exception("Bad Prop");

            // Implementation thanks to Phalanx team
            // Translated from C++ implementation
            // Translated from ActionScript implementation (Turtle)

            return RenderByteArray(inflatedData);
        }
        private static Bitmap RenderCustom32bit(AssetRec asset)
        {
            var result = RenderByteArray(asset.data);
            if (result != null)
                return result;

            return null;
        }
        private static Bitmap RenderByteArray(byte[] data)
        {
            using (var memInput = new MemoryStream(data))
            {
                try
                {
                    var result = new Bitmap(memInput);
                    if (result == null) throw new OutOfMemoryException();

                    return result;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                    return null;
                }
            }
        }

        private static byte[] InflateData(byte[] byteInput)
        {
            var types = new Type[]
            {
                    typeof(InflaterInputStream),
                    typeof(ZipInputStream),
                    typeof(GZipInputStream),
            };

            foreach (var type in types)
                try
                {
                    using (var memOutput = new MemoryStream())
                    {
                        using (var memInput = new MemoryStream(byteInput))
                        using (var zipInput = type.GetInstance(memInput) as InflaterInputStream)
                            zipInput.CopyTo(memOutput);

                        return memOutput.ToArray();
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

            return null;
        }

        public static int SizeOf => 32;
    }
}
