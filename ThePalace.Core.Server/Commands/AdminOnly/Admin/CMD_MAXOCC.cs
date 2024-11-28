using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_MAXOCC : ICommand
    {
        public const string Help = @"[<n>|default] -- Control the maximum occupancy limit for the current room.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var maxOccupany = (UInt32)0;
            var xtlk = new Protocols.MSG_XTALK();

            if (args.Length > 0)
            {
                ConfigManager.SetValue("MaxOccupany", args[0].TryParse<short>(0).ToString());
            }

            maxOccupany = ConfigManager.GetValue<UInt32>("MaxOccupany", 100).Value;

            xtlk.text = $"The server's maximum occupancy is currently: {maxOccupany}";

            _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);

            return true;
        }
    }
}
