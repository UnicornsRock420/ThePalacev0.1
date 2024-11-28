using System;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Protocols
{
    public class RegistrationRec : IProtocolRec
    {
        public uint crc;
        public uint counter;
        public string userName;
        public string wizPassword;
        public int auxFlags;
        public uint puidCtr;
        public uint puidCRC;
        public uint demoElapsed;
        public uint totalElapsed;
        public uint demoLimit;
        public short desiredRoom;
        public string reserved;
        public uint ulRequestedProtocolVersion;
        public uint ulUploadCaps;
        public uint ulDownloadCaps;
        public uint ul2DEngineCaps;
        public uint ul2DGraphicsCaps;
        public uint ul3DEngineCaps;

        public RegistrationRec() { }
        public RegistrationRec(RegistrationRec source)
        {
            crc = source.crc;
            counter = source.counter;
            userName = source.userName;
            wizPassword = source.wizPassword;
            auxFlags = source.auxFlags;
            puidCRC = source.puidCRC;
            puidCtr = source.puidCtr;
            demoElapsed = source.demoElapsed;
            totalElapsed = source.totalElapsed;
            demoLimit = source.demoLimit;
            desiredRoom = source.desiredRoom;
            reserved = source.reserved;
            ulRequestedProtocolVersion = source.ulRequestedProtocolVersion;
            ulUploadCaps = source.ulUploadCaps;
            ulDownloadCaps = source.ulDownloadCaps;
            ul2DEngineCaps = source.ul2DEngineCaps;
            ul2DGraphicsCaps = source.ul2DGraphicsCaps;
            ul3DEngineCaps = source.ul3DEngineCaps;
        }

        public RegistrationRec(Packet packet, params object[] values) =>
            Deserialize(packet, values);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            crc = packet.ReadUInt32();                                      // crc
            counter = packet.ReadUInt32();                                  // counter
            userName = packet.ReadPString(32, 1);                           // userName
            wizPassword = packet.ReadPString(32, 1);                        // wizPassword
            auxFlags = packet.ReadSInt32();                                 // auxFlags
            puidCtr = packet.ReadUInt32();                                  // puidCtr
            puidCRC = packet.ReadUInt32();                                  // puidCRC
            demoElapsed = packet.ReadUInt32();                              // demoElapsed
            totalElapsed = packet.ReadUInt32();                             // totalElapsed
            demoLimit = packet.ReadUInt32();                                // demoLimit
            desiredRoom = packet.ReadSInt16();                              // desiredRoom
            reserved += (char)packet.ReadByte();                            // reserved[0]
            reserved += (char)packet.ReadByte();                            // reserved[1]
            reserved += (char)packet.ReadByte();                            // reserved[2]
            reserved += (char)packet.ReadByte();                            // reserved[3]
            reserved += (char)packet.ReadByte();                            // reserved[4]
            reserved += (char)packet.ReadByte();                            // reserved[5]
            ulRequestedProtocolVersion = packet.ReadUInt32();               // ulRequestedProtocolVersion
            ulUploadCaps = packet.ReadUInt32();                             // ulUploadCaps
            ulDownloadCaps = packet.ReadUInt32();                           // ulDownloadCaps
            ul2DEngineCaps = packet.ReadUInt32();                           // ul2DEngineCaps
            ul2DGraphicsCaps = packet.ReadUInt32();                         // ul2DGraphicsCaps
            ul3DEngineCaps = packet.ReadUInt32();                           // ul3DEngineCaps
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt32(crc);                                     // crc
                packet.WriteInt32(counter);                                 // counter
                packet.WritePString(userName, 32, 1);                       // userName
                packet.WritePString(wizPassword ?? string.Empty, 32, 1);    // wizPassword
                packet.WriteInt32(auxFlags);                                // auxFlags
                packet.WriteInt32(puidCtr);                                 // puidCtr
                packet.WriteInt32(puidCRC);                                 // puidCRC
                packet.WriteInt32(demoElapsed);                             // demoElapsed
                packet.WriteInt32(totalElapsed);                            // totalElapsed
                packet.WriteInt32(demoLimit);                               // demoLimit
                packet.WriteInt16(desiredRoom);                             // desiredRoom
                packet.WriteBytes(reserved.GetBytes(6));                    // reserved[6]
                packet.WriteInt32(ulRequestedProtocolVersion);              // ulRequestedProtocolVersion
                packet.WriteInt32(ulUploadCaps);                            // ulUploadCaps
                packet.WriteInt32(ulDownloadCaps);                          // ulDownloadCaps
                packet.WriteInt32(ul2DEngineCaps);                          // ul2DEngineCaps
                packet.WriteInt32(ul2DGraphicsCaps);                        // ul2DGraphicsCaps
                packet.WriteInt32(ul3DEngineCaps);                          // ul3DEngineCaps

                return packet.GetData();
            }
        }

        public static int SizeOf => 128;
    }
}
