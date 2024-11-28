namespace ThePalace.Core.Client.Core.Enums
{
    public enum ThreadQueues : short
    {
        GUI,
        Audio,
        Core,
        Devices,
        Media,
        Assets,
        Network,
        ScriptEngine,
#if WINDOWS10_0_17763_0_OR_GREATER
        Toast,
#endif
    }
}
