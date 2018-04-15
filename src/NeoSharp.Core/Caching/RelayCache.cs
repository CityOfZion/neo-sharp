using NeoSharp.Core.Types;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Caching
{
    internal class RelayCache : FifoCache<UInt256, IInventory>
    {
        public RelayCache(int maxCapacity)
            : base(maxCapacity)
        {
        }

        protected override UInt256 GetKeyForItem(IInventory item)
        {
            return item.Hash;
        }
    }
}
