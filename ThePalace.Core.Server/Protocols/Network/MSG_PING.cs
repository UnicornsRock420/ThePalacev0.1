using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("ping")]
    public class MSG_PING : IProtocolReceive, IProtocolSend
    {
        public void Deserialize(Packet packet, params object[] args)
        {

        }

        public byte[] Serialize(params object[] args)
        {
            return null;
        }

        public void DeserializeJSON(string json)
        {

        }

        public string SerializeJSON(params object[] args)
        {
            return string.Empty;
        }
    }
}
