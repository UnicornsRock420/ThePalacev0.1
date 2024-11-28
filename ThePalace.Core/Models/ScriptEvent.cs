using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models
{
    public sealed class ScriptEvent : EventArgs
    {
        public IptEventTypes EventType { get; set; }
        public IProtocolRec Packet { get; set; }
        public object ScriptState { get; set; }
    }
}
