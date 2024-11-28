using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("ofNs")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_SPOTINFO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_SPOTINFO)args.FirstOrDefault();

            if (_sessionState.RoomID == inboundPacket.roomID)
            {
                var room = dbContext.GetRoom(_sessionState.RoomID);

                if (!room.NotFound)
                {
                    foreach (var spot in room.Hotspots)
                    {
                        if (spot.id == inboundPacket.spot.id)
                        {
                            spot.name = inboundPacket.spot.name;
                            spot.type = inboundPacket.spot.type;
                            spot.state = inboundPacket.spot.state;
                            spot.States = inboundPacket.spot.States;
                            spot.script = inboundPacket.spot.script;
                            spot.dest = inboundPacket.spot.dest;
                            spot.flags = inboundPacket.spot.flags;
                            spot.Vortexes = inboundPacket.spot.Vortexes;

                            if (inboundPacket.pictureList != null)
                            {
                                room.Pictures = inboundPacket.pictureList;
                            }

                            break;
                        }
                    }

                    room.HasUnsavedAuthorChanges = true;
                    room.HasUnsavedChanges = true;

                    ServerState.FlushRooms(dbContext);

                    Network.SessionManager.SendToRoomID(_sessionState.RoomID, _sessionState.UserID, room, EventTypes.MSG_ROOMSETDESC, 0);
                }
            }
        }
    }
}
