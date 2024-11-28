using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ICommand
    {
        bool Command(ISessionState sessionState, ISessionState targetState, params string[] args);
    }
}
