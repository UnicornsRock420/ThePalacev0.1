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
    [Description("ofNr")]
    public sealed class MSG_ROOMINFO : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_ROOMINFO[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Rooms.MSG_ROOMINFO;
            if (inboundPacket == null) return;

            _sessionState.RoomInfo.roomID = inboundPacket.roomInfo.roomID;
            _sessionState.RoomInfo.roomName = inboundPacket.roomInfo.roomName;
            _sessionState.RoomInfo.roomPicture = inboundPacket.roomInfo.roomPicture;
            _sessionState.RoomInfo.roomArtist = inboundPacket.roomInfo.roomArtist;
            _sessionState.RoomInfo.facesID = inboundPacket.roomInfo.facesID;
            _sessionState.RoomInfo.roomFlags = inboundPacket.roomInfo.roomFlags;

            if ((sessionState as IUISessionState) == null)
            {
                ScriptEvents.Current.Invoke(IptEventTypes.RoomReady, sessionState, inboundHeader, sessionState.ScriptState);
                ScriptEvents.Current.Invoke(IptEventTypes.Enter, sessionState, inboundHeader, sessionState.ScriptState);
            }
            else
                ScriptEvents.Current.Invoke(IptEventTypes.RoomLoad, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_ROOMINFO[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
