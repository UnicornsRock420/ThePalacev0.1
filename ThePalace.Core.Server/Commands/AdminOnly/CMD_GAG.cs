using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_GAG : ICommand
    {
        public const string Help = @"-- Gag currently connected user <user>.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var _targetState = targetState as SessionState;
            if (_targetState == null) return false;

            _targetState.UserFlags |= UserFlags.U_Gag;

            var xtlk = new Protocols.MSG_XTALK
            {
                text = $"User {_targetState.UserInfo.name} is now gagged!",
            };

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
