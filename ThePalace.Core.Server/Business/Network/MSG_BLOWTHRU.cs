using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("blow")]
    [SuccessfullyConnectedProtocol]
    public class MSG_BLOWTHRU : IBusinessReceive, ISendUserBusiness
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_BLOWTHRU)args.FirstOrDefault();

            if (inboundPacket.nbrUsers > 0)
            {
                for (var j = 0; j < inboundPacket.userIDs.Count; j++)
                {
                    SendToUser(sessionState, inboundPacket.userIDs[j], args);
                }
            }
        }

        public void SendToUser(ISessionState sessionState, UInt32 TargetID, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var header = (Header)args[0];
            var inboundPacket = (Protocols.MSG_BLOWTHRU)args[1];

            Network.SessionManager.SendToUserID(TargetID, inboundPacket, EventTypes.MSG_BLOWTHRU, header.refNum);
        }
    }
}
