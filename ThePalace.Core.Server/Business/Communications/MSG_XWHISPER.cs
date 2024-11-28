using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Commands;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("xwis")]
    [SuccessfullyConnectedProtocol]
    public class MSG_XWHISPER : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_XWHISPER)args.FirstOrDefault();
            var chatStr = inboundPacket.text;

            Logger.ConsoleLog($"MSG_XWHISPER[{_sessionState.UserID} -> {inboundPacket.targetID}]: {chatStr}");

            if (!Network.SessionManager.sessionStates.ContainsKey(inboundPacket.targetID)) return;
            var targetState = Network.SessionManager.sessionStates[inboundPacket.targetID];

            if (CommandsEngine.Eval(sessionState, targetState, chatStr))
            {
                return;
            }

            if (!_sessionState.Authorized)
            {
                if ((_sessionState.UserFlags & UserFlags.U_Gag) != 0)
                {
                    return;
                }
            }

            if (!Network.SessionManager.sessionStates.ContainsKey(inboundPacket.targetID))
            {
                var outboundPack = new Protocols.MSG_XTALK
                {
                    text = "Sorry, was unable to locate that user."
                };

                _sessionState.Send(outboundPack, EventTypes.MSG_XTALK, 0);

                return;
            }

            var targetSessionState = Network.SessionManager.sessionStates[inboundPacket.targetID];

            if (targetSessionState.RoomID == _sessionState.RoomID && (targetSessionState.UserFlags & UserFlags.U_RejectWhisper) != 0)
            {
                var outboundPack = new Protocols.MSG_XTALK
                {
                    text = "Sorry, but this user has whispers turned off."
                };

                _sessionState.Send(outboundPack, EventTypes.MSG_XTALK, 0);
            }
            else if (targetSessionState.RoomID != _sessionState.RoomID && (targetSessionState.UserFlags & UserFlags.U_RejectEsp) != 0)
            {
                var outboundPack = new Protocols.MSG_XTALK
                {
                    text = "Sorry, but this user has ESP turned off."
                };

                _sessionState.Send(outboundPack, EventTypes.MSG_XTALK, 0);
            }
            else
            {
                _sessionState.Send(inboundPacket, EventTypes.MSG_XWHISPER, (Int32)_sessionState.UserID);

                targetSessionState.Send(inboundPacket, EventTypes.MSG_XWHISPER, (Int32)_sessionState.UserID);
            }
        }
    }
}
