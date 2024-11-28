using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Protocols
{
    public class ListRec : IProtocolReceive, IProtocolSend
    {
        public uint primaryID;
        public short flags;
        public short refNum;
        public string name;

        public ListRec() { }
        public ListRec(Packet packet) =>
            Deserialize(packet);

        public void Deserialize(Packet packet, params object[] values)
        {
            primaryID = packet.ReadUInt32();
            flags = packet.ReadSInt16();
            refNum = packet.ReadSInt16();
            var nameLength = (int)packet.PeekByte(0, false);
            //if ((nameLength % 4) != 0)
            nameLength += 4 - (nameLength % 4);
            name = packet.PeekPString(32, 1);
            packet.DropBytes(nameLength);
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }

        public byte[] Serialize(params object[] values)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = string.Empty;
            }

            using (var packet = new Packet())
            {
                packet.WriteInt32(primaryID);
                packet.WriteInt16(flags);
                packet.WriteInt16(refNum);
                packet.WritePString(name, 32, 1, false);
                packet.AlignBytes(4);

                return packet.GetData();
            }
        }

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
