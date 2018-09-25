using NeoSharp.Core.Models;

namespace NeoSharp.Core.Network
{
    public interface IBlockchainContext
    {
        Block CurrentBlock { get; set; }

        BlockHeader LastBlockHeader { get; set; }

        bool NeedPeerSync { get; }

        bool IsSyncing { get; set; }

        bool IsPeerConnected { get; }

        void SetPeerCurrentBlockIndex(uint index);
    }
}
