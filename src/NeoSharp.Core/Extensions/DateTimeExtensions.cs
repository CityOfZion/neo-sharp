using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static uint ToTimestamp(this DateTime time)
        {
            return (uint)(time.ToUniversalTime() - ULongExtensions.unixEpoch).TotalSeconds;
        }
    }
}
