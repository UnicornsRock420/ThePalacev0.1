using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Models.Protocols
{
    public class HotspotRec : IProtocolRec
    {
        [JsonIgnore]
        public int scriptEventMask;
        public int flags;
        //[JsonIgnore]
        //public int secureInfo;
        //[JsonIgnore]
        //public int refCon;
        public Point loc;
        public short id;
        public short dest;
        [JsonIgnore]
        public short nbrPts;
        [JsonIgnore]
        public short ptsOfst;
        public HotspotTypes type;
        //[JsonIgnore]
        //public short groupID;
        //[JsonIgnore]
        //public short nbrScripts;
        //[JsonIgnore]
        //public short scriptRecOfst;
        public short state;
        [JsonIgnore]
        public short nbrStates;
        [JsonIgnore]
        public short stateRecOfst;
        [JsonIgnore]
        public short nameOfst;
        [JsonIgnore]
        public short scriptTextOfst;
        //[JsonIgnore]
        //public short alignReserved;

        public string name;
        public string script;

        public List<HotspotStateRec> States;
        public List<Point> Vortexes;

        public HotspotRec() { }
        public HotspotRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
        }

        public byte[] Serialize(params object[] values) => Array.Empty<byte>();

        public static int SizeOf => 48;
    }
}
