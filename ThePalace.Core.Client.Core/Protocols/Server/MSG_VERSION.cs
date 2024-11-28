using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Reflection;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Client.Core.Protocols.Server
{
    [Description("vers")]
    public sealed class MSG_VERSION : IProtocolReceive, IProtocolSend
    {
        public int major;
        public int minor;
        public int revision;
        public int build;

        public void Deserialize(Packet packet, params object[] values) { }

        public void DeserializeJSON(string json)
        {
            var jsonResponse = JsonConvert.DeserializeObject<JObject>(json) as dynamic;
            if (jsonResponse != null)
            {
                major = jsonResponse.Major;
                minor = jsonResponse.Minor;
                revision = jsonResponse.Revision;
                build = jsonResponse.Build;
            }
        }
        public byte[] Serialize(params object[] values) => Array.Empty<byte>();

        public string SerializeJSON(params object[] values)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            return JsonConvert.SerializeObject(new
            {
                major = version.Major,
                minor = version.Minor,
                revision = version.Revision,
                build = version.Build,
            });
        }
    }
}
