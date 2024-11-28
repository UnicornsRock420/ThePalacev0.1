using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("usrF")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERFACE : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_USERFACE)args.FirstOrDefault();

            _sessionState.UserInfo.faceNbr = inboundPacket.faceNbr;

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_USERFACE, (Int32)_sessionState.UserID);
        }
    }
}
