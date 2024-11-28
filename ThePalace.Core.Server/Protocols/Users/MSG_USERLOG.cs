using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("log ")]
    public class MSG_USERLOG : IProtocolSend
    {
        public UInt32 nbrUsers;

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(nbrUsers);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                nbrUsers = nbrUsers,
            });
        }
    }
}
