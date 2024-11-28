using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    [AdminOnlyCommand]
    public class CMD_PURGEPROPS : ICommand
    {
        public static string Help => @"-- Purge Props from Server.";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext == null)
                return false;

            var xtlk = new Protocols.MSG_XTALK
            {
                text = $"Purging Props",
            };

            if (args.Length <= 0)
            {
                xtlk.text = $"Clearing Room[{_sessionState.RoomID}] Loose Props, issued by [{_sessionState.UserID}] {_sessionState.UserInfo.name}";

                Network.SessionManager.SendToStaff(xtlk, EventTypes.MSG_XTALK, 0);
                Logger.ConsoleLog(xtlk.text);

                dbContext.LooseProps2.RemoveRange(dbContext.LooseProps2.AsNoTracking().Where(m => m.RoomId == _sessionState.RoomID).ToList());

            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];

                    if (!arg.Substring(0, 1).Equals("-"))
                    {

                        xtlk.text = "Invalid Paramters for PurgeProps specified";

                        if (_sessionState.UserID != 0)
                        {
                            Network.SessionManager.SendToUserID(_sessionState.UserID, xtlk, EventTypes.MSG_XTALK, 0);
                        }

                        Logger.ConsoleLog(xtlk.text);
                        break;
                    }

                    if (arg.Contains("T"))
                    {
                        if ((i + 1) >= args.Length)
                        {
                            xtlk.text = "[T]ime Operation requires a DateTime string to be passed";

                            if (_sessionState.UserID != 0)
                            {
                                Network.SessionManager.SendToUserID(_sessionState.UserID, xtlk, EventTypes.MSG_XTALK, 0);
                            }

                            Logger.ConsoleLog(xtlk.text);
                            break;
                        }

                        var time = args[i + 1].TryParse(DateTime.UnixEpoch);

                        if (time > DateTime.UnixEpoch)
                        {
                            xtlk.text = "Invalid DateTime format";

                            if (_sessionState.UserID != 0)
                            {
                                Network.SessionManager.SendToUserID(_sessionState.UserID, xtlk, EventTypes.MSG_XTALK, 0);
                            }

                            Logger.ConsoleLog("Invalid DateTime format");
                            return true;
                        }

                        xtlk.text = $"Removing Props Older than {time.ToString()}, issued by [{_sessionState.UserID}] {_sessionState.UserInfo.name}";
                        Network.SessionManager.SendToStaff(xtlk, EventTypes.MSG_XTALK, 0);
                        Logger.ConsoleLog(xtlk.text);
                        dbContext.Assets.RemoveRange(dbContext.Assets.AsNoTracking().Where(m => m.LastUsed <= time));
                        i++;
                        break;
                    }
                    else if (arg.Contains("C"))
                    {
                        xtlk.text = $"Removing All Cached Props, issued by [{_sessionState.UserID}] {_sessionState.UserInfo.name}";
                        Network.SessionManager.SendToStaff(xtlk, EventTypes.MSG_XTALK, 0);
                        Logger.ConsoleLog(xtlk.text);

                        dbContext.Assets.RemoveRange(dbContext.Assets);
                        break;
                    }

                    if (arg.Contains("L"))
                    {
                        xtlk.text = $"Purging All Loose Props, issued by [{_sessionState.UserID}] {_sessionState.UserInfo.name}";
                        Network.SessionManager.SendToStaff(xtlk, EventTypes.MSG_XTALK, 0);
                        Logger.ConsoleLog(xtlk.text);
                        dbContext.LooseProps.RemoveRange(dbContext.LooseProps);
                        break;
                    }
                }
            }

            dbContext.SaveChanges();

            return true;
        }
    }
}
