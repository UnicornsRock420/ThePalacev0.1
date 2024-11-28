using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Client.Core.Interfaces;

namespace ThePalace.Core.Client.Core
{
    public sealed class ScriptManager : Disposable
    {
        private static readonly Lazy<ScriptManager> _current = new();
        public static ScriptManager Current => _current.Value;

        private DisposableDictionary<string, IScriptEngine> _engines = new();
        public IReadOnlyDictionary<string, IScriptEngine> Engines => this._engines;

        public ScriptManager() { }
        ~ScriptManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();
        }
    }
}
