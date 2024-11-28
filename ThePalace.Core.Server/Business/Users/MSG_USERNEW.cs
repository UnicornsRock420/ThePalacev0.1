using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("nprs")]
    public class MSG_USERNEW : ISendRoomBusiness
    {
        public void SendToRoomID(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var userNew = new Protocols.MSG_USERNEW();

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, userNew, EventTypes.MSG_USERNEW, (int)_sessionState.UserID);
        }
    }
}
