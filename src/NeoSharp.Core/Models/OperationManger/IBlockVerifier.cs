using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public interface IBlockVerifier
    {
        bool Verify(Block block);
    }
}