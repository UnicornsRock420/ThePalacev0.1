using System;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Models.Protocols
{
    public class HotspotStateRec : IProtocolRec
    {
        public Int16 pictID;
        //[JsonIgnore]
        //public short reserved;
        public Point picLoc;

        public HotspotStateRec() { }
        public HotspotStateRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            pictID = packet.ReadSInt16();
            packet.ReadSInt16(); //reserved
            picLoc = new(packet);
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(pictID);
                packet.WriteInt16(0); //reserved
                packet.WriteBytes(picLoc.Serialize());

                return packet.GetData();
            }
        }

        public static int SizeOf => 8;
    }
}
