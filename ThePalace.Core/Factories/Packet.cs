using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThePalace.Core.ExtensionMethods;

namespace ThePalace.Core.Factories
{
    public class Packet : IDisposable
    {
        protected List<byte> _data;
        protected int _carat = 0;

        public int Length =>
            _data?.Count ?? 0;

        public IReadOnlyList<byte> Data =>
            _data;

        public Packet() =>
            _data = new List<byte>();

        public Packet(IEnumerable<byte> source) =>
            _data = new List<byte>(source);

        public void Dispose()
        {
            if (_data != null)
            {
                _data.Clear();
                _data = null;
            }
        }

        public static Packet FromBytes(IEnumerable<byte> data) =>
            new Packet(data);

        public byte[] GetData(int max = 0, int offset = 0, bool purge = false)
        {
            if (max < 1)
            {
                max = 0;
            }

            if (offset < 1)
            {
                offset = 0;
            }

            if (max > 0)
            {
                var result = _data
                    .Skip(offset)
                    .Take(max)
                    .ToArray();

                if (purge)
                {
                    _data.RemoveRange(0, max);
                }

                return result;
            }
            else
            {
                return _data
                    .Skip(offset)
                    .ToArray();
            }
        }

        public void SetData(IEnumerable<byte> source) =>
            _data = new List<byte>(source);

        #region Read Methods
        public byte ReadByte(int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var result = _data[offset];

            _data.RemoveAt(offset);

            return result;
        }

        public Int16 ReadSInt16(int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var result = _data.ReadSInt16(offset);

            _data.RemoveRange(offset, 2);

            return result;
        }

        public Int32 ReadSInt32(int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var result = _data.ReadSInt32(offset);

            _data.RemoveRange(offset, 4);

            return result;
        }

        public UInt16 ReadUInt16(int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var result = _data.ReadUInt16(offset);

            _data.RemoveRange(offset, 2);

            return result;
        }

        public UInt32 ReadUInt32(int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var result = _data.ReadUInt32(offset);

            _data.RemoveRange(offset, 4);

            return result;
        }

        public string ReadPString(int max, int size = 0, int offset = 0, int delta = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (size < 1)
            {
                size = 1;
            }

            var length = 0;

            switch (size)
            {
                case 4:
                    length = ReadSInt32();

                    break;
                case 2:
                    length = ReadSInt16();

                    break;
                default:
                    length = ReadByte();
                    size = 1;

                    break;
            }

            if (delta > 0)
            {
                length -= delta;
            }

            if (max > 0 && length > max)
            {
                length = max;
            }

            var data = _data
                .ToList()
                .Skip(offset)
                .Take(length)
                .ToArray();

            max -= size;

            if (max > _data.Count)
            {
                max = _data.Count;
            }

            if (max > 0)
            {
                _data.RemoveRange(offset, max);
            }

            return (data.GetString() ?? string.Empty).TrimEnd('\0');
        }

        public string ReadCString(int offset = 0, bool? peek = false)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            var length = _data
                .Skip(offset)
                .ToList()
                .IndexOf(0);

            var data = _data
                .ToList()
                .Skip(offset)
                .Take(length)
                .ToArray();

            if (peek != null)
                if (peek.Value != true)
                    _data.RemoveRange(offset, length);

            return data.GetString();
        }
        #endregion

        #region Peek Methods
        public int Seek(int offset = 0, SeekOrigin origin = SeekOrigin.Begin)
        {
            switch (origin)
            {
                case SeekOrigin.End:
                    {
                        offset = _data.Count - offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        offset += _carat;
                        break;
                    }
            }

            if (offset < 0)
                return 0;
            else if (offset > _data.Count)
                return 0;

            _carat = offset;

            return _carat;
        }
        public byte PeekByte(int offset = 0, bool useCarat = true)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (useCarat)
            {
                offset += _carat;
                _carat++;
            }

            return _data[offset];
        }

        public Int16 PeekSInt16(int offset = 0, bool useCarat = true)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (useCarat)
            {
                offset += _carat;
                _carat += 2;
            }

            return _data.ReadSInt16(offset);
        }

        public Int32 PeekSInt32(int offset = 0, bool useCarat = true)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (useCarat)
            {
                offset += _carat;
                _carat += 4;
            }

            return _data.ReadSInt32(offset);
        }

        public UInt16 PeekUInt16(int offset = 0, bool useCarat = true)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (useCarat)
            {
                offset += _carat;
                _carat += 2;
            }

            return _data.ReadUInt16(offset);
        }

        public UInt32 PeekUInt32(int offset = 0, bool useCarat = true)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (useCarat)
            {
                offset += _carat;
                _carat += 4;
            }

            return _data.ReadUInt32(offset);
        }

        public string PeekPString(int max, int size = 0, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (size < 1)
            {
                size = 1;
            }

            var length = 0;

            switch (size)
            {
                case 4:
                    length = PeekSInt32(offset, false);

                    break;
                case 2:
                    length = PeekSInt16(offset, false);

                    break;
                default:
                    length = PeekByte(offset, false);
                    size = 1;

                    break;
            }

            if (length > max)
            {
                length = max;
            }

            var data = _data.ToList()
                .Skip(offset + size)
                .Take(length)
                .ToArray();

            return data.GetString();

        }
        #endregion

        #region Write Methods
        public void WriteByte(byte source) =>
            _data.Add(source);

        public void WriteBytes(byte[] source, int max = 0, int offset = 0)
        {
            if (max < 1 &&
                offset < 1)
                _data.AddRange(source);
            else
                _data.AddRange(source
                    .Skip(offset)
                    .Take(max)
                    .ToList());
        }

        public void WriteInt16(Int16 source) =>
            _data.AddRange(source.WriteInt16());

        public void WriteInt32(Int32 source) =>
            _data.AddRange(source.WriteInt32());

        public void WriteInt16(UInt16 source) =>
            _data.AddRange(source.WriteInt16());

        public void WriteInt32(UInt32 source) =>
            _data.AddRange(source.WriteInt32());

        public void WritePString(string source, int max, int size = 0, bool padding = true)
        {
            if (size < 1)
            {
                size = 1;
            }

            _data.AddRange(source.WritePString(max, size, padding));
        }

        public void WriteCString(string source) =>
            _data.AddRange(source.WriteCString());
        #endregion

        #region Helper Methods
        public void Clear() =>
            _data.Clear();

        public void DropBytes(int length = 0, int offset = 0)
        {
            if (offset < 1)
            {
                offset = 0;
            }

            if (length < 1)
            {
                if (offset < 1)
                {
                    _data.Clear();

                    return;
                }

                length = _data.Count - offset;
            }

            _data.RemoveRange(offset, length);
        }

        public void PadBytes(int source)
        {
            for (var j = _data.Count % source; j > 0; j--)
            {
                _data.Add(0);
            }
        }

        public int PadOffset(int source, int value)
        {
            value += value % source;
            return value;
        }

        public void AlignBytes(int source)
        {
            for (var j = source - (_data.Count % source); j > 0; j--)
            {
                _data.Add(0);
            }
        }
        #endregion
    }
}
