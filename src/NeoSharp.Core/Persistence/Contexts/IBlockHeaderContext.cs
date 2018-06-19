using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public interface IBlockHeaderContext
    {
        Task Add(BlockHeader blockHeader);

        Task<BlockHeader> GetBlockHeaderByHash(UInt256 blockHash);

        // Is there any use case where this method is need???
        //Task<byte[]> GetRawBlockHeader(UInt256 blockHash);
    }
}
