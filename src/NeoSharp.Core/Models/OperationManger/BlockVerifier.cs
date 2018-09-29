using NeoSharp.Core.Blockchain.Repositories;

namespace NeoSharp.Core.Models.OperationManger
{
    public class BlockVerifier : IBlockVerifier
    {
        #region Private Fields
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly IBlockRepository _blockRepository;
        #endregion

        #region Constructor 
        public BlockVerifier(
            IWitnessOperationsManager witnessOperationsManager,
            IBlockRepository blockRepository)
        {
            this._witnessOperationsManager = witnessOperationsManager;
            this._blockRepository = blockRepository;
        }
        #endregion

        public bool Verify(Block block)
        {
            var prevHeader = this._blockRepository.GetBlockHeader(block.PreviousBlockHash).Result;

            if (prevHeader == null)
            {
                return false;
            }

            if (prevHeader.Index + 1 != block.Index)
            {
                return false;
            }

            if (prevHeader.Timestamp >= block.Timestamp)
            {
                return false;
            }

            if (!_witnessOperationsManager.Verify(block.Witness))
            {
                return false;
            }
            
            return true;
        }
    }
}
