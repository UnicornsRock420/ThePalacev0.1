using Microsoft.ClearScript.V8;

namespace ThePalace.Core.Console.Bots.JavaScript.Models
{
    public sealed class ScriptState
    {
        public V8ScriptEngine ScriptEngine { get; set; } = null;
    }
}
