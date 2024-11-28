using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("sInf")]
    public class MSG_EXTENDEDINFO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_EXTENDEDINFO)args.FirstOrDefault();

            _sessionState.Send(inboundPacket, EventTypes.MSG_EXTENDEDINFO, (Int32)_sessionState.UserID);
        }
    }
}
