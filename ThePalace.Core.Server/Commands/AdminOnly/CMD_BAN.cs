using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_BAN : ICommand
    {
        public static string Help => @"[<target user>] or [<IP|REG|PUID>] -- Permanently ban <target user> from the server.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext == null) return false;

            var xtlk = new Protocols.MSG_XTALK();

            if (targetState == null && args.Length < 1)
            {
                xtlk.text = "A target user or parameter is required for this command.";
            }

            if (targetState != null)
            {
                var _targetState = targetState as SessionState;
                if (_targetState == null)
                    return false;

                if (_targetState.Authorized)
                {
                    xtlk.text = "Sorry, you may not perform this command on another staff member.";

                    _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
                }
                else
                {
                    var ipAddress = _targetState.driver.GetIPAddress();
                    var serverDown = new Business.MSG_SERVERDOWN
                    {
                        reason = ServerDownFlags.SD_Banished,
                        whyMessage = "You have been banned!",
                    };

                    Network.SessionManager.sessionStates.Values.ToList().ForEach(state =>
                    {
                        if (state.driver.GetIPAddress() == ipAddress)
                        {
                            serverDown.Send(sessionState);

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
                            UntilDate = null,
                        });
                    });
                }
            }

            if (args != null && args.Length > 0)
            {
                var bans = new List<Bans>();
                var serverDown = new Business.MSG_SERVERDOWN
                {
                    reason = ServerDownFlags.SD_Banished,
                    whyMessage = "You have been banned!",
                };

                foreach (var _arg in args)
                {
                    var arg = _arg.Trim();

                    if (Regex.IsMatch(arg, "^[0-9]+[.][0-9]+[.][0-9]+[.][0-9]+$"))
                    {
                        bans.Add(new Bans
                        {
                            Ipaddress = arg,
                            UntilDate = null,
                        });
                    }

                    if (Regex.IsMatch(arg, @"^[\{]*[A-Q][\}]*$"))
                    {
                        var seed = Cipher.WizKeytoSeed(arg);
                        var crc = Cipher.ComputeLicenseCrc((UInt32)seed);
                        var ctr = Cipher.GetSeedFromReg((UInt32)seed, crc);

                        bans.Add(new Bans
                        {
                            RegCrc = (Int32)crc,
                            RegCtr = ctr,
                            UntilDate = null,
                        });
                    }

                    if (Regex.IsMatch(arg, @"^[\{]*[Z][A-Q][\}]*$"))
                    {
                        var seed = Cipher.WizKeytoSeed(arg);
                        var crc = Cipher.ComputeLicenseCrc((UInt32)seed);
                        var ctr = Cipher.GetSeedFromPUID((UInt32)seed, crc);

                        bans.Add(new Bans
                        {
                            Puidcrc = (Int32)crc,
                            Puidctr = ctr,
                            UntilDate = null,
                        });
                    }
                }

                Network.SessionManager.sessionStates.Values.ToList().ForEach(state =>
                {
                    foreach (var ban in bans)
                    {
                        if (state.driver.GetIPAddress() == ban.Ipaddress ||
                            (state.RegInfo.crc == ban.RegCrc && state.RegInfo.counter == ban.RegCtr) ||
                            (state.RegInfo.puidCRC == ban.Puidcrc && state.RegInfo.puidCtr == ban.Puidctr))
                        {
                            ban.Note = state.UserInfo.name;

                            serverDown.Send(sessionState);

                            state.driver.DropConnection();
                        }
                    }
                });

                if (bans.Count > 0)
                {
                    dbContext.Bans.AddRange(bans);
                }
            }

            if (dbContext.HasUnsavedChanges())
            {
                dbContext.SaveChanges();

                xtlk.text = "Ban record(s) added...";
            }
            else
            {
                xtlk.text = "Usage: `ban [<target user>] or [<IP|REG|PUID>]";
            }

            if (_sessionState.UserID == 0)
            {
                Logger.ConsoleLog(xtlk.text);
            }
            else
            {
                _sessionState.Send(xtlk, EventTypes.MSG_XTALK, 0);
            }

            return true;
        }
    }
}
