using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Authorization;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("susr")]
    [SuccessfullyConnectedProtocol]
    public class MSG_SUPERUSER : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_SUPERUSER)args.FirstOrDefault();

            Logger.Log(MessageTypes.Info, $"MSG_SUPERUSER[{_sessionState.UserID}]: {inboundPacket.password}");

            AuthEngine.AuthorizeUser(sessionState, out int AuthUserID, out List<int> AuthRoleIDs, out List<int> AuthMsgIDs, out List<string> AuthCmds, args);

            //sessionState = ThePalace.Core.Server.Network.SessionManager.sessionStates[sessionState.UserID];
            _sessionState.Authorized = (AuthUserID != 0);
            _sessionState.AuthUserID = AuthUserID;
            _sessionState.AuthRoleIDs = AuthRoleIDs;
            _sessionState.AuthMsgIDs = AuthMsgIDs;
            _sessionState.AuthCmds = AuthCmds;
        }
    }
}
