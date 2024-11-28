using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Network
{
    [Description("bye ")]
    public sealed class MSG_LOGOFF : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_LOGOFF[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Network.MSG_LOGOFF;
            if (inboundPacket == null) return;

            _sessionState.ServerPopulation = inboundPacket.nbrUsers;

            var userID = (uint)inboundHeader.refNum;

            lock (_sessionState.RoomUsersInfo)
                if (_sessionState.RoomUsersInfo.ContainsKey(userID))
                {
                    if (_sessionState.RoomUsersInfo[userID].assetSpec != null)
                        AssetsManager.Current.FreeAssets(
                            false,
                            _sessionState.RoomUsersInfo[userID].assetSpec
                                .Select(p => p.id)
                                .ToArray());

                    _sessionState.RoomUsersInfo.TryRemove(userID, out var _);
                }

            ScriptEvents.Current.Invoke(IptEventTypes.UserLeave, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_LOGOFF[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_LOGOFF[" + nameof(sessionState) + "]");

            var outboundPacket = args.FirstOrDefault() as Header;
            if (outboundPacket != null)
            {
#if DEBUG
                Debug.WriteLine($"MSG_LOGOFF[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

                NetworkManager.Current.Send(sessionState, outboundPacket);
            }
        }
    }
}
