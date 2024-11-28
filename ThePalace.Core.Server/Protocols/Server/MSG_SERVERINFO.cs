using Newtonsoft.Json;
using System;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("sinf")]
    public class MSG_SERVERINFO : IProtocolSend
    {
        public Int32 serverPermissions;
        public string serverName;
        //public UInt32 serverOptions;
        //public UInt32 ulUploadCaps;
        //public UInt32 ulDownloadCaps;

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(serverPermissions);
                packet.WritePString(serverName, 64);
                packet.PadBytes(4);
                //packet.WriteInt32(serverOptions);
                //packet.WriteInt32(ulUploadCaps);
                //packet.WriteInt32(ulDownloadCaps);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                serverPermissions,
                serverName,
            });
        }
    }
}
