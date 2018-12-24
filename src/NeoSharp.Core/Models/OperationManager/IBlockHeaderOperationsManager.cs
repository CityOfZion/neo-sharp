namespace NeoSharp.Core.Models.OperationManager
{
    public interface IBlockHeaderOperationsManager : ISigner<BlockHeader>, IVerifier<BlockHeader>
    {
    }
}
