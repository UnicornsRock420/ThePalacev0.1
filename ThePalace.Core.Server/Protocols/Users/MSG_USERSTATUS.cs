using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("uSta")]
    public class MSG_USERSTATUS : IProtocolSend
    {
        public Int16 flags;
        public Guid hash;

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(flags);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                flags = flags,
                hash = hash.ToString(),
            });
        }
    }
}
