using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using ThePalace.Core.Database.Core.Model;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Authorization;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Business
{
    [Description("regi")]
    public class MSG_LOGON : IBusinessReceive, IBusinessSend
    {
        public void Receive(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            if (_sessionState.successfullyConnected)
            {
                new MSG_SERVERDOWN
                {
                    reason = ServerDownFlags.SD_CommError,
                    whyMessage = "Communication Error!",
                }.Send(sessionState, args);

                _sessionState.driver.DropConnection();

                return;
            }

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var serverRequiresAuthentication = ConfigManager.GetValue<bool>("ServerRequiresAuthentication", false, true).Value;
            var inboundPacket = (Protocols.MSG_LOGON)args.FirstOrDefault();

            if (!Cipher.ValidUserSerialNumber(inboundPacket.reg.crc, inboundPacket.reg.counter))
            {
                new MSG_SERVERDOWN
                {
                    reason = ServerDownFlags.SD_InvalidSerialNumber,
                    whyMessage = "Invalid Serial Number!",
                }.Send(sessionState, args);

                _sessionState.driver.DropConnection();

                return;
            }

            lock (Network.SessionManager.sessionStates)
            {
                foreach (var userState in Network.SessionManager.sessionStates.Values)
                {
                    if (userState.RegInfo != null && userState.UserID != _sessionState.UserID && inboundPacket.reg.crc == userState.RegInfo.crc && inboundPacket.reg.counter == userState.RegInfo.counter)
                    {
                        new MSG_SERVERDOWN
                        {
                            reason = ServerDownFlags.SD_DuplicateUser,
                            whyMessage = "Duplicate Serial Number!",
                        }.Send(sessionState);

                        userState.driver.DropConnection();
                    }
                }
            }

            _sessionState.RoomID = 0;

            if (string.IsNullOrWhiteSpace(inboundPacket.reg.userName) || Regex.IsMatch(inboundPacket.reg.userName, @"^User\s*[0-9]+$", RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                inboundPacket.reg.userName = $"User {_sessionState.UserID}";
            }

            AuthEngine.AuthorizeUser(sessionState, out int AuthUserID, out List<int> AuthRoleIDs, out List<int> AuthMsgIDs, out List<string> AuthCmds, args);

            _sessionState.UserInfo.name = inboundPacket.reg.userName;
            _sessionState.RegInfo = inboundPacket.reg;

            _sessionState.Authorized = (AuthUserID != 0);
            _sessionState.AuthUserID = AuthUserID;
            _sessionState.AuthRoleIDs = AuthRoleIDs;
            _sessionState.AuthMsgIDs = AuthMsgIDs;
            _sessionState.AuthCmds = AuthCmds;

            Logger.Log(MessageTypes.Info, $"Authorized[{_sessionState.AuthUserID}] = {_sessionState.Authorized.ToString()}, RegCode = {Cipher.RegRectoSeed(inboundPacket.reg)}, PUID = {Cipher.RegRectoSeed(inboundPacket.reg, true)}");

            var ipAddress = _sessionState.driver.GetIPAddress();

            if (!_sessionState.Authorized)
            {
                var now = DateTime.UtcNow;
                var bans = dbContext.Bans.AsNoTracking()
                    .AsEnumerable()
                    .Where(b =>
                        (b.Ipaddress == ipAddress ||
                        (b.RegCtr == inboundPacket.reg.counter && b.RegCrc == inboundPacket.reg.crc) ||
                        (b.Puidctr == inboundPacket.reg.puidCtr && b.Puidcrc == inboundPacket.reg.puidCRC)) &&
                        (!b.UntilDate.HasValue || b.UntilDate.Value < now))
                    .Count();

                if (bans > 0)
                {
                    new MSG_SERVERDOWN
                    {
                        reason = ServerDownFlags.SD_Banished,
                        whyMessage = "You have been banned!",
                    }.Send(sessionState, args);

                    _sessionState.driver.DropConnection();

                    return;
                }
            }

            if (!_sessionState.Authorized && serverRequiresAuthentication)
            {
                new MSG_AUTHENTICATE().Send(sessionState, args);
            }
            else
            {
                Send(sessionState, args);
            }
        }

        public void Send(ISessionState sessionState, params object[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return;

            var dbContext = _sessionState.dbContext as ThePalaceEntities;
            if (dbContext != null) return;

            var roomID = dbContext.FindRoomID(_sessionState.RegInfo.desiredRoom);

            if (roomID == 0)
            {
                new MSG_SERVERDOWN
                {
                    reason = ServerDownFlags.SD_ServerFull,
                    whyMessage = "The Server is full!",
                }.Send(sessionState, args);

                _sessionState.driver.DropConnection();

                return;
            }

            _sessionState.RoomID = roomID;
            _sessionState.successfullyConnected = true;

            var sendBusinesses = new List<IBusinessSend>
            {
                new MSG_ALTLOGONREPLY(),
                new MSG_VERSION(),
                new MSG_SERVERINFO(),
                new MSG_USERSTATUS(),
            };

            foreach (var sendBusiness in sendBusinesses)
            {
                sendBusiness.Send(sessionState, args);
            }

            new MSG_USERLOG().SendToServer(sessionState, args);

            using (var dbContextTransaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.Users1.Add(new Users1
                    {
                        UserId = (Int32)_sessionState.UserID,
                        Name = _sessionState.RegInfo.userName,
                        RoomId = _sessionState.RoomID,
                        Flags = (short)_sessionState.UserFlags,
                    });

                    dbContext.UserData.Add(new UserData
                    {
                        UserId = (Int32)_sessionState.UserID,
                        RoomPosH = _sessionState.UserInfo.roomPos.h,
                        RoomPosV = _sessionState.UserInfo.roomPos.v,
                        FaceNbr = _sessionState.UserInfo.faceNbr,
                        ColorNbr = _sessionState.UserInfo.colorNbr,
                        AwayFlag = 0,
                        OpenToMsgs = 0,
                    });

                    dbContext.SaveChanges();

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();

                    ex.DebugLog();
                }
            }

            new MSG_HTTPSERVER().Send(sessionState, args);

            new MSG_ROOMGOTO().Receive(sessionState, new Protocols.MSG_ROOMGOTO
            {
                dest = _sessionState.RoomID,
            });
        }
    }
}
