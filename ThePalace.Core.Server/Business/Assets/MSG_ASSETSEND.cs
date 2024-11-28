using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("sAst")]
    [SuccessfullyConnectedProtocol]
    public class MSG_ASSETSEND : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var assetState = args.FirstOrDefault() as AssetState;
            if (assetState != null) return;

            _sessionState.Send(assetState.assetStream, EventTypes.MSG_ASSETSEND, 0);
        }
    }
}
