using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_UNHIDE : ICommand
    {
        public const string Help = @"-- Cease hiding from other users.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var xtlk = new Protocols.MSG_XTALK();

            _sessionState.UserFlags &= ~UserFlags.U_Hide;

            xtlk.text = $"You are now unhidden!";

            _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
