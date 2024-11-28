using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Protocols
{
    public class Header : Packet, IProtocolRec, IProtocolReceive, IProtocolSend
    {
        [JsonProperty]
        public EventTypes eventType;

        [JsonIgnore]
        public uint length;

        [JsonProperty]
        public int refNum;

        [JsonIgnore]
        public IProtocolRec protocolRec;

        [JsonIgnore]
        public Type protocolReceiveType;

        [JsonIgnore]
        public IProtocolReceive protocolReceive;

        [JsonIgnore]
        public Type protocolSendType;

        [JsonIgnore]
        public IProtocolSend protocolSend;

        [JsonProperty]
        public string message;

        public Header() { }
        public Header(Header header)
        {
            eventType = header.eventType;
            length = header.length;
            refNum = header.refNum;
            _data = new List<byte>(header.Data);
            protocolRec = header.protocolRec;
            protocolReceive = header.protocolReceive;
            protocolSend = header.protocolSend;
            message = header.message;
        }

        public Header(IEnumerable<byte> data, bool copyData = false, params object[] values)
            : base(data) =>
            Deserialize(this, copyData, values);

        public Header(Packet packet, bool copyData = false, params object[] values) =>
            Deserialize(packet, copyData, values);

        public void Deserialize(Packet packet, params object[] values) =>
            Deserialize(packet, false, values);

        public void Deserialize(Packet packet, bool copyData = false, params object[] values)
        {
            eventType = (EventTypes)packet.ReadUInt32();
            length = packet.ReadUInt32();
            refNum = packet.ReadSInt32();

            if (copyData)
                _data = new List<byte>(packet.Data);
        }

        public byte[] Serialize(params object[] values)
        {
            var data = _data?.ToArray();

            if ((data?.Length ?? 0) < 1)
            {
                if (protocolSend != null)
                    data = protocolSend.Serialize(values);
                else if (protocolRec != null)
                    data = protocolRec.Serialize(values);
            }

            length = (uint)(data?.Length ?? 0);

            _data = new List<byte>();

            WriteInt32((uint)eventType);
            WriteInt32(length);
            WriteInt32(refNum);

            if (length > 0)
                WriteBytes(data);

            return GetData();
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                eventType = (EventTypes)jsonResponse?.eventType;
                length = jsonResponse?.length ?? 0;
                refNum = jsonResponse?.refNum ?? 0;
                message = jsonResponse?.message;
            }
        }

        public string SerializeJSON(params object[] values) =>
            JsonConvert.SerializeObject(this);

        public static int SizeOf => 12;
    }
}
