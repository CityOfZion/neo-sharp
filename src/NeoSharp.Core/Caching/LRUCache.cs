using System;

namespace NeoSharp.Core.Caching
{
    internal abstract class LruCache<TKey, TValue> : Cache<TKey, TValue>
    {
        public LruCache(int maxCapacity)
            : base(maxCapacity)
        {
        }

        protected override void OnAccess(CacheItem item)
        {
            item.Time = DateTime.UtcNow;
        }
    }
}
