using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("HTTP")]
    public class MSG_HTTPSERVER : IBusinessSend, ISendBroadcast
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            if (!string.IsNullOrWhiteSpace(ServerState.mediaUrl))
            {
                // Send HTTP Server 'HTTP'
                var outboundPacket = new Protocols.MSG_HTTPSERVER
                {
                    url = ServerState.mediaUrl,
                };

                _sessionState.Send(outboundPacket, EventTypes.MSG_HTTPSERVER, 0);
            }
        }

        public void SendToServer(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            if (!string.IsNullOrWhiteSpace(ServerState.mediaUrl))
            {
                // Send HTTP Server 'HTTP'
                var outboundPacket = new Protocols.MSG_HTTPSERVER
                {
                    url = ServerState.mediaUrl,
                };

                Network.SessionManager.SendToServer(outboundPacket, EventTypes.MSG_HTTPSERVER, 0);
            }
        }
    }
}
