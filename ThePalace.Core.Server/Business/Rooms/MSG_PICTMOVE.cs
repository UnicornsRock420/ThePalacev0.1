using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("pLoc")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_PICTMOVE : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_PICTMOVE)args.FirstOrDefault();

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_PICTMOVE, 0);
        }
    }
}
