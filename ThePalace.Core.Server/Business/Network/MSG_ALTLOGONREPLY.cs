using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("rep2")]
    public class MSG_ALTLOGONREPLY : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            //var inboundPacket = (ThePalace.Core.Server.Protocols.MSG_LOGON)message.protocol;

            // Send 'rep2'
            var outboundPacket = new Protocols.MSG_ALTLOGONREPLY(_sessionState.RegInfo);

            _sessionState.Send(outboundPacket, EventTypes.MSG_ALTLOGONREPLY, (Int32)_sessionState.UserID);
        }
    }
}
