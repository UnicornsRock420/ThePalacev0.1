using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using ThePalace.Core.Constants;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Models
{
    public sealed class PalaceConnectionState : Disposable, IConnectionState
    {
        public Dictionary<DateTime, long> FloodControl { get; private set; } = new();

        public DateTime? LastPacketReceived { get; set; } = null;
        public DateTime? LastPacketSent { get; set; } = null;
        public DateTime? LastPinged { get; set; } = null;
        public int LatencyCounter { get; set; } = 0;

        public uint BytesRemaining { get; set; } = 0;
        public ConcurrentQueue<Header> Queue { get; set; } = new();
        public Header Packet { get; set; } = new();
        public byte[] Buffer { get; set; } = new byte[NetworkConstants.PALACE_PACKET_BUFFER_SIZE];

        public string ConnectionID { get; set; } = Guid.NewGuid().ToString();
        public string IPAddress { get; set; } = null;
        public string Host { get; set; }
        public ushort Port { get; set; }

        public object InitializeSocket() =>
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public object Socket { get; set; }
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (Socket is Socket _socket)
                        return _socket?.Connected ?? false;
                }
                catch { return false; }

                return false;
            }
        }

        public PalaceConnectionState() { }
        ~PalaceConnectionState() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            if (this.Socket is Socket _socket)
            {
                try { _socket?.Dispose(); this.Socket = null; } catch { }
            }

            FloodControl.Clear();
            FloodControl = null;
            Buffer = null;
        }
    }
}
