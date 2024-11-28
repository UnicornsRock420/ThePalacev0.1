using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Server
{
    [Description("sinf")]
    public sealed class MSG_SERVERINFO : IProtocolReceive, IProtocolSend
    {
        public int serverPermissions;
        public string serverName;
        //public UInt32 serverOptions;
        //public UInt32 ulUploadCaps;
        //public UInt32 ulDownloadCaps;

        public void Deserialize(Packet packet, params object[] values)
        {
            serverPermissions = packet.ReadSInt32();
            serverName = packet.ReadPString(64, 1);
            //packet.DropBytes(4);
            //serverOptions = packet.ReadUInt32();
            //ulUploadCaps = packet.ReadUInt32();
            //ulDownloadCaps = packet.ReadUInt32();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                serverPermissions = jsonResponse.serverPermissions;
                serverName = jsonResponse.serverName;
            }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(serverPermissions);
                packet.WritePString(serverName, 64, 1);
                //packet.PadBytes(4);
                //packet.WriteInt32(serverOptions);
                //packet.WriteInt32(ulUploadCaps);
                //packet.WriteInt32(ulDownloadCaps);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                serverPermissions,
                serverName,
                //serverOptions,
                //ulUploadCaps,
                //ulDownloadCaps,
            });
        }
    }
}
