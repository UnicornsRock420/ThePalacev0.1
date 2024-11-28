﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThePalace.Core.Constants;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Utility
{
    public sealed class DbPropStream : StreamBase
    {
        public DbPropStream() { }
        ~DbPropStream() =>
            Dispose(false);

        public void Read(out List<AssetRec> _assets)
        {
            var _fileHeader = new FilePRPHeaderRec();
            var _mapHeader = new MapHeaderRec();
            //var _types = new List<AssetTypeRec>();
            _assets = new List<AssetRec>();

            var nameData = Array.Empty<byte>();
            var data = Array.Empty<byte>();
            var read = 0;

            _fileStream.Seek(0, SeekOrigin.Begin);
            data = Array.Empty<byte>();
            read = _fileStream.Read(data, 0, FilePRPHeaderRec.SizeOf);

            if (read == FilePRPHeaderRec.SizeOf)
            {
                using (var tmp = Packet.FromBytes(data))
                {
                    _fileHeader.dataOffset = tmp.ReadSInt32();
                    _fileHeader.dataSize = tmp.ReadSInt32();
                    _fileHeader.assetMapOffset = tmp.ReadSInt32();
                    _fileHeader.assetMapSize = tmp.ReadSInt32();
                    //tmp.Clear();
                }
                //data = null;
            }
            else
            {
                throw new Exception("Bad Read");
            }

            _fileStream.Seek(_fileHeader.assetMapOffset, SeekOrigin.Begin);
            data = Array.Empty<byte>();
            read = _fileStream.Read(data, 0, MapHeaderRec.SizeOf);

            if (read == MapHeaderRec.SizeOf)
            {
                using (var tmp = Packet.FromBytes(data))
                {
                    _mapHeader.nbrTypes = tmp.ReadSInt32();
                    _mapHeader.nbrAssets = tmp.ReadSInt32();
                    _mapHeader.lenNames = tmp.ReadSInt32();
                    _mapHeader.typesOffset = tmp.ReadSInt32();
                    _mapHeader.recsOffset = tmp.ReadSInt32();
                    _mapHeader.namesOffset = tmp.ReadSInt32();
                    //tmp.Clear();
                }
                //data = null;
            }
            else
            {
                throw new Exception("Bad Read");
            }

            if (_mapHeader.nbrTypes < 0 || _mapHeader.nbrAssets < 0 || _mapHeader.lenNames < 0)
            {
                throw new Exception("Invalid Map Header");
            }

            #region Asset Types


            _fileStream.Seek(_mapHeader.typesOffset + _fileHeader.assetMapOffset, SeekOrigin.Begin);
            data = new byte[_mapHeader.nbrTypes * AssetTypeRec.SizeOf];
            read = _fileStream.Read(data, 0, _mapHeader.nbrTypes * AssetTypeRec.SizeOf);

            if (read == _mapHeader.nbrTypes * AssetTypeRec.SizeOf)
            {
                // Deprecated
                //using (var tmp = Packet.FromBytes(data))
                //{
                //    for (int i = 0; i < _mapHeader.nbrTypes; i++)
                //    {
                //        var t = new AssetTypeRec();
                //        t.Type = (LegacyAssetTypes)tmp.ReadUInt32();
                //        t.nbrAssets = tmp.ReadUInt32();
                //        t.firstAsset = tmp.ReadUInt32();

                //        _types.Add(t);
                //    }
                //    //data.Clear();
                //}
                //data = null;
            }
            else
            {
                throw new Exception("Bad Read");
            }

            #endregion

            #region Prop Names

            if (_mapHeader.lenNames > 0)
            {
                _fileStream.Seek(_mapHeader.namesOffset + _fileHeader.assetMapOffset, SeekOrigin.Begin);
                nameData = Array.Empty<byte>();
                read = _fileStream.Read(nameData, 0, _mapHeader.lenNames);

                //if (read != mapHeader.lenNames)
                //{
                //    mapHeader.namesOffset = 0;
                //    mapHeader.lenNames = read;
                //}
            }

            #endregion

            #region Asset Records

            data = new byte[_mapHeader.nbrAssets * AssetRec.SizeOf];
            _fileStream.Seek(_mapHeader.recsOffset + _fileHeader.assetMapOffset, SeekOrigin.Begin);
            read = _fileStream.Read(data, 0, _mapHeader.nbrAssets * AssetRec.SizeOf);

            if (read == _mapHeader.nbrAssets * AssetRec.SizeOf)
            {
                using (var tmp = Packet.FromBytes(data))
                {
                    for (int i = 0; i < _mapHeader.nbrAssets; i++)
                    {
                        var t = new AssetRec();
                        t.assetSpec.id = tmp.ReadSInt32();
                        tmp.DropBytes(4); //rHandle
                        t.blockOffset = tmp.ReadSInt32();
                        t.blockSize = tmp.ReadUInt32();
                        t.lastUseTime = tmp.ReadSInt32();
                        t.nameOffset = tmp.ReadSInt32();
                        t.assetFlags = tmp.ReadUInt16();
                        t.assetSpec.crc = tmp.ReadUInt32();
                        t.name = nameData.ReadPString(32, t.nameOffset);
                        t.data = Array.Empty<byte>();

                        _fileStream.Seek(_fileHeader.dataOffset + t.blockOffset, SeekOrigin.Begin);
                        read = _fileStream.Read(t.data, 0, (int)t.blockSize);

                        if (read == t.blockSize)
                        {
                            var crc = Cipher.ComputeCrc(t.data, 12, true);
                            if (t.assetSpec.crc == crc)
                            {
                                //t.type = LegacyAssetTypes.RT_PROP;
                                _assets.Add(t);
                            }
                        }
                        else
                        {
                            throw new Exception("Bad Read");
                        }
                    }
                    //data.Clear();
                }
                //data = null;
            }
            else
            {
                throw new Exception("Bad Read");
            }

            #endregion

            #region Flush to Database

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                _assets
                    //.Where(m => m.type == LegacyAssetTypes.RT_PROP)
                    //.ToList()
                    .ForEach(a =>
                    {
                        if (!dbContext.Assets.Any(m => m.AssetId == a.assetSpec.id))
                        {
                            var asset = new Assets
                            {
                                AssetId = a.assetSpec.id,
                                AssetCrc = (int)a.assetSpec.crc,
                                Flags = a.propFlags,
                                Name = a.name,
                                Data = a.data,
                            };

                            asset.LastUsed = AssetConstants.epoch.AddSeconds(a.lastUseTime);

                            if (asset.LastUsed.CompareTo(AssetConstants.pepoch) < 0)
                            {
                                asset.LastUsed = AssetConstants.pepoch;
                            }

                            dbContext.Assets.Add(asset);
                        }
                    });

                if (dbContext.HasUnsavedChanges())
                {
                    dbContext.SaveChanges();
                }
            }

            #endregion
        }

        public void Write()
        {
            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var list = dbContext.Assets.AsNoTracking()
                    .Where(a => (a.Flags & (int)PropFormats.PF_Custom32Bit) == 0)
                    .ToList();

                var assetRecData = new List<byte>();
                var assetData = new List<byte>();
                var nameData = new List<byte>();

                list.ForEach(a =>
                    {
                        assetRecData.AddRange(new AssetRec
                        {
                            assetSpec = new AssetSpec
                            {
                                id = a.AssetId,
                                crc = (uint)a.AssetCrc,
                            },
                            blockOffset = assetData.Count,
                            blockSize = (uint)a.Data.Length,
                            lastUseTime = (int)a.LastUsed.Subtract(AssetConstants.epoch).TotalSeconds,
                            nameOffset = nameData.Count,
                            propFlags = (ushort)a.Flags,
                        }.SerializePRP(false));
                        assetData.AddRange(a.Data);
                        nameData.AddRange(a.Name.WritePString(32, 1, false));
                    });

                // File Header
                _fileStream.Write(new FilePRPHeaderRec
                {
                    dataOffset = FilePRPHeaderRec.SizeOf,
                    dataSize = assetData.Count,
                    assetMapOffset = FilePRPHeaderRec.SizeOf + assetData.Count,
                    assetMapSize = MapHeaderRec.SizeOf + 2 * AssetTypeRec.SizeOf + list.Count * AssetRec.SizeOf + nameData.Count,
                }.Serialize());

                // Asset Data
                _fileStream.Write(assetData.ToArray());

                // Map Header
                _fileStream.Write(new MapHeaderRec
                {
                    nbrTypes = 2,
                    nbrAssets = list.Count,
                    lenNames = nameData.Count,
                    typesOffset = MapHeaderRec.SizeOf,
                    recsOffset = MapHeaderRec.SizeOf + 2 * AssetTypeRec.SizeOf,
                    namesOffset = MapHeaderRec.SizeOf + 2 * AssetTypeRec.SizeOf + list.Count * AssetRec.SizeOf,
                }.Serialize());

                // Asset Type Rec
                _fileStream.Write(new AssetTypeRec
                {
                    type = LegacyAssetTypes.RT_PROP,
                    nbrAssets = (uint)list.Count,
                    firstAsset = 0,
                }.Serialize());

                _fileStream.Write(new AssetTypeRec
                {
                    type = LegacyAssetTypes.RT_FAVE,
                    nbrAssets = 1,
                    firstAsset = (uint)list.Count,
                }.Serialize());

                // Asset Recs
                _fileStream.Write(assetRecData.ToArray());

                // Asset Names
                _fileStream.Write(nameData.ToArray());
            }
        }
    }
}
