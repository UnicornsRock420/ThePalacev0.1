using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Media
{
    [Description("fnfe")]
    public sealed class MSG_FILENOTFND : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_FILENOTFND[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.MSG_FILENOTFND;
            //if (inboundPacket == null) return;

            //TODO:

#if DEBUG
            Debug.WriteLine($"MSG_FILENOTFND[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
