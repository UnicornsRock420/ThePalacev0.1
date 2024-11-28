using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("qFil")]
    [SuccessfullyConnectedProtocol]
    public class MSG_FILEQUERY : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_FILEQUERY)args.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(inboundPacket.fileName))
            {
                Logger.Log(MessageTypes.Info, $"MSG_FILEQUERY[{_sessionState.UserID}]: {inboundPacket.fileName}");

                MediaStream media = new MediaStream(inboundPacket.fileName);

                if (media.Open() && media.hasData)
                {
                    FileLoader.QueueTransfer(_sessionState, media);
                }
                else if (!media.FileExists)
                {
                    new MSG_FILENOTFND().Send(sessionState, args);
                }
            }
        }
    }
}
