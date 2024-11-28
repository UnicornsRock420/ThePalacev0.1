using Newtonsoft.Json;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Server.Protocols
{
    [Description("rep2")]
    public class MSG_ALTLOGONREPLY : IProtocolSend
    {
        public RegistrationRec reg;

        public MSG_ALTLOGONREPLY() { }
        public MSG_ALTLOGONREPLY(RegistrationRec regRec)
        {
            reg = regRec;
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(reg.crc);
                packet.WriteInt32(reg.counter);
                packet.WritePString(reg.userName, 32);
                packet.WritePString(reg.wizPassword, 32);
                packet.WriteInt32(reg.auxFlags);
                packet.WriteInt32(reg.puidCtr);
                packet.WriteInt32(reg.puidCRC);
                packet.WriteInt32(reg.demoElapsed);
                packet.WriteInt32(reg.totalElapsed);
                packet.WriteInt32(reg.demoLimit);
                packet.WriteInt16(reg.desiredRoom);
                packet.WriteByte((byte)reg.reserved[0]);
                packet.WriteByte((byte)reg.reserved[1]);
                packet.WriteByte((byte)reg.reserved[2]);
                packet.WriteByte((byte)reg.reserved[3]);
                packet.WriteByte((byte)reg.reserved[4]);
                packet.WriteByte((byte)reg.reserved[5]);
                packet.WriteInt32(reg.ulRequestedProtocolVersion);
                packet.WriteInt32(reg.ulUploadCaps);
                packet.WriteInt32(reg.ulDownloadCaps);
                packet.WriteInt32(reg.ul2DEngineCaps);
                packet.WriteInt32(reg.ul2DGraphicsCaps);
                packet.WriteInt32(reg.ul3DEngineCaps);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                reg = new
                {
                    userName = reg.userName,
                    auxFlags = reg.auxFlags,
                }
            });
        }
    }
}
