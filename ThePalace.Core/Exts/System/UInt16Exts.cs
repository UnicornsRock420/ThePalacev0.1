using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class UInt16Exts
    {
        public static class Types
        {
            public static readonly Type UInt16 = typeof(UInt16);
            public static readonly Type UInt16Array = typeof(UInt16[]);
            public static readonly Type UInt16List = typeof(List<UInt16>);
        }

        //static UInt16Exts() { }

        public static UInt16 SwapShort(this UInt16 value) =>
            BitConverter.ToUInt16(
                BitConverter
                    .GetBytes(value)
                    .Reverse()
                    .ToArray());
    }
}
