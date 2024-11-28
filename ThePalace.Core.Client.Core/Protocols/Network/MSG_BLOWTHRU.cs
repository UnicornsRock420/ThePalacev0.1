using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Network
{
    [Description("blow")]
    public sealed class MSG_BLOWTHRU : IProtocolReceive, IProtocolSend
    {
        public UInt32 flags;
        //public UInt32 nbrUsers;
        public List<UInt32> userIDs; /* iff nbrUsers >= 0 */
        //public uint pluginTag;
        public byte[] embedded;

        public void Deserialize(Packet packet, params object[] values)
        {
            //flags = packet.ReadUInt32();
            //pluginTag = packet.ReadUInt32();
            embedded = packet.GetData();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                //flags = jsonResponse.flags;
                //pluginTag = jsonResponse.pluginTag;
                embedded = jsonResponse.embedded;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(flags);
                packet.WriteInt32(userIDs?.Count ?? 0);

                if ((userIDs?.Count ?? 0) > 0)
                    foreach (var userID in userIDs)
                        packet.WriteInt32(userID);

                //packet.WriteInt32(pluginTag);
                packet.WriteBytes(embedded);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values) =>
            JsonConvert.SerializeObject(new
            {
                flags,
                userIDs = userIDs.ToArray(),
                //pluginTag,
                embedded,
            });
    }
}
