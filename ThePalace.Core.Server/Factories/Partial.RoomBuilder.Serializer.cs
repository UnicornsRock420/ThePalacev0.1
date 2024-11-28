using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core.Factories;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Factories
{
    public partial class RoomBuilder : ChangeTracking, IProtocolReceive, IProtocolSend, IDisposable
    {
        private void Flush(bool all = false)
        {
            passwordOfst = 0;
            roomNameOfst = 0;
            artistNameOfst = 0;
            hotspotOfst = 0;
            pictureOfst = 0;
            firstLProp = 0;
            firstDrawCmd = 0;
            nbrDrawCmds = 0;
            nbrHotspots = 0;
            nbrLProps = 0;
            nbrPictures = 0;
            Clear();

            if (all)
            {
                ID = 0;
                Name = null;
                Artist = null;
                Password = null;
                LooseProps.Clear();
                Pictures.Clear();
                DrawCommands.Clear();
                Hotspots.Clear();
            }
        }

        #region Serializer
        public void Deserialize(Packet inPacket = null, params object[] values)
        {
            NotifyPropertyChanged();
            //isDirty = true;
            HasUnsavedAuthorChanges = true;
            HasUnsavedChanges = true;

            if (inPacket != null)
            {
                Flush(true);
                _data = new List<byte>(inPacket.GetData());
            }

            try
            {
                roomFlags = ReadSInt32();
                facesID = ReadSInt32();
                roomID = ReadSInt16();
                roomNameOfst = ReadSInt16();
                pictNameOfst = ReadSInt16();
                artistNameOfst = ReadSInt16();
                passwordOfst = ReadSInt16();
                nbrHotspots = ReadSInt16();
                hotspotOfst = ReadSInt16();
                nbrPictures = ReadSInt16();
                pictureOfst = ReadSInt16();
                nbrDrawCmds = ReadSInt16();
                firstDrawCmd = ReadSInt16();
                DropBytes(2); //nbrPeople
                nbrLProps = ReadSInt16();
                firstLProp = ReadSInt16();
                DropBytes(2); // reserved
                lenVars = ReadSInt16();

                // Get the strings
                roomName = PeekPString(32, 1, roomNameOfst);
                roomPicture = PeekPString(32, 1, pictNameOfst);
                roomArtist = PeekPString(32, 1, artistNameOfst);
                roomPassword = PeekPString(32, 1, passwordOfst);

                #region DrawCmds

                for (int i = 0; i < nbrDrawCmds; i++)
                {
                    Seek(firstDrawCmd + i * DrawCmdRec.SizeOf);

                    var drawCmd = new DrawCmdRec();
                    drawCmd.nextOfst = PeekSInt16();
                    //drawCmd.reserved = PeekSInt16();
                    drawCmd.drawCmd = PeekSInt16();
                    drawCmd.cmdLength = PeekUInt16();
                    drawCmd.dataOfst = PeekSInt16();
                    drawCmd.data = _data
                        .Skip(drawCmd.dataOfst)
                        .Take(drawCmd.cmdLength)
                        .ToArray();

                    _drawCmds.Add(drawCmd);
                }

                #endregion

                #region Loose Props
                for (int i = 0; i < nbrLProps; i++)
                {
                    Seek(firstLProp + i * LoosePropRec.SizeOf);

                    var prop = new LoosePropRec();
                    prop.nextOfst = PeekSInt16();
                    //prop.reserved = PeekSInt16();

                    prop.assetSpec = new AssetSpec();
                    prop.assetSpec.id = PeekSInt32();
                    prop.assetSpec.crc = (UInt32)PeekSInt32();
                    prop.flags = PeekSInt32();
                    //prop.refCon = PeekSInt32();

                    prop.loc = new Point();
                    prop.loc.v = PeekSInt16();
                    prop.loc.h = PeekSInt16();

                    _looseProps.Add(prop);
                }
                #endregion

                #region Pictures
                for (int i = 0; i < nbrPictures; i++)
                {
                    Seek(pictureOfst + PictureRec.SizeOf * i);

                    var picture = new PictureRec();
                    picture.refCon = PeekSInt32();
                    picture.picID = PeekSInt16();
                    picture.picNameOfst = PeekSInt16();
                    picture.transColor = PeekSInt16();
                    //picture.reserved = PeekSInt16();

                    if (picture.picNameOfst > 0 && picture.picNameOfst < Length)
                    {
                        picture.name = PeekPString(32, 1, picture.picNameOfst);

                        _pictures.Add(picture);
                    }

                }
                #endregion

                #region Hotspots

                for (int i = 0; i < nbrHotspots; i++)
                {
                    Seek(hotspotOfst + HotspotRec.SizeOf * i);

                    var hotspot = new HotspotRec
                    {
                        Vortexes = new List<Point>(),
                        States = new List<HotspotStateRec>(),
                    };
                    hotspot.scriptEventMask = PeekSInt32();
                    hotspot.flags = PeekSInt32();
                    //hotspot.secureInfo = PeekSInt32();
                    //hotspot.refCon = PeekSInt32();

                    hotspot.loc = new Point();
                    hotspot.loc.v = PeekSInt16();
                    hotspot.loc.h = PeekSInt16();

                    hotspot.id = PeekSInt16();
                    hotspot.dest = PeekSInt16();
                    hotspot.nbrPts = PeekSInt16();
                    hotspot.ptsOfst = PeekSInt16();
                    hotspot.type = (HotspotTypes)PeekSInt16();
                    //hotspot.groupID = PeekSInt16();
                    //hotspot.nbrScripts = PeekSInt16();
                    //hotspot.scriptRecOfst = PeekSInt16();
                    hotspot.state = PeekSInt16();
                    hotspot.nbrStates = PeekSInt16();
                    hotspot.stateRecOfst = PeekSInt16();
                    hotspot.nameOfst = PeekSInt16();
                    hotspot.scriptTextOfst = PeekSInt16();
                    //hotspot.alignReserved = PeekSInt16();

                    if (hotspot.nameOfst > 0 && hotspot.nameOfst < Length)
                    {
                        hotspot.name = PeekPString(32, 1, hotspot.nameOfst);
                    }

                    if (hotspot.scriptTextOfst > 0 && hotspot.scriptTextOfst < Length)
                    {
                        hotspot.script = ReadCString(hotspot.scriptTextOfst, true);
                    }

                    if (hotspot.nbrPts > 0 && hotspot.ptsOfst > 0 && hotspot.ptsOfst < Length - (Point.SizeOf * hotspot.nbrPts))
                    {
                        for (int s = 0; s < hotspot.nbrPts; s++)
                        {
                            Seek(hotspot.ptsOfst + s * Point.SizeOf);

                            var p = new Point();
                            p.v = PeekSInt16();
                            p.h = PeekSInt16();

                            hotspot.Vortexes.Add(p);
                        }
                    }

                    for (int s = 0; s < hotspot.nbrStates; s++)
                    {
                        Seek(hotspot.stateRecOfst + s * HotspotStateRec.SizeOf);

                        var hs = new HotspotStateRec();
                        hs.pictID = PeekSInt16();
                        //hs.reserved = PeekSInt16();

                        hs.picLoc = new Point();
                        hs.picLoc.v = PeekSInt16();
                        hs.picLoc.h = PeekSInt16();

                        hotspot.States.Add(hs);
                    }

                    _hotspots.Add(hotspot);
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public byte[] Serialize(params object[] values)
        {
            using (var _blobData = new Packet())
            {
                Flush(); // Flush(true) will clear ALL data

                // ALIGN header
                _blobData.PadBytes(4);

                // Room Name
                roomNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomName ?? $"Room {roomID}", 32, 1);

                // Artist Name
                artistNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomArtist ?? string.Empty, 32, 1);

                pictNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomPicture ?? "clouds.gif", 32, 1);

                // Password
                passwordOfst = (short)_blobData.Length;
                _blobData.WritePString(roomPassword ?? string.Empty, 32, 1);

                //Start Spots

                using (var tmp = new Packet())
                {
                    if (_hotspots != null && _hotspots.Count > 0)
                    {
                        foreach (var spot in _hotspots)
                        {
                            // Buffer spot scripts

                            spot.scriptTextOfst = (short)_blobData.Length;
                            _blobData.WriteCString(spot.script ?? string.Empty);

                            //Buffer spot states
                            spot.nbrStates = (short)(spot?.States?.Count ?? 0);
                            spot.stateRecOfst = (short)((spot.nbrStates > 0) ? _blobData.Length : 0);

                            if (spot.nbrStates > 0)
                            {
                                foreach (var state in spot.States)
                                {
                                    _blobData.WriteInt16(state.pictID);
                                    _blobData.WriteInt16(0); //reserved
                                    _blobData.WriteBytes(state.picLoc?.Serialize() ?? new Point(0, 0).Serialize());
                                }
                            }

                            spot.ptsOfst = 0;

                            if (spot.Vortexes != null && spot.Vortexes.Count > 0)
                            {
                                spot.ptsOfst = (short)_blobData.Length;

                                foreach (Point point in spot.Vortexes)
                                {
                                    _blobData.WriteBytes(point.Serialize());
                                }
                            }

                            spot.nameOfst = (short)_blobData.Length;
                            _blobData.WritePString(spot.name ?? string.Empty, 32, 1);

                            //Buffer spotrecs
                            tmp.WriteInt32(spot.scriptEventMask);
                            tmp.WriteInt32(spot.flags);
                            tmp.WriteInt32(0); //secureInfo
                            tmp.WriteInt32(0); //refCon
                            tmp.WriteBytes(spot.loc?.Serialize() ?? new Point(0, 0).Serialize());
                            tmp.WriteInt16(spot.id);
                            tmp.WriteInt16(spot.dest);
                            tmp.WriteInt16(spot.nbrPts);
                            tmp.WriteInt16(spot.ptsOfst);
                            tmp.WriteInt16((Int16)spot.type);
                            tmp.WriteInt16(0); //groupID
                            tmp.WriteInt16(0); //nbrScripts
                            tmp.WriteInt16(0); //scriptRecOfst
                            tmp.WriteInt16(spot.state);
                            tmp.WriteInt16(spot.nbrStates);
                            tmp.WriteInt16(spot.stateRecOfst);
                            tmp.WriteInt16(spot.nameOfst);
                            tmp.WriteInt16(spot.scriptTextOfst);
                            tmp.WriteInt16(0); //alignReserved
                        }
                    }

                    _blobData.PadBytes(4);

                    hotspotOfst = (short)(((nbrHotspots = (short)_hotspots.Count) > 0) ? _blobData.Length : 0);

                    _blobData.WriteBytes(tmp.GetData());
                }

                //Start Pictures

                using (var tmp = new Packet())
                {
                    if (_pictures != null && _pictures.Count > 0)
                    {
                        foreach (var pict in _pictures)
                        {
                            pict.picNameOfst = (short)_blobData.Length;
                            _blobData.WritePString(pict.name, 32, 1);

                            tmp.WriteInt32(pict.refCon);
                            tmp.WriteInt16(pict.picID);
                            tmp.WriteInt16(pict.picNameOfst);
                            tmp.WriteInt16(pict.transColor);
                            tmp.WriteInt16(0); //reserved
                        }
                    }

                    pictureOfst = (short)(((nbrPictures = (short)_pictures.Count) > 0) ? _blobData.Length : 0);

                    _blobData.WriteBytes(tmp.GetData());
                }


                // Start DrawCmds
                using (var tmp = new Packet())
                {
                    _blobData.PadBytes(4);
                    firstDrawCmd = (short)(((nbrDrawCmds = (short)_drawCmds.Count) > 0) ? _blobData.Length : 0);

                    using (var data = new Packet())
                    {
                        for (int i = 0; i < nbrDrawCmds; i++)
                        {
                            _drawCmds[i].cmdLength = (ushort)_drawCmds[i].data.Length;
                            _drawCmds[i].dataOfst = (short)(firstDrawCmd + data.Length + DrawCmdRec.SizeOf * nbrDrawCmds);
                            _drawCmds[i].nextOfst = (short)((i == nbrDrawCmds - 1) ? 0 : firstDrawCmd + tmp.Length + DrawCmdRec.SizeOf);

                            tmp.WriteInt16(_drawCmds[i].nextOfst);
                            tmp.WriteInt16(0); //reserved
                            tmp.WriteInt16(_drawCmds[i].drawCmd);
                            tmp.WriteInt16(_drawCmds[i].cmdLength);
                            tmp.WriteInt16(_drawCmds[i].dataOfst);
                            data.WriteBytes(_drawCmds[i].data);
                        }
                        tmp.WriteBytes(data.GetData());
                    }
                    _blobData.WriteBytes(tmp.GetData());
                }

                // Start Loose Props

                nbrLProps = (short)_looseProps.Count;
                firstLProp = (short)((nbrLProps > 0) ? _blobData.Length : 0);

                for (int i = 0; i < nbrLProps; i++)
                {
                    var ofst = ((i == nbrLProps - 1) ? 0 : (firstLProp + ((i + 1) * LoosePropRec.SizeOf)));

                    _looseProps[i].nextOfst = (short)(ofst);
                    _blobData.WriteBytes(_looseProps[i].Serialize());
                }

                // Write Map Header
                {
                    lenVars = (short)_blobData.Length;

                    WriteInt32(roomFlags);              // Room Flags
                    WriteInt32(facesID);                // Default Face ID
                    WriteInt16(roomID);                 // The Rooms ID
                    WriteInt16(roomNameOfst);    // Room Name
                    WriteInt16(pictNameOfst);    // Background Image Offset
                    WriteInt16(artistNameOfst);  // Artist
                    WriteInt16(passwordOfst);    // Password
                    WriteInt16(nbrHotspots);            // Number of Hotspots
                    WriteInt16(hotspotOfst);     // Hotspot Offset
                    WriteInt16(nbrPictures);            // Number of Pictures
                    WriteInt16(pictureOfst);     // Pictures Offset
                    WriteInt16(nbrDrawCmds);            // Number of Draw Commands
                    WriteInt16(firstDrawCmd);    // Draw Command Offset
                    WriteInt16(0);                      // Number of People ( Obsolete )
                    WriteInt16(nbrLProps);              // Number of Props
                    WriteInt16(firstLProp);      // Loose Props Offset
                    WriteInt16(reserved);               // Reserved Padding
                    WriteInt16(lenVars);                // Length of Data Blob
                }

                _data.AddRange(_blobData.GetData());

                return _data.ToArray();
            }
        }

        public void DeserializeJSON(string json)
        {
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                roomID = ID,
            });
        }
        #endregion
    }
}
