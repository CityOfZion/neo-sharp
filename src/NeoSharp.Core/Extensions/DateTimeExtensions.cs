using System;

namespace NeoSharp.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static uint ToTimestamp(this DateTime time)
        {
            return (uint)(time.ToUniversalTime() - ULongExtensions.UnixEpoch).TotalSeconds;
        }
    }
}
