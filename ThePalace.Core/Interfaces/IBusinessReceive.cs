namespace ThePalace.Core.Interfaces
{
    public interface IBusinessReceive
    {
        void Receive(ISessionState sessionState, params object[] args);
    }
}
