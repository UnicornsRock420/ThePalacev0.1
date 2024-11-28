using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Interfaces
{
    public interface ISessionState : IDisposable
    {
        Guid SessionID { get; }
        object ScriptState { get; set; }

        UInt32 UserID { get; set; }
        Int16 RoomID { get; set; }
        string Name { get; set; }
        UserFlags UserFlags { get; set; }

        IConnectionState ConnectionState { get; set; }

        UserRec UserInfo { get; }
        RegistrationRec RegInfo { get; }
    }
}
