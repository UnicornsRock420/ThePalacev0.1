using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Models
{
    using IptAlarms = List<IptAlarm>;
    using IptAtomList = List<IptVariable>;
    using IptEvents = ConcurrentDictionary<IptEventTypes, List<IptVariable>>;
    using IptVariables = ConcurrentDictionary<string, IptMetaVariable>;

    public delegate void IptCommandFnc(IptTracking iptTracking, int recursionDepth);
    public delegate IptVariable IptOperatorFnc(IptVariable register1, IptVariable register2);

    public sealed class IptAlarm
    {
        public DateTime Created { get; private set; }
        public Int32 Delay { get; private set; }
        public IptAtomList Value { get; private set; }

        public IptAlarm(IptAtomList value, Int32 delay)
        {
            Created = DateTime.Now;
            Delay = delay;
            Value = value;
        }
    }

    public sealed class IptVariable
    {
        public IptVariableTypes Type { get; set; }
        public object Value { get; set; }
    }

    public sealed class IptMetaVariable
    {
        public bool IsReadOnly { get; set; } = false;
        public bool IsSpecial { get; set; } = false;
        public bool IsGlobal { get; set; } = false;
        public int Depth { get; set; } = 0;
        private IptVariable _value;
        public IptVariable Value
        {
            get => this._value;
            set
            {
                if (this.IsReadOnly) return;
                else this._value = value;
            }
        }
    }

    public sealed class IptOperator
    {
        public IptOperatorFlags Flags { get; set; }
        public IptOperatorFnc OpFnc { get; set; }
    }

    public class IptTracking : IDisposable
    {
        public static int TicksToMilliseconds(int ticks) => ticks / 6 * 100;

        public readonly Timer Timer = new Timer
        {
            Interval = 10, //IptTracking.TicksToMilliseconds(1)
        };

        public bool Return { get; set; } = false;
        public bool Break { get; set; } = false;
        public Match[] Grep { get; set; } = null;
        public IptAtomList Stack { get; private set; } = new();
        public IptAlarms Alarms { get; private set; } = new();
        public IptEvents Events { get; private set; } = new();
        public IptVariables Variables { get; private set; } = new();

        public IptTracking() { }

        public void Dispose()
        {
            Events?.Clear();
            Events = null;
            Variables?.Clear();
            Variables = null;
            Stack?.Clear();
            Stack = null;
            Grep = null;
        }
    }
}
