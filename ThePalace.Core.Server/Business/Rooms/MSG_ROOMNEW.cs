using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("nRom")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_ROOMNEW : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var maxRoomId = dbContext.Rooms
                .Select(r => r.RoomId)
                .Max();
            var maxOrderId = dbContext.Rooms
                .Select(r => r.OrderID)
                .Max();

            maxRoomId++;
            maxOrderId++;

            var newRoom = new Rooms
            {
                RoomId = maxRoomId,
                Name = $"New Room {maxRoomId}",
                CreateDate = DateTime.UtcNow,
                OrderID = maxOrderId,
                MaxOccupancy = 0,
                Flags = 0,
            };
            dbContext.Rooms.Add(newRoom);

            var newRoomData = new RoomData
            {
                RoomId = maxRoomId,
                FacesId = 0,
                Password = null,
                PictureName = "clouds.png",
                ArtistName = _sessionState.Name,
            };
            dbContext.RoomData.Add(newRoomData);

            dbContext.SaveChanges();

            Logger.Log(MessageTypes.Info, $"MSG_ROOMNEW[{_sessionState.AuthUserID}]: {newRoom.Name}");

            var room = dbContext.GetRoom(maxRoomId);

            if (!room.NotFound)
            {
                _sessionState.RoomID = room.ID;

                var sendBusinesses = new List<IBusinessSend>
                {
                    new MSG_ROOMDESC(),
                    new MSG_USERLIST(),
                    new MSG_ROOMDESCEND(),
                };

                foreach (var sendBusiness in sendBusinesses)
                {
                    sendBusiness.Send(sessionState, args);
                }

                new MSG_USERNEW().SendToRoomID(sessionState, args);
            }
        }
    }
}
