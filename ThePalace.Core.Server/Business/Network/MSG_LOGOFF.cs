using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("bye ")]
    public class MSG_LOGOFF : IBusinessReceive, ISendBroadcast
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            _sessionState.driver.DropConnection();

            SendToServer(sessionState, args);
        }

        public void SendToServer(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var outboundPacket = new Protocols.MSG_LOGOFF
            {
                nbrUsers = (Int32)Network.SessionManager.GetServerUserCount(),
            };

            Network.SessionManager.SendToServer(outboundPacket, EventTypes.MSG_LOGOFF, (Int32)_sessionState.UserID);
        }
    }
}
