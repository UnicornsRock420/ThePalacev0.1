namespace ThePalace.Core.Enums
{
    public enum ServerExtInfoTypes : uint
    {
        SI_EXT_NAME = 0x4E414D4,
        SI_EXT_PASS = 0x5041535,
        SI_EXT_TYPE = 0x5459504,
        SI_ERR_AUTH = 0x4155544,
        SI_ERR_UNKN = 0x554E4B4,
        SI_INF_AURL = 0x4155524,
        SI_INF_VERS = 0x5645525,
        SI_INF_TYPE = 0x5459504,
        SI_INF_FLAG = 0x464C414,
        SI_INF_NUM_USERS = 0x4E55535,
        SI_INF_NAME = 0x4E414D4,
        SI_INF_HURL = 0x4855524,
    }

    public enum ServerExtInfoInFlags : uint
    {
        SI_Avatar_URL = 0x00000001,
        SI_Server_Version = 0x00000002,
        SI_SERVER_TYPE = 0x00000004,
        SI_SERVER_FLAGS = 0x00000008,
        SI_NUM_USERS = 0x00000010,
        SI_SERVER_NAME = 0x00000020,
        SI_HTTP_URL = 0x00000040
    };

    public enum ServerExtInfoOutFlags : short
    {
        FF_DirectPlay = 0x0001,
        FF_ClosedServer = 0x0002,
        FF_GuestsAreMembers = 0x0004,
        FF_Unused1 = 0x0008,
        FF_InstantPalace = 0x0010,
        FF_PalacePresents = 0x0020,
    };

    public enum ServerExtInfoPlatform : byte
    {
        PLAT_Macintosh = 0,
        PLAT_Windows95 = 1,
        PLAT_WindowsNT = 2,
        PLAT_Unix = 3,
    };
}
