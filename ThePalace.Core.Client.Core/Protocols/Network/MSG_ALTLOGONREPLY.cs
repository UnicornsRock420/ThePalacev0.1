using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Network
{
    [Description("rep2")]
    public sealed class MSG_ALTLOGONREPLY : RegistrationRec, IProtocolReceive, IProtocolSend
    {
        public MSG_ALTLOGONREPLY() { }
        public MSG_ALTLOGONREPLY(RegistrationRec source) : base(source) { }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            crc = (uint)(int)jsonResponse.reg.crc;
            counter = (uint)(int)jsonResponse.reg.counter;
            userName = jsonResponse.reg.userName;
            wizPassword = jsonResponse.reg.wizPassword ?? string.Empty;
            auxFlags = jsonResponse.reg.auxFlags;
            puidCtr = (uint)(int)jsonResponse.reg.puidCtr;
            puidCRC = (uint)(int)jsonResponse.reg.puidCRC;
            desiredRoom = jsonResponse.reg.desiredRoom;
            reserved = jsonResponse.reg.reserved;
            ulRequestedProtocolVersion = jsonResponse.reg.ulRequestedProtocolVersion;
            ulUploadCaps = (uint)(int)jsonResponse.reg.ulUploadCaps;
            ulDownloadCaps = (uint)(int)jsonResponse.reg.ulDownloadCaps;
            ul2DEngineCaps = (uint)(int)jsonResponse.reg.ul2DEngineCaps;
            ul2DGraphicsCaps = (uint)(int)jsonResponse.reg.ul2DGraphicsCaps;
            ul3DEngineCaps = (uint)(int)jsonResponse.reg.ul3DEngineCaps;
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                crc,
                counter,
                userName,
                wizPassword = wizPassword ?? string.Empty,
                auxFlags,
                puidCtr,
                puidCRC,
                desiredRoom,
                reserved,
                ulRequestedProtocolVersion,
                ulUploadCaps,
                ulDownloadCaps,
                ul2DEngineCaps,
                ul2DGraphicsCaps,
                ul3DEngineCaps,
            });
        }
    }
}
