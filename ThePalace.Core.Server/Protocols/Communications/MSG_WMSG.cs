﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("wmsg")]
    public class MSG_WMSG : IProtocolReceive
    {
        public void Deserialize(Packet packet, params object[] args)
        {
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }
    }
}