using System;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_KILLPROP : ICommand
    {
        public const string Help = @"-- Remove any and all props the targeted user is wearing from the server.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext == null) return false;

            var xtlk = new Protocols.MSG_XTALK();

            if (targetState == null)
            {
                xtlk.text = "Sorry, you must target a user to use this command.";

                _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }
            else
            {
                var _targetState = targetState as SessionState;
                if (_targetState != null) return false;

                foreach (var asset in _targetState.UserInfo.assetSpec)
                {
                    var dbAsset = dbContext.Assets
                        .Where(a => a.AssetId == asset.id)
                        .FirstOrDefault();

                    if (dbAsset != null)
                    {
                        dbContext.Assets.Remove(dbAsset);
                    }
                }

                if (dbContext.HasUnsavedChanges())
                {
                    dbContext.SaveChanges();
                }

                _targetState.UserInfo.nbrProps = 0;
                _targetState.UserInfo.assetSpec = null;

                Network.SessionManager.SendToRoomID(_targetState.RoomID, 0, new Protocols.MSG_USERPROP
                {
                    nbrProps = 0,
                    assetSpec = null,
                }, EventTypes.MSG_USERPROP, (Int32)_targetState.UserID);

                xtlk.text = $"{_targetState.UserInfo.name}'s avatar has been eraised!";

                _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
