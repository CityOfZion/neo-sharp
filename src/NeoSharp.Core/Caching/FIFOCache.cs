namespace NeoSharp.Core.Caching
{
    internal abstract class FifoCache<TKey, TValue> : Cache<TKey, TValue>
    {
        public FifoCache(int maxCapacity)
            : base(maxCapacity)
        {
        }

        protected override void OnAccess(CacheItem item)
        {
        }
    }
}
