using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("usrC")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERCOLOR : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_USERCOLOR)args.FirstOrDefault();

            _sessionState.UserInfo.colorNbr = inboundPacket.colorNbr;

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_USERCOLOR, 0);
        }
    }
}
