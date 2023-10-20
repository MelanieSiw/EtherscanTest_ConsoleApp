using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest_ConsoleApp.Extension
{
    public static class Extension
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static int FromHex(this string value)
        {
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                logger.Info(value);
            }
            ;
            return Convert.ToInt32(value, 16);
        }

        public static decimal FromHexDecimal(this string value)
        {
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                logger.Info(value);
            }
            ;
            return Convert.ToInt64(value, 16);
        }

        public static string ToHex(this int value)
        {
            return $"0x{value:X}";
        }
    }
}
