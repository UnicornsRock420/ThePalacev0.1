using System;
using System.Diagnostics;
using System.IO;
using ThePalace.Core.Enums;

namespace ThePalace.Core.ExtensionMethods
{
    public static class UtilityExts
    {
        public static TReturn InvokePHP<TInput, TReturn>(this TInput value, PHPCommands command)
        {
            if (value == null) return default(TReturn);

            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Libraries");
            var filePath = null as string;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                dirPath = Path.Combine(dirPath, "Linux");
            }
            else
            {
                dirPath = Path.Combine(dirPath, "Win");
            }

            // Currently only x64 supported!
            if (true || Environment.Is64BitOperatingSystem)
            {
                dirPath = Path.Combine(dirPath, "x64");
            }
            else
            {
                dirPath = Path.Combine(dirPath, "x86");
            }

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                filePath = Path.Combine(dirPath, "php");
            }
            else
            {
                filePath = Path.Combine(dirPath, "php.exe");
            }

            if (File.Exists(filePath))
            {
                var b64 = null as string;
                if (value is byte[] _bytes)
                    b64 = _bytes.ToBase64();
                else if (value is string _str)
                    b64 = _str.ToBase64();
                if (b64 == null) return default(TReturn);

                using (var proc = new Process())
                {
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.FileName = filePath;
                    proc.StartInfo.WorkingDirectory = dirPath;
                    proc.StartInfo.RedirectStandardError = false;
                    proc.StartInfo.RedirectStandardOutput = true;

                    var cmd = command.ToString().ToLowerInvariant();
                    proc.StartInfo.Arguments = $"-r \"error_reporting(0);echo base64_encode({cmd}(base64_decode('{b64}')))\";";

                    try
                    {
                        proc.Start();

                        using (var hOutput = proc.StandardOutput)
                        {
                            proc.WaitForExit(5000);

                            if (proc.HasExited)
                            {
                                var type = typeof(TReturn);

                                if (type == ByteExts.Types.ByteArray)
                                    return (TReturn)(object)hOutput.ReadToEnd().FromBase64<byte[]>();
                                else if (type == StringExts.Types.String)
                                    return (TReturn)(object)hOutput.ReadToEnd().FromBase64<string>();
                            }
                        }
                    }
                    catch { }
                }
            }

            return default(TReturn);
        }
    }
}
