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
    [Description("rLst")]
    public sealed class MSG_LISTOFALLROOMS : List<ListRec>, IProtocolReceive, IProtocolSend
    {
        public int nbrRooms;

        public void Deserialize(Packet packet, params object[] values)
        {
            this.Clear();

            nbrRooms = (int)values.FirstOrDefault();

            if (nbrRooms > 0 &&
                packet.Length > 0)
                for (var i = 0; i < nbrRooms &&
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
