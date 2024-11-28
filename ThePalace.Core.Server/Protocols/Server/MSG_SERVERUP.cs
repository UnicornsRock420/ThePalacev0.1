using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("init")]
    public class MSG_SERVERUP : IProtocolReceive
    {
        public void Deserialize(Packet packet, params object[] args)
        {

        }

        public void DeserializeJSON(string json)
        {

        }
    }
}
