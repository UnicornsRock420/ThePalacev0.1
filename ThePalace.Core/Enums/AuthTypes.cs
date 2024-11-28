using System;

namespace ThePalace.Core.Enums
{
    [Flags]
    public enum AuthTypes : uint
    {
        Password = 0x01,
        IPAddress = 0x02,
        RegCode = 0x04,
        PUID = 0x08,
    };
}
