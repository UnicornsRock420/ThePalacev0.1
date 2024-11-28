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
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Users
{
    [Description("rprs")]
    public sealed class MSG_USERLIST : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERLIST[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERLIST;
            if (inboundPacket == null) return;

            lock (_sessionState.RoomUsersInfo)
            {
                _sessionState.RoomUsersInfo.Clear();

                var appendUser = (Action<UserRec>)(u =>
                {
                    if (!_sessionState.RoomUsersInfo.ContainsKey(u.userID))
                    {
                        u.Extended.TryAdd(@"MessageQueue", new DisposableQueue<MsgBubble>());
                        u.Extended.TryAdd(@"CurrentMessage", null);

                        if (!_sessionState.RoomUsersInfo.ContainsKey(u.userID))
                            _sessionState.RoomUsersInfo.TryAdd(u.userID, u);

                        if (u.userID > 0 &&
                            (u.assetSpec?.Count ?? 0) > 0)
                            ThreadManager.Current.DownloadAsset(_sessionState, u.assetSpec.ToArray());
                    }
                });

                foreach (var user in inboundPacket)
                {
                    if (_sessionState.UserID == user.userID)
                    {
                        _sessionState.UserInfo.roomPos = user.roomPos;
                        _sessionState.UserInfo.assetSpec = user.assetSpec;
                        _sessionState.UserInfo.faceNbr = user.faceNbr;
                        _sessionState.UserInfo.colorNbr = user.colorNbr;
                        _sessionState.UserInfo.awayFlag = user.awayFlag;
                        _sessionState.UserInfo.openToMsgs = user.openToMsgs;
                        _sessionState.UserInfo.name = user.name;
                    }
                    else
                    {
                        appendUser(user);
                    }
                }

                appendUser(new UserRec
                {
                    userID = 0,
                    roomPos = new Point(0, 0),
                });

                if (!_sessionState.RoomUsersInfo.ContainsKey(_sessionState.UserID))
                    _sessionState.RoomUsersInfo.TryAdd(_sessionState.UserID, _sessionState.UserInfo);
            }

            ScriptEvents.Current.Invoke(IptEventTypes.MsgUserList, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERLIST[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
