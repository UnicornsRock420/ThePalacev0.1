using System.ComponentModel;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("endr")]
    public class MSG_ROOMDESCEND : IProtocolSend
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
