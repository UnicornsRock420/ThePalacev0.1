using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Authorization;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_SUSR : ICommand
    {
        public const string Help = @"Authorized user escalation.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            if (args.Length > 0)
            {
                Logger.Log(MessageTypes.Info, $"CMD_SUSR[{_sessionState.UserID}]: {args.FirstOrDefault()}");

                AuthEngine.AuthorizeUser(sessionState, out int AuthUserID, out List<int> AuthRoleIDs, out List<int> AuthMsgIDs, out List<string> AuthCmds, new Protocols.MSG_SUPERUSER
                {
                    password = args.FirstOrDefault(),
                });

                _sessionState.Authorized = (AuthUserID != 0);
                _sessionState.AuthUserID = AuthUserID;
                _sessionState.AuthRoleIDs = AuthRoleIDs;
                _sessionState.AuthMsgIDs = AuthMsgIDs;
                _sessionState.AuthCmds = AuthCmds;
            }

            return true;
        }
    }
}
