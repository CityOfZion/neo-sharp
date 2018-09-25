using NeoSharp.Core.Models;

namespace NeoSharp.Core.Network
{
    public class BlockchainContext : IBlockchainContext
    {
        #region Private Fields 
        private uint _peerCurrentBlockIndex;
        #endregion

        #region IBlockchainContext implementation 
        public Block CurrentBlock { get; set; }

        public BlockHeader LastBlockHeader { get; set; }

        public bool NeedPeerSync
        {
            get
            {
                if (this.LastBlockHeader == null) return true;

                return this.LastBlockHeader.Index < this._peerCurrentBlockIndex;
            }
        }

        public bool IsPeerConnected => this._peerCurrentBlockIndex != 0;

        public bool IsSyncing { get; set; }

        public void SetPeerCurrentBlockIndex(uint index)
        {
            this._peerCurrentBlockIndex = index;
        }
        #endregion
    }
}