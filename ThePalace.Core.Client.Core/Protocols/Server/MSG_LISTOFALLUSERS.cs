using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Server
{
    [Description("uLst")]
    public sealed class MSG_LISTOFALLUSERS : List<ListRec>, IProtocolReceive, IProtocolSend
    {
        public int nbrUsers;

        public void Deserialize(Packet packet, params object[] values)
        {
            this.Clear();

            nbrUsers = (int)values.FirstOrDefault();

            if (nbrUsers > 0 &&
                packet.Length > 0)
                for (var i = 0; i < nbrUsers &&
                    packet.Length > 0; i++)
                    Add(new ListRec(packet));
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }

        public byte[] Serialize(params object[] values) => Array.Empty<byte>();

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
