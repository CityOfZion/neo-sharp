using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public interface IBlockSigner
    {
        void Sign(Block block);
    }
}