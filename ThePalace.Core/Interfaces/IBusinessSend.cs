namespace ThePalace.Core.Interfaces
{
    public interface IBusinessSend
    {
        void Send(ISessionState sessionState, params object[] args);
    }
}
