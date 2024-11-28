using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Plugins.Commands
{
    public class CMD_TEST : ICommand
    {
        public const string Help = @"-- Example: Hello World!";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            if (_sessionState.UserID == 0)
            {
                Logger.ConsoleLog("Example: Hello World!");
            }
            else
            {
                var xtalk = new Server.Protocols.MSG_XTALK
                {
                    text = "Example: Hello World!",
                };
                Network.SessionManager.SendToUserID(_sessionState.UserID, xtalk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
