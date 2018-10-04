using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class BlockHeaderOperationsManager : IBlockHeaderOperationsManager
    {
        #region Private Fields
        private readonly Crypto _crypto;
        private readonly IBinarySerializer _binarySerializer;
        private readonly ISigner<Witness> _witnessSigner;
        #endregion

        #region Constructor 
        public BlockHeaderOperationsManager(
            Crypto crypto,
            IBinarySerializer binarySerializer,
            ISigner<Witness> witnessSigner)
        {
            _crypto = crypto;
            _binarySerializer = binarySerializer;
            _witnessSigner = witnessSigner;
        }
        #endregion

        #region IBlockHeaderOperationsManager implementation 
        public void Sign(BlockHeader blockHeader)
        {
            // Check if the BlockHeader is already signed.
            if (blockHeader.Hash != null && blockHeader.Hash != UInt256.Zero) return;       

            if (blockHeader.MerkleRoot == null)
            {
                // Compute hash
                blockHeader.MerkleRoot = MerkleTree.ComputeRoot(blockHeader.TransactionHashes);
            }

            var serializedBlockHeader = _binarySerializer.Serialize(blockHeader, new BinarySerializerSettings()
            {
                Filter = a => a != nameof(Witness) && 
                              a != nameof(Type) && 
                              a != nameof(blockHeader.TransactionHashes)
            });

            blockHeader.Hash = new UInt256(_crypto.Hash256(serializedBlockHeader));

            _witnessSigner.Sign(blockHeader.Witness);
        }

        public bool Verify(BlockHeader blockHeader)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}