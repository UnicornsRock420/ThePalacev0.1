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
    [Description("wprs")]
    public sealed class MSG_USERENTER : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERENTER[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERENTER;
            if (inboundPacket == null) return;

            if (_sessionState.UserID == inboundHeader.refNum &&
               _sessionState.UserID == inboundPacket.userID)
            {
                _sessionState.UserInfo.roomPos = inboundPacket.roomPos;
                _sessionState.UserInfo.assetSpec = inboundPacket.assetSpec;
                _sessionState.UserInfo.faceNbr = inboundPacket.faceNbr;
                _sessionState.UserInfo.colorNbr = inboundPacket.colorNbr;
                _sessionState.UserInfo.awayFlag = inboundPacket.awayFlag;
                _sessionState.UserInfo.openToMsgs = inboundPacket.openToMsgs;
                _sessionState.UserInfo.name = inboundPacket.name;
            }

            lock (_sessionState.RoomUsersInfo)
                if (!_sessionState.RoomUsersInfo.ContainsKey(inboundPacket.userID))
                {
                    inboundPacket.Extended.TryAdd(@"MessageQueue", new DisposableQueue<MsgBubble>());
                    inboundPacket.Extended.TryAdd(@"CurrentMessage", null);

                    _sessionState.RoomUsersInfo.TryAdd(inboundPacket.userID, inboundPacket);
                }

            lock (_sessionState.RoomUsersInfo)
                if (!_sessionState.RoomUsersInfo.ContainsKey(_sessionState.UserID))
                    _sessionState.RoomUsersInfo.TryAdd(_sessionState.UserID, _sessionState.UserInfo);

            if ((inboundPacket.assetSpec?.Count ?? 0) > 0)
                ThreadManager.Current.DownloadAsset(_sessionState, inboundPacket.assetSpec.ToArray());

            ScriptEvents.Current.Invoke(IptEventTypes.UserEnter, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERENTER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERENTER[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_USERENTER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
