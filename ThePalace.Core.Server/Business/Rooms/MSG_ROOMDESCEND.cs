using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("endr")]
    public class MSG_ROOMDESCEND : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            // Send User Log 'endr'
            _sessionState.Send(null, EventTypes.MSG_ROOMDESCEND, (Int32)_sessionState.UserID);
        }
    }
}
