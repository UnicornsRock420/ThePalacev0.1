using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("rAst")]
    [SuccessfullyConnectedProtocol]
    public class MSG_ASSETREGI : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_ASSETREGI)args.FirstOrDefault();

            if (inboundPacket.assetSpec.id != 0)
            {
                Logger.Log(MessageTypes.Info, $"MSG_ASSETREGI[{_sessionState.UserID}]: {inboundPacket.assetSpec.id}, {inboundPacket.assetSpec.crc}");

                var assetStream = new AssetStream(inboundPacket);

                AssetLoader.AppendInboundChunk(_sessionState, assetStream);

                ThreadManager.manageAssetsInboundQueueSignalEvent.Set();
            }
        }
    }
}
