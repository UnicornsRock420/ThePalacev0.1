using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_OFFER : ICommand
    {
        public const string Help = @"-- Offer your avatar to someone.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var _targetState = targetState as SessionState;
            if (_targetState == null) return false;

            _targetState.UserInfo.Extended["OfferBuffer"] = _sessionState.UserInfo.assetSpec;

            var xWhis = new Protocols.MSG_XWHISPER
            {
                text = $"I'm offering my avatar to you, type `accept to accept it!",
            };

            _targetState.Send(xWhis, EventTypes.MSG_XWHISPER, (Int32)_sessionState.UserID);

            return true;
        }
    }
}
