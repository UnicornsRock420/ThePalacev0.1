using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("usrD")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERDESC : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var header = (Header)args[0];
            var inboundPacket = (Protocols.MSG_USERDESC)args[1];
            var user = dbContext.UserData
                .Where(u => u.UserId == _sessionState.UserID)
                .FirstOrDefault();

            user.FaceNbr = inboundPacket.faceNbr;
            user.ColorNbr = inboundPacket.colorNbr;

            dbContext.SaveChanges();

            if (!_sessionState.Authorized)
            {
                if ((_sessionState.UserFlags & (UserFlags.U_PropGag | UserFlags.U_Pin)) != 0)
                {
                    _sessionState.UserInfo.faceNbr = inboundPacket.faceNbr;
                    _sessionState.UserInfo.colorNbr = inboundPacket.colorNbr;


                    _sessionState.UserInfo.nbrProps = 0;
                    _sessionState.UserInfo.assetSpec = null;

                    return;
                }
            }

            _sessionState.UserInfo.faceNbr = inboundPacket.faceNbr;
            _sessionState.UserInfo.colorNbr = inboundPacket.colorNbr;
            _sessionState.UserInfo.nbrProps = (Int16)inboundPacket.nbrProps;
            _sessionState.UserInfo.assetSpec = inboundPacket.assetSpec;

            AssetLoader.CheckAssets(_sessionState, inboundPacket.assetSpec);

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, header.eventType, header.refNum);
        }
    }
}
