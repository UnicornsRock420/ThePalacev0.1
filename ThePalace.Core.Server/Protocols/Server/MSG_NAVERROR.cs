using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sErr")]
    public class MSG_NAVERROR : IProtocolSend
    {
        public byte[] Serialize(params object[] args)
        {
            return null;
        }

        public string SerializeJSON(params object[] args)
        {
            return string.Empty;
        }
    }
}
