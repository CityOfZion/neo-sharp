using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processing
{
    /// <inheritdoc />
    public class BlockHeaderPersister : IBlockHeaderPersister
    {
        private readonly IRepository _repository;

        public BlockHeader LastBlockHeader { get; set; }

        public event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        public BlockHeaderPersister(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Persist(params BlockHeader[] blockHeaders)
        {
            if (blockHeaders == null) throw new ArgumentNullException(nameof(blockHeaders));

            var blockHeadersToPersist = blockHeaders
                .Where(bh => bh != null && bh.Index > LastBlockHeader?.Index)
                .Distinct(bh => bh.Index)
                .OrderBy(bh => bh.Index)
                .ToList();

            foreach (var blockHeader in blockHeadersToPersist)
            {
                if (blockHeader.Hash == null)
                {
                    blockHeader.UpdateHash();
                }

                if (!Validate(blockHeader)) break;

                await _repository.AddBlockHeader(blockHeader);

                LastBlockHeader = blockHeader;

                await _repository.SetTotalBlockHeaderHeight(LastBlockHeader.Index);
            }

            var persistedBlockHeaders = blockHeadersToPersist
                .TakeWhile(bh => bh.Index <= LastBlockHeader?.Index)
                .ToArray();

            if (persistedBlockHeaders.Length != 0)
            {
                OnBlockHeadersPersisted?.Invoke(this, persistedBlockHeaders);
            }
        }

        private bool Validate(BlockHeader blockHeader)
        {
            if (LastBlockHeader != null)
            {
                if (LastBlockHeader.Index + 1 != blockHeader.Index ||
                    LastBlockHeader.Hash != blockHeader.PreviousBlockHash)
                {
                    return false;
                }
            }
            else
            {
                if (blockHeader.Index != 0 ||
                    blockHeader.Hash != Genesis.GenesisBlock.Hash)
                {
                    return false;
                }
            }

            return blockHeader.Type == BlockHeader.HeaderType.Extended;
        }
    }
}