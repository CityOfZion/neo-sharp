using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public interface IBlockOperationsManager
    {
        void Sign(Block block);

        bool Verify(Block block);
    }
}