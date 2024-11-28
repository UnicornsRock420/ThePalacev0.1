using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("uLst")]
    [SuccessfullyConnectedProtocol]
    public class MSG_LISTOFALLUSERS : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var listOfAllUsers = new Protocols.MSG_LISTOFALLUSERS();

            _sessionState.Send(listOfAllUsers, EventTypes.MSG_LISTOFALLUSERS, 0);
        }
    }
}
