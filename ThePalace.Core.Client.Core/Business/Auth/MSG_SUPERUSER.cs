using System;
using System.ComponentModel;
using System.Diagnostics;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Business.Auth
{
    [Description("susr")]
    public sealed class MSG_SUPERUSER : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_SUPERUSER[" + nameof(sessionState) + "]");

#if DEBUG
            Debug.WriteLine($"MSG_SUPERUSER[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif

            //NetworkManager.Current.Send(sessionState, packet);
        }
    }
}
