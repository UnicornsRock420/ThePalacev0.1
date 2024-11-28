using System;
using ThePalace.Core.Models;

namespace ThePalace.Core.Client.Core.Interfaces
{
    public interface IScriptEngine : IDisposable
    {
        void Initialize(IClientSessionState sessionState, ScriptEvent @event, params object[] args);
        void PreInvoke(IClientSessionState sessionState, ScriptEvent @event, params object[] args);
        void Invoke(IClientSessionState sessionState, ScriptEvent @event, params object[] args);
    }
}
