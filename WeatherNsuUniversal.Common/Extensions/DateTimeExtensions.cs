using System;

namespace WeatherNsuUniversal.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private static DateTime _unixEpochZero = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime FromUnixEpochToDateTime(this long epoch)
        {
            var epochDate = _unixEpochZero.AddSeconds(epoch);
            return epochDate;
        }
    }
}
