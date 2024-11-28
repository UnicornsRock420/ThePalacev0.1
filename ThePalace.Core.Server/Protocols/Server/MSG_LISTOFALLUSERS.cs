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
    [Description("uLst")]
    public class MSG_LISTOFALLUSERS : IProtocolReceive, IProtocolSend
    {
        public UInt32 nbrUsers;

        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public byte[] Serialize(params object[] args)
        {
            var message = (Message)args.FirstOrDefault();

            using (var dbContext = DbConnection.For<ThePalaceEntities>())
            {
                var query = Network.SessionManager.sessionStates.Values
                    .Where(s => s.successfullyConnected == true)
                    .AsQueryable();

                if (!message.sessionState.Authorized)
                {
                    query = query.Where(u => (u.UserFlags & UserFlags.U_Hide) == 0);
                }

                var users = query
                    .AsEnumerable()
                    .Select(u => new ListRec
                    {
                        primaryID = u.UserID,
                        name = u.UserInfo.name,
                        flags = (Int16)u.UserFlags,
                        refNum = u.RoomID,
                    }.Serialize())
                    .ToList();

                nbrUsers = (UInt32)users.Count;

                return users
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
                var query = Network.SessionManager.sessionStates.Values
                    .Where(s => s.successfullyConnected == true)
                    .AsQueryable();

                if (!message.sessionState.Authorized)
                {
                    query = query.Where(u => (u.UserFlags & (UserFlags.U_Hide)) == 0);
                }

                var users = query
                    .AsEnumerable()
                    .Select(u => new ListRec
                    {
                        primaryID = u.UserID,
                        name = u.UserInfo.name,
                        flags = (Int16)u.UserFlags,
                        refNum = u.RoomID,
                    })
                    .ToList();

                return JsonConvert.SerializeObject(new
                {
                    list = users,
                });
            }
        }
    }
}
