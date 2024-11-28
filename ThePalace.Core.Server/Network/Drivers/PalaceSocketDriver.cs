using System;
using System.Net.Sockets;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Sockets;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Network.Drivers
{
    public class PalaceSocketDriver : INetworkDriver
    {
        public PalaceConnectionState connectionState;

        public void Send(ISessionState sessionState, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            if (sendProtocol != null)
            {
                var header = new Header
                {
                    eventType = eventType,
                    refNum = refNum,
                };
                var data = Array.Empty<byte>();

                try
                {
                    data = sendProtocol.Serialize(sessionState, header);
                }
                catch (Exception ex)
                {
                    ex.Log();
                }

                header.length = (UInt32)(data == null ? 0 : data.Length);

                switch (eventType)
                {
                    case EventTypes.MSG_LISTOFALLROOMS:
                        header.refNum = (Int32)((Protocols.MSG_LISTOFALLROOMS)sendProtocol).nbrRooms;

                        break;
                    case EventTypes.MSG_LISTOFALLUSERS:
                        header.refNum = (Int32)((Protocols.MSG_LISTOFALLUSERS)sendProtocol).nbrUsers;

                        break;
                }

                PalaceAsyncSocket.Send(sessionState, header.Serialize(data));
            }
            else
            {
                var header = new Header
                {
                    eventType = eventType,
                    length = 0,
                    refNum = refNum,
                };

                PalaceAsyncSocket.Send(sessionState, header.Serialize());
            }
        }

        public string GetIPAddress()
        {
            var _socket = connectionState.Socket as Socket;
            return _socket.GetIPAddress();
        }

        public bool IsConnected()
        {
            return connectionState.IsConnected();
        }

        public void DropConnection()
        {
            var _socket = connectionState.Socket as Socket;
            _socket.DropConnection();
        }
    }
}
