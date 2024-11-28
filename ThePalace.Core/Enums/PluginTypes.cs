namespace ThePalace.Core.Enums
{
    public enum FeatureTypes : short
    {
        CORE,
        GUI,
        MEDIA,
        NETWORK,
    }

    public enum SubFeatureTypes : short
    {
        NONE,
        AUDIO,
        BUSINESS,
        CODEC,
        COMMUNICATION,
        LIBRARY,
        LOGGING,
        LOGIC,
        MANAGER,
        PROTOCOL,
        SCRIPTING,
        SYSTRAYICON,
        VIDEO,
    }

    public enum PurposeTypes : short
    {
        VOID,
        PROVIDER,
        CONSUMER,
    }

    public enum DeviceTypes : short
    {
        NONE,
        KEYBOARD,
        MOUSE,
        USB,
        BLUETOOTH,
        STORAGE,
        VIRTUAL,
    }
}
