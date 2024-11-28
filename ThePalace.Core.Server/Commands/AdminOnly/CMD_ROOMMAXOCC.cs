using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_ROOMMAXOCC : ICommand
    {
        public const string Help = @"[<n>|default] -- Control the maximum occupancy limit for the current room.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            if (args.Length > 0)
            {
                ServerState.roomsCache[_sessionState.RoomID].MaxOccupancy = args[0].TryParse<short>(0);
            }

            var xtlk = new Protocols.MSG_XTALK
            {
                text = $"The room's maximum occupancy is currently: {ServerState.roomsCache[_sessionState.RoomID].MaxOccupancy}",
            };

            _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
