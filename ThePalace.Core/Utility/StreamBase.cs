using System;
using System.IO;

namespace ThePalace.Core.Utility
{
    public abstract class StreamBase : Disposable, IDisposable
    {
        protected FileStream _fileStream;
        protected string _pathToFile;

        public StreamBase() { }
        ~StreamBase() =>
            Dispose(false);

        public override void Dispose()
        {
            base.Dispose();

            _fileStream?.Dispose();
            _fileStream = null;
        }

        public bool Open(string pathToFile, bool write = false)
        {
            _pathToFile = pathToFile;

            _fileStream?.Dispose();
            _fileStream = null;

            if (write)
            {
                if (File.Exists(_pathToFile))
                {
                    _fileStream = new FileStream(_pathToFile, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    _fileStream = new FileStream(_pathToFile, FileMode.OpenOrCreate, FileAccess.Write);
                }
            }
            else
            {
                if (!File.Exists(_pathToFile))
                {
                    return false;
                }

                _fileStream = File.Open(_pathToFile, FileMode.Open, FileAccess.Read);
            }

            return true;
        }
    }
}
