using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("dPrp")]
    [SuccessfullyConnectedProtocol]
    public class MSG_PROPDEL : IBusinessReceive
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var inboundPacket = (Protocols.MSG_PROPDEL)args.FirstOrDefault();
            var room = dbContext.GetRoom(_sessionState.RoomID);

            if (!room.NotFound)
            {
                if ((room.Flags & (int)RoomFlags.RF_NoLooseProps) != 0)
                {
                    room.LooseProps.Clear();

                    var xtalk = new Protocols.MSG_XTALK
                    {
                        text = "Loose props are disabled in this room.",
                    };

                    Network.SessionManager.Send(_sessionState, xtalk, EventTypes.MSG_XTALK, 0);

                    return;
                }

                if (inboundPacket.propNum < 0 || inboundPacket.propNum >= room.LooseProps.Count)
                {
                    room.LooseProps.Clear();
                }
                else
                {
                    room.LooseProps.RemoveAt(inboundPacket.propNum);
                }

                room.HasUnsavedChanges = true;

                Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_PROPDEL, 0);
            }
        }
    }
}
