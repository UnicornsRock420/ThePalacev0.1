using Newtonsoft.Json;
using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Sockets;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Network.Drivers
{
    public class WebSocketDriver : INetworkDriver
    {
        public WebSocketConnectionState connectionState;

        public void Send(ISessionState sessionState, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            var header = new Header
            {
                eventType = eventType,
                refNum = refNum,
            };
            try
            {
                header.message = sendProtocol?.SerializeJSON(sessionState, header);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
            var json = JsonConvert.SerializeObject(new object[] {
                header.eventType.ToString(),
                header.refNum,
                header.message,
            });

            WebAsyncSocket.Send(sessionState, connectionState, json);
        }

        public string GetIPAddress()
        {
            return connectionState.IPAddress;
        }

        public bool IsConnected()
        {
            return WebAsyncSocket.IsConnected(connectionState);
        }

        public void DropConnection()
        {
            WebAsyncSocket.DropConnection(connectionState);
        }
    }
}
