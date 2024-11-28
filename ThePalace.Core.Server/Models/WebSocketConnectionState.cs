using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Constants;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Network.Sockets;

namespace ThePalace.Core.Server.Models
{
    public class WebSocketConnectionState : IConnectionState, IDisposable
    {
        public Dictionary<DateTime, long> FloodControl { get; } = new();

        public DateTime? LastPacketReceived { get; set; } = null;
        public DateTime? LastPacketSent { get; set; } = null;
        public DateTime? LastPinged { get; set; } = null;
        public int LatencyCounter { get; set; } = 0;

        public uint BytesRemaining { get; set; } = 0;
        public ConcurrentQueue<Header> Queue { get; set; } = new();
        public Header Packet { get; set; } = new Header();
        public byte[] Buffer { get; set; } = new byte[NetworkConstants.PALACE_PACKET_BUFFER_SIZE];

        public string ConnectionID { get; set; } = null;
        public string IPAddress { get; set; } = null;
        public string Host { get; set; }
        public ushort Port { get; set; }

        public object InitializeSocket() =>
            new WebSocketHub();
        public object Socket { get; set; } = null;
        public bool IsConnected
        {
            get
            {
                return false;
            }
        }

        public WebSocketConnectionState() { }

        public void Dispose()
        {
            FloodControl.Clear();
            Buffer = null;
        }
    }
}
