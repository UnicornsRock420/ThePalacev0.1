using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sInf")]
    public class MSG_EXTENDEDINFO : IProtocolReceive, IProtocolSend
    {
        public UInt32 flags;

        public void Deserialize(Packet packet, params object[] args)
        {
            var error = false;

            flags = packet.ReadUInt32();

            while (packet.Length > 0 && !error)
            {
                var id = packet.ReadSInt32();
                var length = packet.ReadSInt32();
                var buf = packet.GetData(length, 0, true);

                switch ((ServerExtInfoTypes)id)
                {
                    case ServerExtInfoTypes.SI_EXT_NAME:
                        var userName = buf.ReadPString(32, 0, 1);

                        break;
                    case ServerExtInfoTypes.SI_EXT_PASS:
                        var password = buf.ReadPString(32, 0, 1);

                        break;
                    case ServerExtInfoTypes.SI_EXT_TYPE:
                        var clientType = buf.ReadPString(32, 0, 1);

                        break;
                    default:
                        error = true;
                        break;
                }
            }
        }

        public byte[] Serialize(params object[] args)
        {
            var maxUserID = ConfigManager.GetValue<UInt32>("MaxUserID", 9999).Value;
            var serverName = ConfigManager.GetValue("ServerName", string.Empty);
            var mediaUrl = ConfigManager.GetValue("MediaUrl", string.Empty);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var response = new List<byte[]>();
            var data = new byte[0];

            using (var packet = new Packet())
            {
                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_Avatar_URL) == ServerExtInfoInFlags.SI_Avatar_URL)
                {
                    data = string.Empty.WriteCString();
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_AURL);
                    packet.WriteInt32(data.Length);
                    packet.WriteBytes(data);
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_Server_Version) == ServerExtInfoInFlags.SI_Server_Version)
                {
                    data = $"{version.Major}.{version.Minor}.{version.Revision}.{version.Build}".WriteCString();
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_VERS);
                    packet.WriteInt32(data.Length);
                    packet.WriteBytes(data);
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_SERVER_TYPE) == ServerExtInfoInFlags.SI_SERVER_TYPE)
                {
                    data = $"{Environment.OSVersion}".WriteCString();
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_TYPE);
                    packet.WriteInt32(data.Length);
                    packet.WriteBytes(data);
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_SERVER_FLAGS) == ServerExtInfoInFlags.SI_SERVER_FLAGS)
                {
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_FLAG);
                    packet.WriteInt32(9);
                    packet.WriteInt32((int)ServerExtInfoOutFlags.FF_GuestsAreMembers);
                    packet.WriteInt32(maxUserID);
                    packet.WriteByte(2);
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_NUM_USERS) == ServerExtInfoInFlags.SI_NUM_USERS)
                {
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_NUM_USERS);
                    packet.WriteInt32(4);
                    packet.WriteInt32(Network.SessionManager.GetServerUserCount());
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_SERVER_NAME) == ServerExtInfoInFlags.SI_SERVER_NAME)
                {
                    data = serverName.WriteCString();
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_NAME);
                    packet.WriteInt32(data.Length);
                    packet.WriteBytes(data);
                    response.Add(packet.GetData());

                    packet.Clear();
                }

                if (((ServerExtInfoInFlags)flags & ServerExtInfoInFlags.SI_HTTP_URL) == ServerExtInfoInFlags.SI_HTTP_URL)
                {
                    data = mediaUrl.WriteCString();
                    packet.WriteInt32((int)ServerExtInfoTypes.SI_INF_HURL);
                    packet.WriteInt32(data.Length);
                    packet.WriteBytes(data);
                    response.Add(packet.GetData());

                    //packet.Clear();
                }
            }

            return response.SelectMany(b => b).ToArray();
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
