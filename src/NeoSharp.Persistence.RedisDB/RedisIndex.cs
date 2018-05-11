using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RedisDB
{
    /// <summary>
    /// Enumeration of redis indexes
    /// </summary>
    public enum RedisIndex
    {
        BlockHeight,
        BlockTimestamp,
        TransactionBlockHeight
    }
}
