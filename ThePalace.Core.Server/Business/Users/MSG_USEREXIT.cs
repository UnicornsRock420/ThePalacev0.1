using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("eprs")]
    public class MSG_USEREXIT : ISendRoomBusiness
    {
        public void SendToRoomID(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, _sessionState.UserID, null, EventTypes.MSG_USEREXIT, (Int32)_sessionState.UserID);
        }
    }
}
