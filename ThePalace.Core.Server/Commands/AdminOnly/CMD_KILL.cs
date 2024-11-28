using System;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_KILL : ICommand
    {
        public const string Help = @"[<user>] -- Kill a currently connected user.";

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
                if (_targetState == null) return false;

                if (_targetState.Authorized)
                {
                    xtlk.text = "Sorry, you may not perform this command on another staff member.";

                    _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
                }
                else
                {
                    var killDuration_InMinutes = ConfigManager.GetValue<Int16>("KillDuration_InMinutes", 10).Value;
                    var killDuration_DT = DateTime.UtcNow.AddMinutes(killDuration_InMinutes);

                    var ipAddress = _targetState.driver.GetIPAddress();
                    var serverDown = new Business.MSG_SERVERDOWN
                    {
                        reason = ServerDownFlags.SD_KilledBySysop,
                        whyMessage = "You have been dispatched!",
                    };

                    Network.SessionManager.sessionStates.Values.ToList().ForEach(state =>
                    {
                        if (state.driver.GetIPAddress() == ipAddress)
                        {
                            serverDown.Send(sessionState, new Message
                            {
                                sessionState = state,
                            });

                            state.driver.DropConnection();
                        }

                        dbContext.Bans.Add(new Bans
                        {
                            Ipaddress = ipAddress,
                            RegCtr = (Int32)state.RegInfo.counter,
                            RegCrc = (Int32)state.RegInfo.crc,
                            Puidctr = (Int32)state.RegInfo.puidCtr,
                            Puidcrc = (Int32)state.RegInfo.puidCRC,
                            Note = state.UserInfo.name,
                            UntilDate = killDuration_DT,
                        });
                    });
                }
            }

            return true;
        }
    }
}
