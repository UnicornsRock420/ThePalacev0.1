using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Server
{
    [Description("down")]
    public sealed class MSG_SERVERDOWN : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_SERVERDOWN[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.MSG_SERVERDOWN;
            //if (inboundPacket == null) return;

            //TODO:

            NetworkManager.Current.Disconnect(sessionState);

#if DEBUG
            Debug.WriteLine($"MSG_SERVERDOWN[{_sessionState.SessionID}:{_sessionState.UserID}]:{(ServerDownFlags)inboundHeader.refNum}");
#endif
        }
    }
}
