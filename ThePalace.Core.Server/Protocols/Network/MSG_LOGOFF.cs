using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("bye ")]
    public class MSG_LOGOFF : IProtocolReceive, IProtocolSend
    {
        public Int32 nbrUsers;

        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(nbrUsers);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
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
