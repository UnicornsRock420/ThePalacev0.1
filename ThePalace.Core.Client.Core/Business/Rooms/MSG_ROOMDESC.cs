using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Rooms
{
    [Description("room")]
    public sealed class MSG_ROOMDESC : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_ROOMDESC[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Rooms.MSG_ROOMDESC;
            if (inboundPacket == null) return;

            _sessionState.RoomInfo = inboundPacket;

            if (!string.IsNullOrWhiteSpace(_sessionState.MediaUrl) &&
                !string.IsNullOrWhiteSpace(_sessionState.ServerName))
            {
                if (!string.IsNullOrWhiteSpace(_sessionState.RoomInfo.roomPicture))
                    ThreadManager.Current.DownloadMedia(_sessionState, _sessionState.RoomInfo.roomPicture);

                if ((_sessionState.RoomInfo?.Pictures?.Count ?? 0) > 0)
                    foreach (var pict in _sessionState.RoomInfo.Pictures)
                        ThreadManager.Current.DownloadMedia(_sessionState, pict.name);

                if ((_sessionState.RoomInfo?.LooseProps?.Count ?? 0) > 0)
                    ThreadManager.Current.DownloadAsset(_sessionState, _sessionState.RoomInfo.LooseProps
                        .Select(p => p.assetSpec)
                        .ToArray());
            }

#if DEBUG
            Debug.WriteLine($"MSG_ROOMDESC[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
