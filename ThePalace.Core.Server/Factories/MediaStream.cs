using System;
using System.IO;
using System.Linq;
using ThePalace.Core.Constants;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Factories
{
    public class MediaStream : Packet, IDisposable, IProtocolSend
    {
        public UInt32 transactionID { get; private set; }
        public UInt32 blockSize { get; private set; }
        public UInt16 blockNbr { get; private set; }
        public UInt16 nbrBlocks { get; private set; }
        public UInt32 fileSize { get; private set; }

        private string _fileName;
        private UInt32 _chunkMaxSize;
        private string _pathToFile;
        private UInt32 _bytesRead;
        private Stream _fileStream;
        private byte[] _buffer;

        public bool hasData
        {
            get => ((fileSize - _bytesRead) > 0);
        }

        public bool FileExists
        {
            get => File.Exists(_pathToFile);
        }

        public MediaStream(string fileName, UInt32 chunkMaxSize = NetworkConstants.FILE_STREAM_BUFFER_SIZE)
        {
            fileSize = 0;
            _bytesRead = 0;
            _fileName = fileName;
            _chunkMaxSize = chunkMaxSize;
            _buffer = new byte[NetworkConstants.FILE_STREAM_BUFFER_SIZE];

            var now = DateTime.UtcNow;
            transactionID = (UInt32)(((UInt16)(now.Millisecond & 0xFFFF) << 16) | (UInt16)(now.Ticks & 0xFFFF));

            _pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media", _fileName);
        }

        public bool Open()
        {
            if (!FileExists)
            {
                return false;
            }

            try
            {
                _fileStream = new FileStream(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileSize = (UInt32)_fileStream.Length;
                nbrBlocks = (UInt16)((fileSize / _chunkMaxSize) + ((fileSize % _chunkMaxSize) > 0 ? 1 : 0));
                blockNbr = 0;
                _bytesRead = 0;

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                return false;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            if (_fileStream.CanRead && hasData)
            {
                blockSize = (fileSize - _bytesRead > _chunkMaxSize) ?
                    ((_chunkMaxSize > fileSize - _bytesRead) ? fileSize - _bytesRead : _chunkMaxSize) :
                    (fileSize - _bytesRead > 0) ? fileSize - _bytesRead : 0;

                var read = _fileStream.Read(_buffer, 0, (int)blockSize);
                var buffer = _buffer;

                if (read != blockSize)
                {
                    // This should never be reached but..whatever?
                    if (read > _chunkMaxSize)
                        blockSize = _chunkMaxSize;
                    else
                        blockSize = (UInt32)read;
                }

                _bytesRead += blockSize;

                try
                {
                    _data.Clear();

                    WriteInt32(transactionID); //4
                    WriteInt32(blockSize); // + 4 = 8
                    WriteInt16(blockNbr); // + 2 = 10
                    WriteInt16(nbrBlocks); // + 2 = 12

                    if (blockNbr < 1)
                    {
                        WriteInt32(fileSize); // + 4 = 16
                        WritePString(_fileName, 64, 1); // + 64 = 80

                        //header size becomes 80bytes when this is included. Seems legit? 
                        //Mansionsrc refers to the PString for the name as a 64Byte structure.
                    }
                    else if (_bytesRead >= fileSize)
                    {
                        buffer = _buffer.Take((int)blockSize).ToArray();
                    }

                    WriteBytes(buffer);  // Dump 

                    float percent = (_bytesRead / (float)fileSize);

                    Logger.ConsoleLog($"MediaStream: TID({transactionID}) Sending {blockNbr + 1}/{nbrBlocks} {(int)(percent * 100)}%");

                    blockNbr++;
                    return GetData();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    Dispose();
                    return null;
                }
            }
            else if (hasData)
            {
                if (!Open())
                {
                    Dispose();
                    return null;
                }
            }

            return null;
        }

        public string SerializeJSON(params object[] values)
        {
            return string.Empty;
        }

        public new void Dispose()
        {
            base.Dispose();

            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}
