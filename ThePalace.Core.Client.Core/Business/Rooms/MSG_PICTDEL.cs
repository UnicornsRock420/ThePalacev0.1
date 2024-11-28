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
    [Description("FPSq")]
    public sealed class MSG_PICTDEL : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_PICTDEL[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.Rooms.MSG_PICTDEL;
            //if (inboundPacket == null) return;

            ScriptEvents.Current.Invoke(IptEventTypes.MsgPictDel, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_PICTDEL[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_PICTDEL[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_PICTDEL[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
