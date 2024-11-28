using System;
using System.IO;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Factories
{
    public sealed class PropPROPSStream : StreamBase
    {
        private const int INT_NAMEBLOCKSIZE = 32;

        public PropPROPSStream() { }
        ~PropPROPSStream() =>
            this.Dispose(false);

        public long Filesize
        {
            get
            {
                _fileStream.Seek(0, SeekOrigin.End);
                return _fileStream.Position;
            }
        }

        public long Position =>
            _fileStream.Position;

        public void Read(Int32 dataOffset, Int32 dataSize, ref AssetRec asset)
        {
            _fileStream.Seek(dataOffset, SeekOrigin.Begin);

            var data = new byte[dataSize + INT_NAMEBLOCKSIZE];
            var read = _fileStream.Read(data, 0, data.Length);

            if (read == data.Length)
            {
                using (var _data = new Packet(data))
                {
                    asset.name = _data.ReadPString(INT_NAMEBLOCKSIZE, 1);
                    asset.DeserializePROPS(_data);

                    if (asset.IsCustom32Bit)
                        asset.data = _data.GetData(0, 4856);
                    else
                        asset.data = _data.GetData();
                }

                return;
            }

            throw new Exception("Bad Read");
        }

        public int Write(AssetRec asset)
        {
            var dataSize = 0;

            var buffer = asset.name.WritePString(INT_NAMEBLOCKSIZE, 1, true);
            _fileStream.Write(buffer);
            dataSize += buffer.Length;

            buffer = asset.SerializePROPS();
            _fileStream.Write(buffer);
            dataSize += buffer.Length;

            if (asset.IsCustom32Bit)
            {
                //mysteryBytes
                buffer = new byte[4856];
                _fileStream.Write(buffer);
                dataSize += buffer.Length;
            }

            _fileStream.Write(asset.data);
            dataSize += asset.data.Length;

            return dataSize;
        }
    }
}
