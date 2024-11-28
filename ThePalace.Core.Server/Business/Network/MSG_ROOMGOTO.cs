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
    [Description("navR")]
    [SuccessfullyConnectedProtocol]
    public class MSG_ROOMGOTO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            if (!_sessionState.Authorized)
            {
                if ((_sessionState.UserFlags & UserFlags.U_Pin) != 0)
                {
                    return;
                }
            }

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var maxRoomOccupancy = ConfigManager.GetValue<int>("MaxRoomOccupancy", 45);
            var inboundPacket = (Protocols.MSG_ROOMGOTO)args.FirstOrDefault();
            var destRoom = dbContext.GetRoom(inboundPacket.dest);

            if (destRoom.NotFound)
            {
                new MSG_NAVERROR
                {
                    reason = NavErrors.SE_RoomUnknown,
                }.Send(sessionState, args);

                return;
            }
            else if (!_sessionState.Authorized)
            {
                var destRoomUserCount = Network.SessionManager.GetRoomUserCount(inboundPacket.dest);

                if ((destRoom.Flags & (int)RoomFlags.RF_WizardsOnly) != 0 || (destRoom.Flags & (int)RoomFlags.RF_Closed) != 0)
                {
                    new MSG_NAVERROR
                    {
                        reason = NavErrors.SE_RoomClosed,
                    }.Send(sessionState, args);

                    return;
                }
                else if ((destRoom.MaxOccupancy > 0 && destRoomUserCount >= destRoom.MaxOccupancy) || (destRoom.MaxOccupancy == 0 && destRoomUserCount >= maxRoomOccupancy))
                {
                    new MSG_NAVERROR
                    {
                        reason = NavErrors.SE_RoomFull,
                    }.Send(sessionState, args);

                    return;
                }
            }

            var nbrUsers = Network.SessionManager.GetRoomUserCount(_sessionState.RoomID, _sessionState.UserID);
            var currentRoom = dbContext.GetRoom(_sessionState.RoomID);

            if (nbrUsers > 0)
            {
                new MSG_USEREXIT().SendToRoomID(sessionState, args);
            }
            else if (!currentRoom.NotFound)
            {
                currentRoom.Flags &= ~(int)RoomFlags.RF_Closed;
            }

            _sessionState.RoomID = inboundPacket.dest;

            if (!_sessionState.Authorized)
            {
                var spots = destRoom.Hotspots
                    .Where(s => ((HotspotFlags)s.flags & HotspotFlags.HS_LandingPad) == HotspotFlags.HS_LandingPad)
                    .ToList();
                if (spots.Any())
                {
                    var offset = (Int32)RndGenerator.NextSecure((UInt32)spots.Count);
                    var vortexes = new List<Point>();
                    var spot = spots
                        .Skip(offset)
                        .Take(1)
                        .FirstOrDefault();
                    var minH = (Int16)(spot.loc.h + spot.Vortexes[0].h);
                    var maxH = (Int16)(spot.loc.h + spot.Vortexes[0].h);
                    var minV = (Int16)(spot.loc.v + spot.Vortexes[0].v);
                    var maxV = (Int16)(spot.loc.v + spot.Vortexes[0].v);

                    foreach (var vortex in spot.Vortexes)
                    {
                        var p = new Point
                        {
                            h = (Int16)(spot.loc.h + vortex.h),
                            v = (Int16)(spot.loc.v + vortex.v),
                        };

                        vortexes.Add(p);

                        if (p.h < minH) minH = p.h;
                        if (p.h > maxH) maxH = p.h;
                        if (p.v < minV) minV = p.v;
                        if (p.v > maxV) maxV = p.v;
                    }

                    do
                    {
                        _sessionState.UserInfo.roomPos.h = (Int16)RndGenerator.NextSecure((UInt32)minH, (UInt32)maxH);
                        _sessionState.UserInfo.roomPos.v = (Int16)RndGenerator.NextSecure((UInt32)minV, (UInt32)maxV);

                        if (_sessionState.UserInfo.roomPos.IsPointInPolygon(vortexes))
                        {
                            break;
                        }
                    } while (true);
                }
                else
                {
                    _sessionState.UserInfo.roomPos.h = (Int16)RndGenerator.NextSecure((UInt32)destRoom.Width);
                    _sessionState.UserInfo.roomPos.v = (Int16)RndGenerator.NextSecure((UInt32)destRoom.Height);
                }
            }
            else
            {
                _sessionState.UserInfo.roomPos.h = (Int16)RndGenerator.NextSecure((UInt32)destRoom.Width);
                _sessionState.UserInfo.roomPos.v = (Int16)RndGenerator.NextSecure((UInt32)destRoom.Height);
            }

            var sendBusinesses = new List<IBusinessSend>
            {
                new MSG_ROOMDESC(),
                new MSG_USERLIST(),
                new MSG_ROOMDESCEND(),
            };

            foreach (var sendBusiness in sendBusinesses)
            {
                sendBusiness.Send(sessionState, args);
            }

            new MSG_USERNEW().SendToRoomID(sessionState, args);
        }
    }
}
