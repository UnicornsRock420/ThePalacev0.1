using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Users
{
    [Description("uSta")]
    public sealed class MSG_USERSTATUS : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERSTATUS[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERSTATUS;
            if (inboundPacket == null) return;

            var userID = (uint)inboundHeader.refNum;

            if (_sessionState.UserID == userID)
            {
                _sessionState.UserInfo.userFlags = inboundPacket.flags;
            }

            lock (_sessionState.RoomUsersInfo)
                if (_sessionState.RoomUsersInfo.ContainsKey(userID))
                {
                    _sessionState.RoomUsersInfo[userID].userFlags = inboundPacket.flags;
                }

            ScriptEvents.Current.Invoke(IptEventTypes.MsgUserStatus, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERSTATUS[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERSTATUS[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_USERSTATUS[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
