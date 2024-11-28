using System;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Models.Palace
{
    [Serializable]
    public class Point : IProtocolRec
    {
        public short h;
        public short v;

        public Point()
        {
            h = (short)RndGenerator.NextSecure(0, 512);
            v = (short)RndGenerator.NextSecure(0, 384);
        }

        public Point(short hAxis, short vAxis)
        {
            h = hAxis;
            v = vAxis;
        }

        public Point(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            v = packet.ReadSInt16();
            h = packet.ReadSInt16();
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(v);
                packet.WriteInt16(h);

                return packet.GetData();
            }
        }

        public static int SizeOf => 4;
    }
}
