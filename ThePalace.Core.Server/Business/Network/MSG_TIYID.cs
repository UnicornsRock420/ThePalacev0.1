using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("tiyr")]
    public class MSG_TIYID : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var tiyid = new Protocols.MSG_TIYID
            {
                ipAddress = _sessionState.driver.GetIPAddress(),
            };

            _sessionState.Send(tiyid, EventTypes.MSG_TIYID, (Int32)_sessionState.UserID);
        }
    }
}
