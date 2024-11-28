using System;
using System.ComponentModel;

namespace ThePalace.Core.Enums
{
    //public enum HotSpotEventMask : int
    //{
    //    PE_Select = 0x00000001,
    //    PE_Lock = 0x00000002,
    //    PE_Unlock = 0x00000004,
    //    PE_Hide = 0x00000008,
    //    PE_Show = 0x00000010,
    //    PE_Startup = 0x00000020,
    //    PE_Alarm = 0x00000040,
    //    PE_Custom = 0x00000080,
    //    PE_InChat = 0x00000100,
    //    PE_PropChange = 0x00000200,
    //    PE_Enter = 0x00000400,
    //    PE_Leave = 0x00000800,
    //    PE_OutChat = 0x00001000,
    //    PE_SignOn = 0x00002000,
    //    PE_SignOff = 0x00004000,
    //    PE_Macro0 = 0x00008000,
    //    PE_Macro1 = 0x00010000,
    //    PE_Macro2 = 0x00020000,
    //    PE_Macro3 = 0x00040000,
    //    PE_Macro4 = 0x00080000,
    //    PE_Macro5 = 0x00100000,
    //    PE_Macro6 = 0x00200000,
    //    PE_Macro7 = 0x00400000,
    //    PE_Macro8 = 0x00800000,
    //    PE_Macro9 = 0x01000000,
    //};

    [Flags]
    public enum HotspotFlags : short
    {
        HS_Draggable = 0x0001,
        HS_DontMoveHere = 0x0002,
        HS_Invisible = 0x0004,
        HS_ShowName = 0x0008,
        HS_ShowFrame = 0x0010,
        HS_Shadow = 0x0020,
        HS_Fill = 0x0040,
        HS_Forbidden = 0x0080,
        HS_Mandatory = 0x0100,
        HS_LandingPad = 0x0200,
    };

    public enum HotspotTypes : short
    {
        [Description("SPOT")]
        HS_Normal = 0,
        [Description("DOOR")]
        HS_Door = 1,
        [Description("DOOR")]
        HS_ShutableDoor = 2,
        [Description("DOOR")]
        HS_LockableDoor = 3,
        [Description("BOLT")]
        HS_Bolt = 4,
        [Description("NAVAREA")]
        HS_NavArea = 5
    };

    public enum HotspotStates : byte
    {
        HS_Unlock = 0,
        HS_Lock = 1,
    };
}
