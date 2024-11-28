using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("rprs")]
    public class MSG_USERLIST : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var userList = new Protocols.MSG_USERLIST();

            _sessionState.Send(userList, EventTypes.MSG_USERLIST, (Int32)Network.SessionManager.GetRoomUserCount(_sessionState.RoomID, _sessionState.UserID));
        }
    }
}
