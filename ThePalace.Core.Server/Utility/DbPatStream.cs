using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core.Factories;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Factories
{
    public class DbPatStream : StreamBase
    {
        public DbPatStream() { }
        ~DbPatStream() =>
            this.Dispose(false);

        public void Read()
        {
            var hotspots = new List<HotspotRec>();
            var pictures = new List<PictureRec>();
            var looseProps = new List<LoosePropRec>();
            //var drawCmds = new List<DrawCmdRec>();

            var workingHotspot = new HotspotRec();
            var workingPicture = new PictureRec();
            var workingLooseProp = new LoosePropRec();
            var workingScript = new StringBuilder();
            //var workingDrawCmd = new DrawCmdRec();
            var workingRoom = new RoomRec();

            var insideRoom = false;
            var insideProp = false;
            var insideScript = false;
            var insidePicture = false;
            var insideHotspot = false;
            var insidePicts = false;

            var line = string.Empty;

            using (var reader = new StreamReader(_fileStream, Encoding.ASCII))
            {
                using (var dbContext = DbConnection.For<ThePalaceEntities>())
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        var tokens = Regex.Split(line, @"\s+", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        var value = tokens.Length > 1 ? Regex.Match(line, @"^[a-z]+\s+""(.*)""$", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value : string.Empty;

                        switch (tokens[0].ToUpper())
                        {
                            case "ROOM":
                                insideRoom = true;
                                workingRoom = new RoomRec();
                                hotspots = new List<HotspotRec>();
                                pictures = new List<PictureRec>();
                                looseProps = new List<LoosePropRec>();
                                //drawCmds = new List<DrawCmdRec>();

                                break;
                            case "ENDROOM":
                                if (!insideHotspot)
                                {
                                    insideRoom = false;

                                    if (!dbContext.Peek(workingRoom.roomID, out RoomRec roomData) && roomData == null)
                                    {
                                        dbContext.WriteRoom(workingRoom.roomID, workingRoom);
                                        dbContext.WriteHotspots(workingRoom.roomID, hotspots);
                                        dbContext.WritePictures(workingRoom.roomID, pictures);
                                        dbContext.WriteLooseProps(workingRoom.roomID, looseProps);
                                        //RoomData.WriteDrawCmds(dbContext, workingRoom.roomID, drawCmds);

                                        if (dbContext.HasUnsavedChanges())
                                        {
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }

                                break;
                            case "DOOR":
                                if (insideRoom)
                                {
                                    insideHotspot = true;
                                    workingHotspot = new HotspotRec();
                                    workingScript = new StringBuilder();

                                    workingHotspot.type = HotspotTypes.HS_Door;
                                }

                                break;
                            case "ENDDOOR":
                                if (insideRoom && insideHotspot)
                                {
                                    insideHotspot = false;

                                    hotspots.Add(workingHotspot);
                                }

                                break;
                            case "SPOT":
                            case "HOTSPOT":
                                if (insideRoom)
                                {
                                    insideHotspot = true;
                                    workingHotspot = new HotspotRec();
                                    workingScript = new StringBuilder();

                                    workingHotspot.type = HotspotTypes.HS_Normal;
                                }

                                break;
                            case "ENDSPOT":
                            case "ENDHOTSPOT":
                                if (insideRoom && insideHotspot)
                                {
                                    insideHotspot = false;

                                    hotspots.Add(workingHotspot);
                                }

                                break;
                            case "BOLT":
                                if (insideRoom)
                                {
                                    insideHotspot = true;
                                    workingHotspot = new HotspotRec();
                                    workingScript = new StringBuilder();

                                    workingHotspot.type = HotspotTypes.HS_Bolt;
                                }

                                break;
                            case "ENDBOLT":
                                if (insideRoom && insideHotspot)
                                {
                                    insideHotspot = false;

                                    hotspots.Add(workingHotspot);
                                }

                                break;
                            case "NAVAREA":
                                if (insideRoom)
                                {
                                    insideHotspot = true;
                                    workingHotspot = new HotspotRec();
                                    workingScript = new StringBuilder();

                                    workingHotspot.type = HotspotTypes.HS_NavArea;
                                }

                                break;
                            case "ENDNAVAREA":
                                if (insideRoom && insideHotspot)
                                {
                                    insideHotspot = false;

                                    hotspots.Add(workingHotspot);
                                }

                                break;
                            case "PICTURE":
                                if (insideRoom)
                                {
                                    insidePicture = true;
                                    workingPicture = new PictureRec();
                                }

                                if (insidePicture && tokens.Length > 2 && tokens[1] == "ID")
                                {
                                    workingPicture.picID = tokens[2].TryParse<short>(0);
                                }

                                break;
                            case "ENDPICTURE":
                                if (insideRoom && insidePicture)
                                {
                                    insidePicture = false;

                                    pictures.Add(workingPicture);
                                }

                                break;
                            case "PROP":
                                if (insideRoom)
                                {
                                    insideProp = true;
                                    workingLooseProp = new LoosePropRec();
                                    workingLooseProp.assetSpec = new AssetSpec();
                                }

                                break;
                            case "ENDPROP":
                                if (insideRoom && insideProp)
                                {
                                    insideProp = false;

                                    looseProps.Add(workingLooseProp);
                                }

                                break;
                            case "SCRIPT":
                                if (insideRoom && insideHotspot)
                                {
                                    insideScript = true;
                                }

                                break;
                            case "ENDSCRIPT":
                                if (insideRoom && insideHotspot && insideScript)
                                {
                                    insideScript = false;

                                    workingHotspot.script = workingScript.ToString();
                                }

                                break;
                            case "PICTS":
                                if (insideRoom && insideHotspot)
                                {
                                    insidePicts = true;
                                    workingHotspot.States = new List<HotspotStateRec>();
                                }

                                break;
                            case "ENDPICTS":
                                if (insideRoom && insideHotspot && insidePicts)
                                {
                                    insidePicts = false;
                                }

                                break;
                            case "ID":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insidePicture)
                                {
                                    workingPicture.picID = tokens[1].TryParse<short>(0);
                                }
                                else if (insideHotspot)
                                {
                                    workingHotspot.id = tokens[1].TryParse<short>(0);
                                }
                                else if (insideRoom)
                                {
                                    workingRoom.roomID = tokens[1].TryParse<short>(0);
                                }

                                break;
                            case "NAME":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insidePicture)
                                {
                                    workingPicture.name = value;
                                }
                                else if (insideHotspot)
                                {
                                    workingHotspot.name = value;
                                }
                                else if (insideRoom)
                                {
                                    workingRoom.roomName = value;
                                }

                                break;
                            case "ARTIST":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideRoom)
                                {
                                    workingRoom.roomArtist = value;
                                }

                                break;
                            case "PICT":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideRoom)
                                {
                                    workingRoom.roomPicture = value;
                                }

                                break;
                            case "LOCKED":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideRoom && !string.IsNullOrWhiteSpace(value))
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_AuthorLocked;
                                    workingRoom.roomPassword = value.ReadPalaceString().DecryptString();
                                }

                                break;
                            case "MAXMEMBERS":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideRoom)
                                {
                                    workingRoom.roomMaxOccupancy = tokens[1].TryParse<short>(0);
                                }

                                break;
                            case "FACES":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideRoom)
                                {
                                    workingRoom.facesID = tokens[1].TryParse<short>(0);
                                }

                                break;
                            case "DROPZONE":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_DropZone;
                                }

                                break;
                            case "NOLOOSEPROPS":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_NoLooseProps;
                                }

                                break;
                            case "PRIVATE":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_Private;
                                }

                                break;
                            case "NOPAINTING":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_NoPainting;
                                }

                                break;
                            case "NOCYBORGS":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_CyborgFreeZone;
                                }

                                break;
                            case "HIDDEN":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_Hidden;
                                }

                                break;
                            case "NOGUESTS":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_NoGuests;
                                }

                                break;
                            case "WIZARDSONLY":
                            case "OPERATORSONLY":
                                if (insideRoom)
                                {
                                    workingRoom.roomFlags |= (int)RoomFlags.RF_WizardsOnly;
                                }

                                break;
                            case "LOCKABLE":
                            case "SHUTABLE":
                                if (insideRoom)
                                {
                                    workingHotspot.type |= HotspotTypes.HS_LockableDoor;
                                }

                                break;
                            case "DRAGGABLE":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Draggable;
                                }

                                break;
                            case "FORBIDDEN":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Forbidden;
                                }

                                break;
                            case "MANDATORY":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Mandatory;
                                }

                                break;
                            case "LANDINGPAD":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_LandingPad;
                                }

                                break;
                            case "DONTMOVEHERE":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_DontMoveHere;
                                }

                                break;
                            case "INVISIBLE":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Invisible;
                                }

                                break;
                            case "SHOWNAME":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_ShowName;
                                }

                                break;
                            case "SHOWFRAME":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_ShowFrame;
                                }

                                break;
                            case "SHADOW":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Shadow;
                                }

                                break;
                            case "FILL":
                                if (insideRoom)
                                {
                                    workingHotspot.flags |= (int)HotspotFlags.HS_Fill;
                                }

                                break;
                            case "DEST":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideHotspot)
                                {
                                    workingHotspot.dest = tokens[1].TryParse<short>(0);
                                }

                                break;
                            case "OUTLINE":
                                if (tokens.Length < 3)
                                {
                                    break;
                                }

                                if (insideHotspot)
                                {
                                    workingHotspot.Vortexes = new List<Point>();

                                    for (var j = 1; j < tokens.Length; j++)
                                    {
                                        var coords = tokens[j].Split(',');
                                        var h = coords[0].TryParse<short>(0);
                                        var v = coords[1].TryParse<short>(0);

                                        workingHotspot.Vortexes.Add(new Point(h, v));
                                    }
                                }

                                break;
                            case "LOC":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                {
                                    var coords = tokens[1].Split(',');
                                    var h = coords[0].TryParse<short>(0);
                                    var v = coords[1].TryParse<short>(0);

                                    if (insideHotspot)
                                    {
                                        workingHotspot.loc = new Point(h, v);
                                    }
                                    else if (insideProp)
                                    {
                                        workingLooseProp.loc = new Point(h, v);
                                    }
                                }

                                break;
                            case "PROPID":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideProp)
                                {
                                    workingLooseProp.assetSpec.id = Convert.ToInt32(tokens[1], 16);
                                }

                                break;
                            case "CRC":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideProp)
                                {
                                    workingLooseProp.assetSpec.crc = Convert.ToUInt32(tokens[1], 16);
                                }

                                break;
                            case "TRANSCOLOR":
                                if (tokens.Length < 2)
                                {
                                    break;
                                }

                                if (insideProp)
                                {
                                    workingPicture.transColor = tokens[1].TryParse<short>(0);
                                }

                                break;
                            default:
                                if (insidePicts)
                                {
                                    for (var j = 0; j < tokens.Length; j++)
                                    {
                                        var state = tokens[j].Split(',');

                                        workingHotspot.States.Add(new HotspotStateRec
                                        {
                                            pictID = state[0].TryParse<short>(0),
                                            picLoc = new Point
                                            {
                                                h = state[1].TryParse<short>(0),
                                                v = state[2].TryParse<short>(0),
                                            },
                                        });
                                    }
                                }
                                else if (insideScript)
                                {
                                    workingScript.AppendLine(line);
                                }

                                break;
                        }
                    }
                }
            }
        }

        public void Write(List<short> RoomIDs = null)
        {
            var hotspots = new List<HotspotRec>();
            var pictures = new List<PictureRec>();
            var looseProps = new List<LoosePropRec>();
            //var drawCmds = new List<DrawCmdRec>();

            var workingRoom = new RoomRec();

            using (var writer = new StreamWriter(_fileStream, Encoding.ASCII))
            {
                using (var dbContext = DbConnection.For<ThePalaceEntities>())
                {
                    var query = dbContext.Rooms.AsNoTracking()
                        .AsQueryable();

                    if (RoomIDs != null)
                    {
                        query = query.Where(r => RoomIDs.Contains(r.RoomId))
                            .AsQueryable();
                    }

                    var rooms = query.ToList();

                    var entranceID = rooms
                        .Where(r => (r.Flags & (int)RoomFlags.RF_DropZone) != 0)
                        .OrderBy(r => r.RoomId)
                        .Select(r => r.RoomId)
                        .FirstOrDefault();

                    writer.WriteLine($"ENTRANCE {entranceID}");
                    writer.WriteLine();

                    rooms
                        .ForEach(r =>
                        {
                            dbContext.ReadRoom(r.RoomId, out workingRoom);
                            dbContext.ReadHotspots(r.RoomId, out hotspots);
                            dbContext.ReadPictures(r.RoomId, out pictures);
                            dbContext.ReadLooseProps(r.RoomId, out looseProps);
                            //dbContext.ReadDrawCmds(r.RoomId, out drawCmds);

                            writer.WriteLine("ROOM");

                            if (!string.IsNullOrWhiteSpace(workingRoom.roomPassword)) writer.WriteLine($"\tLOCKED \"{workingRoom.roomPassword.EncryptString().WritePalaceString()}\"");

                            writer.WriteLine($"\tID {r.RoomId}");

                            if (workingRoom.roomMaxOccupancy > 0) writer.WriteLine($"\tMAXMEMBERS {workingRoom.roomMaxOccupancy}");

                            if (((RoomFlags)r.Flags & RoomFlags.RF_CyborgFreeZone) == RoomFlags.RF_CyborgFreeZone) writer.WriteLine("\tNOCYBORGS");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_DropZone) == RoomFlags.RF_DropZone) writer.WriteLine("\tDROPZONE");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_Hidden) == RoomFlags.RF_Hidden) writer.WriteLine("\tHIDDEN");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_NoGuests) == RoomFlags.RF_NoGuests) writer.WriteLine("\tNOGUESTS");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_NoLooseProps) == RoomFlags.RF_NoLooseProps) writer.WriteLine("\tNOLOOSEPROPS");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_NoPainting) == RoomFlags.RF_NoPainting) writer.WriteLine("\tNOPAINTING");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_Private) == RoomFlags.RF_Private) writer.WriteLine("\tPRIVATE");
                            if (((RoomFlags)r.Flags & RoomFlags.RF_WizardsOnly) == RoomFlags.RF_WizardsOnly) writer.WriteLine("\tOPERATORSONLY");

                            writer.WriteLine($"\tNAME \"{workingRoom.roomName}\"");
                            if (!string.IsNullOrWhiteSpace(workingRoom.roomPicture)) writer.WriteLine($"\tPICT \"{workingRoom.roomPicture}\"");
                            if (!string.IsNullOrWhiteSpace(workingRoom.roomArtist)) writer.WriteLine($"\tARTIST \"{workingRoom.roomArtist}\"");

                            if (looseProps.Count > 0)
                            {
                                foreach (var looseProp in looseProps)
                                {
                                    writer.WriteLine("\tPROP");
                                    writer.WriteLine($"\t\tPROPID 0x{string.Format("{0:X8}", looseProp.assetSpec.id)}");

                                    if (looseProp.assetSpec.crc != 0) writer.WriteLine($"\t\tCRC 0x{string.Format("{0:X8}", looseProp.assetSpec.crc)}");

                                    writer.WriteLine($"\t\tLOC {looseProp.loc.h},{looseProp.loc.v}");
                                    writer.WriteLine("\tENDPROP");
                                }
                            }

                            if (pictures != null && pictures.Count > 0)
                            {
                                foreach (var picture in pictures)
                                {
                                    writer.WriteLine($"\tPICTURE ID {picture.picID}");
                                    writer.WriteLine($"\t\tNAME \"{picture.name}\"");
                                    if (picture.transColor > 0) writer.WriteLine($"\tTRANSCOLOR {picture.transColor}");
                                    writer.WriteLine("\tENDPICTURE");
                                }
                            }

                            if (hotspots != null && hotspots.Count > 0)
                            {
                                foreach (var hotspot in hotspots)
                                {
                                    writer.WriteLine($"\t{((HotspotTypes)hotspot.type).GetDescription()}");

                                    if ((HotspotTypes)hotspot.type == HotspotTypes.HS_LockableDoor || (HotspotTypes)hotspot.type == HotspotTypes.HS_ShutableDoor) writer.WriteLine("\t\tLOCKABLE");

                                    writer.WriteLine($"\t\tID {hotspot.id}");

                                    if (hotspot.dest != 0) writer.WriteLine($"\t\tDEST {hotspot.dest}");

                                    if (!string.IsNullOrWhiteSpace(hotspot.name)) writer.WriteLine($"\t\tID \"{hotspot.name}\"");

                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_DontMoveHere) == HotspotFlags.HS_DontMoveHere) writer.WriteLine("\t\tDONTMOVEHERE");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Draggable) == HotspotFlags.HS_Draggable) writer.WriteLine("\t\tDRAGGABLE");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Fill) == HotspotFlags.HS_Fill) writer.WriteLine("\t\tFILL");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Forbidden) == HotspotFlags.HS_Forbidden) writer.WriteLine("\t\tFORBIDDEN");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Invisible) == HotspotFlags.HS_Invisible) writer.WriteLine("\t\tINVISIBLE");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_LandingPad) == HotspotFlags.HS_LandingPad) writer.WriteLine("\t\tLANDINGPAD");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Mandatory) == HotspotFlags.HS_Mandatory) writer.WriteLine("\t\tMANDATORY");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_Shadow) == HotspotFlags.HS_Shadow) writer.WriteLine("\t\tSHADOW");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_ShowFrame) == HotspotFlags.HS_ShowFrame) writer.WriteLine("\t\tSHOWFRAME");
                                    if (((HotspotFlags)hotspot.flags & HotspotFlags.HS_ShowName) == HotspotFlags.HS_ShowName) writer.WriteLine("\t\tSHOWNAME");

                                    if (hotspot.nbrPts > 0 && hotspot.Vortexes.Count > 0)
                                    {
                                        writer.Write($"\t\tOUTLINE");

                                        foreach (var vortex in hotspot.Vortexes)
                                        {
                                            writer.Write($"  {vortex.h},{vortex.v}");
                                        }

                                        writer.WriteLine();
                                    }

                                    writer.WriteLine($"\t\tLOC {hotspot.loc.h},{hotspot.loc.v}");

                                    if (hotspot.States.Count > 0)
                                    {
                                        writer.WriteLine($"\t\tPICTS");

                                        foreach (var state in hotspot.States)
                                        {
                                            writer.WriteLine($"\t\t\t{state.pictID},{state.picLoc.h},{state.picLoc.v}");
                                        }

                                        writer.WriteLine($"\t\tENDPICTS");
                                    }

                                    if (!string.IsNullOrWhiteSpace(hotspot.script))
                                    {
                                        writer.WriteLine($"\t\t\tSCRIPT");

                                        writer.WriteLine(hotspot.script.Trim());

                                        writer.WriteLine($"\t\t\tENDSCRIPT");
                                    }

                                    writer.WriteLine($"\tEND{((HotspotTypes)hotspot.type).GetDescription()}");
                                }
                            }

                            writer.WriteLine("ENDROOM");
                            writer.WriteLine();
                        });

                    writer.WriteLine("END");
                }
            }
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}
