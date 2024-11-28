using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("sErr")]
    public class MSG_NAVERROR : IBusinessSend
    {
        public NavErrors reason;

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            _sessionState.Send(null, EventTypes.MSG_NAVERROR, (Int32)reason);
        }
    }
}
