using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Models.Protocols
{
    public class RoomRec : IProtocolRec
    {
        public short roomID;
        public int roomFlags;
        public int facesID;
        public string roomName;
        public string roomPicture;
        public string roomArtist;
        public string roomPassword;
        public short roomMaxOccupancy;
        public DateTime? roomLastModified;

        public List<HotspotRec> HotSpots;
        public List<PictureRec> Pictures;
        public List<DrawCmdRec> DrawCmds;
        public List<LoosePropRec> LooseProps;

        public RoomRec() { }
        public RoomRec(Packet packet, params object[] values) =>
            Deserialize(packet, values);

        public void Dispose()
        {
            HotSpots?.Clear();
            HotSpots = null;

            Pictures?.Clear();
            Pictures = null;

            DrawCmds?.Clear();
            DrawCmds = null;

            LooseProps?.Clear();
            LooseProps = null;
        }

        public void Deserialize(Packet packet, params object[] values)
        {
            HotSpots = new();
            Pictures = new();
            DrawCmds = new();
            LooseProps = new();

            var roomNameOfst = (Int16)0;
            var pictNameOfst = (Int16)0;
            var artistNameOfst = (Int16)0;
            var passwordOfst = (Int16)0;
            var nbrHotspots = (Int16)0;
            var hotspotOfst = (Int16)0;
            var nbrPictures = (Int16)0;
            var pictureOfst = (Int16)0;
            var nbrDrawCmds = (Int16)0;
            var firstDrawCmd = (Int16)0;
            var nbrPeople = (Int16)0;
            var nbrLProps = (Int16)0;
            var firstLProp = (Int16)0;
            var reserved = (Int16)0;
            var lenVars = (Int16)0;

            try
            {
                roomFlags = packet.ReadSInt32();
                facesID = packet.ReadSInt32();
                roomID = packet.ReadSInt16();
                roomNameOfst = packet.ReadSInt16();
                pictNameOfst = packet.ReadSInt16();
                artistNameOfst = packet.ReadSInt16();
                passwordOfst = packet.ReadSInt16();
                nbrHotspots = packet.ReadSInt16();
                hotspotOfst = packet.ReadSInt16();
                nbrPictures = packet.ReadSInt16();
                pictureOfst = packet.ReadSInt16();
                nbrDrawCmds = packet.ReadSInt16();
                firstDrawCmd = packet.ReadSInt16();
                nbrPeople = packet.ReadSInt16();
                nbrLProps = packet.ReadSInt16();
                firstLProp = packet.ReadSInt16();
                reserved = packet.ReadSInt16();
                lenVars = packet.ReadSInt16();

                // Get the strings
                roomName = packet.PeekPString(32, 1, roomNameOfst);
                roomPicture = packet.PeekPString(32, 1, pictNameOfst);
                roomArtist = packet.PeekPString(32, 1, artistNameOfst);
                roomPassword = packet.PeekPString(32, 1, passwordOfst);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RoomRec.Header: {ex.Message}");
#endif
            }

            #region Hotspots

            try
            {
                for (var i = 0; i < nbrHotspots; i++)
                {
                    packet.Seek(hotspotOfst + (HotspotRec.SizeOf * i));

                    var h = new HotspotRec
                    {
                        Vortexes = new(),
                        States = new(),
                    };
                    packet.PeekSInt32(); //scriptEventMask
                    h.flags = packet.PeekSInt32();
                    packet.PeekSInt32(); //secureInfo
                    packet.PeekSInt32(); //refCon

                    h.loc = new Point();
                    h.loc.v = packet.PeekSInt16();
                    h.loc.h = packet.PeekSInt16();

                    h.id = packet.PeekSInt16();
                    h.dest = packet.PeekSInt16();
                    h.nbrPts = packet.PeekSInt16();
                    h.ptsOfst = packet.PeekSInt16();
                    h.type = (HotspotTypes)packet.PeekSInt16();
                    packet.PeekSInt16(); //groupID
                    packet.PeekSInt16(); //nbrScripts
                    packet.PeekSInt16(); //scriptRecOfst
                    h.state = packet.PeekSInt16();
                    h.nbrStates = packet.PeekSInt16();
                    h.stateRecOfst = packet.PeekSInt16();
                    h.nameOfst = packet.PeekSInt16();
                    h.scriptTextOfst = packet.PeekSInt16();
                    packet.PeekSInt16(); //alignReserved

                    if (h.nameOfst > 0 && h.nameOfst < packet.Length)
                        h.name = packet.PeekPString(32, 1, h.nameOfst);

                    if (h.scriptTextOfst > 0 && h.scriptTextOfst < packet.Length)
                        h.script = packet.ReadCString(h.scriptTextOfst, true);

                    if (h.nbrPts > 0 && h.ptsOfst > 0 && h.ptsOfst < packet.Length - Point.SizeOf * h.nbrPts)
                        for (var s = 0; s < h.nbrPts; s++)
                        {
                            packet.Seek(h.ptsOfst + s * Point.SizeOf);

                            var p = new Point();
                            p.v = packet.PeekSInt16();
                            p.h = packet.PeekSInt16();

                            h.Vortexes.Add(p);
                        }

                    for (var s = 0; s < h.nbrStates; s++)
                    {
                        packet.Seek(h.stateRecOfst + s * HotspotStateRec.SizeOf);

                        var hs = new HotspotStateRec();
                        hs.pictID = packet.PeekSInt16();
                        packet.PeekSInt16(); //reserved

                        hs.picLoc = new Point();
                        hs.picLoc.v = packet.PeekSInt16();
                        hs.picLoc.h = packet.PeekSInt16();

                        h.States.Add(hs);
                    }

                    HotSpots.Add(h);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RoomRec.Hotspots: {ex.Message}");
#endif
            }

            #endregion

            #region Pictures

            try
            {
                for (var i = 0; i < nbrPictures; i++)
                {
                    packet.Seek(pictureOfst + PictureRec.SizeOf * i);

                    var pict = new PictureRec();
                    pict.refCon = packet.PeekSInt32();
                    pict.picID = packet.PeekSInt16();
                    pict.picNameOfst = packet.PeekSInt16();
                    pict.transColor = packet.PeekSInt16();
                    packet.PeekSInt16(); //reserved

                    if (pict.picNameOfst > 0 &&
                        pict.picNameOfst < packet.Length)
                    {
                        pict.name = packet.PeekPString(32, 1, pict.picNameOfst);

                        Pictures.Add(pict);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RoomRec.Pictures: {ex.Message}");
#endif
            }

            #endregion

            #region DrawCmds

            try
            {
                var ofst = firstDrawCmd;

                for (var i = 0; i < nbrDrawCmds; i++)
                {
                    packet.Seek(ofst);

                    var drawCmd = new DrawCmdRec();
                    ofst = drawCmd.nextOfst = packet.PeekSInt16();
                    packet.PeekSInt16(); //reserved
                    drawCmd.drawCmd = packet.PeekSInt16();
                    drawCmd.cmdLength = packet.PeekUInt16();
                    drawCmd.dataOfst = packet.PeekSInt16();
                    drawCmd.data = packet.Data
                        .Skip(drawCmd.dataOfst)
                        .Take(drawCmd.cmdLength)
                        .ToArray();
                    drawCmd.DeserializeData();

                    DrawCmds.Add(drawCmd);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RoomRec.DrawCmds: {ex.Message}");
#endif
            }

            #endregion

            #region Loose Props

            try
            {
                var ofst = firstLProp;

                for (var i = 0; i < nbrLProps; i++)
                {
                    packet.Seek(ofst);

                    var prop = new LoosePropRec();
                    ofst = prop.nextOfst = packet.PeekSInt16();
                    packet.PeekSInt16(); //reserved

                    prop.assetSpec = new AssetSpec();
                    prop.assetSpec.id = packet.PeekSInt32();
                    prop.assetSpec.crc = (uint)packet.PeekSInt32();

                    prop.flags = packet.PeekSInt32();
                    packet.PeekSInt32(); //refCon

                    prop.loc = new Point();
                    prop.loc.v = packet.PeekSInt16();
                    prop.loc.h = packet.PeekSInt16();

                    LooseProps.Add(prop);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"RoomRec.LooseProps: {ex.Message}");
#endif
            }

            #endregion
        }

        public byte[] Serialize(params object[] values)
        {
            using (var _data = new Packet())
            using (var _blobData = new Packet())
            {
                // ALIGN header
                _blobData.PadBytes(4);

                // Room Name
                var roomNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomName ?? $"Room {roomID}", 32, 1);

                // Artist Name
                var artistNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomArtist ?? string.Empty, 32, 1);

                var pictNameOfst = (short)_blobData.Length;
                _blobData.WritePString(roomPicture ?? "clouds.gif", 32, 1);

                // Password
                var passwordOfst = (short)_blobData.Length;
                _blobData.WritePString(roomPassword ?? string.Empty, 32, 1);

                //Start Spots
                var hotspotOfst = (short)0;

                using (var tmp = new Packet())
                {
                    if (HotSpots != null && HotSpots.Count > 0)
                    {
                        foreach (var spot in HotSpots)
                        {
                            // Buffer spot scripts

                            if (!string.IsNullOrEmpty(spot.script))
                            {
                                spot.scriptTextOfst = (short)_blobData.Length;
                                _blobData.WriteCString(spot.script);
                            }
                            else
                                spot.scriptTextOfst = 0;

                            //Buffer spot states
                            spot.nbrStates = (short)(spot?.States?.Count ?? 0);

                            if (spot.nbrStates > 0)
                            {
                                spot.stateRecOfst = (short)(spot.nbrStates > 0 ? _blobData.Length : 0);

                                foreach (var state in spot.States)
                                {
                                    _blobData.WriteInt16(state.pictID);
                                    _blobData.WriteInt16(0); //reserved
                                    _blobData.WriteBytes((state.picLoc ?? new Point(0, 0)).Serialize());
                                }
                            }
                            else
                                spot.stateRecOfst = 0;

                            spot.ptsOfst = 0;

                            if ((spot.Vortexes?.Count ?? 0) > 0)
                            {
                                spot.ptsOfst = (short)_blobData.Length;

                                foreach (var point in spot.Vortexes)
                                {
                                    _blobData.WriteBytes(point.Serialize());
                                }
                            }
                            else
                                spot.ptsOfst = 0;

                            if (!string.IsNullOrEmpty(spot.name))
                            {
                                spot.nameOfst = (short)_blobData.Length;
                                _blobData.WritePString(spot.name, 32, 1);
                            }
                            else
                                spot.nameOfst = 0;

                            //Buffer spotrecs
                            tmp.WriteInt32(spot.scriptEventMask);
                            tmp.WriteInt32(spot.flags);
                            tmp.WriteInt32(0); //secureInfo
                            tmp.WriteInt32(0); //refCon
                            tmp.WriteBytes((spot.loc ?? new Point(0, 0)).Serialize());
                            tmp.WriteInt16(spot.id);
                            tmp.WriteInt16(spot.dest);
                            tmp.WriteInt16(spot.nbrPts);
                            tmp.WriteInt16(spot.ptsOfst);
                            tmp.WriteInt16((short)spot.type);
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

                    hotspotOfst = (short)(HotSpots.Count > 0 ? _blobData.Length : 0);

                    _blobData.WriteBytes(tmp.GetData());
                }

                //Start Pictures
                var pictureOfst = (short)0;

                using (var tmp = new Packet())
                {
                    if ((Pictures?.Count ?? 0) > 0)
                    {
                        foreach (var pict in Pictures)
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

                    pictureOfst = (short)(Pictures.Count > 0 ? _blobData.Length : 0);

                    _blobData.WriteBytes(tmp.GetData());
                }


                // Start DrawCmds
                var firstDrawCmd = (short)0;

                using (var tmp1 = new Packet())
                {
                    _blobData.PadBytes(4);

                    firstDrawCmd = (short)(DrawCmds.Count > 0 ? _blobData.Length : 0);

                    using (var tmp2 = new Packet())
                    {
                        for (var i = 0; i < DrawCmds.Count; i++)
                        {
                            DrawCmds[i].cmdLength = (ushort)DrawCmds[i].data.Length;
                            DrawCmds[i].dataOfst = (short)(firstDrawCmd + tmp2.Length + DrawCmdRec.SizeOf * DrawCmds.Count);
                            DrawCmds[i].nextOfst = (short)(i == DrawCmds.Count - 1 ? 0 : firstDrawCmd + tmp1.Length + DrawCmdRec.SizeOf);

                            tmp1.WriteInt16(DrawCmds[i].nextOfst);
                            tmp1.WriteInt16(0); //reserved
                            tmp1.WriteInt16(DrawCmds[i].drawCmd);
                            tmp1.WriteInt16(DrawCmds[i].cmdLength);
                            tmp1.WriteInt16(DrawCmds[i].dataOfst);
                            tmp2.WriteBytes(DrawCmds[i].data);
                        }

                        tmp1.WriteBytes(tmp2.GetData());
                    }

                    _blobData.WriteBytes(tmp1.GetData());
                }

                // Start Loose Props
                var firstLProp = (short)(LooseProps.Count > 0 ? _blobData.Length : 0);

                for (int i = 0; i < LooseProps.Count; i++)
                {
                    LooseProps[i].nextOfst = (short)(i == LooseProps.Count - 1 ? 0 : firstLProp + (i + 1) * LoosePropRec.SizeOf);

                    _blobData.WriteBytes(LooseProps[i].Serialize());
                }

                // Write Map Header
                {
                    var lenVars = (short)_blobData.Length;

                    _data.WriteInt32(roomFlags);                // Room Flags
                    _data.WriteInt32(facesID);                  // Default Face ID
                    _data.WriteInt16(roomID);                   // The Rooms ID
                    _data.WriteInt16(roomNameOfst);      // Room Name
                    _data.WriteInt16(pictNameOfst);      // Background Image Offset
                    _data.WriteInt16(artistNameOfst);    // Artist
                    _data.WriteInt16(passwordOfst);      // Password
                    _data.WriteInt16((short)HotSpots.Count);    // Number of Hotspots
                    _data.WriteInt16(hotspotOfst);       // Hotspot Offset
                    _data.WriteInt16((short)HotSpots.Count);    // Number of Pictures
                    _data.WriteInt16(pictureOfst);       // Pictures Offset
                    _data.WriteInt16((short)HotSpots.Count);    // Number of Draw Commands
                    _data.WriteInt16(firstDrawCmd);      // Draw Command Offset
                    _data.WriteInt16(0);                        // Number of People ( Obsolete )
                    _data.WriteInt16((short)LooseProps.Count);  // Number of Props
                    _data.WriteInt16(firstLProp);        // Loose Props Offset
                    _data.WriteInt16(0);                        // Reserved Padding
                    _data.WriteInt16(lenVars);                  // Length of Data Blob
                }

                _data.WriteBytes(_blobData.GetData());

                return _data.GetData();
            }
        }
    }
}
