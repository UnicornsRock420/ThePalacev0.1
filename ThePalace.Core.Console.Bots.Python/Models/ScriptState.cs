using Microsoft.Scripting.Hosting;

namespace ThePalace.Core.Console.Bots.Python.Models
{
    public sealed class ScriptState
    {
        public ScriptEngine ScriptEngine { get; set; } = null;
        public ScriptScope ScriptScope { get; set; } = null;
    }
}
