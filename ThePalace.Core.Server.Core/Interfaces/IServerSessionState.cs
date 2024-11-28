using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Core.Interfaces
{
    public interface IServerSessionState : ISessionState, IDisposable
    {
        bool Authorized { get; set; }
        Int32 AuthUserID { get; set; }
        List<int> AuthRoleIDs { get; set; }
        List<int> AuthMsgIDs { get; set; }
        List<string> AuthCmds { get; set; }
    }
}
