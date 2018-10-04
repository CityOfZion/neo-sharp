using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockPersister
    {
        Task Persist(params Block[] block);

        Task<IEnumerable<BlockHeader>> Persist(params BlockHeader[] blockHeaders);

        Task<bool> IsBlockPersisted(Block blocks);
    }
}
