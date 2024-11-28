using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("nprs")]
    public class MSG_USERNEW : IProtocolSend
    {
        public UserRec user;

        public byte[] Serialize(params object[] args)
        {
            var message = (Message)args.FirstOrDefault();
            SessionState sessionState;

            try
            {
                sessionState = Network.SessionManager.sessionStates[(UInt16)message.header.refNum];
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                return null;
            }

            user = sessionState.UserInfo;

            using (var packet = new Packet())
            {
                packet.WriteInt32(user.userID);
                packet.WriteBytes(user.roomPos.Serialize());

                for (var j = 0; j < 9; j++)
                {
                    if (j < user.nbrProps && j < user.assetSpec.Count)
                    {
                        packet.WriteBytes(user.assetSpec[j].Serialize());
                    }
                    else
                    {
                        packet.WriteInt32(0);
                        packet.WriteInt32(0);
                    }
                }

                packet.WriteInt16(user.roomID);
                packet.WriteInt16(user.faceNbr);
                packet.WriteInt16(user.colorNbr);
                packet.WriteInt16(user.awayFlag);
                packet.WriteInt16(user.openToMsgs);
                packet.WriteInt16(user.nbrProps);
                packet.WritePString(user.name, 32);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            var message = (Message)args.FirstOrDefault();
            var sessionState = Network.SessionManager.sessionStates[(UInt16)message.header.refNum];

            user = sessionState.UserInfo;

            return JsonConvert.SerializeObject(new
            {
                user,
            });
        }
    }
}
