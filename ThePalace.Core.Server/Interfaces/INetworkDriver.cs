using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface INetworkDriver
    {
        void Send(ISessionState sessionState, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum);

        string GetIPAddress();

        bool IsConnected();

        void DropConnection();
    }
}
