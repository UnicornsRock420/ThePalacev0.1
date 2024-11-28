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
    [Description("usrP")]
    public sealed class MSG_USERPROP : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERPROP[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERPROP;
            if (inboundPacket == null) return;

            var userID = (uint)inboundHeader.refNum;

            if (_sessionState.UserID == userID)
            {
                if (_sessionState.UserInfo.assetSpec != null)
                    AssetsManager.Current.FreeAssets(
                        false,
                        _sessionState.UserInfo.assetSpec
                            .Select(p => p.id)
                            .ToArray());

                _sessionState.UserInfo.assetSpec = inboundPacket.assetSpec;
            }

            lock (_sessionState.RoomUsersInfo)
                if (_sessionState.RoomUsersInfo.ContainsKey(userID))
                {
                    if (_sessionState.RoomUsersInfo[userID].assetSpec != null)
                        AssetsManager.Current.FreeAssets(
                            false,
                            _sessionState.RoomUsersInfo[userID].assetSpec
                                .Select(p => p.id)
                                .ToArray());

                    _sessionState.RoomUsersInfo[userID].assetSpec = inboundPacket.assetSpec;
                }

            if ((inboundPacket.assetSpec?.Count ?? 0) > 0)
                ThreadManager.Current.DownloadAsset(_sessionState, inboundPacket.assetSpec.ToArray());

            ScriptEvents.Current.Invoke(IptEventTypes.MsgUserProp, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERPROP[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERPROP[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_USERPROP[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
