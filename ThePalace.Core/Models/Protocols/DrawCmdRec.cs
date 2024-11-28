using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Models.Protocols
{
    public class DrawCmdRec : IProtocolRec, IProtocolCommunications
    {
        [JsonIgnore]
        public short nextOfst;
        [JsonIgnore]
        //public short reserved;
        public short drawCmd;
        public ushort cmdLength;
        [JsonIgnore]
        public short dataOfst;
        public byte[] data;

        public DrawCmdTypes type
        {
            get => (DrawCmdTypes)(drawCmd & 0x00FF);
            set => drawCmd = (short)(drawCmd & 0xFF00 | (short)value & 0x00FF);
        }

        public bool layer
        {
            get => (drawCmd & 0x8000) != 0;
            set => drawCmd = (short)(drawCmd & 0x00FF | (value ? 0x8000 : 0x0000));
        }

        public bool filled
        {
            get => (drawCmd & 0x0100) != 0;
            set => drawCmd = (short)(drawCmd & 0x00FF | (value ? 0x0100 : 0x0000));
        }

        public short penSize;
        public byte red;
        public byte green;
        public byte blue;
        public Palace.Point pos;

        public List<Palace.Point> Points;
        public Rectangle Rect;
        public string text { get; set; }

        public DrawCmdRec() { }
        public DrawCmdRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose() { }

        public void Deserialize(Packet packet, params object[] values)
        {
            nextOfst = packet.ReadSInt16();
            packet.DropBytes(2); //reserved
            drawCmd = packet.ReadSInt16();
            cmdLength = packet.ReadUInt16();
            dataOfst = packet.ReadSInt16();
            data = packet.Data
                .Skip(dataOfst)
                .Take(cmdLength)
                .ToArray();
        }

        public void DeserializeData()
        {
            using (var packet = new Packet(data))
                switch (type)
                {
                    case DrawCmdTypes.DC_Path:
                        {
                            penSize = packet.ReadSInt16();
                            var nbrPoints = packet.ReadSInt16();
                            red = (byte)packet.ReadSInt16().SwapShort();
                            green = (byte)packet.ReadSInt16().SwapShort();
                            blue = (byte)packet.ReadSInt16().SwapShort();

                            pos = new();
                            pos.v = packet.ReadSInt16();
                            pos.h = packet.ReadSInt16();

                            Points = new();
                            while (Points.Count < nbrPoints &&
                                packet.Length >= Palace.Point.SizeOf)
                            {
                                var p = new Palace.Point();
                                p.v = packet.ReadSInt16();
                                p.h = packet.ReadSInt16();

                                Points.Add(p);
                            }
                        }

                        break;
                    case DrawCmdTypes.DC_Ellipse:
                        {
                            //penSize = packet.ReadSInt16();
                            //red = (byte)packet.ReadSInt16().SwapShort();
                            //green = (byte)packet.ReadSInt16().SwapShort();
                            //blue = (byte)packet.ReadSInt16().SwapShort();

                            //Rect = new();
                            //Rect.X = packet.ReadSInt16();
                            //Rect.Y = packet.ReadSInt16();
                            //Rect.Width = packet.ReadSInt16();
                            //Rect.Height = packet.ReadSInt16();

                            throw new NotImplementedException(nameof(DrawCmdTypes.DC_Ellipse));
                        }

                        break;
                    case DrawCmdTypes.DC_Text:
                        {
                            //penSize = packet.ReadSInt16();
                            //red = (byte)packet.ReadSInt16().SwapShort();
                            //green = (byte)packet.ReadSInt16().SwapShort();
                            //blue = (byte)packet.ReadSInt16().SwapShort();

                            //pos = new();
                            //pos.v = packet.ReadSInt16();
                            //pos.h = packet.ReadSInt16();

                            //text = packet.ReadPString(128, 1);

                            throw new NotImplementedException(nameof(DrawCmdTypes.DC_Text));
                        }

                        break;
                    case DrawCmdTypes.DC_Shape:
                        {
                            //penSize = packet.ReadSInt16();
                            //red = (byte)packet.ReadSInt16().SwapShort();
                            //green = (byte)packet.ReadSInt16().SwapShort();
                            //blue = (byte)packet.ReadSInt16().SwapShort();

                            //pos = new();
                            //pos.v = packet.ReadSInt16();
                            //pos.h = packet.ReadSInt16();

                            // TODO:

                            throw new NotImplementedException(nameof(DrawCmdTypes.DC_Shape));
                        }

                        break;
                }
        }

        public byte[] Serialize(params object[] values)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(nextOfst);
                packet.WriteInt16(0); //reserved
                packet.WriteInt16(drawCmd);
                packet.WriteInt16(cmdLength);
                packet.WriteInt16(dataOfst);
                packet.WriteBytes(data, cmdLength);

                return packet.GetData();
            }
        }

        public static int SizeOf => 10;
    }
}
