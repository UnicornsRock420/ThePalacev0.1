using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_UNNAMEGAG : ICommand
    {
        public const string Help = @"-- Unnamegag a previously propgagged user (see the `namegag command).";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var xtlk = new Protocols.MSG_XTALK();

            if (targetState == null)
            {
                xtlk.text = "Sorry, you must target a user to use this command.";

                _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }
            else
            {
                var _targetState = targetState as SessionState;
                if (_targetState == null) return false;

                _targetState.UserFlags &= ~UserFlags.U_NameGag;

                xtlk.text = $"User {_targetState.UserInfo.name} is now unnamegagged!";

                Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, xtlk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
