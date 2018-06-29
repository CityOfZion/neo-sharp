using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public interface IBlockHeaderContext
    {
        Task Add(BlockHeader blockHeader);

        Task<BlockHeader> GetBlockHeaderByHash(UInt256 blockHeaderHash);
    }
}
