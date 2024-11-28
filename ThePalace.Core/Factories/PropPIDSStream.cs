using System;
using System.Collections.Generic;
using System.IO;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Factories
{
    public sealed class PropPIDSStream : StreamBase
    {
        public PropPIDSStream() { }
        ~PropPIDSStream() =>
            this.Dispose(false);

        public void Read(PropPROPSStream filePROPSReader, out List<AssetRec> assets)
        {
            assets = new();

            var fileSize = filePROPSReader.Filesize;
            var _fileHeader = new FilePIDSHeaderRec();
            var data = new byte[FilePIDSHeaderRec.SizeOf];
            var read = 0;

            _fileStream.Seek(0, SeekOrigin.Begin);

            while (true)
            {
                read = _fileStream.Read(data, 0, FilePIDSHeaderRec.SizeOf);

                if (read == 0 ||
                    read < FilePIDSHeaderRec.SizeOf) break;

                else if (read == FilePIDSHeaderRec.SizeOf)
                {
                    var asset = null as AssetRec;
                    try
                    {
                        using (var tmp = Packet.FromBytes(data))
                            _fileHeader.Deserialize(tmp);

                        if (_fileHeader.dataOffset > fileSize ||
                            (_fileHeader.dataOffset + _fileHeader.dataSize) > fileSize)
                            continue;

                        asset = new(_fileHeader.assetSpec);

                        filePROPSReader.Read(_fileHeader.dataOffset, _fileHeader.dataSize, ref asset);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    if (asset != null)
                        assets.Add(asset);

                    continue;
                }

                else break;
            }
        }

        public void Write(PropPROPSStream filePROPSReader, params AssetRec[] assets)
        {
            var dataOffset = 0;

            foreach (var asset in assets)
            {
                var dataSize = filePROPSReader.Write(asset);
                _fileStream.Write(
                    new FilePIDSHeaderRec(asset.assetSpec)
                    {
                        dataSize = dataSize,
                        dataOffset = dataOffset,
                    }.Serialize());
                dataOffset += dataSize;
            }
        }
    }
}
