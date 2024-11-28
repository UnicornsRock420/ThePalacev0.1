using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThePalace.Core.Enums;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Factories
{
    public sealed class PatStream : StreamBase
    {
        private static readonly Regex REGEX_WHITESPACE = new Regex(@"\s+", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex REGEX_TOKENS = new Regex(@"^[a-z]+\s+""(.*)""$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public PatStream() { }
        ~PatStream() =>
            this.Dispose(false);

        public void Read(out List<RoomRec> rooms)
        {
            rooms = new List<RoomRec>();

            var workingRoom = null as RoomRec;
            var workingHotspot = null as HotspotRec;
            var workingPicture = null as PictureRec;
            var workingLooseProp = null as LoosePropRec;

            var workingScript = null as StringBuilder;

            var insideRoom = false;
            var insideProp = false;
            var insideScript = false;
            var insidePicture = false;
            var insideHotspot = false;
            var insidePicts = false;

            var line = string.Empty;

            using (var reader = new StreamReader(_fileStream, Encoding.ASCII))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    var tokens = REGEX_WHITESPACE.Split(line);
                    var value = tokens.Length > 1 ? REGEX_TOKENS.Match(line).Groups[1].Value : string.Empty;

                    switch (tokens[0].ToUpper())
                    {
                        case "ROOM":
                            insideRoom = true;
                            workingRoom = new RoomRec();
                            workingRoom.HotSpots = new();
                            workingRoom.Pictures = new();
                            workingRoom.LooseProps = new();

                            break;
                        case "ENDROOM":
                            if (!insideHotspot)
                            {
                                insideRoom = false;
                            }

                            rooms.Add(workingRoom);
                            workingRoom = null;

                            break;
                        case "DOOR":
                            if (insideRoom)
                            {
                                insideHotspot = true;
                                workingHotspot = new();
                                workingScript = new StringBuilder();

                                workingHotspot.type = HotspotTypes.HS_Door;
                            }

                            break;
                        case "ENDDOOR":
                            if (insideRoom && insideHotspot)
                            {
                                insideHotspot = false;

                                workingRoom.HotSpots.Add(workingHotspot);
                                workingHotspot = null;
                            }

                            break;
                        case "SPOT":
                        case "HOTSPOT":
                            if (insideRoom)
                            {
                                insideHotspot = true;
                                workingHotspot = new();
                                workingScript = new();

                                workingHotspot.type = HotspotTypes.HS_Normal;
                            }

                            break;
                        case "ENDSPOT":
                        case "ENDHOTSPOT":
                            if (insideRoom && insideHotspot)
                            {
                                insideHotspot = false;

                                workingRoom.HotSpots.Add(workingHotspot);
                                workingHotspot = null;
                            }

                            break;
                        case "BOLT":
                            if (insideRoom)
                            {
                                insideHotspot = true;
                                workingHotspot = new();
                                workingScript = new();

                                workingHotspot.type = HotspotTypes.HS_Bolt;
                            }

                            break;
                        case "ENDBOLT":
                            if (insideRoom && insideHotspot)
                            {
                                insideHotspot = false;

                                workingRoom.HotSpots.Add(workingHotspot);
                                workingHotspot = null;
                            }

                            break;
                        case "NAVAREA":
                            if (insideRoom)
                            {
                                insideHotspot = true;
                                workingHotspot = new();
                                workingScript = new();

                                workingHotspot.type = HotspotTypes.HS_NavArea;
                            }

                            break;
                        case "ENDNAVAREA":
                            if (insideRoom && insideHotspot)
                            {
                                insideHotspot = false;

                                workingRoom.HotSpots.Add(workingHotspot);
                                workingHotspot = null;
                            }

                            break;
                        case "PICTURE":
                            if (insideRoom)
                            {
                                insidePicture = true;
                                workingPicture = new();
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

                                workingRoom.Pictures.Add(workingPicture);
                                workingPicture = null;
                            }

                            break;
                        case "PROP":
                            if (insideRoom)
                            {
                                insideProp = true;
                                workingLooseProp = new();
                                workingLooseProp.assetSpec = new();
                            }

                            break;
                        case "ENDPROP":
                            if (insideRoom && insideProp)
                            {
                                insideProp = false;

                                workingRoom.LooseProps.Add(workingLooseProp);
                                workingLooseProp = null;
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
                                workingScript = null;
                            }

                            break;
                        case "PICTS":
                            if (insideRoom && insideHotspot)
                            {
                                insidePicts = true;
                                workingHotspot.States = new();
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

        public void Write(bool printHeader = true, params RoomRec[] rooms)
        {
            using (var writer = new StreamWriter(_fileStream, Encoding.ASCII))
            {
                if (printHeader)
                {
                    var entrance = rooms
                        .Where(r => (r.roomFlags & (int)RoomFlags.RF_DropZone) != 0)
                        .OrderBy(r => r.roomID)
                        .FirstOrDefault();

                    writer.WriteLine($"ENTRANCE {entrance?.roomID ?? 0}");
                    writer.WriteLine();
                }

                rooms
                    .ToList()
                    .ForEach(r =>
                    {
                        writer.WriteLine("ROOM");

                        if (!string.IsNullOrWhiteSpace(r.roomPassword)) writer.WriteLine($"\tLOCKED \"{r.roomPassword.EncryptString().WritePalaceString()}\"");

                        writer.WriteLine($"\tID {r.roomID}");

                        if (r.roomMaxOccupancy > 0) writer.WriteLine($"\tMAXMEMBERS {r.roomMaxOccupancy}");

                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_CyborgFreeZone) == RoomFlags.RF_CyborgFreeZone) writer.WriteLine("\tNOCYBORGS");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_DropZone) == RoomFlags.RF_DropZone) writer.WriteLine("\tDROPZONE");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_Hidden) == RoomFlags.RF_Hidden) writer.WriteLine("\tHIDDEN");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_NoGuests) == RoomFlags.RF_NoGuests) writer.WriteLine("\tNOGUESTS");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_NoLooseProps) == RoomFlags.RF_NoLooseProps) writer.WriteLine("\tNOLOOSEPROPS");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_NoPainting) == RoomFlags.RF_NoPainting) writer.WriteLine("\tNOPAINTING");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_Private) == RoomFlags.RF_Private) writer.WriteLine("\tPRIVATE");
                        if (((RoomFlags)r.roomFlags & RoomFlags.RF_WizardsOnly) == RoomFlags.RF_WizardsOnly) writer.WriteLine("\tOPERATORSONLY");

                        writer.WriteLine($"\tNAME \"{r.roomName}\"");
                        if (!string.IsNullOrWhiteSpace(r.roomPicture)) writer.WriteLine($"\tPICT \"{r.roomPicture}\"");
                        if (!string.IsNullOrWhiteSpace(r.roomArtist)) writer.WriteLine($"\tARTIST \"{r.roomArtist}\"");

                        if (r.LooseProps?.Count > 0)
                        {
                            foreach (var looseProp in r.LooseProps)
                            {
                                writer.WriteLine("\tPROP");
                                writer.WriteLine($"\t\tPROPID 0x{string.Format("{0:X8}", looseProp.assetSpec.id)}");

                                if (looseProp.assetSpec.crc != 0) writer.WriteLine($"\t\tCRC 0x{string.Format("{0:X8}", looseProp.assetSpec.crc)}");

                                writer.WriteLine($"\t\tLOC {looseProp.loc.h},{looseProp.loc.v}");
                                writer.WriteLine("\tENDPROP");
                            }
                        }

                        if (r.Pictures != null && r.Pictures?.Count > 0)
                        {
                            foreach (var picture in r.Pictures)
                            {
                                writer.WriteLine($"\tPICTURE ID {picture.picID}");
                                writer.WriteLine($"\t\tNAME \"{picture.name}\"");
                                if (picture.transColor > 0) writer.WriteLine($"\tTRANSCOLOR {picture.transColor}");
                                writer.WriteLine("\tENDPICTURE");
                            }
                        }

                        if (r.HotSpots != null && r.HotSpots?.Count > 0)
                        {
                            foreach (var hotspot in r.HotSpots)
                            {
                                writer.WriteLine($"\t{hotspot.type.GetDescription()}");

                                if (hotspot.type == HotspotTypes.HS_LockableDoor || hotspot.type == HotspotTypes.HS_ShutableDoor) writer.WriteLine("\t\tLOCKABLE");

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

                                if (hotspot.nbrPts > 0 && hotspot.Vortexes?.Count > 0)
                                {
                                    writer.Write($"\t\tOUTLINE");

                                    foreach (var vortex in hotspot.Vortexes)
                                    {
                                        writer.Write($"  {vortex.h},{vortex.v}");
                                    }

                                    writer.WriteLine();
                                }

                                writer.WriteLine($"\t\tLOC {hotspot.loc.h},{hotspot.loc.v}");

                                if (hotspot.States?.Count > 0)
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

                                writer.WriteLine($"\tEND{hotspot.type.GetDescription()}");
                            }
                        }

                        writer.WriteLine("ENDROOM");
                        writer.WriteLine();
                    });

                if (printHeader)
                {
                    writer.WriteLine("END");
                }
            }
        }
    }
}
