using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Interfaces;

namespace ThePalace.Core.Server.Business
{
    [Description("sinf")]
    public class MSG_SERVERINFO : IBusinessSend, ISendBroadcast
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            // Send Server Info 'sinf'
            var outboundPacket = new Protocols.MSG_SERVERINFO
            {
                serverName = ServerState.serverName,
                serverPermissions = ServerState.serverPermissions,
                //ulDownloadCaps = 0,
                //serverOptions = 0,
                //ulUploadCaps = 0,
            };

            Network.SessionManager.SendToServer(outboundPacket, EventTypes.MSG_SERVERINFO, 0);
        }

        public void SendToServer(ISessionState sessionState, params object[] args)
        {
            // Send Server Info 'sinf'
            var outboundPacket = new Protocols.MSG_SERVERINFO
            {
                serverName = ServerState.serverName,
                serverPermissions = ServerState.serverPermissions,
                //ulDownloadCaps = 0,
                //serverOptions = 0,
                //ulUploadCaps = 0,
            };

            Network.SessionManager.SendToServer(outboundPacket, EventTypes.MSG_SERVERINFO, 0);
        }
    }
}
