using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Server.Network;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Authorization
{
    public static class AuthEngine
    {
        public static void AuthorizeUser(ISessionState sessionState, out int AuthUserID, out List<int> AuthRoleIDs, out List<int> AuthMsgIDs, out List<string> AuthCmds, params object[] args)
        {
            AuthUserID = 0;
            AuthRoleIDs = null;
            AuthMsgIDs = null;
            AuthCmds = null;

            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var protocol = args.FirstOrDefault() as IProtocolReceive;
            if (protocol != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var ipAddress = _sessionState.driver.GetIPAddress();
            var xTalkB = new Business.MSG_XTALK();
            var xTalkP = new Protocols.MSG_XTALK();
            var authUserID = 0;

            var actions = new Dictionary<Type, Action>
            {
                { typeof(Business.MSG_LOGON), () => {
                    var inboundPacket = (ThePalace.Core.Server.Protocols.MSG_LOGON)args.FirstOrDefault();

                    authUserID = dbContext.Auth.AsNoTracking()
                        .AsEnumerable()
                        .Where(a =>
                            ((a.AuthType & (byte)AuthTypes.Password) == 0 || ((a.AuthType & (byte)AuthTypes.Password) != 0 && a.Value.Trim() == (inboundPacket.reg.wizPassword ?? string.Empty).Trim())) &&
                            ((a.AuthType & (byte)AuthTypes.IPAddress) == 0 || ((a.AuthType & (byte)AuthTypes.IPAddress) != 0 && a.Value.Trim() == ipAddress)) &&
                            ((a.AuthType & (byte)AuthTypes.RegCode) == 0 || ((a.AuthType & (byte)AuthTypes.RegCode) != 0 && a.Ctr.HasValue && a.Crc.HasValue && a.Ctr == _sessionState.RegInfo.counter && a.Crc == _sessionState.RegInfo.crc)) &&
                            ((a.AuthType & (byte)AuthTypes.PUID) == 0 || ((a.AuthType & (byte)AuthTypes.PUID) != 0 && a.Ctr.HasValue && a.Crc.HasValue && a.Ctr == _sessionState.RegInfo.puidCtr && a.Crc == _sessionState.RegInfo.puidCRC)))
                        .Select(a => a.UserId)
                        .FirstOrDefault();

                    if (authUserID > 0)
                    {
                        xTalkP.text = $"{_sessionState.UserInfo.name} ({_sessionState.UserID}) [{ipAddress}] is now authorized!";

                        Logger.Log(MessageTypes.Info, xTalkP.text);

                        xTalkB.SendToUserID(sessionState, xTalkP);

                        xTalkB.SendToStaff(sessionState, xTalkP);
                    }
                } },
                { typeof(Business.MSG_SUPERUSER), () => {
                    var inboundPacket = (ThePalace.Core.Server.Protocols.MSG_SUPERUSER)args.FirstOrDefault();

                    authUserID = dbContext.Auth.AsNoTracking()
                        .AsEnumerable()
                        .Where(a =>
                            ((a.AuthType & (byte)AuthTypes.Password) == 0 || ((a.AuthType & (byte)AuthTypes.Password) != 0 && a.Value.Trim() == (inboundPacket.password ?? string.Empty).Trim())) &&
                            ((a.AuthType & (byte)AuthTypes.IPAddress) == 0 || ((a.AuthType & (byte)AuthTypes.IPAddress) != 0 && a.Value.Trim() == ipAddress)) &&
                            ((a.AuthType & (byte)AuthTypes.RegCode) == 0 || ((a.AuthType & (byte)AuthTypes.RegCode) != 0 && a.Ctr.HasValue && a.Crc.HasValue && a.Ctr == _sessionState.RegInfo.counter && a.Crc == _sessionState.RegInfo.crc)) &&
                            ((a.AuthType & (byte)AuthTypes.PUID) == 0 || ((a.AuthType & (byte)AuthTypes.PUID) != 0 && a.Ctr.HasValue && a.Crc.HasValue && a.Ctr == _sessionState.RegInfo.puidCtr && a.Crc == _sessionState.RegInfo.puidCRC)))
                        .Select(a => a.UserId)
                        .FirstOrDefault();

                    if (authUserID > 0)
                    {
                        xTalkP.text = $"{_sessionState.UserInfo.name} ({_sessionState.UserID}) [{ipAddress}] is now authorized!";

                        xTalkB.SendToUserID(sessionState, xTalkP);
                    }
                    else
                    {
                        xTalkP.text = $"{_sessionState.UserInfo.name} ({_sessionState.UserID}) [{ipAddress}] attempted authorization and failed...";
                    }

                    Logger.Log(MessageTypes.Info, xTalkP.text);

                    xTalkB.SendToStaff(sessionState, xTalkP);
                } },
                { typeof(Business.MSG_AUTHRESPONSE), () => {
                    var inboundPacket = (ThePalace.Core.Server.Protocols.MSG_AUTHRESPONSE)args.FirstOrDefault();

                    authUserID = dbContext.Auth.AsNoTracking()
                        .Where(a =>
                            ((a.AuthType & (byte)AuthTypes.Password) != 0 && a.Value.Trim() == inboundPacket.password.Trim()))
                        .Join(
                            dbContext.Users.AsNoTracking(),
                            a => a.UserId,
                            u => u.UserId,
                            (a, u) => new {a, u}
                        )
                        .Where(u => u.u.Name == inboundPacket.userName.Trim())
                        .Select(a => a.a.UserId)
                        .FirstOrDefault();

                    if (authUserID > 0)
                    {
                        xTalkP.text = $"{_sessionState.UserInfo.name} ({_sessionState.UserID}) [{ipAddress}] is now authorized!";

                        xTalkB.SendToUserID(sessionState, xTalkP);
                    }
                    else
                    {
                        xTalkP.text = $"{_sessionState.UserInfo.name} ({_sessionState.UserID}) [{ipAddress}] attempted authorization and failed...";
                    }

                    Logger.Log(MessageTypes.Info, xTalkP.text);

                    xTalkB.SendToStaff(sessionState, xTalkP);
                } }
            };

            var type = protocol.GetType();

            if (type != null && actions.ContainsKey(type))
            {
                actions[type]();
            }

            if (authUserID > 0)
            {
                AuthUserID = authUserID;
                AuthRoleIDs = dbContext.GroupUsers.AsNoTracking()
                    .Where(gu => gu.UserId == authUserID)
                    .Join(
                        dbContext.GroupRoles.AsNoTracking(),
                        gu => gu.GroupId,
                        gr => gr.GroupId,
                        (gu, gr) => new { gu, gr }
                    )
                    .Select(g => g.gr.RoleId)
                    .Distinct()
                    .ToList();
                AuthMsgIDs = new List<int>();
                AuthCmds = new List<string>();

                _sessionState.UserFlags |= (UserFlags.U_Moderator | UserFlags.U_Administrator);

                var now = DateTime.UtcNow;
                var sessionDuration_InMinutes = ConfigManager.GetValue<UInt32>("SessionDuration_InMinutes", 1440).Value;
                var expireDate = now.AddMinutes(sessionDuration_InMinutes);
                var sessionRec = dbContext.Sessions
                    .Where(s => s.UserId == authUserID)
                    .SingleOrDefault();

                if (sessionRec == null)
                {
                    sessionRec = new Sessions
                    {
                        UserId = authUserID,
                        Hash = Guid.NewGuid(),
                        UntilDate = expireDate,
                        LastUsed = now,
                    };

                    dbContext.Sessions.Add(sessionRec);
                }
                else if (sessionRec.LastUsed < now)
                {
                    sessionRec.Hash = Guid.NewGuid();
                    sessionRec.UntilDate = expireDate;
                    sessionRec.LastUsed = now;
                }
                else
                {
                    sessionRec.LastUsed = now;
                }

                if (dbContext.HasUnsavedChanges())
                {
                    dbContext.SaveChanges();
                }

                if (_sessionState.successfullyConnected)
                {
                    var uSta = new Protocols.MSG_USERSTATUS
                    {
                        flags = (short)_sessionState.UserFlags,
                        hash = sessionRec.Hash,
                    };

                    _sessionState.Send(uSta, EventTypes.MSG_USERSTATUS, (Int32)_sessionState.UserID);
                }
            }
            else
            {
                AuthUserID = 0;
                AuthRoleIDs = null;
                AuthMsgIDs = null;
                AuthCmds = null;
            }
        }
    }
}
