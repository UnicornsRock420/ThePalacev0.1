using System.Text.RegularExpressions;

namespace ThePalace.Core.Client.Core.Constants
{
    public static class ClientConstants
    {
        public static readonly Regex REGEX_WHITESPACE = new(@"[\s]+", RegexOptions.Singleline | RegexOptions.Compiled);
        public static readonly Regex REGEX_PALACEURL = new(@"^palace[:][/]{2}([\w\d\.\-]+)[:]{0,1}([\d]*)[/]{0,1}([\d]*)", RegexOptions.Singleline | RegexOptions.Compiled);

        public const string RegCodeSeed = @"CANADA";
        public const string ClientAgent = @"PC4237";
    }
}
