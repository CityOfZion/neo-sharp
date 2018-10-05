using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing.BlockHeaderProcessing
{
    public interface IBlockHeaderValidator
    {
        bool IsValid(BlockHeader blockHeader);
    }
}
