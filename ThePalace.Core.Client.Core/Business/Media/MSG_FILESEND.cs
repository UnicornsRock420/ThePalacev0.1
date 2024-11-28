using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Media
{
    [Description("sFil")]
    public sealed class MSG_FILESEND : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_FILESEND[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.MSG_FILESEND;
            //if (inboundPacket == null) return;

            //TODO:

#if DEBUG
            Debug.WriteLine($"MSG_FILESEND[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
