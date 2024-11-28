using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("uLoc")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERMOVE : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_USERMOVE)args.FirstOrDefault();
            var room = dbContext.GetRoom(_sessionState.RoomID);

            if (!room.NotFound)
            {
                if (inboundPacket.pos.h < 0 || inboundPacket.pos.v < 0)
                {
                    inboundPacket.pos = _sessionState.UserInfo.roomPos;

                    Network.SessionManager.Send(_sessionState, inboundPacket, EventTypes.MSG_USERMOVE, (Int32)_sessionState.UserID);

                    return;
                }
                else if (inboundPacket.pos.h > room.Width || inboundPacket.pos.v > room.Height)
                {
                    inboundPacket.pos = _sessionState.UserInfo.roomPos;

                    Network.SessionManager.Send(_sessionState, inboundPacket, EventTypes.MSG_USERMOVE, (Int32)_sessionState.UserID);

                    return;
                }

                if (!_sessionState.Authorized)
                {
                    if ((_sessionState.UserFlags & UserFlags.U_Pin) != 0)
                    {
                        inboundPacket.pos = _sessionState.UserInfo.roomPos;

                        Network.SessionManager.Send(_sessionState, inboundPacket, EventTypes.MSG_USERMOVE, (Int32)_sessionState.UserID);

                        return;
                    }

                    var spots = room.Hotspots
                        .Where(s => ((HotspotFlags)s.flags & (HotspotFlags.HS_Forbidden | HotspotFlags.HS_Mandatory)) != 0)
                        .ToList();

                    if (spots.Any())
                    {
                        var valid = (bool?)null;

                        foreach (var spot in spots)
                        {
                            var vortexes = new List<Point>();

                            foreach (var vortex in spot.Vortexes)
                            {
                                vortexes.Add(new Point
                                {
                                    h = (Int16)(spot.loc.h + vortex.h),
                                    v = (Int16)(spot.loc.v + vortex.v),
                                });
                            }

                            if (inboundPacket.pos.IsPointInPolygon(vortexes))
                            {
                                if (((HotspotFlags)spot.flags & HotspotFlags.HS_Mandatory) == HotspotFlags.HS_Mandatory)
                                {
                                    valid = true;

                                    break;
                                }
                                else if (((HotspotFlags)spot.flags & HotspotFlags.HS_Forbidden) == HotspotFlags.HS_Forbidden)
                                {
                                    valid = false;

                                    break;
                                }
                            }
                            else if (((HotspotFlags)spot.flags & HotspotFlags.HS_Mandatory) == HotspotFlags.HS_Mandatory && !valid.HasValue)
                            {
                                valid = false;
                            }
                        }

                        if (valid.HasValue && valid.Value == false)
                        {
                            inboundPacket.pos = _sessionState.UserInfo.roomPos;

                            Network.SessionManager.Send(_sessionState, inboundPacket, EventTypes.MSG_USERMOVE, (Int32)_sessionState.UserID);

                            return;
                        }
                    }
                }

                _sessionState.UserInfo.roomPos = inboundPacket.pos;

                Network.SessionManager.SendToRoomID(_sessionState.RoomID, _sessionState.UserID, inboundPacket, EventTypes.MSG_USERMOVE, (Int32)_sessionState.UserID);
            }
        }
    }
}
