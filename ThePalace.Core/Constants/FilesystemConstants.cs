using System.Text.RegularExpressions;

namespace ThePalace.Core.Constants
{
    public static class FilesystemConstants
    {
        public static readonly Regex REGEX_FILESYSTEMCHARS = new Regex(@"[\/:*?""<>|]+", RegexOptions.Singleline | RegexOptions.Compiled);
    }
}
