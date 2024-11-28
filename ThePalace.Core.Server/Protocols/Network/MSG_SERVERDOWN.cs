using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("down")]
    public class MSG_SERVERDOWN : IProtocolSend
    {
        public ServerDownFlags whyMessageFlags;
        public string whyMessage;

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                if (!string.IsNullOrWhiteSpace(whyMessage))
                {
                    packet.WriteCString(whyMessage);
                }

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                whyMessage = whyMessage,
            });
        }
    }
}
