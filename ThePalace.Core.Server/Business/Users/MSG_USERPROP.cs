using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("usrP")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERPROP : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_USERPROP)args.FirstOrDefault();

            if (!_sessionState.Authorized)
            {
                if ((_sessionState.UserFlags & (UserFlags.U_PropGag | UserFlags.U_Pin)) != 0)
                {
                    inboundPacket.nbrProps = 0;
                    inboundPacket.assetSpec = null;

                    Network.SessionManager.Send(_sessionState, inboundPacket, EventTypes.MSG_USERPROP, (Int32)_sessionState.UserID);

                    return;
                }
            }

            _sessionState.UserInfo.nbrProps = inboundPacket.nbrProps;
            _sessionState.UserInfo.assetSpec = inboundPacket.assetSpec;

            AssetLoader.CheckAssets(_sessionState, inboundPacket.assetSpec);

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_USERPROP, (Int32)_sessionState.UserID);
        }
    }
}
