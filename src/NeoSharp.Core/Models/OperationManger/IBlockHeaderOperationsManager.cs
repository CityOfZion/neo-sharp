namespace NeoSharp.Core.Models.OperationManger
{
    public interface IBlockHeaderOperationsManager
    {
        void Sign(BlockHeader blockHeader);

        bool Verify(BlockHeader blockHeader);
    }
}
