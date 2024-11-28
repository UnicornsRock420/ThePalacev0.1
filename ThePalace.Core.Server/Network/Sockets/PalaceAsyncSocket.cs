using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Drivers;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Network.Sockets
{
    public static class PalaceAsyncSocket
    {
        private static readonly Regex _cleanMnemonic = new Regex(@"[^\w\d]+", RegexOptions.Singleline | RegexOptions.Compiled);

        private static volatile ManualResetEvent signalEvent = new ManualResetEvent(false);

        public static volatile ConcurrentDictionary<UInt32, PalaceConnectionState> connectionStates = new();

        public static void Initialize()
        {
            var bindAddress = ConfigManager.GetValue("BindAddress", string.Empty);
            var bindPalacePort = ConfigManager.GetValue<short>("BindPalacePort", 9998).Value;
            var listenBacklog = ConfigManager.GetValue<int>("ListenBacklog", 100).Value;
            IPAddress ipAddress = null;

            if (string.IsNullOrWhiteSpace(bindAddress) || !IPAddress.TryParse(bindAddress, out ipAddress))
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[0];
            }

            if (ipAddress == null)
            {
                throw new Exception($"Cannot bind to {bindAddress}:{bindPalacePort} (address:port)!");
            }

            var localEndPoint = new IPEndPoint(IPAddress.Any, bindPalacePort);
            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);

                Logger.ConsoleLog("Palace Socket Listener Operational. Waiting for connections...");

                listener.Listen(listenBacklog);

                while (!ServerState.isShutDown)
                {
                    signalEvent.Reset();

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    signalEvent.WaitOne();
                }

                ServerState.RefreshSettings();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void Shutdown()
        {
            signalEvent.Set();
        }

        public static void Dispose()
        {
            lock (connectionStates)
            {
                connectionStates.Keys.ToList().ForEach(key =>
                {
                    connectionStates[key].DropConnection();
                });
                connectionStates.Clear();
                connectionStates = null;
            }
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            signalEvent.Set();

            var listener = (Socket)ar.AsyncState;
            Socket handler = null;

            try
            {
                handler = listener.EndAccept(ar);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var sessionState = SessionManager.CreateSession(SessionTypes.TcpSocket);
            sessionState.ConnectionState = ConnectionManager.Current.CreateConnection<PalaceConnectionState>();
            sessionState.ConnectionState.Socket = handler;
            sessionState.ConnectionState.IPAddress = handler.GetIPAddress();

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var now = DateTime.UtcNow;
                var bans = dbContext.Bans.AsNoTracking()
                    .AsEnumerable()
                    .Where(b =>
                        b.Ipaddress == sessionState.ConnectionState.IPAddress &&
                        (!b.UntilDate.HasValue || b.UntilDate.Value < now))
                    .Count();

                if (bans > 0)
                {
                    Logger.Log(MessageTypes.Info, $"Banned connection from: {sessionState.ConnectionState.IPAddress}");

                    var data = new Protocols.MSG_SERVERDOWN
                    {
                        whyMessage = "You have been banned!",
                    }.Serialize();

                    handler.Send(new Header
                    {
                        eventType = EventTypes.MSG_SERVERDOWN,
                        length = (UInt32)data.Length,
                        refNum = (Int32)ServerDownFlags.SD_Banished,
                    }.Serialize(data));

                    handler.DropConnection();

                    return;
                }
            }

            handler.SetKeepAlive();

            if (sessionState == null)
            {
                Logger.Log(MessageTypes.Info, $"Server is full, turned away: {sessionState.ConnectionState.IPAddress}");

                var data = new Protocols.MSG_SERVERDOWN
                {
                    whyMessage = "The Server is full!",
                }.Serialize();

                handler.Send(new Header
                {
                    eventType = EventTypes.MSG_SERVERDOWN,
                    length = (UInt32)data.Length,
                    refNum = (Int32)ServerDownFlags.SD_ServerFull,
                }.Serialize(data));

                handler.DropConnection();

                return;
            }

            lock (connectionStates)
            {
                connectionStates.TryAdd(
                    sessionState.UserID,
                    sessionState.ConnectionState as PalaceConnectionState);

                ((PalaceSocketDriver)sessionState.driver).connectionState = connectionStates[sessionState.UserID];
            }

            Logger.Log(MessageTypes.Info, $"Connection from: {sessionState.ConnectionState.IPAddress}[{sessionState.UserID}]");

            try
            {
                handler.BeginReceive(connectionStates[sessionState.UserID].Buffer, 0, connectionStates[sessionState.UserID].Buffer.Length, 0, new AsyncCallback(ReadCallback), sessionState);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            new Business.MSG_TIYID().Send(null, new Message
            {
                sessionState = sessionState,
            });
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            var floodControlThreadshold_InMilliseconds = ConfigManager.GetValue<int>("FloodControlThreadshold_InMilliseconds", 1000).Value;
            var floodControlThreadshold_RawCount = ConfigManager.GetValue<int>("FloodControlThreadshold_RawCount", 100).Value;
            //var floodControlThreadshold_RawSize = ConfigManager.GetValue<int>("FloodControlThreadshold_RawSize", ???).Value;
            var floodControlThreadshold_TimeSpan = new TimeSpan(0, 0, 0, 0, floodControlThreadshold_InMilliseconds);

            var sessionState = (SessionState)ar.AsyncState;
            PalaceConnectionState connectionState;

            try
            {
                connectionState = connectionStates[sessionState.UserID];
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                return;
            }

            var handler = connectionState.Socket as Socket;
            var bytesReceived = (uint)0;

            try
            {
                bytesReceived = (uint)handler.EndReceive(ar);
            }
            catch (SocketException ex)
            {
                ex.DebugLog();

                sessionState.driver.DropConnection();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            if (bytesReceived > 0)
            {
                var data = Packet.FromBytes(connectionState.Buffer);

                connectionState.LastPacketReceived = DateTime.UtcNow;

                if (!sessionState.Authorized)
                {
                    #region Flood Control
                    //connectionState.FloodControl[DateTime.UtcNow] = bytesReceived;
                    ////var rawSize = state.floodControl.Values.Sum();

                    //var expired = connectionState.FloodControl
                    //    .Where(f => f.Key > DateTime.UtcNow.Subtract(floodControlThreadshold_TimeSpan))
                    //    .Select(f => f.Key)
                    //    .ToList();

                    //expired.ForEach(f =>
                    //{
                    //    connectionState.FloodControl.Remove(f);
                    //});

                    //if (connectionState.FloodControl.Count > floodControlThreadshold_RawCount)
                    //{
                    //    Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Flood Control", "PalaceAsyncSocket.ReadCallback()");

                    //    new Business.MSG_SERVERDOWN
                    //    {
                    //        reason = ServerDownFlags.SD_Flood,
                    //        whyMessage = "Flood Control!",
                    //    }.Send(null, new Message
                    //    {
                    //        sessionState = sessionState,
                    //    });

                    //    connectionState.DropConnection();

                    //    return;
                    //}
                    #endregion
                }

                while (bytesReceived > 0)
                {
                    if (connectionState.Packet.eventType != 0 || connectionState.BytesRemaining < 1)
                    {
                        try
                        {
                            connectionState.Packet = new Header();
                            connectionState.Packet.Deserialize(data);
                        }
                        catch { }

                        var mnemonic = _cleanMnemonic.Replace(connectionState.Packet.eventType.ToString(), string.Empty);
                        var type = $"ThePalace.Server.Plugins.Protocols.{mnemonic}".GetType();

                        if (type == null)
                        {
                            type = Type.GetType($"ThePalace.Server.Protocols.{connectionState.Packet.eventType}");
                        }

                        if (type == null)
                        {
                            new Business.MSG_SERVERDOWN
                            {
                                reason = ServerDownFlags.SD_CommError,
                                whyMessage = "Communication Error!",
                            }.Send(null, new Message
                            {
                                sessionState = sessionState,
                            });

                            connectionState.DropConnection();
                        }

                        bytesReceived -= (uint)Header.SizeOf;

                        connectionState.BytesRemaining = connectionState.Packet.length;
                    }

                    var toRead = (int)(connectionState.BytesRemaining > bytesReceived ? bytesReceived : connectionState.BytesRemaining);

                    if (toRead > 0)
                    {
                        try
                        {
                            connectionState.Packet.WriteBytes(data.GetData(toRead), toRead);
                            connectionState.BytesRemaining -= (uint)toRead;
                            bytesReceived -= (uint)toRead;

                            data.DropBytes(toRead);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();

                            new Business.MSG_SERVERDOWN
                            {
                                reason = ServerDownFlags.SD_CommError,
                                whyMessage = "Communication Error!",
                            }.Send(null, new Message
                            {
                                sessionState = sessionState,
                            });

                            connectionState.DropConnection();
                        }
                    }

                    if (connectionState.BytesRemaining < 1)
                    {
                        var mnemonic = _cleanMnemonic.Replace(connectionState.Packet.eventType.ToString(), string.Empty);
                        var type = $"ThePalace.Server.Plugins.Protocols.{mnemonic}".GetType();
                        var message = (Message)null;

                        if (type == null)
                        {
                            type = Type.GetType($"ThePalace.Server.Protocols.{connectionState.Packet.eventType}");
                        }

                        if (type != null)
                        {
                            message = new Message
                            {
                                protocol = (IProtocolReceive)Activator.CreateInstance(type),
                                header = new Header(connectionState.Packet),
                                sessionState = sessionState,
                            };
                        }

                        if (message != null)
                        {
                            try
                            {
                                message.protocol.Deserialize(connectionState.Packet);

                                lock (SessionManager.messages)
                                {
                                    SessionManager.messages.Enqueue(message);

                                    ThreadManager.manageMessagesQueueSignalEvent.Set();
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            connectionState.LastPinged = null;
                            connectionState.LastPacketReceived = DateTime.UtcNow;
                        }
                    }
                }
            }
            else if (!connectionState.IsConnected())
            {
                connectionState.DropConnection();
            }

            try
            {
                handler.BeginReceive(connectionState.Buffer, 0, connectionState.Buffer.Length, 0, new AsyncCallback(ReadCallback), sessionState);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void Send(this ISessionState sessionState, byte[] byteData)
        {
            var _sessionState = sessionState as SessionState;

            if (_sessionState == null || !_sessionState.driver.IsConnected() || !connectionStates.ContainsKey(_sessionState.UserID))
            {
                return;
            }
            else if (byteData != null)
            {
                try
                {
                    var connectionState = connectionStates[_sessionState.UserID];

                    lock (_sessionState.driver)
                    {
                        var _socket = connectionState.Socket as Socket;
                        _socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), sessionState);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var sessionState = (SessionState)ar.AsyncState;

            if (sessionState == null || !sessionState.driver.IsConnected() || !connectionStates.ContainsKey(sessionState.UserID))
            {
                return;
            }

            try
            {
                var connectionState = connectionStates[sessionState.UserID];
                var handler = connectionState.Socket as Socket;
                var bytesSent = handler.EndSend(ar);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void ManageConnections()
        {
            var idleTimeout_InMinutes = ConfigManager.GetValue<Int16>("IdleTimeout_InMinutes", 10).Value;
            var idleTimeout_Timespan = new TimeSpan(0, idleTimeout_InMinutes, 0);
            var dosTimeout_InSeconds = ConfigManager.GetValue<Int16>("DOSTimeout_InSeconds", 25).Value;
            var dosTimeout_Timespan = new TimeSpan(0, 0, dosTimeout_InSeconds);
            var latencyMaxCounter = ConfigManager.GetValue<Int16>("LatencyMaxCounter", 25).Value;

            ThePalace.Core.Server.Network.SessionManager.sessionStates.Values.ToList().ForEach(sessionState =>
            {
                try
                {
                    var connectionState = sessionState.ConnectionState as PalaceConnectionState;

                    if (
                        // Disconnected client cleanup
                        !connectionState.IsConnected() ||
                        // DOS client cleanup
                        (connectionState.LatencyCounter > latencyMaxCounter) ||
                        (connectionState.LastPacketReceived.HasValue && connectionState.LastPacketReceived.HasValue && (connectionState.LastPacketReceived.Value.Subtract(connectionState.LastPacketReceived.Value) > dosTimeout_Timespan)))
                    {
                        if (!connectionState.IsConnected())
                        {
                            Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Client-side", "ManageConnections()");
                        }

                        if (connectionState.LatencyCounter > latencyMaxCounter)
                        {
                            Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Latency Counter", "ManageConnections()");
                        }

                        if (connectionState.LastPacketReceived.HasValue && connectionState.LastPacketReceived.HasValue && (connectionState.LastPacketReceived.Value.Subtract(connectionState.LastPacketReceived.Value) > dosTimeout_Timespan))
                        {
                            Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: DOS Attempt", "ManageConnections()");
                        }

                        lock (connectionStates)
                        {
                            connectionState.DropConnection();
                        }
                    }
                    else if (connectionState.LastPacketReceived.HasValue && !connectionState.LastPacketReceived.HasValue && (DateTime.UtcNow.Subtract(connectionState.LastPacketReceived.Value) <= dosTimeout_Timespan))
                    {
                        connectionState.LatencyCounter++;
                    }
                    else if (!connectionState.LastPinged.HasValue && connectionState.LastPacketReceived.HasValue && (DateTime.UtcNow.Subtract(connectionState.LastPacketReceived.Value) > idleTimeout_Timespan))
                    {
                        connectionState.LastPinged = DateTime.UtcNow;

                        // Idle clients

                        Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Idle user", "ManageConnections()");

                        sessionState.Send(null, EventTypes.MSG_PING, (Int32)sessionState.UserID);

                        new Business.MSG_PING().Send(sessionState);
                    }
                    else if (connectionState.LastPinged.HasValue && (DateTime.UtcNow.Subtract(connectionState.LastPinged.Value) > idleTimeout_Timespan))
                    {
                        // Idle clients

                        new Business.MSG_SERVERDOWN
                        {
                            reason = ServerDownFlags.SD_Unresponsive,
                            whyMessage = "Idle Disconnect!",
                        }.Send(sessionState, null);

                        lock (connectionStates)
                        {
                            connectionState.DropConnection();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        public static bool IsConnected(this PalaceConnectionState connectionState)
        {
            var passiveIdleTimeout_InSeconds = ConfigManager.GetValue<Int16>("PassiveIdleTimeout_InSeconds", 15).Value;
            var passiveIdleTimeout_Timespan = new TimeSpan(0, 0, passiveIdleTimeout_InSeconds);

            try
            {
                var _socket = connectionState.Socket as Socket;

                if (connectionState.LastPacketReceived.HasValue && (DateTime.UtcNow.Subtract(connectionState.LastPacketReceived.Value) > passiveIdleTimeout_Timespan))
                {
                    return !_socket.Poll(1, SelectMode.SelectRead);
                }

                return _socket.Connected;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                return false;
            }
        }

        public static void DropConnection(this PalaceConnectionState connectionState)
        {
            try
            {
                var sessionState = SessionManager.sessionStates.Values
                    .Where(s => s.ConnectionState == connectionState)
                    .FirstOrDefault();
                var _socket = connectionState.Socket as Socket;

                if (sessionState.successfullyConnected)
                {
                    using (new TransactionScope(TransactionScopeOption.Required,
                        new TransactionOptions
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        }))
                    using (var dbContext = DbConnection.For<ThePalaceEntities>())
                    {
                        dbContext.ExecStoredProcedure("EXEC Users.FlushUserDetails",
                            new SqlParameter("@userID", (int)sessionState.UserID));
                    }
                }

                _socket.DropConnection();
                connectionState.Dispose();

                if (connectionStates.ContainsKey(sessionState.UserID))
                {
                    lock (connectionStates)
                    {
                        if (connectionStates.ContainsKey(sessionState.UserID))
                        {
                            connectionStates.Remove(sessionState.UserID);
                        }
                    }
                }

                lock (SessionManager.sessionStates)
                {
                    if (SessionManager.sessionStates.ContainsKey(sessionState.UserID))
                    {
                        SessionManager.sessionStates[sessionState.UserID].Dispose();
                        SessionManager.sessionStates.Remove(sessionState.UserID);
                    }
                }

                if (sessionState.successfullyConnected)
                {
                    new Business.MSG_LOGOFF().SendToServer(sessionState);
                }

                if (SessionManager.GetRoomUserCount(sessionState.RoomID) < 1 && ServerState.roomsCache.ContainsKey(sessionState.RoomID))
                {
                    ServerState.roomsCache[sessionState.RoomID].Flags &= (~(int)RoomFlags.RF_Closed);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
