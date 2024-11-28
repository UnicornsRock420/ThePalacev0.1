using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_HIDE : ICommand
    {
        public const string Help = @"-- Control hiding yourself from other users.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            _sessionState.UserFlags |= UserFlags.U_Hide;

            var xtlk = new Protocols.MSG_XTALK
            {
                text = $"You are now hidden!",
            };

            _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
