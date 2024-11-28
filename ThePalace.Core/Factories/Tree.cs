using System;
using System.Collections.Generic;

namespace ThePalace.Core.Factories
{
    public class Tree<T> : List<T>, IDisposable
    {
        protected bool IsDisposed { get; private set; } = false;

        public Tree<T> Children { get; set; } = new();

        public Tree() { }
        ~Tree() =>
            this.Dispose(false);

        public void Dispose()
        {
            if (this.IsDisposed) return;

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                this.Children?.Clear();
                this.Children?.Dispose();
                this.Children = null;
            }

            IsDisposed = true;
        }
    }
}
