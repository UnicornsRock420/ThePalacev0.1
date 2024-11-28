using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Plugins.Protocols
{
    public class MSG_TEST : IProtocolReceive
    {
        public void Deserialize(Packet packet, params object[] values)
        {
            return;
        }

        public void DeserializeJSON(string json)
        {
            return;
        }
    }
}
