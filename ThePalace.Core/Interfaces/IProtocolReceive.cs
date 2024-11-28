using ThePalace.Core.Factories;

namespace ThePalace.Core.Interfaces
{
    public interface IProtocolReceive : IProtocol
    {
        void Deserialize(Packet packet, params object[] values);

        void DeserializeJSON(string json);
    }
}
