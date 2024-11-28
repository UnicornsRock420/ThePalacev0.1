using System;
using System.ComponentModel;
using System.Diagnostics;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Business.Auth
{
    [Description("autr")]
    public sealed class MSG_AUTHRESPONSE : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_AUTHRESPONSE[" + nameof(sessionState) + "]");

#if DEBUG
            Debug.WriteLine($"MSG_AUTHRESPONSE[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

            //NetworkManager.Current.Send(sessionState, packet);
        }
    }
}
