using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThePalace.Core.Client.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Models.Threads
{
    public sealed class CmdTask : Disposable
    {
        public Task Task { get; set; } = null;
        public List<IProvider> Providers { get; set; } = new();
        public List<IConsumer> Consumers { get; set; } = new();
        public ConcurrentQueue<Cmd> Queue { get; set; } = new();
        public ManualResetEvent SignalEvent { get; set; } = new(false);

        public CmdTask() { }
        ~CmdTask() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            if ((this.Queue?.Count ?? 0) > 0)
            {
                this.Queue
                    .ToList()
                    .ForEach(c => { try { c.Dispose(); } catch { } });
                this.Queue.Clear();
            }
            this.Queue = null;

            if ((this.Providers?.Count ?? 0) > 0)
            {
                this.Providers.ForEach(p => { try { p.Dispose(); } catch { } });
                this.Providers.Clear();
            }
            this.Providers = null;

            if ((this.Consumers?.Count ?? 0) > 0)
            {
                this.Consumers.ForEach(c => { try { c.Dispose(); } catch { } });
                this.Consumers.Clear();
            }
            this.Consumers = null;

            this.SignalEvent?.Set();
            this.SignalEvent = null;
        }
    }
}
