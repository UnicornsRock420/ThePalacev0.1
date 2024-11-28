using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("fnfe")]
    public class MSG_FILENOTFND : IProtocolSend
    {
        public string fileName;

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WritePString(fileName, 64);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                fileName = fileName,
            });
        }
    }
}
