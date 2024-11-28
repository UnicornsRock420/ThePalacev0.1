using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using ThePalace.Core.Interfaces;

namespace ThePalace.Core.Server.Protocols
{
    [Description("vers")]
    public class MSG_VERSION : IProtocolSend
    {
        public byte[] Serialize(params object[] args)
        {
            return null;
        }

        public string SerializeJSON(params object[] args)
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
