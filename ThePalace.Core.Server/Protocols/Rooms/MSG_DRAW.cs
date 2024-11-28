using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Protocols
{
    [Description("draw")]
    public class MSG_DRAW : IProtocolReceive, IProtocolSend
    {
        public DrawCmdRec command;

        public void Deserialize(Packet packet, params object[] args)
        {
            command = new DrawCmdRec();
            command.nextOfst = packet.ReadSInt16();
            packet.DropBytes(2); //reserved
            command.drawCmd = packet.ReadSInt16();
            command.cmdLength = packet.ReadUInt16();
            command.dataOfst = packet.ReadSInt16();
            command.data = packet.GetData(command.cmdLength, 0, true);
        }

        public byte[] Serialize(params object[] args)
        {
            using (var packet = new Packet())
            {
                packet.WriteInt16(command.nextOfst);
                packet.WriteInt16(0); //reserved
                packet.WriteInt16(command.drawCmd);
                packet.WriteInt16(command.cmdLength);
                packet.WriteInt16(command.dataOfst);
                packet.WriteBytes(command.data);

                return packet.GetData();
            }
        }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<JObject>(json);

                command = new DrawCmdRec
                {
                    type = jsonResponse.type,
                    layer = jsonResponse.layer,
                    data = ((string)jsonResponse.data ?? string.Empty).ReadPalaceString(),
                };
            }
            catch
            {
            }
        }

        public string SerializeJSON(params object[] args)
        {
            return JsonConvert.SerializeObject(new
            {
                type = command.type.ToString(),
                layer = command.layer,
                data = command.data.WritePalaceString(),
            });
        }
    }
}
