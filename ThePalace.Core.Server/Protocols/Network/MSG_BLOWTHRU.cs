using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("blow")]
    public class MSG_BLOWTHRU : IProtocolReceive, IProtocolSend
    {
        public UInt32 flags;
        public UInt32 nbrUsers;
        public List<UInt32> userIDs; /* iff nbrUsers >= 0 */
        public UInt32 pluginTag;
        public byte[] embedded;

        public void Deserialize(Packet packet, params object[] args)
        {
            flags = packet.ReadUInt32();
            nbrUsers = packet.ReadUInt32();

            userIDs = new List<UInt32>();

            for (var j = 0; j < nbrUsers; j++)
            {
                var userID = packet.ReadUInt32();

                userIDs.Add(userID);
            }

            pluginTag = packet.ReadUInt32();
            embedded = packet.GetData();
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(pluginTag);
                packet.WriteBytes(embedded);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                flags = jsonResponse.flags;
                userIDs = new List<UInt32>((UInt32[])jsonResponse.userIDs);
                pluginTag = jsonResponse.pluginTag;
                embedded = jsonResponse.embedded;
            }
            catch
            {
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                pluginTag = pluginTag,
                embedded = embedded,
            });
        }
    }
}
