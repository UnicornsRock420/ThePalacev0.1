namespace ThePalace.Core.Interfaces
{
    public interface IProtocolSend
    {
        byte[] Serialize(params object[] values);

        string SerializeJSON(params object[] values);
    }
}
