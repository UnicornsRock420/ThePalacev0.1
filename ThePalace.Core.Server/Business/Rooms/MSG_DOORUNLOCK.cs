using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("unlo")]
    [SuccessfullyConnectedProtocol]
    public class MSG_DOORUNLOCK : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_DOORUNLOCK)args.FirstOrDefault();

            if (inboundPacket.roomID == _sessionState.RoomID)
            {
                var room = dbContext.GetRoom(_sessionState.RoomID);

                if (!room.NotFound)
                {
                    var hotspotType = room.Hotspots
                        .Where(s => s.id == inboundPacket.spotID)
                        .Select(s => (short)s.type)
                        .FirstOrDefault();

                    if (hotspotType == (short)HotspotTypes.HS_Bolt)
                    {
                        room.Flags &= (~(int)RoomFlags.RF_Closed);

                        Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_DOORUNLOCK, (Int32)_sessionState.UserID);
                    }
                    else
                    {
                        var outboundPacket = new Protocols.MSG_XTALK
                        {
                            text = "The door you are attempting to unlock is not a deadbolt!",
                        };

                        _sessionState.Send(outboundPacket, EventTypes.MSG_XTALK, 0);
                    }
                }
            }
        }
    }
}
