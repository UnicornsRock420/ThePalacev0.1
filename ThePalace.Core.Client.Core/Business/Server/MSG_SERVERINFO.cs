using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Server
{
    [Description("sinf")]
    public sealed class MSG_SERVERINFO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_SERVERINFO[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Server.MSG_SERVERINFO;
            if (inboundPacket == null) return;

            _sessionState.ServerName = inboundPacket.serverName;
            _sessionState.ServerPermissions = inboundPacket.serverPermissions;

            ScriptEvents.Current.Invoke(IptEventTypes.MsgServerInfo, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_SERVERINFO[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
