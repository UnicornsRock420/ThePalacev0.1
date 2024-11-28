using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using ThePalace.Core.Constants;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core.Factories;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Factories
{
    public class AssetStream : ChangeTracking, IDisposable, IProtocolSend
    {
        public AssetRec assetRec;

        private UInt32 _chunkMaxSize;
        private UInt32 _bytesRead;
        private MemoryStream _stream;
        private byte[] _buffer;

        public AssetStream(UInt32 chunkMaxSize = NetworkConstants.ASSET_STREAM_BUFFER_SIZE) : base()
        {
            _chunkMaxSize = (chunkMaxSize > NetworkConstants.ASSET_STREAM_BUFFER_SIZE) ? NetworkConstants.ASSET_STREAM_BUFFER_SIZE : chunkMaxSize;
            _buffer = new byte[NetworkConstants.FILE_STREAM_BUFFER_SIZE];
        }

        public AssetStream(AssetRec asset, UInt32 chunkMaxSize = NetworkConstants.ASSET_STREAM_BUFFER_SIZE) : base()
        {
            _buffer = new byte[NetworkConstants.FILE_STREAM_BUFFER_SIZE];

            assetRec = asset;

            AlignBytes((int)assetRec.size);
        }

        public bool Open(AssetSpec assetSpec)
        {
            try
            {
                if (AssetLoader.assetsCache.ContainsKey(assetSpec.id))
                {
                    assetRec = AssetLoader.assetsCache[assetSpec.id];
                }
                else
                {
                    using (var dbContext = DbConnection.For<ThePalaceEntities>())
                    {
                        assetRec = dbContext.Assets.AsNoTracking()
                            .Where(a => a.AssetId == assetSpec.id)
                            .Where(a => (int)assetSpec.crc == 0 || a.AssetCrc == (int)assetSpec.crc)
                            .Where(a => (a.Flags & (Int32)PropFormats.PF_Custom32Bit) == 0)
                            .AsEnumerable()
                            .Select(a => new AssetRec
                            {
                                //type = LegacyAssetTypes.RT_PROP,
                                assetSpec = new AssetSpec(a.AssetId, (UInt32)a.AssetCrc),
                                name = a.Name,
                                propFlags = (UInt16)a.Flags,
                                size = (UInt32)(a.Data?.Length ?? 0),
                                data = a.Data,
                            })
                            .FirstOrDefault();
                    }
                }

                if (assetRec != null && assetRec.size > 0)
                {
                    assetRec.nbrBlocks = (UInt16)((assetRec.size / _chunkMaxSize) + ((assetRec.size % _chunkMaxSize) > 0 ? 1 : 0));
                    assetRec.blockSize = (assetRec.size - _bytesRead > _chunkMaxSize) ?
                        ((_chunkMaxSize > assetRec.size - _bytesRead) ? assetRec.size - _bytesRead : _chunkMaxSize) :
                        (assetRec.size - _bytesRead > 0) ? assetRec.size - _bytesRead : 0;

                    AlignBytes((int)assetRec.size);

                    if (assetRec.size <= 9000)
                    {
                        AssetLoader.assetsCache[assetSpec.id] = assetRec;
                    }

                    _stream = new MemoryStream(assetRec.data);

                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public AssetRec AsRecord
        {
            get => new AssetRec
            {
                //type = assetRec.type,
                assetSpec = assetRec.assetSpec,
                propFlags = assetRec.propFlags,
                size = assetRec.size,
                name = assetRec.name,
                data = GetData(),
            };
        }

        public bool Write()
        {
            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                try
                {
                    var asset = dbContext.Assets
                        .Where(a => a.AssetId == assetRec.assetSpec.id)
                        //.Where(a => (int)assetRec.propSpec.crc == 0 || a.AssetCrc == (int)assetRec.propSpec.crc)
                        .FirstOrDefault();

                    if (asset == null)
                    {
                        if (!HasUnsavedChanges)
                        {
                            assetRec.data = GetData();
                        }
                        asset = new Assets
                        {
                            AssetId = assetRec.assetSpec.id,
                            AssetCrc = (int)assetRec.assetSpec.crc,
                            Flags = assetRec.propFlags,
                            Name = assetRec.name,
                            LastUsed = DateTime.UtcNow,
                            Data = assetRec.data,
                        };

                        dbContext.Assets.Add(asset);

                        if (assetRec.size <= 9000)
                        {
                            AssetLoader.assetsCache[asset.AssetId] = AsRecord;
                        }

                        Logger.ConsoleLog($"Storing Asset {assetRec.assetSpec.id}");
                    }

                    if (dbContext.HasUnsavedChanges())
                    {
                        dbContext.SaveChanges();
                        AcceptChanges();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return false;
        }

        public bool hasData
        {
            get => (assetRec.size - _bytesRead) > 0 && assetRec.blockSize > 0;
        }

        public void CopyChunkData(AssetStream chunk)
        {
            if (!HasUnsavedChanges)
            {
                NotifyPropertyChanged();
                _bytesRead = 0;
            }

            if (chunk.hasData)
            {
                for (int i = 0; i < chunk.assetRec.blockSize; i++)
                {
                    _data[i + chunk.assetRec.blockOffset] = chunk.assetRec.data[i];
                }
                _bytesRead += chunk.assetRec.blockSize;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            if (_stream != null && _stream.CanRead && hasData)
            {
                assetRec.blockSize = (assetRec.size - _bytesRead > _chunkMaxSize) ?
                    ((_chunkMaxSize > assetRec.size - _bytesRead) ? assetRec.size - _bytesRead : _chunkMaxSize) :
                    (assetRec.size - _bytesRead > 0) ? assetRec.size - _bytesRead : 0;

                try
                {
                    var read = _stream.Read(_buffer, 0, (int)assetRec.blockSize);
                    var buffer = _buffer;

                    _bytesRead += (UInt32)read;

                    _data.Clear();
                    WriteInt32((int)LegacyAssetTypes.RT_PROP);
                    WriteBytes(assetRec.assetSpec.Serialize());
                    WriteInt32(assetRec.blockSize);
                    WriteInt32(assetRec.blockOffset);
                    WriteInt16(assetRec.blockNbr);
                    WriteInt16(assetRec.nbrBlocks);

                    if (assetRec.blockNbr < 1)
                    {
                        WriteInt32(assetRec.propFlags);
                        WriteInt32(assetRec.size);
                        WritePString(assetRec.name, 32);
                    }
                    else if (_bytesRead >= assetRec.size)
                    {
                        buffer = _buffer.Take((int)assetRec.blockSize).ToArray();
                    }

                    WriteBytes(buffer);

                    assetRec.blockNbr++;
                    assetRec.blockOffset += read;

                    return GetData();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    Dispose();
                }
            }

            return null;
        }

        public string SerializeJSON(params object[] values)
        {
            return string.Empty;
        }

        public new void Dispose()
        {
            base.Dispose();

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            if (_buffer != null)
            {
                _buffer = null;
            }
        }
    }
}
