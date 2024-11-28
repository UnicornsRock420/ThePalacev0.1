using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Users
{
    [Description("rprs")]
    public sealed class MSG_USERLIST : List<UserRec>, IProtocolReceive, IProtocolSend
    {
        public uint nbrUsers =>
            (uint)Count;

        public void Deserialize(Packet packet, params object[] values)
        {
            Clear();

            var _nbrUsers = (int)values.FirstOrDefault();

            if (packet.Length > 0)
                for (var i = 0; i < _nbrUsers &&
                    packet.Length > 0; i++)
                {
                    var userRec = new UserRec(packet);

                    Add(userRec);
                }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
                for (var i = 0; i > (jsonResponse?.users?.Length ?? 0); i++)
                {
                    Add(new UserRec
                    {
                        userID = jsonResponse.users[i].userID,
                        name = jsonResponse.users[i].name,
                        faceNbr = jsonResponse.users[i].faceNbr,
                        colorNbr = jsonResponse.users[i].colorNbr,
                        awayFlag = jsonResponse.users[i].awayFlag,
                        openToMsgs = jsonResponse.users[i].openToMsgs,
                        roomPos = jsonResponse.users[i].roomPos,
                        assetSpec = jsonResponse.users[i].propSpec,
                    });
                }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                foreach (var user in this)
                    packet.WriteBytes(user.Serialize());

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values) =>
            JsonConvert.SerializeObject(new
            {
                users = this
                    .Select(u => new
                    {
                        u.userID,
                        u.name,
                        u.faceNbr,
                        u.colorNbr,
                        u.awayFlag,
                        u.openToMsgs,
                        u.roomPos,
                        u.assetSpec,
                    })
                    .ToArray(),
            });
    }
}
