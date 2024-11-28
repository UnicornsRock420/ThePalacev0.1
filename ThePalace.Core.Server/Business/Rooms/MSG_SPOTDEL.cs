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
    [Description("opSd")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_SPOTDEL : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_SPOTDEL)args.FirstOrDefault();
            var room = dbContext.GetRoom(_sessionState.RoomID);

            if (!room.NotFound)
            {
                room.Hotspots = room.Hotspots
                    .Where(m => m.id != inboundPacket.spotID)
                    .ToList();

                room.HasUnsavedAuthorChanges = true;
                room.HasUnsavedChanges = true;

                ServerState.FlushRooms(dbContext);

                Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, room, EventTypes.MSG_ROOMSETDESC, 0);
            }
        }
    }
}
