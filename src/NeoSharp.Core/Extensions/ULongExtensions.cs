using System;

namespace NeoSharp.Core.Extensions
{
    public static class ULongExtensions
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this uint timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        public static DateTime ToDateTime(this ulong timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
        }
    }
}
