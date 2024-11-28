using System;
using System.Collections.Generic;
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
    [Description("draw")]
    public sealed class MSG_DRAW : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_DRAW[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Rooms.MSG_DRAW;
            if (inboundPacket == null) return;

            inboundPacket.DeserializeData();
            switch (inboundPacket.type)
            {
                case DrawCmdTypes.DC_Delete:
                    _sessionState.RoomInfo.DrawCmds.Pop();
                    break;
                case DrawCmdTypes.DC_Detonate:
                    _sessionState.RoomInfo.DrawCmds.Clear();
                    break;
                //case DrawCmdTypes.DC_Ellipse:
                //case DrawCmdTypes.DC_Shape:
                //case DrawCmdTypes.DC_Path:
                //case DrawCmdTypes.DC_Text:
                default:
                    _sessionState.RoomInfo.DrawCmds.Add(inboundPacket);
                    break;
            }

            ScriptEvents.Current.Invoke(IptEventTypes.MsgDraw, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_DRAW[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_DRAW[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_DRAW[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
