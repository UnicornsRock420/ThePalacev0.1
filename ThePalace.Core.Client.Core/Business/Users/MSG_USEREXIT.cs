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
    [Description("eprs")]
    public sealed class MSG_USEREXIT : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_USEREXIT[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            //var inboundPacket = inboundHeader.protocolReceive as Protocols.MSG_USEREXIT;
            //if (inboundPacket == null) return;

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

                    ScriptEvents.Current.Invoke(IptEventTypes.UserLeave, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
                    Debug.WriteLine($"MSG_USEREXIT[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
                }
        }
    }
}
