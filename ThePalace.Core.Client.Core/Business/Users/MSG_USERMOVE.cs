using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Users
{
    [Description("uLoc")]
    public sealed class MSG_USERMOVE : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERMOVE[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERMOVE;
            if (inboundPacket == null) return;

            var sourceID = (uint)inboundHeader.refNum;
            var user = null as UserRec;

            if (sourceID > 0)
            {
                if (_sessionState.UserID == sourceID)
                    _sessionState.UserInfo.roomPos = inboundPacket.pos;

                user = _sessionState.RoomUsersInfo.GetValueLocked(sourceID);
            }

            if (user != null)
            {
                user.roomPos = inboundPacket.pos;
                user.Extended["CurrentMessage"] = null;

                var queue = user.Extended["MessageQueue"] as DisposableQueue<MsgBubble>;
                if (queue != null) queue.Clear();

                ScriptEvents.Current.Invoke(IptEventTypes.UserMove, sessionState, inboundHeader, sessionState.ScriptState);
            }

#if DEBUG
            Debug.WriteLine($"MSG_USERMOVE[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERMOVE[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_USERMOVE[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
