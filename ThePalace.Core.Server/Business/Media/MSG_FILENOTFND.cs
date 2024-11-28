using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("fnfe")]
    public class MSG_FILENOTFND : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_FILEQUERY)args.FirstOrDefault();
            var fileNotFound = new Protocols.MSG_FILENOTFND
            {
                fileName = inboundPacket.fileName,
            };

            _sessionState.Send(fileNotFound, EventTypes.MSG_FILENOTFND, (Int32)_sessionState.UserID);
        }
    }
}
