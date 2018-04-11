using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Extensions
{
    public static class ULongExtensions
    {
        public static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this uint timestamp)
        {
            return unixEpoch.AddSeconds(timestamp).ToLocalTime();
        }

        public static DateTime ToDateTime(this ulong timestamp)
        {
            return unixEpoch.AddSeconds(timestamp).ToLocalTime();
        }
    }
}
