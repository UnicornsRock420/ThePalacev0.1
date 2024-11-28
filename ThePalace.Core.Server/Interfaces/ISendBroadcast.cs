using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ISendBroadcast
    {
        void SendToServer(ISessionState sessionState, params object[] args);
    }
}
