using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Factories;

namespace ThePalace.Core.Utility
{
    public static class UtilityExtensionMethods
    {
        public static RoomBuilder GetRoom(this ThePalaceEntities dbContext, Int16 roomID)
        {
            var room = null as RoomBuilder;

            if (ServerState.roomsCache.ContainsKey(roomID))
            {
                room = ServerState.roomsCache[roomID];
            }
            else
            {
                room = new RoomBuilder();

                if (room == null)
                {
                    throw new OutOfMemoryException("Unable to initiate RoomBuilder instance.");
                }

                room.ID = roomID;

                room.Read(dbContext);

                if (!room.NotFound)
                {
                    lock (ServerState.roomsCache)
                    {
                        ServerState.roomsCache.TryAdd(room.ID, room);
                    }
                }
            }

            return room;
        }

        public static Int16 FindRoomID(this ThePalaceEntities dbContext, Int16 roomID = 0, bool isAuthorized = false)
        {
            var maxRoomOccupancy = ConfigManager.GetValue<int>("MaxRoomOccupancy", 45);
            var entryRoomIDKeys = ServerState.entryRoomIDs.ToArray();
            var roomCacheKeys = ServerState.roomsCache.Keys.ToArray();
            var fullRoomIDs = new List<Int16>();

            var roomIDs = dbContext.Rooms.AsNoTracking()
                .OrderBy(r => r.OrderID)
                .Select(r => r.RoomId)
                .ToList();

            Func<bool> findRoom = () =>
            {
                if (roomID != 0)
                {
                    var roomCount = isAuthorized ? 0 : Server.Network.SessionManager.GetRoomUserCount(roomID);

                    try
                    {
                        if (
                            isAuthorized ||
                            (entryRoomIDKeys.Contains(roomID) && !roomCacheKeys.Contains(roomID)) ||
                            (roomCacheKeys.Contains(roomID) && ServerState.roomsCache[roomID].MaxOccupancy == 0 && roomCount < maxRoomOccupancy) ||
                            (roomCacheKeys.Contains(roomID) && (ServerState.roomsCache[roomID].MaxOccupancy > 0) && roomCount < ServerState.roomsCache[roomID].MaxOccupancy))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    fullRoomIDs.Add(roomID);
                }

                return false;
            };

            if (roomID > 0)
            {
                if (findRoom()) return roomID;
            }

            while (true)
            {
                entryRoomIDKeys = ServerState.entryRoomIDs
                    .Where(r => !fullRoomIDs.Contains(r))
                    .ToArray();
                roomCacheKeys = ServerState.roomsCache.Keys
                    .Where(r => !fullRoomIDs.Contains(r))
                    .ToArray();

                if (entryRoomIDKeys.Length > 0)
                {
                    roomID = entryRoomIDKeys[RndGenerator.NextSecure((uint)entryRoomIDKeys.Length)];

                    if (findRoom()) return roomID;
                }
                else if (roomCacheKeys.Length > 0)
                {
                    roomID = roomCacheKeys[RndGenerator.NextSecure((uint)roomCacheKeys.Length)];

                    if (findRoom()) return roomID;
                }
                else
                {
                    roomID = roomIDs
                        .Where(ID => !fullRoomIDs.Contains(ID))
                        .FirstOrDefault();

                    if (roomID == 0)
                    {
                        break;
                    }
                    else
                    {
                        var room = dbContext.GetRoom(roomID);
                        if (!room.NotFound)
                        {
                            if (findRoom()) return roomID;
                        }
                    }
                }
            }

            return 0;
        }

        public static bool AttributeWrapper(this Type objectType, Type attributeType, string methodName, params object[] values)
        {
            var attribute = objectType.GetCustomAttributes(attributeType, false).SingleOrDefault();
            if (attribute != null)
            {
                var cstrPtr = attributeType.GetConstructor(Type.EmptyTypes);
                var attributeClassObj = cstrPtr.Invoke(new object[] { });
                var method = attributeType.GetMethod(methodName);

                if (method == null) return false;

                return (bool)method.Invoke(attributeClassObj, values);
            }

            return true;
        }
    }
}
