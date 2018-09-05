using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processors
{
    public interface IBlockHeaderPersister
    {
        BlockHeader LastBlockHeader { get; set; }

        event EventHandler<IReadOnlyCollection<BlockHeader>> OnBlockHeadersPersisted;

        Task Persist(IReadOnlyCollection<BlockHeader> blockHeaders);
    }
}