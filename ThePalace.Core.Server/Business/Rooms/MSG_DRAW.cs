using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Core;
using ThePalace.Core.Server.Models;

namespace ThePalace.Core.Server.Business
{
    [Description("draw")]
    [SuccessfullyConnectedProtocol]
    public class MSG_DRAW : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var inboundPacket = (Protocols.MSG_DRAW)args.FirstOrDefault();

            if (ServerState.roomsCache.ContainsKey(_sessionState.RoomID))
            {
                var room = ServerState.roomsCache[_sessionState.RoomID];

                switch (inboundPacket.command.type)
                {
                    case DrawCmdTypes.DC_Delete:
                        if (room.DrawCommands.Count > 0)
                        {
                            room.DrawCommands.RemoveAt(room.DrawCommands.Count - 1);
                        }

                        break;

                    case DrawCmdTypes.DC_Detonate:
                        if (room.DrawCommands.Count > 0)
                        {
                            room.DrawCommands.Clear();
                        }

                        break;
                    //case DrawCommandTypes.DC_Path:
                    //case DrawCommandTypes.DC_Text:
                    //case DrawCommandTypes.DC_Shape:
                    //case DrawCommandTypes.DC_Ellipse:
                    default:
                        room.DrawCommands.Add(inboundPacket.command);

                        break;
                }

                room.HasUnsavedChanges = true;
            }

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_DRAW, (Int32)_sessionState.UserID);
        }
    }
}
