using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("ryit")]
    public class MSG_DIYIT : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var diyit = new Protocols.MSG_DIYIT
            {
                ipAddress = _sessionState.driver.GetIPAddress(),
            };

            _sessionState.Send(diyit, EventTypes.MSG_DIYIT, (Int32)_sessionState.UserID);
        }
    }
}
