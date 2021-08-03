using System;

namespace StockSymbolsApi.Extensions
{
    public static class DateTimeConverter
    {
        private static readonly DateTime DatetimeMinTime =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixEpoch(long timestamp)
        {
            if (timestamp < 1)
            {
                return DateTime.MinValue;
            }
            return DatetimeMinTime.AddSeconds(timestamp);
        }

        public static long ToUnixEpoch(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - DatetimeMinTime.Ticks) / 10000;
        }
    }
}
