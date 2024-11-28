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

namespace ThePalace.Core.Client.Core.Business.Communications
{
    [Description("whis")]
    public sealed class MSG_WHISPER : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_WHISPER[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Communications.MSG_WHISPER;
            if (inboundPacket == null) return;

            var sourceID = (uint)inboundHeader.refNum;

            var user = null as UserRec;
            if (sourceID > 0)
                user = _sessionState.RoomUsersInfo.GetValueLocked(sourceID);

            if (user == null)
            {
                ScriptEvents.Current.Invoke(IptEventTypes.ServerMsg, sessionState, inboundHeader, sessionState.ScriptState);
            }
            else
            {
                var queue = user.Extended["MessageQueue"] as DisposableQueue<MsgBubble>;
                if (queue == null) return;

                queue.Enqueue(new MsgBubble(user.colorNbr, inboundPacket.text));

                ScriptEvents.Current.Invoke(IptEventTypes.Chat, sessionState, inboundHeader, sessionState.ScriptState);
                ScriptEvents.Current.Invoke(IptEventTypes.InChat, sessionState, inboundHeader, sessionState.ScriptState);
            }

#if DEBUG
            Debug.WriteLine($"MSG_WHISPER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_WHISPER[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_WHISPER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
