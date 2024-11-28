using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Core.Interfaces;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Models
{
    public sealed class SessionState : IServerSessionState, IDisposable
    {
        // Session Info
        public Guid SessionID { get; } = Guid.NewGuid();
        public object ScriptState { get; set; }

        public DbContext dbContext { get; set; } = null;
        public INetworkDriver driver { get; set; } = null;
        public bool successfullyConnected { get; set; } = false;
        public IConnectionState ConnectionState { get; set; } = null;

        public UInt32 UserID
        {
            get => UserInfo.userID;
            set => UserInfo.userID = value;
        }
        public Int16 RoomID
        {
            get => UserInfo.roomID;
            set => UserInfo.roomID = value;
        }
        public string Name
        {
            get => UserInfo.name;
            set
            {
                UserInfo.name = value;
                RegInfo.userName = value;
            }
        }
        public UserFlags UserFlags
        {
            get => UserInfo.userFlags;
            set => UserInfo.userFlags = value;
        }

        public UserRec UserInfo { get; set; }
        public RegistrationRec RegInfo { get; set; } = null;

        public bool Authorized { get; set; } = false;
        public Int32 AuthUserID { get; set; } = 0;
        public List<int> AuthRoleIDs { get; set; } = null;
        public List<int> AuthMsgIDs { get; set; } = null;
        public List<string> AuthCmds { get; set; } = null;

        public SessionState()
        {
            UserInfo = new UserRec
            {
                userID = 0,
                roomID = 0,
                faceNbr = (Int16)RndGenerator.NextSecure(0, 16),
                colorNbr = (Int16)RndGenerator.NextSecure(0, 16),
                roomPos = new Point
                {
                    h = (Int16)RndGenerator.NextSecure(0, 512),
                    v = (Int16)RndGenerator.NextSecure(0, 384),
                },
            };
        }

        public void Dispose()
        {
            UserInfo = null;
            RegInfo = null;
        }
    }
}
