using System;
using System.Collections.Generic;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_ACCEPT : ICommand
    {
        public const string Help = @"-- Accept someone's offered avatar.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            if (_sessionState.UserInfo.Extended.ContainsKey("OfferBuffer"))
            {
                var userProp = new Protocols.MSG_USERPROP
                {
                    assetSpec = (List<AssetSpec>)_sessionState.UserInfo.Extended["OfferBuffer"],
                };
                userProp.nbrProps = (Int16)userProp.assetSpec.Count;

                _sessionState.Send(userProp, EventTypes.MSG_USERPROP, (Int32)_sessionState.UserID);
            }
            else
            {
                var xtlk = new Protocols.MSG_XTALK
                {
                    text = $"No one has offered you their avatar.",
                };

                _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
