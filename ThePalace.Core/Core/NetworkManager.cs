using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core
{
    public sealed class NetworkManager : Disposable
    {
        private static readonly Regex REGEX_NONALPHANUMERIC = new Regex(@"[^\w\d]+", RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Lazy<NetworkManager> _current = new();
        public static NetworkManager Current => _current.Value;

        private AsyncCallback _acceptCallback;
        private AsyncCallback _receiveCallback;
        private AsyncCallback _sendCallback;

        private ManualResetEvent signalEvent = null;

        public CancellationTokenSource IsRunning { get; private set; } = new();

        public Type ProtocolsType { get; set; }
        public Type SessionStateType { get; set; }
        public Type ConnectionStateType { get; set; }

        public event EventHandler ConnectionAccepted = null;
        public event EventHandler ConnectionDisconnected = null;
        public event EventHandler ConnectionEstablished = null;
        public event EventHandler DataReceived = null;
        public event EventHandler DataSent = null;

        public NetworkManager()
        {
            _managedResources.Add(signalEvent);

            _acceptCallback = new AsyncCallback(AcceptCallback);
            _receiveCallback = new AsyncCallback(ReceiveCallback);
            _sendCallback = new AsyncCallback(SendCallback);
        }
        ~NetworkManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            Shutdown();

            _acceptCallback = null;
            _receiveCallback = null;
            _sendCallback = null;

            signalEvent = null;
            ProtocolsType = null;
            SessionStateType = null;
            ConnectionStateType = null;

            ConnectionAccepted?.Clear();
            ConnectionAccepted = null;
            ConnectionDisconnected?.Clear();
            ConnectionDisconnected = null;
            DataReceived?.Clear();
            DataReceived = null;
            DataSent?.Clear();
            DataSent = null;
        }

        public static Type GetType(Type AssemblyType, EventTypes eventType, params string[] keyElements)
        {
            if (eventType == 0) return null;

            var mnemonic = REGEX_NONALPHANUMERIC.Replace(eventType.ToString(), string.Empty);
            if (string.IsNullOrWhiteSpace(mnemonic)) return null;

            var lambda = (Func<Type, bool>)(t =>
            {
                if (!t.FullName.EndsWith(mnemonic)) return false;

                foreach (var k in keyElements)
                    if (!t.FullName.Contains(k))
                        return false;

                return true;
            });

            var type = null as Type;

            if (type == null)
                type = PluginManager.Current
                    .GetTypes()
                    .Where(lambda)
                    .FirstOrDefault();

            if (type == null)
                type = typeof(NetworkManager).Assembly
                    .GetTypes()
                    .Where(lambda)
                    .FirstOrDefault();

            if (type == null &&
                AssemblyType != null)
                type = AssemblyType.Assembly
                    .GetTypes()
                    .Where(lambda)
                    .FirstOrDefault();

            return type;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            if (IsRunning.IsCancellationRequested) return;
            else if (this.IsDisposed) return;

            signalEvent = new(false);
            signalEvent.Set();

            var listener = (Socket)ar.AsyncState;
            var sessionState = null as ISessionState;

            try
            {
                sessionState = SessionManager.Current.CreateSession(SessionStateType) as ISessionState;
                sessionState.ConnectionState = ConnectionManager.Current.CreateConnection(ConnectionStateType) as IConnectionState;

                var handler = listener.EndAccept(ar);
                sessionState.ConnectionState.Socket = handler;
                sessionState.ConnectionState.IPAddress = handler.GetIPAddress();
                handler.SetKeepAlive();

                handler.BeginReceive(sessionState.ConnectionState.Buffer, 0, sessionState.ConnectionState.Buffer.Length, 0, _receiveCallback, sessionState);

                this.ConnectionAccepted?.Invoke(sessionState, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (this.IsDisposed) return;

            var sessionState = (ISessionState)ar.AsyncState;
            if (sessionState == null) return;

            var handler = sessionState.ConnectionState.Socket as Socket;
            if (handler == null) return;

            var bytesRemaining = (uint)0;
            var bytesReceived = (uint)0;
            var packet = null as Header;
            var data = null as Packet;

            if (sessionState.ConnectionState != null)
                lock (sessionState.ConnectionState)
                    if (sessionState.ConnectionState.BytesRemaining > 0)
                    {
                        packet = sessionState.ConnectionState.Packet;
                        sessionState.ConnectionState.Packet = null;

                        bytesRemaining = sessionState.ConnectionState.BytesRemaining;
                        sessionState.ConnectionState.BytesRemaining = 0;
                    }

            try
            {
                bytesReceived = (uint)(handler?.EndReceive(ar) ?? 0);

                if (bytesReceived == 0)
                {
                    this.Disconnect(sessionState, true);

                    return;
                }
            }
            catch (SocketException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif

                this.Disconnect(sessionState, true);

                return;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif

                this.Disconnect(sessionState, true);

                return;
            }

            if (bytesReceived > 0)
            {
                try
                {
                    sessionState.ConnectionState.LastPacketReceived = DateTime.UtcNow;

                    data = new Packet(sessionState.ConnectionState.Buffer
                        .Take((int)bytesReceived)
                        .ToList());

                    while (bytesReceived > 0)
                    {
                        if (packet == null ||
                            bytesRemaining < 1)
                        {
                            try
                            {
                                packet = new Header(data);
                                bytesRemaining = packet.length;
                                bytesReceived -= (uint)Header.SizeOf;
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(ex.Message);
#endif

                                this.Disconnect(sessionState, true);

                                return;
                            }

                            try
                            {
                                packet.protocolReceiveType = GetType(this.ProtocolsType, packet.eventType, "Protocols");
                                if (packet.protocolReceiveType == null)
                                {
                                    this.Disconnect(sessionState, true);

                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine($"{packet.eventType}: {ex.Message}");
#endif

                                this.Disconnect(sessionState, true);

                                return;
                            }
                        }

                        var toRead = bytesRemaining > bytesReceived ? bytesReceived : bytesRemaining;
                        if (toRead > 0)
                        {
                            try
                            {
                                packet.WriteBytes(data.GetData((int)toRead), (int)toRead);
                                bytesRemaining -= toRead;
                                bytesReceived -= toRead;

                                data.DropBytes((int)toRead);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(nameof(NetworkManager) + $": {packet.eventType}: {ex.Message}");
#endif

                                this.Disconnect(sessionState, true);

                                return;
                            }
                        }

                        if (bytesRemaining > 0 &&
                            bytesReceived < 1)
                        {
                            sessionState.ConnectionState.Packet = packet;
                            sessionState.ConnectionState.BytesRemaining = bytesRemaining;

                            break;
                        }
                        else if (bytesRemaining < 1)
                        {
                            try
                            {
                                packet.protocolReceiveType = GetType(this.ProtocolsType, packet.eventType, "Protocols");
                                if (packet.protocolReceiveType == null)
                                {
                                    this.Disconnect(sessionState, true);

                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(nameof(NetworkManager) + $": {packet.eventType}: {ex.Message}");
#endif

                                this.Disconnect(sessionState, true);

                                return;
                            }

#if DEBUG
                            Debug.WriteLine(nameof(NetworkManager) + $": {packet.eventType}");
#endif

                            try
                            {
                                if (packet.eventType == EventTypes.MSG_PING)
                                {
                                    sessionState.ConnectionState.LastPinged = DateTime.UtcNow;
                                }

                                lock (sessionState.ConnectionState.Queue)
                                {
                                    sessionState.ConnectionState.Queue.Enqueue(packet);
                                    packet = null;
                                }

                                this.DataReceived?.Invoke(sessionState, null);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Debug.WriteLine(ex.Message);
#endif

                                this.Disconnect(sessionState, true);

                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif

                    this.Disconnect(sessionState, true);

                    return;
                }
            }

            try
            {
                handler.BeginReceive(sessionState.ConnectionState.Buffer, 0, sessionState.ConnectionState.Buffer.Length, 0, _receiveCallback, sessionState);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif

                this.Disconnect(sessionState, true);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            if (this.IsDisposed) return;

            var sessionState = (ISessionState)ar.AsyncState;

            try
            {
                var handler = sessionState.ConnectionState.Socket as Socket;
                if (handler == null) return;

                var bytesSent = handler.EndSend(ar);

                sessionState.ConnectionState.LastPacketSent = DateTime.UtcNow;

                this.DataSent?.Invoke(sessionState, null);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif

                this.Disconnect(sessionState, true);
            }
        }

        public void Shutdown()
        {
            if (!this.IsRunning.IsCancellationRequested)
            {
                this.IsRunning.Cancel();

                signalEvent?.Set();
            }
        }
        public void Disconnect(ISessionState sessionState, bool connectionError = false)
        {
            if (this.IsDisposed) return;

            if (sessionState.ConnectionState != null)
            {
                lock (sessionState.ConnectionState.Queue)
                {
                    sessionState.ConnectionState.Queue.Clear();
                }

                if (sessionState.ConnectionState.Socket != null)
                {
                    if (sessionState.ConnectionState.Socket is Socket _socket)
                    {
                        try { _socket?.Disconnect(false); } catch { }
                        try { _socket?.Dispose(); } catch { }
                    }
                    else if (sessionState.ConnectionState.Socket is IDisposable _dispose)
                    {
                        try { _dispose?.Dispose(); } catch { }
                    }

                    sessionState.ConnectionState.Socket = null;
                }

                sessionState.ConnectionState.BytesRemaining = 0;
                sessionState.ConnectionState.Packet = null;
                sessionState.ConnectionState.Host = string.Empty;
                sessionState.ConnectionState.Port = 0;
            }

            this.ConnectionDisconnected?.Invoke(sessionState, null);

            if (connectionError)
                ScriptEvents.Current.Invoke(IptEventTypes.ConnectionError, sessionState, null, sessionState.ScriptState);
            else
                ScriptEvents.Current.Invoke(IptEventTypes.Disconnect, sessionState, null, sessionState.ScriptState);
        }
        public void Listen(string bindAddress, ushort bindPort, int backlog = 100)
        {
            if (this.IsDisposed) return;

            var ipAddress = null as IPAddress;

            if (string.IsNullOrWhiteSpace(bindAddress) || !IPAddress.TryParse(bindAddress, out ipAddress))
                ipAddress = Dns.GetHostEntry(Dns.GetHostName())?.AddressList?.FirstOrDefault();

            if (ipAddress == null)
                throw new Exception($"Cannot bind to {bindAddress}:{bindPort} (address:port)...");

            try
            {
                var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Any, bindPort);

                listener.Bind(endPoint);

#if DEBUG
                Debug.WriteLine("Palace Socket Listener Operational. Waiting for connections...");
#endif

                listener.Listen(backlog);

                while (!this.IsRunning.IsCancellationRequested)
                {
                    signalEvent.Reset();

                    listener.BeginAccept(_acceptCallback, listener);

                    signalEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
        }
        public bool Connect(ISessionState sessionState, string host, ushort port = 9998)
        {
            if (this.IsDisposed) return false;

            ArgumentNullException.ThrowIfNull(sessionState, nameof(sessionState));
            ArgumentNullException.ThrowIfNull(host, nameof(host));

            if (sessionState.ConnectionState == null)
            {
                sessionState.ConnectionState = ConnectionManager.Current.CreateConnection(ConnectionStateType) as IConnectionState;
            }

            if (sessionState.ConnectionState.Socket == null)
            {
                if (sessionState.ConnectionState.InitializeSocket == null)
                    throw new Exception("Undefined InitializeSocket()...");

                sessionState.ConnectionState.Socket = sessionState.ConnectionState.InitializeSocket();
            }

            if (sessionState.ConnectionState.Socket is Socket handler)
            {
                if (handler.Connected)
                    this.Disconnect(sessionState);

                try
                {
                    handler.Connect(host, port);

                    sessionState.ConnectionState.Host = host;
                    sessionState.ConnectionState.Port = port;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif

                    this.Disconnect(sessionState, true);
                }

                try
                {
                    handler.BeginReceive(sessionState.ConnectionState.Buffer, 0, sessionState.ConnectionState.Buffer.Length, 0, _receiveCallback, sessionState);

                    this.ConnectionEstablished?.Invoke(sessionState, null);

                    ScriptEvents.Current.Invoke(IptEventTypes.ConnectionEstablished, sessionState, null, sessionState.ScriptState);

                    return true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif

                    this.Disconnect(sessionState, true);
                }
            }

            return false;
        }
        public void Send(ISessionState sessionState, IProtocolSend protocol)
        {
            if (this.IsDisposed) return;

            if ((sessionState.ConnectionState?.IsConnected ?? false) == false)
            {
                this.Disconnect(sessionState, true);

                return;
            }

            try
            {
                var handler = sessionState.ConnectionState.Socket as Socket;
                if (handler == null) return;

                var byteData = protocol.Serialize();
                if (byteData == null ||
                    byteData.Length < 1) return;

                handler.BeginSend(byteData, 0, byteData.Length, 0, _sendCallback, sessionState);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif

                this.Disconnect(sessionState, true);
            }
        }
    }
}
