using System;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Business;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_DELETEROOM : ICommand
    {
        public const string Help = @"[<ID>] -- Delete a room.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext == null) return false;

            var roomID = (args.Length > 0 ? args[0].TryParse<Int16>() : _sessionState?.RoomID ?? 0);

            if (roomID > 0)
            {
                var roomUsers = Network.SessionManager.sessionStates.Values
                    .Where(s => s.RoomID == roomID)
                    .ToList();
                var room = dbContext.GetRoom(roomID);

                if (!room.NotFound)
                {
                    room.Delete(dbContext);
                }

                var sendBusinesses = new List<IBusinessSend>
                {
                    new MSG_ROOMDESC(),
                    new MSG_USERLIST(),
                    new MSG_ROOMDESCEND(),
                };
                var userNew = new MSG_USERNEW();

                foreach (var roomUser in roomUsers)
                {
                    roomID = dbContext.FindRoomID(0, roomUser.Authorized);
                    room = dbContext.GetRoom(roomID);

                    if (!room.NotFound)
                    {
                        roomUser.RoomID = room.ID;

                        foreach (var sendBusiness in sendBusinesses)
                        {
                            sendBusiness.Send(sessionState, roomUser);
                        }

                        userNew.SendToRoomID(sessionState, roomUser);
                    }
                }

                Logger.Log(MessageTypes.Info, $"CMD_DELETEROOM[{_sessionState?.AuthUserID ?? -1}]: {room.Name}");
            }

            return true;
        }
    }
}
