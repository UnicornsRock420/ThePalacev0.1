using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class UInt32Exts
    {
        public static class Types
        {
            public static readonly Type UInt32 = typeof(UInt32);
            public static readonly Type UInt32Array = typeof(UInt32[]);
            public static readonly Type UInt32List = typeof(List<UInt32>);
        }

        //static UInt32Exts() { }

        public static byte[] ToUInt31(this UInt32 value)
        {
            var data = BitConverter.GetBytes(value);
            data[3] = data[3].ClearBit(7);

            return data;
        }

        public static UInt32 SwapInt(this UInt32 value) =>
            BitConverter.ToUInt32(
                BitConverter
                    .GetBytes(value)
                    .Reverse()
                    .ToArray());
    }
}
