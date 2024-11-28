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
using System.Transactions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Drivers;
using ThePalace.Core.Utility;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ThePalace.Core.Server.Network.Sockets
{
    public static class WebAsyncSocket
    {
        private static readonly Regex _cleanMnemonic = new Regex(@"[^\w\d]+", RegexOptions.Singleline | RegexOptions.Compiled);

        public static volatile ConcurrentDictionary<UInt32, WebSocketConnectionState> connectionStates = new ConcurrentDictionary<UInt32, WebSocketConnectionState>();

        private static WebSocketServer listener = null;

        public static void Initialize()
        {
            var bindAddress = ConfigManager.GetValue("BindAddress", string.Empty);
            var bindWebSocketPort = ConfigManager.GetValue<short>("BindWebSocketPort", 10000).Value;
            var listenBacklog = ConfigManager.GetValue<int>("ListenBacklog", 100).Value;
            var ipAddress = null as IPAddress;

            if (string.IsNullOrWhiteSpace(bindAddress) || !IPAddress.TryParse(bindAddress, out ipAddress))
            {
                var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[0];
            }

            if (ipAddress == null)
            {
                throw new Exception($"Cannot bind to {bindAddress}:{bindWebSocketPort} (address:port)!");
            }

            listener = new WebSocketServer(ipAddress, bindWebSocketPort);
            listener.AddWebSocketService<WebSocketHub>("/PalaceWebSocket");

            try
            {
                ThePalace.Core.Utility.Logger.ConsoleLog("WebSocket Socket Listener Operational. Waiting for connections...");

                listener.Start();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void Shutdown()
        {
            listener.Stop();
        }

        public static void Dispose()
        {
            lock (connectionStates)
            {
                connectionStates.Values.ToList().ForEach(c =>
                {
                    c.DropConnection();
                });
                connectionStates.Clear();
                connectionStates = null;
            }
        }

        public static WebSocketConnectionState Accept(WebSocketHub hub)
        {
            var sessionState = SessionManager.CreateSession(SessionTypes.WebSocket);
            sessionState.ConnectionState = ConnectionManager.Current.CreateConnection<WebSocketConnectionState>();
            sessionState.ConnectionState.Socket = hub.Context.WebSocket;
            sessionState.ConnectionState.IPAddress = hub.Context.UserEndPoint.Address.MapToIPv4().ToString();

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
                    ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Banned connection from: {sessionState.ConnectionState.IPAddress}");

                    hub.Context.WebSocket.Send(new Header
                    {
                        eventType = EventTypes.MSG_SERVERDOWN,
                        refNum = (int)ServerDownFlags.SD_Banished,
                        message = new Protocols.MSG_SERVERDOWN
                        {
                            whyMessage = "You have been banned!",
                        }.SerializeJSON(),
                    }.SerializeJSON());

                    hub.Context.WebSocket.Close();

                    return null;
                }
            }

            if (sessionState == null)
            {
                ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Server is full, turned away: {sessionState.ConnectionState.IPAddress}");

                hub.Context.WebSocket.Send(new Header
                {
                    eventType = EventTypes.MSG_SERVERDOWN,
                    refNum = (int)ServerDownFlags.SD_ServerFull,
                    message = new Protocols.MSG_SERVERDOWN
                    {
                        whyMessage = "The Server is full!",
                    }.SerializeJSON(),
                }.SerializeJSON());

                hub.Context.WebSocket.Close();

                return null;
            }

            lock (connectionStates)
            {
                connectionStates.TryAdd(
                    sessionState.UserID,
                    sessionState.ConnectionState as WebSocketConnectionState);

                ((WebSocketDriver)sessionState.driver).connectionState = connectionStates[sessionState.UserID];
            }

            ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Connection from: {sessionState.ConnectionState.IPAddress}[{sessionState.UserID}]");

            new Business.MSG_TIYID().Send(sessionState);

            return connectionStates[sessionState.UserID];
        }

        public static void Receive(WebSocketConnectionState connectionState, MessageEventArgs e)
        {
            var floodControlThreadshold_InMilliseconds = ConfigManager.GetValue<int>("FloodControlThreadshold_InMilliseconds", 1000).Value;
            var floodControlThreadshold_RawCount = ConfigManager.GetValue<int>("FloodControlThreadshold_RawCount", 100).Value;
            //var floodControlThreadshold_RawSize = ConfigManager.GetValue<int>("FloodControlThreadshold_RawSize", ???).Value;
            var floodControlThreadshold_TimeSpan = new TimeSpan(0, 0, 0, 0, floodControlThreadshold_InMilliseconds);

            var sessionState = SessionManager.sessionStates.Values
                .Where(s => s.ConnectionState == connectionState)
                .FirstOrDefault();
            var handler = connectionState.Socket;
            var bytesReceived = e.Data?.Length ?? 0;
            var data = e.Data;

            if (bytesReceived > 0)
            {
                connectionState.LastPacketReceived = DateTime.UtcNow;

                if (!sessionState.Authorized)
                {
                    #region Flood Control
                    //connectionState.floodControl[DateTime.UtcNow] = bytesReceived;
                    ////var rawSize = state.floodControl.Values.Sum();

                    //var expired = connectionState.floodControl
                    //    .Where(f => f.Key > DateTime.UtcNow.Subtract(floodControlThreadshold_TimeSpan))
                    //    .Select(f => f.Key)
                    //    .ToList();

                    //expired.ForEach(f =>
                    //{
                    //    connectionState.floodControl.Remove(f);
                    //});

                    //if (connectionState.floodControl.Count > floodControlThreadshold_RawCount)
                    //{
                    //    Utility.Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Flood Control", "WebAsyncSocket.Receive()");

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

                if (bytesReceived > 0)
                {
                    try
                    {
                        connectionState.Packet = new Header();
                        connectionState.Packet.DeserializeJSON(data);
                    }
                    catch { }

                    var mnemonic = _cleanMnemonic.Replace(connectionState.Packet.eventType.ToString(), string.Empty);
                    var type = $"ThePalace.Server.Plugins.Protocols.{mnemonic}".GetType();
                    var message = (Message)null;

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
                            message.protocol.DeserializeJSON(connectionState.Packet.message);

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
            else if (!connectionState.IsConnected())
            {
                connectionState.DropConnection();
            }
        }

        public static void Send(ISessionState sessionState, WebSocketConnectionState connectionState, string data)
        {
            var _sessionState = sessionState as SessionState;

            if (_sessionState == null || !_sessionState.driver.IsConnected())
            {
                return;
            }

            try
            {
                lock (_sessionState.driver)
                {
                    var _socket = connectionState.Socket as WebSocketHub;
                    _socket.Send(data);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static bool IsConnected(this WebSocketConnectionState connectionState)
        {
            try
            {
                var _socket = connectionState.Socket as WebSocketHub;
                return _socket.Context.WebSocket.IsAlive;
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

        public static void DropConnection(this WebSocketConnectionState connectionState)
        {
            try
            {
                var sessionState = SessionManager.sessionStates.Values
                    .Where(s => s.ConnectionState == connectionState)
                    .FirstOrDefault();
                var _socket = connectionState.Socket as WebSocketHub;

                if (sessionState.successfullyConnected)
                {
                    try
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
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                try
                {
                    _socket.Context.WebSocket.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                try
                {
                    connectionState.Dispose();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

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

                if (SessionManager.sessionStates.ContainsKey(sessionState.UserID))
                {
                    lock (SessionManager.sessionStates)
                    {
                        if (SessionManager.sessionStates.ContainsKey(sessionState.UserID))
                        {
                            SessionManager.sessionStates[sessionState.UserID].Dispose();
                            SessionManager.sessionStates.Remove(sessionState.UserID);
                        }
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

        public static void ManageConnections()
        {
            var idleTimeout_InMinutes = ConfigManager.GetValue<Int16>("IdleTimeout_InMinutes", 10).Value;
            var idleTimeout_Timespan = new TimeSpan(0, idleTimeout_InMinutes, 0);
            var dosTimeout_InSeconds = ConfigManager.GetValue<Int16>("DOSTimeout_InSeconds", 25).Value;
            var dosTimeout_Timespan = new TimeSpan(0, 0, dosTimeout_InSeconds);
            var latencyMaxCounter = ConfigManager.GetValue<Int16>("LatencyMaxCounter", 25).Value;

            Server.Network.SessionManager.sessionStates.Values.ToList().ForEach(sessionState =>
            {
                try
                {
                    var connectionState = sessionState.ConnectionState as PalaceConnectionState;

                    if (
                        // Disconnected client cleanup
                        !connectionState.IsConnected() ||
                        // DOS client cleanup
                        (connectionState.LastPacketReceived.HasValue && connectionState.LastPacketReceived.HasValue && (connectionState.LastPacketReceived.Value.Subtract(connectionState.LastPacketReceived.Value) > dosTimeout_Timespan)))
                    {
                        if (!connectionState.IsConnected())
                        {
                            ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Client-side", "WebAsyncSocket.ManageConnections()");
                        }

                        if (connectionState.LastPacketReceived.HasValue && connectionState.LastPacketReceived.HasValue && (connectionState.LastPacketReceived.Value.Subtract(connectionState.LastPacketReceived.Value) > dosTimeout_Timespan))
                        {
                            ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: DOS Attempt", "WebAsyncSocket.ManageConnections()");
                        }

                        lock (connectionStates)
                        {
                            connectionState.DropConnection();
                        }
                    }
                    else if (!connectionState.LastPinged.HasValue && connectionState.LastPacketReceived.HasValue && (DateTime.UtcNow.Subtract(connectionState.LastPacketReceived.Value) > idleTimeout_Timespan))
                    {
                        connectionState.LastPinged = DateTime.UtcNow;

                        // Idle clients

                        ThePalace.Core.Utility.Logger.Log(MessageTypes.Info, $"Disconnect[{sessionState.UserID}]: Idle user", "WebAsyncSocket.ManageConnections()");

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
                        }.Send(sessionState);

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
    }
}
