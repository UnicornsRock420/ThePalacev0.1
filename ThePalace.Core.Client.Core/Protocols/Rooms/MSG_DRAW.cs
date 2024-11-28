using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Protocols.Rooms
{
    [Description("draw")]
    public sealed class MSG_DRAW : DrawCmdRec, IProtocolReceive, IProtocolSend
    {
        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                type = jsonResponse.type;
                layer = jsonResponse.layer;
                data = ((string)jsonResponse.data ?? string.Empty).ReadPalaceString();
            }
        }

        public string SerializeJSON(params object[] values)
        {
            return JsonConvert.SerializeObject(new
            {
                type = type.ToString(),
                layer,
                data = data.WritePalaceString(),
            });
        }
    }
}
