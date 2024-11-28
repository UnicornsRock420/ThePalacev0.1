using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Business.Assets
{
    [Description("sAst")]
    public sealed class MSG_ASSETSEND : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as IClientSessionState;
            if (_sessionState == null)
                throw new ArgumentNullException("MSG_ASSETSEND[" + nameof(sessionState) + "]");

            var inboundHeader = args.FirstOrDefault() as Header;
            if (inboundHeader == null) return;

            var inboundPacket = inboundHeader.protocolReceive as Protocols.Assets.MSG_ASSETSEND;
            if (inboundPacket == null) return;

            if (inboundPacket.assetSpec.crc != 0 &&
                !inboundPacket.ValidateCrc()) return;

            AssetsManager.Current.RegisterAsset(inboundPacket);

            ScriptEvents.Current.Invoke(IptEventTypes.MsgAssetSend, sessionState, inboundHeader, sessionState.ScriptState);

#if DEBUG
            Debug.WriteLine($"MSG_ASSETSEND[{_sessionState.SessionID}:{_sessionState.UserID}]");
#endif
        }
    }
}
