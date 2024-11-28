using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("sFil")]
    [SuccessfullyConnectedProtocol]
    public class MSG_FILESEND : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var mediaState = (MediaState)args.FirstOrDefault();
            if (mediaState != null) return;

            _sessionState.Send(mediaState.mediaStream, EventTypes.MSG_FILESEND, 0);
        }
    }
}
