﻿using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("opSs")]
    public class MSG_SPOTSETDESC : IProtocolReceive
    {
        public void Deserialize(Packet packet, params object[] args)
        {

        }

        public void DeserializeJSON(string json)
        {

        }
    }
}
