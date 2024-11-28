using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("sRom")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_ROOMSETDESC : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_ROOMSETDESC)args.FirstOrDefault();
            var room = dbContext.GetRoom(_sessionState.RoomID);

            if (!room.NotFound)
            {
                if (room.ID != inboundPacket.room.ID)
                {
                    inboundPacket.room.ID = room.ID;
                }

                ServerState.roomsCache[room.ID] = new RoomBuilder(inboundPacket.room);
            }

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_ROOMSETDESC, 0);
        }
    }
}
