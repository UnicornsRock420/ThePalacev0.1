using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_SHUTDOWN : ICommand
    {
        public const string Help = @"-- Shutdown the server.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var xtlk = new Protocols.MSG_XTALK();
            xtlk.text = "Initiating server shutdown...";

            if (_sessionState.UserID == 0)
            {
                Logger.ConsoleLog(xtlk.text);
            }

            Network.SessionManager.SendToStaff(xtlk, EventTypes.MSG_XTALK, 0);

            ServerState.Shutdown();

            return true;
        }
    }
}
