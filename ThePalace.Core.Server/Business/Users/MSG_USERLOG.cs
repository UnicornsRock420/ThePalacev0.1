using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("log ")]
    public class MSG_USERLOG : ISendBroadcast
    {
        public void SendToServer(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            // Send User Log 'log '
            var outboundPacket = new Protocols.MSG_USERLOG
            {
                nbrUsers = Network.SessionManager.GetServerUserCount(),
            };

            Network.SessionManager.SendToServer(outboundPacket, EventTypes.MSG_USERLOG, (Int32)_sessionState.UserID);
        }
    }
}
