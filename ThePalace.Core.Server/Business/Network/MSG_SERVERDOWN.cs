using System;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("down")]
    public class MSG_SERVERDOWN : IBusinessSend, ISendBroadcast
    {
        public ServerDownFlags reason;
        public string whyMessage;

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var serverDown = new Protocols.MSG_SERVERDOWN
            {
                whyMessage = whyMessage,
            };

            _sessionState.Send(serverDown, EventTypes.MSG_SERVERDOWN, (Int32)reason);
        }

        public void SendToServer(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var serverDown = new Protocols.MSG_SERVERDOWN
            {
                whyMessage = whyMessage,
            };

            Network.SessionManager.SendToServer(serverDown, EventTypes.MSG_SERVERDOWN, (Int32)reason);
        }
    }
}
