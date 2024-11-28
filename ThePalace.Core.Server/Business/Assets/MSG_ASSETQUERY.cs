using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("qAst")]
    [SuccessfullyConnectedProtocol]
    public class MSG_ASSETQUERY : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_ASSETQUERY)args.FirstOrDefault();

            if (inboundPacket.assetSpec.id != 0)
            {
                Logger.Log(MessageTypes.Info, $"MSG_ASSETQUERY[{_sessionState.UserID}]: {inboundPacket.assetSpec.id}, {inboundPacket.assetSpec.crc}");

                AssetLoader.OutboundQueueTransfer(_sessionState, inboundPacket.assetSpec);
            }
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var outboundPacket = (Protocols.MSG_ASSETQUERY)args.FirstOrDefault();

            _sessionState.Send(outboundPacket, EventTypes.MSG_ASSETQUERY, 0);
        }
    }
}
