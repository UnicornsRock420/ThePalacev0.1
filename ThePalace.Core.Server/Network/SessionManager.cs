using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Drivers;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Network
{
    public static class SessionManager
    {
        public static volatile ConcurrentDictionary<UInt32, SessionState> sessionStates;
        public static volatile Queue<Message> messages;

        public static void Initialize()
        {
            sessionStates = new ConcurrentDictionary<UInt32, SessionState>();
            messages = new Queue<Message>();
        }

        public static void Dispose()
        {
            lock (messages)
            {
                sessionStates.Clear();
                sessionStates = null;
            }

            lock (messages)
            {
                messages.Clear();
                messages = null;
            }
        }

        public static SessionState CreateSession(SessionTypes sessionType)
        {
            var maxOccupany = ConfigManager.GetValue<UInt32>("MaxOccupany", 100).Value;
            var maxUserID = ConfigManager.GetValue<UInt32>("MaxUserID", 9999).Value;
            var userID = (UInt32)0;
            var loops = 0;

            if (sessionStates.Count >= maxOccupany)
            {
                return null; // Server Full!
            }

            do
            {
                userID = ServerState.lastIssuedUserID = ((ServerState.lastIssuedUserID + 1) % maxUserID);

                if (userID == 0) loops++;
                if (loops > 1) return null; // Server Full!
            } while (userID == 0 || sessionStates.ContainsKey(userID));

            INetworkDriver driver = null;

            switch (sessionType)
            {
                case SessionTypes.TcpSocket:
                    driver = new PalaceSocketDriver();

                    break;
                //case SessionTypes.ProxySocket:
                //    driver = new ProxySocketDriver();

                //    break;
                case SessionTypes.WebSocket:
                    driver = new WebSocketDriver();

                    break;
            }

            lock (sessionStates)
            {
                sessionStates.TryAdd(userID, new SessionState
                {
                    UserID = userID,
                    driver = driver,
                });
            }

            return sessionStates[userID];
        }

        public static UInt32 GetServerUserCount()
        {
            return (UInt32)sessionStates.Values
                .Where(c => c.successfullyConnected)
                .Count();
        }

        public static UInt32 GetRoomUserCount(Int16 RoomID, UInt32 excludeUserID = 0)
        {
            if (RoomID == 0)
            {
                return 0;
            }

            return (UInt32)sessionStates.Values
                .Where(u => excludeUserID == 0 || u.UserID != excludeUserID)
                .Where(u => u.successfullyConnected)
                .Where(u => u.RoomID == RoomID)
                .Count();
        }

        public static void SendToUserID(UInt32 UserID, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            var state = sessionStates[UserID];

            state.Send(sendProtocol, eventType, refNum);
        }

        public static void SendToUserIDs(List<UInt32> UserIDs, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            foreach (var UserID in UserIDs)
            {
                var state = sessionStates[UserID];

                state.Send(sendProtocol, eventType, refNum);
            }
        }

        public static void SendToRoomID(Int16 RoomID, UInt32 excludeUserID, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            sessionStates.Values
                .Where(c => excludeUserID == 0 || c.UserID != excludeUserID)
                .Where(c => c.successfullyConnected)
                .Where(c => c.RoomID == RoomID)
                .ToList()
                .ForEach(s =>
                {
                    s.Send(sendProtocol, eventType, refNum);
                });
        }

        public static void SendToStaff(IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            sessionStates.Values
                .Where(c => c.successfullyConnected)
                .Where(c => c.Authorized)
                .ToList()
                .ForEach(s =>
                {
                    s.Send(sendProtocol, eventType, refNum);
                });
        }

        public static void SendToServer(IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            sessionStates.Values
                .Where(c => c.successfullyConnected)
                .ToList()
                .ForEach(s =>
                {
                    s.Send(sendProtocol, eventType, refNum);
                });
        }

        public static void Send(this SessionState sessionState, IProtocolSend sendProtocol, EventTypes eventType, Int32 refNum)
        {
            sessionState.driver.Send(sessionState, sendProtocol, eventType, refNum);
        }

        public static void ManageMessages()
        {
            Message message = null;

            while (messages.Count > 0)
            {
                lock (messages)
                {
                    if (messages.Count > 0)
                    {
                        message = messages.Dequeue();
                    }
                }

                if (message == null || !message.sessionState.driver.IsConnected())
                {
                    ThreadManager.manageMessagesQueueSignalEvent.Reset();

                    return;
                }

                using (new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadUncommitted,
                    }))
                using (var dbContext = DbConnection.For<ThePalaceEntities>())
                {
                    IBusinessReceive business = null;

                    try
                    {
                        var mnemonic = Regex.Replace(message.header.eventType.ToString(), @"[^\w\d]+", string.Empty);
                        var type = $"ThePalace.Server.Plugins.Business.{mnemonic}".GetType();

                        if (type == null)
                        {
                            type = Type.GetType($"ThePalace.Server.Business.{message.header.eventType}");
                        }

                        var parameters = new object[] {
                            new Dictionary<string, object> {
                                { "UserID", message.sessionState.UserID },
                            } };
                        var value = true;

                        value &= type.AttributeWrapper(typeof(SuccessfullyConnectedProtocolAttribute), "OnBeforeProtocolExecute", parameters);
                        value &= type.AttributeWrapper(typeof(AdminOnlyProtocolAttribute), "OnBeforeProtocolExecute", parameters);

                        if (!value)
                        {
                            return;
                        }

                        if (type != null)
                        {
                            business = (IBusinessReceive)Activator.CreateInstance(type);
                        }

                        if (business != null)
                        {
                            business.Receive(new SessionState
                            {
                                dbContext = dbContext,
                            }, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
        }
    }
}
