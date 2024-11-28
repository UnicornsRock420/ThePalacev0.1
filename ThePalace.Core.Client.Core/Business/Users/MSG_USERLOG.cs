using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Users
{
    [Description("log ")]
    public sealed class MSG_USERLOG : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERLOG[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERLOG;
            if (inboundPacket == null) return;

            _sessionState.ServerPopulation = (int)inboundPacket.nbrUsers;

            ScriptEvents.Current.Invoke(IptEventTypes.MsgUserLog, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERLOG[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
