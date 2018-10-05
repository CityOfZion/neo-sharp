using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Blockchain.Processing.BlockHeaderProcessing
{
    public class BlockHeaderValidator : IBlockHeaderValidator
    {
        #region Private Fields 
        private readonly IBlockchainContext _blockchainContext;
        private readonly IGenesisBuilder _genesisBuilder;
        #endregion

        #region Constructor 
        public BlockHeaderValidator(IBlockchainContext blockchainContext, IGenesisBuilder genesisBuilder)
        {
            this._blockchainContext = blockchainContext;
            this._genesisBuilder = genesisBuilder;
        }
        #endregion

        #region IBlockHeaderValidator Implementation
        public bool IsValid(BlockHeader blockHeader)
        {
            if (_blockchainContext.LastBlockHeader != null)
            {
                if (_blockchainContext.LastBlockHeader.Index + 1 != blockHeader.Index ||
                    _blockchainContext.LastBlockHeader.Hash != blockHeader.PreviousBlockHash)
                {
                    return false;
                }
            }
            else
            {
                if (blockHeader.Index != 0 || blockHeader.Hash != _genesisBuilder.Build().Hash)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
