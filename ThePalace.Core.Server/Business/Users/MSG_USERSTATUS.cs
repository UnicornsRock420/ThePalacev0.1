using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("uSta")]
    public class MSG_USERSTATUS : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            // Send User Status 'uSta'
            var outboundPacket = new Protocols.MSG_USERSTATUS
            {
                flags = (short)_sessionState.UserFlags,
            };

            _sessionState.Send(outboundPacket, EventTypes.MSG_USERSTATUS, (Int32)_sessionState.UserID);
        }
    }
}
