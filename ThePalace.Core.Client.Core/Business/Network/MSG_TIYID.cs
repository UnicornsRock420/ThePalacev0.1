using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Network
{
    [Description("tiyr")]
    public sealed class MSG_TIYID : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            if (sessionState == null)
                throw new ArgumentNullException("MSG_TIYID[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            sessionState.UserInfo.userID = (uint)inboundHeader.refNum;

            NetworkManager.Current.Send(sessionState, new Header
            {
                eventType = EventTypes.MSG_LOGON,
                protocolSend = new Protocols.Network.MSG_LOGON(sessionState.RegInfo),
            });

#if DEBUG
            Debug.WriteLine($"MSG_TIYID[{sessionState.SessionID}:{sessionState.UserID}]");
#endif
        }
    }
}
