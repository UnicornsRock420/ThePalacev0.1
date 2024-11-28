using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_RMSG : ICommand
    {
        public const string Help = @"Room message";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var xtlk = new Protocols.MSG_XTALK();
            xtlk.text = string.Join(" ", args);

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
