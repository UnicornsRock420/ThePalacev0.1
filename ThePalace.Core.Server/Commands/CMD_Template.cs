using System;
using ThePalace.Core.Database.Core;
using ThePalace.Core.Enums;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Server.Protocols;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_Template : ICommand
    {
        public const string Help = @"";

        public bool Command(ThePalaceEntities dbContext, UInt32 UserID, UInt32 TargetID, params string[] args)
        {
            var sessionState = UserID != 0xFFFFFFFF ? ThePalace.Core.Server.Network.SessionManager.sessionStates[UserID] : null;

            if (UserID == 0xFFFFFFFF || sessionState.Authorized)
            {
                // UserID -1 means invokation from the commandline
                // sessionState.Authorized means staff user
            }
            else
            {
                var xtlk = new MSG_XTALK
                {
                    text = "Sorry, this is an Admin only command.",
                };

                sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
