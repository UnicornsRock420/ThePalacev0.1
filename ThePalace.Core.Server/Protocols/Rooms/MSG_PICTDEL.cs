﻿using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("FPSq")]
    public class MSG_PICTDEL : IProtocolReceive, IProtocolSend
    {
        public void Deserialize(Packet packet, params object[] args)
        {

        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                return null;
            }
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
