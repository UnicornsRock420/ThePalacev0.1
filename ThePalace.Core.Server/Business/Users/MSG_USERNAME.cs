using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("usrN")]
    [SuccessfullyConnectedProtocol]
    public class MSG_USERNAME : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            if (!_sessionState.Authorized)
            {
                if ((_sessionState.UserFlags & UserFlags.U_NameGag) != 0)
                {
                    return;
                }
            }

            var inboundPacket = (Protocols.MSG_USERNAME)args.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(inboundPacket.name) || Regex.IsMatch(inboundPacket.name, @"^User\s*[0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                inboundPacket.name = $"User {_sessionState.UserID}";
            }

            Logger.Log(MessageTypes.Info, $"MSG_USERNAME[{_sessionState.UserID}]: {inboundPacket.name}");

            _sessionState.UserInfo.name = inboundPacket.name;

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_USERNAME, (Int32)_sessionState.UserID);
        }
    }
}
