using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Database.Core.Model;

namespace ThePalace.Core.Server.Business
{
    [Description("opSn")]
    [AdminOnlyProtocol]
    [SuccessfullyConnectedProtocol]
    public class MSG_SPOTNEW : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var room = dbContext.GetRoom(_sessionState.RoomID);

            if (!room.NotFound)
            {
                var hq = (short)(room.Width / 4);
                var vq = (short)(room.Height / 4);

                room.Hotspots.Add(new HotspotRec
                {
                    id = (short)(room.Hotspots.Count > 0 ? (room.Hotspots.Max(h => h.id) + 1) : 1),
                    loc = new Point
                    {
                        h = 0,
                        v = 0,
                    },
                    Vortexes = new List<Point>
                    {
                        new Point
                        {
                            h = (short)(hq * 1),
                            v = (short)(vq * 1),
                        },
                        new Point
                        {
                            h = (short)(hq * 3),
                            v = (short)(vq * 1),
                        },
                        new Point
                        {
                            h = (short)(hq * 3),
                            v = (short)(vq * 3),
                        },
                        new Point
                        {
                            h = (short)(hq * 1),
                            v = (short)(vq * 3),
                        },
                    },
                });
                room.HasUnsavedAuthorChanges = true;
                room.HasUnsavedChanges = true;

                ServerState.FlushRooms(dbContext);

                Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, room, EventTypes.MSG_ROOMSETDESC, 0);
            }
        }
    }
}
