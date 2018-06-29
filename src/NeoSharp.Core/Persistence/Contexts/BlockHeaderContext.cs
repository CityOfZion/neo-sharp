using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public class BlockHeaderContext : IBlockHeaderContext
    {

        #region Private Fields 
        private readonly IDbModel _model;
        #endregion

        #region Constructor 
        public BlockHeaderContext(IDbModel model)
        {
            _model = model;
        }
        #endregion

        #region IBlockHeaderContext implementation
        public Task Add(BlockHeader blockHeader)
        {
            return _model.Create(DataEntryPrefix.DataBlock, blockHeader.Hash, blockHeader);
        }

        public Task<BlockHeader> GetBlockHeaderByHash(UInt256 blockHeaderHash)
        {
            return _model.Get<BlockHeader>(DataEntryPrefix.DataBlock, blockHeaderHash);
        }

        #endregion
    }
}
