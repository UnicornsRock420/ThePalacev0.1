using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Server.Models
{
    public class Message
    {
        public SessionState sessionState;
        public MediaState mediaState;
        public AssetState assetState;
        public Header header;
        public IProtocolReceive protocol;
    }
}
