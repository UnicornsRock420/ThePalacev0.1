using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("sSta")]
    [SuccessfullyConnectedProtocol]
    public class MSG_SPOTSTATE : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_SPOTSTATE)args.FirstOrDefault();

            if (_sessionState.RoomID == inboundPacket.roomID)
            {
                var room = dbContext.GetRoom(_sessionState.RoomID);

                if (!room.NotFound)
                {
                    var spot = (HotspotRec)null;

                    for (var j = 0; j < room.Hotspots.Count; j++)
                    {
                        if (room.Hotspots[j].id == inboundPacket.spotID)
                        {
                            spot = room.Hotspots[j];

                            break;
                        }
                    }

                    if (spot != null)
                    {
                        spot.state = inboundPacket.state;
                        room.HasUnsavedAuthorChanges = true;
                        room.HasUnsavedChanges = true;

                        ServerState.FlushRooms(dbContext);

                        Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_SPOTSTATE, 0);
                    }
                }
            }
        }
    }
}
