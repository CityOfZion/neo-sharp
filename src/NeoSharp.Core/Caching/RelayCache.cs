using NeoSharp.Core.Types;
using NeoSharp.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Caching
{
    internal class RelayCache : FIFOCache<UInt256, IInventory>
    {
        public RelayCache(int max_capacity)
            : base(max_capacity)
        {
        }

        protected override UInt256 GetKeyForItem(IInventory item)
        {
            return item.Hash;
        }
    }
}
