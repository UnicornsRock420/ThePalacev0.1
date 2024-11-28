using System;
using System.Collections.Generic;
using System.Linq;
using ThePalace.Core.Models;

namespace ThePalace.Core.Utility
{
    public sealed class TCF : IDisposable
    {
        private bool BreakOnException = false;

        private List<Action> TryBlocks = null;
        private Action<IReadOnlyList<Exception>> CatchBlock = null;
        private Action<TCFResults> FinallyBlock = null;
        private TCFResults Results = null;

        public static class Types
        {
            public const string ClassName = nameof(TCF);
            public static readonly Type TCF = typeof(TCF);
        }

        private TCF()
        {
            this.TryBlocks = new List<Action>();
            this.Results = new TCFResults();
        }

        ~TCF() => this.Dispose();
        public void Dispose()
        {
            TryBlocks?.Clear();
            TryBlocks = null;
            CatchBlock = null;
            FinallyBlock = null;
            Results = null;
        }

        public TCF(bool breakOnException = true)
            : this() =>
                this.BreakOnException = breakOnException;
        public static TCF Options(bool breakOnException = true) =>
            new TCF(breakOnException);

        public TCF(params Action[] tryBlocks)
            : this() =>
                this.Try(tryBlocks);
        public TCF(IEnumerable<Action> tryBlocks)
            : this() =>
                this.Try(tryBlocks);
        public TCF(params Func<object>[] tryBlocks)
            : this() =>
                this.Try(tryBlocks);
        public TCF(IEnumerable<Func<object>> tryBlocks)
            : this() =>
                this.Try(tryBlocks);

        public TCF(params IDisposable[] tryObjects)
            : this() =>
                this.TryDispose(tryObjects);

        public static TCF _Try(params Action[] tryBlocks) =>
            new TCF(tryBlocks);
        public static TCF _Try(IEnumerable<Action> tryBlocks) =>
            new TCF(tryBlocks);
        public static TCF _Try(params Func<object>[] tryBlocks) =>
            new TCF(tryBlocks);
        public static TCF _Try(IEnumerable<Func<object>> tryBlocks) =>
            new TCF(tryBlocks);

        public static TCF _TryDispose(params IDisposable[] tryObjects) =>
            new TCF(tryObjects);

        public TCF Try(params Action[] tryBlocks)
        {
            if (tryBlocks != null)
                this.TryBlocks.AddRange(tryBlocks
                    .Where(b => b != null));

            return this;
        }
        public TCF Try(IEnumerable<Action> tryBlocks)
        {
            if (tryBlocks != null)
                this.TryBlocks.AddRange(tryBlocks
                    .Where(b => b != null));

            return this;
        }
        public TCF Try(params Func<object>[] tryBlocks)
        {
            if (tryBlocks != null)
                this.TryBlocks.AddRange(tryBlocks
                    .Where(b => b != null)
                    .Select(b => (Action)(() => this.Results.IResults.Add(b))));

            return this;
        }
        public TCF Try(IEnumerable<Func<object>> tryBlocks)
        {
            if (tryBlocks != null)
                this.TryBlocks.AddRange(tryBlocks
                    .Where(b => b != null)
                    .Select(b => (Action)(() => this.Results.IResults.Add(b))));

            return this;
        }

        public TCF TryDispose(params IDisposable[] tryObjects)
        {
            if (tryObjects != null)
                this.TryBlocks.AddRange(tryObjects
                    .Select(o => (Action)(() => o?.Dispose())));

            return this;
        }

        public TCF Catch(Action<IReadOnlyList<Exception>> catchBlock)
        {
            this.CatchBlock ??= catchBlock;

            return this;
        }

        public TCF Finally(Action finallyBlock)
        {
            this.FinallyBlock ??= (TCFResults results) => finallyBlock();

            return this;
        }
        public TCF Finally(Action<TCFResults> finallyBlock)
        {
            this.FinallyBlock ??= finallyBlock;

            return this;
        }

        public TCFResults Execute()
        {
            try
            {
                if (this.TryBlocks != null && this.TryBlocks.Count > 0)
                    foreach (var tryBlock in this.TryBlocks)
                        if (tryBlock != null)
                            try
                            {
                                tryBlock();
                            }
                            catch (Exception ex)
                            {
                                if (this.CatchBlock != null)
                                    this.Results.IExceptions.Add(ex);

                                if (this.BreakOnException)
                                    break;
                            }

                if (this.CatchBlock != null)
                    try
                    {
                        this.CatchBlock(this.Results.IExceptions.AsReadOnly());
                    }
                    catch (Exception ex)
                    {
                        this.Results.IExceptions.Add(ex);
                    }
            }
            catch (Exception ex)
            {
                if (this.CatchBlock != null)
                    this.Results.IExceptions.Add(ex);
            }
            finally
            {
                if (this.FinallyBlock != null)
                    try
                    {
                        this.FinallyBlock(this.Results);
                    }
                    catch (Exception ex)
                    {
                        if (this.CatchBlock != null)
                            this.Results.IExceptions.Add(ex);
                    }
            }

            return this.Results;
        }
    }
}
