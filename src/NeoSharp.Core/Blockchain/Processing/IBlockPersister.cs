using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockPersister
    {
        Block LastPersistedBlock { get; }

        event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        Task Persist(params Block[] block);

        Task Persist(params BlockHeader[] blockHeaders);

        Task<bool> IsBlockPersisted(Block blocks);
    }
}
