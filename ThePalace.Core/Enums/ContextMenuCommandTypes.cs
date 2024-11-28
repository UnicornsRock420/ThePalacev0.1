using System.ComponentModel;

namespace ThePalace.Core.Enums
{
    public enum ContextMenuCommandTypes : int
    {
        NONE,
        //[Description("kill")]
        //CMD_KILLUSER,
        [Description("pin")]
        CMD_PIN,
        [Description("unpin")]
        CMD_UNPIN,
        [Description("gag")]
        CMD_GAG,
        [Description("ungag")]
        CMD_UNGAG,
        [Description("propgag")]
        CMD_PROPGAG,
        [Description("unpropgag")]
        CMD_UNPROPGAG,
        MSG_KILLUSER,
        MSG_USERMOVE,
        MSG_PROPDEL,
        MSG_SPOTDEL,
        UI_PROPSELECT,
        UI_SPOTSELECT,
        UI_USERSELECT,
    }
}
