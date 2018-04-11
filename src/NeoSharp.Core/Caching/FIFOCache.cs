using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Caching
{
    internal abstract class FIFOCache<TKey, TValue> : Cache<TKey, TValue>
    {
        public FIFOCache(int max_capacity)
            : base(max_capacity)
        {
        }

        protected override void OnAccess(CacheItem item)
        {
        }
    }
}
