using NeoSharp.Core.Blockchain;

namespace NeoSharp.Core.Models.OperationManger
{
    public class BlockVerifier : IBlockVerifier
    {
        #region Private Fields
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly IBlockchain _blockchain;
        #endregion

        #region Constructor 
        public BlockVerifier(
            IWitnessOperationsManager witnessOperationsManager,
            IBlockchain blockchain)
        {
            _witnessOperationsManager = witnessOperationsManager;
            _blockchain = blockchain;
        }
        #endregion

        public bool Verify(Block block)
        {
            var prevHeader = _blockchain.GetBlockHeader(block.PreviousBlockHash).Result;

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
