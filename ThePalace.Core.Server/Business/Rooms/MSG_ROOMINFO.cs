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
    [Description("ofNr")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_ROOMINFO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_ROOMINFO)args.FirstOrDefault();

            if (_sessionState.RoomID == inboundPacket.room.roomID)
            {
                var room = dbContext.GetRoom(_sessionState.RoomID);

                if (!room.NotFound)
                {
                    room.Name = inboundPacket.room.roomName;
                    room.Flags = inboundPacket.room.roomFlags;
                    room.Picture = inboundPacket.room.roomPicture;
                    room.Artist = inboundPacket.room.roomArtist;
                    room.FacesID = inboundPacket.room.facesID;

                    //room.HasUnsavedAuthorChanges = true;
                    room.HasUnsavedChanges = true;

                    ServerState.FlushRooms(dbContext);

                    Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, room, EventTypes.MSG_ROOMSETDESC, 0);
                }
            }
        }
    }
}
