using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Commands;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("talk")]
    [SuccessfullyConnectedProtocol]
    public class MSG_TALK : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_TALK)args.FirstOrDefault();
            var chatStr = inboundPacket.text;

            Logger.ConsoleLog($"MSG_TALK[{_sessionState.UserID}]: {chatStr}");

            if (CommandsEngine.Eval(sessionState, null, chatStr))
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

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_TALK, (Int32)_sessionState.UserID);
        }
    }
}
