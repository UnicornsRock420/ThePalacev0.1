using System;

namespace ThePalace.Core.Enums
{
    [Flags]
    public enum RoomFlags : short
    {
        RF_AuthorLocked = 0x0001,
        RF_Private = 0x0002,
        RF_NoPainting = 0x0004,
        RF_Closed = 0x0008,
        RF_CyborgFreeZone = 0x0010,
        RF_Hidden = 0x0020,
        RF_NoGuests = 0x0040,
        RF_WizardsOnly = 0x0080,
        RF_DropZone = 0x0100,
        RF_NoLooseProps = 0x0200,
    };

    public enum NavErrors : uint
    {
        SE_publicError = 0,
        SE_RoomUnknown = 1,
        SE_RoomFull = 2,
        SE_RoomClosed = 3,
        SE_CantAuthor = 4,
        SE_PalaceFull = 5,
    };
}
