using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ISendStaffBroadcast
    {
        void SendToStaff(ISessionState sessionState, params object[] args);
    }
}
