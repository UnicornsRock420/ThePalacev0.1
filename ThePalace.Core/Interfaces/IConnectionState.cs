using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Interfaces
{
    public interface IConnectionState : IDisposable
    {
        Dictionary<DateTime, long> FloodControl { get; }

        DateTime? LastPacketReceived { get; set; }
        DateTime? LastPacketSent { get; set; }
        DateTime? LastPinged { get; set; }
        int LatencyCounter { get; set; }

        uint BytesRemaining { get; set; }
        ConcurrentQueue<Header> Queue { get; set; }
        Header Packet { get; set; }
        byte[] Buffer { get; }

        string ConnectionID { get; set; }
        string IPAddress { get; set; }
        string Host { get; set; }
        ushort Port { get; set; }

        object InitializeSocket();
        object Socket { get; set; }
        bool IsConnected { get; }
    }
}
