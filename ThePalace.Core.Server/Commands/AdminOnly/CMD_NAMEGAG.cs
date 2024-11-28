using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_NAMEGAG : ICommand
    {
        public const string Help = @"-- Namegag a currently connected user.";

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

                _targetState.UserFlags |= UserFlags.U_NameGag;

                if (args.Length > 0)
                {
                    _targetState.UserInfo.name = string.Join(" ", args);

                    var userName = new Protocols.MSG_USERNAME
                    {
                        name = _targetState.UserInfo.name,
                    };

                    _sessionState.Send(userName, EventTypes.MSG_USERNAME, 0);
                }

                {
                    xtlk.text = $"User {_targetState.UserInfo.name} is now namegagged!";

                    Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, xtlk, EventTypes.MSG_XTALK, 0);
                }
            }

            return true;
        }
    }
}
