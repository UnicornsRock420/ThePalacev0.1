using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Commands;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("xtlk")]
    [SuccessfullyConnectedProtocol]
    public class MSG_XTALK : IBusinessReceive, ISendStaffBroadcast
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_XTALK)args.FirstOrDefault();
            var chatStr = inboundPacket.text;

            Logger.ConsoleLog($"MSG_XTALK[{_sessionState.UserID}]: {chatStr}");

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

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_XTALK, (Int32)_sessionState.UserID);
        }

        public void SendToUserID(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_XTALK)args.FirstOrDefault();
            Network.SessionManager.SendToUserID(_sessionState.UserID, inboundPacket, EventTypes.MSG_XTALK, 0);
        }

        public void SendToStaff(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_XTALK)args.FirstOrDefault();
            Network.SessionManager.SendToStaff(inboundPacket, EventTypes.MSG_XTALK, 0);
        }
    }
}
