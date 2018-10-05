using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Builders
{
    public class BlockHeaderBuilder
    {
        private UInt256 _hash;
        private uint _index;
        private UInt256 _previousBlockHash;

        public BlockHeaderBuilder()
        {
            this._hash = UInt256.Zero;
            this._previousBlockHash = UInt256.Zero;
        }

        public BlockHeaderBuilder WithHash(UInt256 hash)
        {
            this._hash = hash;
            return this;
        }

        public BlockHeaderBuilder WithIndex(uint index)
        {
            this._index = index;
            return this;
        }

        public BlockHeaderBuilder WithPreviousBlockHash(UInt256 hash)
        {
            this._previousBlockHash = hash;
            return this;
        }

        public BlockHeader Build()
        {
            return new BlockHeader
            {
                Hash = this._hash,
                Index = this._index,
                PreviousBlockHash = this._previousBlockHash
            };
        }
    }
}
