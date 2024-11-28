using Newtonsoft.Json;
using System;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Protocols
{
    public class PictureRec : IProtocolRec
    {
        [JsonIgnore]
        public int refCon;
        public short picID;
        [JsonIgnore]
        public short picNameOfst;
        public short transColor;
        //[JsonIgnore]
        //public short reserved;

        public string name;

        public PictureRec() { }

        public PictureRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
        }

        public byte[] Serialize(params object[] values)
        {
            return Array.Empty<byte>();
        }

        public static int SizeOf => 12;
    }
}
