using System;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ISendUserBusiness
    {
        void SendToUser(ISessionState sessionState, UInt32 TargetID, params object[] args);
    }
}
