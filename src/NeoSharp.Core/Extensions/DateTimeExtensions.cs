using System;

namespace NeoSharp.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts Datetime to UInt
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Timestamp</returns>
        public static uint ToTimestamp(this DateTime time)
        {
            return (uint)(time.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        /// <summary>
        /// Converts uint to Datetime
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(this uint timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToUniversalTime();
        }

        /// <summary>
        /// Converts ulong to Datetime
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(this ulong timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToUniversalTime();
        }
    }
}