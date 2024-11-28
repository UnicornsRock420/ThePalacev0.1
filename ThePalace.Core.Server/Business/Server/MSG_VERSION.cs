using System.ComponentModel;
using System.Reflection;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Server.Network.Sockets;

namespace ThePalace.Core.Server.Business
{
    [Description("vers")]
    public class MSG_VERSION : IBusinessSend
    {
        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            // Send Server Version 'vers'

            _sessionState.Send(null, EventTypes.MSG_VERSION, (((version.Major & 0xFFFF) << 16) | (version.Minor & 0xFFFF)));
        }
    }
}
