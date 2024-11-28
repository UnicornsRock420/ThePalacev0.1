using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Rooms
{
    [Description("lock")]
    public sealed class MSG_DOORLOCK : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_DOORLOCK[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.MSG_DOORLOCK;
            //if (inboundPacket == null) return;

            ScriptEvents.Current.Invoke(IptEventTypes.Lock, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_DOORLOCK[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_DOORLOCK[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_DOORLOCK[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
