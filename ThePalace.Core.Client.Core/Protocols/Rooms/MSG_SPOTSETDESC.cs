using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("opSs")]
    public sealed class MSG_SPOTSETDESC : IProtocolSend
    {
        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                return null;
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return string.Empty;
        }
    }
}
