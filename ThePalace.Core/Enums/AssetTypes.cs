using System;

namespace ThePalace.Core.Enums
{
    public enum LegacyAssetTypes : Int32
    {
        RT_FAVE = 0x46617665,
        RT_PROP = 0x50726F70,
        RT_USERBASE = 0x55736572,
        RT_IPUSERBASE = 0x49557372,
    }

    [Flags]
    public enum AssetFlags : byte
    {
        AssetModified = 0x01,
        AssetLoaded = 0x02,
        AssetPurgeable = 0x04,
        AssetProtected = 0x08,
        AssetInTempFile = 0x10,
    }

    [Flags]
    public enum PropFormats : int
    {
        PF_8Bit = 0x0,
        PF_20Bit = 0x40,
        PF_32Bit = 0x100,
        PF_S20Bit = 0x200,
        PF_16Bit = 0xFF80,
        PF_Custom32Bit = 0x0400,
        PF_Mask = PF_20Bit | PF_S20Bit | PF_32Bit,
    }

    [Flags]
    public enum LoResPropFlags : byte
    {
        PF_Head = 0x02,
        PF_Ghost = 0x04,
        PF_Rare = 0x08,
        PF_Animate = 0x10,
        PF_Bounce = 0x20,
        PF_Palindrome = 0x40,
    }

    [Flags]
    public enum HiResPropFlags : short
    {
        PF_Head = 0x0200,
        PF_Ghost = 0x0400,
        PF_Animate = 0x1000,
        PF_Bounce = 0x2000,
    }
}
