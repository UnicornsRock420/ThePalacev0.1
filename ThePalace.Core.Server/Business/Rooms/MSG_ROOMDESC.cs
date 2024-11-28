using System;
using System.ComponentModel;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("room")]
    public class MSG_ROOMDESC : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            // Send Room 'room'
            var room = dbContext.GetRoom(_sessionState.RoomID);
            if (room.NotFound)
            {
                _sessionState.Send(null, EventTypes.MSG_NAVERROR, (Int32)NavErrors.SE_RoomUnknown);
            }
            else
            {
                _sessionState.Send(room, EventTypes.MSG_ROOMDESC, 0);
            }
        }
    }
}
