using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Protocols.Assets
{
    [Description("sAst")]
    public sealed class MSG_ASSETSEND : AssetRec, IProtocolReceive, IProtocolSend
    {
        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
            }
        }

        public string SerializeJSON(params object[] values) => string.Empty;
    }
}
