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
    [Description("HTTP")]
    public sealed class MSG_HTTPSERVER : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_HTTPSERVER[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Network.MSG_HTTPSERVER;
            if (inboundPacket == null) return;

            _sessionState.MediaUrl = inboundPacket.url;

            ScriptEvents.Current.Invoke(IptEventTypes.MsgHttpServer, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_HTTPSERVER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
