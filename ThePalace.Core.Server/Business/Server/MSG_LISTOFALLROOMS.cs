using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Business
{
    [Description("rLst")]
    [SuccessfullyConnectedProtocol]
    public class MSG_LISTOFALLROOMS : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var listOfAllRooms = new Protocols.MSG_LISTOFALLROOMS();

            _sessionState.Send(listOfAllRooms, EventTypes.MSG_LISTOFALLROOMS, 0);
        }
    }
}
