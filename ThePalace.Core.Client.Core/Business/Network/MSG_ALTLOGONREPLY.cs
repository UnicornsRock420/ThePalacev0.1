using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Network
{
    [Description("rep2")]
    public sealed class MSG_ALTLOGONREPLY : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_ALTLOGONREPLY[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Client.Core.Protocols.Network.MSG_ALTLOGONREPLY;
            //if (inboundPacket == null) return;

            ScriptEvents.Current.Invoke(IptEventTypes.SignOn, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_ALTLOGONREPLY[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
