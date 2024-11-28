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
    [Description("usrD")]
    public sealed class MSG_USERDESC : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERDESC[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Users.MSG_USERDESC;
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

                _sessionState.UserInfo.faceNbr = inboundPacket.faceNbr;
                _sessionState.UserInfo.colorNbr = inboundPacket.colorNbr;
                _sessionState.UserInfo.assetSpec = inboundPacket.assetSpec;
            }

            lock (_sessionState.RoomUsersInfo)
            {
                if (_sessionState.RoomUsersInfo.ContainsKey(userID))
                {
                    if (_sessionState.RoomUsersInfo[userID].assetSpec != null)
                        AssetsManager.Current.FreeAssets(
                            false,
                            _sessionState.RoomUsersInfo[userID].assetSpec
                                .Select(p => p.id)
                                .ToArray());

                    _sessionState.RoomUsersInfo[userID].faceNbr = inboundPacket.faceNbr;
                    _sessionState.RoomUsersInfo[userID].colorNbr = inboundPacket.colorNbr;
                    _sessionState.RoomUsersInfo[userID].assetSpec = inboundPacket.assetSpec;
                }

                if (!_sessionState.RoomUsersInfo.ContainsKey(_sessionState.UserID))
                    _sessionState.RoomUsersInfo.TryAdd(_sessionState.UserID, _sessionState.UserInfo);
            }

            if ((inboundPacket.assetSpec?.Count ?? 0) > 0)
                ThreadManager.Current.DownloadAsset(_sessionState, inboundPacket.assetSpec.ToArray());

            ScriptEvents.Current.Invoke(IptEventTypes.MsgUserDesc, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_USERDESC[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USERDESC[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_USERDESC[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
