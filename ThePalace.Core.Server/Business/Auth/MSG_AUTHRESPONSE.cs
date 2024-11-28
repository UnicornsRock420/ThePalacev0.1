using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Authorization;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("autr")]
    public class MSG_AUTHRESPONSE : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            AuthEngine.AuthorizeUser(sessionState, out int AuthUserID, out List<int> AuthRoleIDs, out List<int> AuthMsgIDs, out List<string> AuthCmds, args);

            //sessionState = ThePalace.Core.Server.Network.SessionManager.sessionStates[sessionState.UserID];
            _sessionState.Authorized = (AuthUserID != 0);
            _sessionState.AuthUserID = AuthUserID;
            _sessionState.AuthRoleIDs = AuthRoleIDs;
            _sessionState.AuthMsgIDs = AuthMsgIDs;
            _sessionState.AuthCmds = AuthCmds;

            if (_sessionState.Authorized)
            {
                new MSG_LOGON().Send(sessionState, args);
            }
            else
            {
                new MSG_SERVERDOWN
                {
                    reason = ServerDownFlags.SD_LoggedOff,
                    whyMessage = "Authentication Failure!",
                }.Send(sessionState, args);
            }
        }
    }
}
