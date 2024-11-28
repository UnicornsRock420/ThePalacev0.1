using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ISendRoomBusiness
    {
        void SendToRoomID(ISessionState sessionState, params object[] args);
    }
}
