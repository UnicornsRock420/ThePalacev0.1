using System;
using System.Security.Cryptography;

namespace ThePalace.Core.Utility
{
    public static class RndGenerator
    {
        private static uint _kvTTL;
        private static Random _RndGenerator;
        private static RNGCryptoServiceProvider _RndSecureGenerator;
        private static DateTime _UpdateDate;

        private static void CheckTTL()
        {
            if (_kvTTL == 0 || DateTime.UtcNow.Subtract(_UpdateDate).Minutes > _kvTTL)
            {
                try
                {
                    _RndGenerator = new Random();
                    _RndSecureGenerator = new RNGCryptoServiceProvider();
                    _kvTTL = ConfigManager.GetValue<UInt32>("AppCacheTTL", 3, true).Value;
                    _UpdateDate = DateTime.UtcNow;
                }
                catch
                {
                }
            }
        }

        public static int Next(int minValue, int maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            return _RndGenerator.Next(minValue, maxValue);
        }

        public static int Next(int maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            return _RndGenerator.Next(maxValue);
        }

        public static int Next()
        {
            CheckTTL();

            return _RndGenerator.Next();
        }

        public static uint Next(uint minValue, uint maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            return (uint)_RndGenerator.Next((int)minValue, (int)maxValue);
        }

        public static uint Next(uint maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            return (uint)_RndGenerator.Next((int)maxValue);
        }

        public static uint NextSecure()
        {
            CheckTTL();

            var buffer = new byte[4];
            _RndSecureGenerator.GetBytes(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

        public static uint NextSecure(uint maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            var buffer = new byte[4];
            _RndSecureGenerator.GetBytes(buffer);

            return BitConverter.ToUInt32(buffer, 0) % maxValue;
        }

        public static uint NextSecure(uint minValue, uint maxValue)
        {
            if (maxValue < 1)
            {
                return 0;
            }

            CheckTTL();

            var buffer = new byte[4];
            _RndSecureGenerator.GetBytes(buffer);

            return (BitConverter.ToUInt32(buffer, 0) + minValue) % maxValue;
        }

        public static byte[] NextBytes(int size)
        {
            if (size < 1)
            {
                size = 1;
            }

            CheckTTL();

            var result = new byte[size];

            _RndGenerator.NextBytes(result);

            return result;
        }

        public static byte[] NextSecureBytes(int size, bool nonZeroBytesOnly = true)
        {
            if (size < 1)
            {
                size = 1;
            }

            CheckTTL();

            var result = new byte[size];

            if (nonZeroBytesOnly)
            {
                _RndSecureGenerator.GetNonZeroBytes(result);
            }
            else
            {
                _RndSecureGenerator.GetBytes(result);
            }

            return result;
        }

        public static double NextDouble()
        {
            CheckTTL();

            return _RndGenerator.NextDouble();
        }

        public static double NextSecureDouble()
        {
            CheckTTL();

            var buffer = new byte[8];
            _RndSecureGenerator.GetBytes(buffer);

            return BitConverter.ToDouble(buffer, 0);
        }
    }
}
