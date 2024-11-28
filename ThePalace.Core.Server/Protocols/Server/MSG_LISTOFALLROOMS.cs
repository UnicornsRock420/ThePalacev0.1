using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Database.Core.Utility;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Protocols
{
    [Description("rLst")]
    public class MSG_LISTOFALLROOMS : IProtocolReceive, IProtocolSend
    {
        public UInt32 nbrRooms;

        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public byte[] Serialize(params object[] args)
        {
            var message = (Message)args.FirstOrDefault();

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var query = dbContext.Rooms.AsNoTracking()
                    .AsQueryable();

                if (!message.sessionState.Authorized)
                {
                    query = query.Where(r => (r.Flags & (Int32)RoomFlags.RF_Hidden) == 0);
                }

                var rooms = query
                    .OrderBy(r => r.OrderID)
                    .AsEnumerable()
                    .Select(r => new ListRec
                    {
                        primaryID = (UInt32)r.RoomId,
                        name = r.Name,
                        flags = (Int16)r.Flags,
                        refNum = (Int16)Network.SessionManager.GetRoomUserCount(r.RoomId),
                    }.Serialize())
                    .ToList();

                nbrRooms = (UInt32)rooms.Count();

                return rooms
                    .SelectMany(b => b)
                    .ToArray();
            }
        }

        public void DeserializeJSON(string json)
        {
        }

        public string SerializeJSON(params object[] args)
        {
            var message = (Message)args.FirstOrDefault();

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var query = dbContext.Rooms.AsNoTracking()
                    .AsQueryable();

                if (!message.sessionState.Authorized)
                {
                    query = query.Where(r => (r.Flags & (Int32)RoomFlags.RF_Hidden) == 0);
                }

                var rooms = query
                    .OrderBy(r => r.OrderID)
                    .AsEnumerable()
                    .Select(r => new ListRec
                    {
                        primaryID = (UInt32)r.RoomId,
                        name = r.Name,
                        flags = (Int16)r.Flags,
                        refNum = (Int16)Network.SessionManager.GetRoomUserCount(r.RoomId),
                    })
                    .ToList();

                return JsonConvert.SerializeObject(new
                {
                    list = rooms,
                });
            }
        }
    }
}
