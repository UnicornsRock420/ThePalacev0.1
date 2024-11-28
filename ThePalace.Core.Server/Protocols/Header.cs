using Newtonsoft.Json;
using System;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    public struct Header : IProtocolReceive, IProtocolSend
    {
        [JsonProperty]
        public string eventType;

        [JsonIgnore]
        public UInt32 eventNbr;

        [JsonIgnore]
        public UInt32 length;

        [JsonProperty]
        public Int32 refNum;

        [JsonProperty]
        public string message;

        public Header(Header header)
        {
            eventType = header.eventType;
            eventNbr = header.eventNbr;
            length = header.length;
            refNum = header.refNum;
            message = header.message;
        }

        public void Deserialize(Packet packet, params object[] args)
        {
            eventNbr = packet.ReadUInt32();
            length = packet.ReadUInt32();
            refNum = packet.ReadSInt32();

            try
            {
                eventType = ((EventTypes)eventNbr).ToString();
            }
            catch { }
        }

        public byte[] Serialize(params object[] args)
        {
            return Serialize(null as byte[]);
        }

        public byte[] Serialize(byte[] append)
        {
            using (var packet = new Packet())
            {
                var evtType = eventNbr;

                if (evtType == 0)
                {
                    try
                    {
                        evtType = (uint)(EventTypes)Enum.Parse(typeof(EventTypes), eventType);
                    }
                    catch { }
                }

                packet.WriteInt32(evtType);
                packet.WriteInt32(length);
                packet.WriteInt32(refNum);

                if (append != null)
                {
                    packet.WriteBytes(append);
                }

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            this = JsonConvert.DeserializeObject<Header>(json);
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(this);
        }

        public static int SizeOf
        {
            get
            {
                return 12;
            }
        }
    }
}
