using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("kill")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_KILLUSER : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_KILLUSER)args.FirstOrDefault();
            var targetSession = Network.SessionManager.sessionStates[inboundPacket.targetID];

            targetSession.Send(null, EventTypes.MSG_SERVERDOWN, (Int32)ServerDownFlags.SD_KilledBySysop);

            targetSession.driver.DropConnection();
        }
    }
}
