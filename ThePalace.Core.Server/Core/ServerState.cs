using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Transactions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Factories;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network.Sockets;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Core
{
    public static class ServerState
    {
        private static readonly Type receiveProtocol = typeof(IProtocolReceive);
        public static volatile ConcurrentDictionary<Int16, RoomBuilder> roomsCache = new ConcurrentDictionary<Int16, RoomBuilder>();
        public static volatile UInt32 lastIssuedUserID = 0;
        public static volatile bool isShutDown = false;

        public static volatile Int32 serverPermissions = (int)(ServerPermissions.PM_AllowCyborgs | ServerPermissions.PM_AllowGuests | ServerPermissions.PM_AllowPainting | ServerPermissions.PM_AllowWizards | ServerPermissions.PM_WizardsMayKill | ServerPermissions.PM_WizardsMayAuthor);
        public static volatile List<Int16> entryRoomIDs = new List<Int16>();
        public static volatile string serverName = string.Empty;
        private static DateTime? _lastCycleDate = null;
        public static volatile string mediaUrl = null;
        public static volatile UInt32 maxOccupancy = 0;
        public static volatile UInt32 nbrRooms = 0;

        public static void Initialize()
        {
            Cipher.InitializeTable();

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                try
                {
                    dbContext.Users1.Clear();
                    dbContext.UserData.Clear();
                    dbContext.Assets1.Clear();

                    dbContext.Rooms
                        .Where(r => (r.Flags & (int)RoomFlags.RF_Closed) != 0)
                        .ToList()
                        .ForEach(r =>
                        {
                            r.Flags &= ~(int)RoomFlags.RF_Closed;
                        });

                    if (dbContext.HasUnsavedChanges())
                    {
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            PluginManager.Current.LoadPlugins();
            Network.SessionManager.Initialize();
            ThreadManager.Initialize();
        }

        public static void FlushRooms(ThePalaceEntities dbContext)
        {
            roomsCache.Values
                .ToList()
                .ForEach(r =>
                {
                    var nbrUsers = Network.SessionManager.GetRoomUserCount(r.ID);

                    if (r.HasUnsavedChanges)
                    {
                        if (nbrUsers < 1 && (r.Flags & (int)RoomFlags.RF_Closed) != 0)
                        {
                            r.Flags &= ~(int)RoomFlags.RF_Closed;
                        }

                        r.Write(dbContext);
                    }

                    if (nbrUsers < 1 && (r.Flags & (int)RoomFlags.RF_DropZone) == 0)
                    {
                        lock (roomsCache)
                        {
                            roomsCache.Remove(r.ID);
                        }
                    }
                });
        }

        public static void RefreshSettings()
        {
            var _serverName = ConfigManager.GetValue("ServerName", string.Empty, true);
            var _mediaUrl = ConfigManager.GetValue("MediaUrl", string.Empty, true);
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                }))
            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var sessionStateContainer = new SessionState
                {
                    dbContext = dbContext,
                };

                if (!isShutDown)
                {
                    try
                    {
                        nbrRooms = (UInt32)dbContext.Rooms.AsNoTracking().Count();

                        entryRoomIDs = dbContext.Rooms.AsNoTracking()
                            .Where(r => (r.Flags & (int)RoomFlags.RF_DropZone) != 0)
                            .Select(r => r.RoomId)
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (serverName != _serverName)
                    {
                        serverName = _serverName;

                        new Business.MSG_SERVERINFO().SendToServer(sessionStateContainer, null);
                    }

                    if (mediaUrl != _mediaUrl)
                    {
                        mediaUrl = _mediaUrl;

                        new Business.MSG_HTTPSERVER().SendToServer(sessionStateContainer, null);
                    }
                }

                FlushRooms(dbContext);

                if (isShutDown)
                {
                    dbContext.Users1.Clear();
                    dbContext.UserData.Clear();
                    dbContext.Assets1.Clear();

                    dbContext.Rooms
                        .Where(r => (r.Flags & (int)RoomFlags.RF_Closed) != 0)
                        .ToList()
                        .ForEach(r =>
                        {
                            r.Flags &= ~(int)RoomFlags.RF_Closed;
                        });
                }
                else
                {
                    dbContext.Rooms.AsNoTracking()
                        .Where(r => roomsCache.Keys.Contains(r.RoomId))
                        .Where(r => _lastCycleDate == null || r.LastModified > _lastCycleDate)
                        .ToList()
                        .ForEach(r =>
                        {
                            if (r.LastModified > roomsCache[r.RoomId].LastModified)
                            {
                                roomsCache[r.RoomId].Read(dbContext);
                            }
                        });

                    dbContext.Rooms.AsNoTracking()
                        .Where(r => !roomsCache.Keys.Contains(r.RoomId))
                        .Where(r => (r.Flags & (int)RoomFlags.RF_DropZone) != 0)
                        .ToList()
                        .ForEach(r =>
                        {
                            roomsCache[r.RoomId] = new RoomBuilder(r.RoomId);
                            roomsCache[r.RoomId].Read(dbContext);
                        });

                    var sessionUserIDs = Network.SessionManager.sessionStates.Keys.Select(i => (Int32)i).ToList();

                    dbContext.Users1
                        .Where(u => !sessionUserIDs.Contains(u.UserId))
                        .ToList()
                        .ForEach(user =>
                        {
                            dbContext.Users1.Remove(user);
                        });

                    dbContext.Users1
                        .Where(u => sessionUserIDs.Contains(u.UserId))
                        .ToList()
                        .ForEach(user =>
                        {
                            var sessionState = Network.SessionManager.sessionStates[(UInt32)user.UserId];

                            user.Flags = (short)sessionState.UserFlags;
                            user.RoomId = sessionState.RoomID;
                            user.Name = sessionState.UserInfo.name;
                        });

                    dbContext.UserData
                        .Where(u => !sessionUserIDs.Contains(u.UserId))
                        .ToList()
                        .ForEach(user =>
                        {
                            dbContext.UserData.Remove(user);
                        });

                    dbContext.UserData
                        .Where(u => sessionUserIDs.Contains(u.UserId))
                        .ToList()
                        .ForEach(user =>
                        {
                            var sessionState = Network.SessionManager.sessionStates[(UInt32)user.UserId];

                            user.RoomPosH = sessionState.UserInfo.roomPos.h;
                            user.RoomPosV = sessionState.UserInfo.roomPos.v;
                            user.FaceNbr = sessionState.UserInfo.faceNbr;
                            user.ColorNbr = sessionState.UserInfo.colorNbr;
                        });

                    Network.SessionManager.sessionStates.Values.ToList().ForEach(sessionState =>
                    {
                        dbContext.ExecStoredProcedure("EXEC Users.FlushUserAssets",
                            new SqlParameter("@userID", (int)sessionState.UserID));

                        if ((sessionState.UserFlags & (UserFlags.U_Pin | UserFlags.U_PropGag)) == 0)
                        {
                            for (var j = 0; j < (sessionState.UserInfo.assetSpec?.Count ?? 0); j++)
                            {
                                dbContext.Assets1.Add(new Assets1
                                {
                                    UserId = (int)sessionState.UserID,
                                    AssetIndex = (byte)(j + 1),
                                    AssetId = sessionState.UserInfo.assetSpec[j].id,
                                    AssetCrc = (int)sessionState.UserInfo.assetSpec[j].crc,
                                });
                            }
                        }
                    });
                }

                if (dbContext.HasUnsavedChanges())
                {
                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }

            _lastCycleDate = DateTime.UtcNow;
        }

        public static void Shutdown()
        {
            var threadShutdownWait_InMilliseconds = ConfigManager.GetValue<int>("ThreadShutdownWait_InMilliseconds", 2500).Value;

            isShutDown = true;

            new Business.MSG_SERVERDOWN
            {
                reason = ServerDownFlags.SD_ServerDown,
                whyMessage = "Server going down!",
            }.SendToServer(null, null);

            Thread.Sleep(threadShutdownWait_InMilliseconds);

            PalaceAsyncSocket.Shutdown();
            PalaceAsyncSocket.Dispose();
            WebAsyncSocket.Shutdown();
            WebAsyncSocket.Dispose();
            Network.SessionManager.Dispose();
            PluginManager.Current.Dispose();
            ThreadManager.serverShutdown.Set();
            ThreadManager.Dispose();
        }
    }
}
