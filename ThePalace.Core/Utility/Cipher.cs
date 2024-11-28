using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ThePalace.Core.Constants;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.ExtensionMethods;

namespace ThePalace.Core.Utility
{
    public static class Cipher
    {
        private static readonly byte[] gEncryptTable = new byte[512];

        private const uint R_A = 16807;
        private const uint R_M = 2147483647;
        private const uint R_Q = 127773;
        private const uint R_R = 2836;

        public static void InitializeTable()
        {
            var gSeed = 666666L;

            Func<double> LongRandom = () =>
            {
                var hi = gSeed / R_Q;
                var lo = gSeed % R_Q;
                var test = R_A * lo - R_R * hi;

                if (test > 0)
                {
                    gSeed = test;
                }
                else
                {
                    gSeed = test + R_M;
                }

                return gSeed / (double)R_M;
            };

            Func<short, byte> MyRandom = (max) =>
            {
                return (byte)(LongRandom() * max);
            };

            for (var j = 0; j < gEncryptTable.Length; j++)
            {
                gEncryptTable[j] = MyRandom(256);
            }
        }

        public static byte[] EncryptString(this string source)
        {
            var inStr = source.GetBytes();
            var outStr = new byte[inStr.Length];

            int rc = 0;
            byte lastChar = 0;

            for (var i = inStr.Length - 1; i >= 0; --i)
            {
                outStr[i] = (byte)(inStr[i] ^ gEncryptTable[rc++] ^ lastChar);
                lastChar = (byte)(outStr[i] ^ gEncryptTable[rc++]);
            }

            return outStr;
        }

        public static string DecryptString(this byte[] inStr)
        {
            var outStr = new byte[inStr.Length];

            int rc = 0;
            byte lastChar = 0;

            for (var i = inStr.Length - 1; i >= 0; --i)
            {
                outStr[i] = (byte)(inStr[i] ^ gEncryptTable[rc++] ^ lastChar);
                lastChar = (byte)(inStr[i] ^ gEncryptTable[rc++]);
            }

            return outStr.GetString();
        }

        public static int GetSeedFromReg(uint counter, uint crc)
        {
            return (int)(counter ^ RegConstants.MAGIC_LONG ^ crc);
        }

        public static int GetSeedFromPUID(uint counter, uint crc)
        {
            return (int)(counter ^ crc);
        }

        public static uint ComputeLicenseCrc(uint seed)
        {
            var ptr = seed.SwapInt32().WriteInt32();

            return ComputeCrc(ptr);
        }

        private static Func<uint, uint, uint> getCrc = (crc, ptr) =>
        {
            return (crc << 1 | (uint)((crc & 0x80000000) != 0 ? 1 : 0)) ^ ptr;
        };
        public static uint ComputeCrc(byte[] ptr, uint offset = 0, bool isAsset = false)
        {
            var len = ptr.Length - offset;
            var crc = (UInt32)0;
            var j = offset;

            if (isAsset)
            {
                crc = AssetConstants.CRC_MAGIC;
            }
            else
            {
                crc = RegConstants.CRC_MAGIC;
            }

            while (len-- > 0)
            {
                if (isAsset)
                {
                    crc = getCrc(crc, ptr[j++]);
                }
                else
                {
                    crc = getCrc(crc, CrcMagic.gCrcMask[ptr[j++]]);
                }
            }

            return crc;
        }

        public static bool ValidUserSerialNumber(uint crc, uint counter)
        {
            var seed = counter ^ RegConstants.MAGIC_LONG ^ crc;
            return crc == ComputeLicenseCrc(seed);
        }

        public static byte[] ReadPalaceString(this string source)
        {
            var srcBytes = source.GetBytes();
            var destBytes = new List<byte>();

            for (var j = 0; j < srcBytes.Length; j++)
            {
                if (srcBytes[j] == (byte)'\\')
                {
                    var byte1 = (char)srcBytes[++j];
                    var byte2 = (char)srcBytes[++j];
                    var hex = $"0x{byte1}{byte2}";

                    destBytes.Add(Convert.ToByte(hex, 16));
                }
                else
                {
                    destBytes.Add(srcBytes[j]);
                }
            }

            return destBytes.ToArray();
        }

        public static string WritePalaceString(this byte[] source)
        {
            var dest = new StringBuilder();

            for (var j = 0; j < source.Length; j++)
            {
                if (Regex.IsMatch($"{(char)source[j]}", @"[a-z0-9]", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                {
                    dest.Append(source[j]);
                }
                else
                {
                    dest.AppendFormat(@"\{0:X2}", source[j]);
                }
            }

            return dest.ToString();
        }

        public static string RegRectoSeed(RegistrationRec regRec, bool puid = false)
        {
            var seedCounter = (UInt32)0;

            if (puid)
            {
                seedCounter = regRec.puidCtr ^ regRec.puidCRC;
            }
            else
            {
                seedCounter = regRec.counter ^ RegConstants.MAGIC_LONG ^ regRec.crc;
            }

            return SeedToWizKey(seedCounter, puid);
        }

        public static string SeedToWizKey(UInt32 seedCounter, bool puid = false)
        {
            var sb = new StringBuilder();

            sb.Append('{');

            if (puid)
            {
                sb.Append('Z');
            }

            while (seedCounter > 0)
            {
                sb.Append((char)(((byte)'A') + ((seedCounter % 13) ^ 4)));
                seedCounter /= 13;
            }

            sb.Append('}');

            return sb.ToString();
        }

        public static Int32 WizKeytoSeed(string wizKey)
        {
            var str = wizKey.GetBytes();
            Int32 ctr = 0, mag = 1;

            for (var j = 0; j < str.Length; j++)
            {
                if (str[j] == (byte)'{' || str[j] == (byte)'Z') continue;
                else if (str[j] == (byte)'}') break;
                else if (str[j] < (byte)'A' || str[j] > (byte)'Q')
                    return -1;

                ctr += ((str[j] - (byte)'A') ^ 4) * mag;
                mag *= 13;
            }

            return ctr;
        }
    }
}
