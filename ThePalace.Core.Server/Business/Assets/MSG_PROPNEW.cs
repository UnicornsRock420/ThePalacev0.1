using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("prPn")]
    [SuccessfullyConnectedProtocol]
    public class MSG_PROPNEW : IBusinessReceive
    {

        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var room = dbContext.GetRoom(_sessionState.RoomID);
            var inboundPacket = (Protocols.MSG_PROPNEW)args.FirstOrDefault();

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

                room.LooseProps.Add(new LoosePropRec
                {
                    assetSpec = inboundPacket.propSpec,
                    loc = inboundPacket.loc,
                    flags = 0,
                });

                room.HasUnsavedChanges = true;
            }

            AssetLoader.CheckAssets(_sessionState, inboundPacket.propSpec);

            Network.SessionManager.SendToRoomID(_sessionState.RoomID, 0, inboundPacket, EventTypes.MSG_PROPNEW, 0);
        }
    }

}
