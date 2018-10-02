using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processing
{
    /// <inheritdoc />
    public class BlockHeaderPersister : IBlockHeaderPersister
    {
        private readonly IRepository _repository;
        private readonly ISigner<BlockHeader> _blockHeaderSigner;
        private readonly IGenesisBuilder _genesisBuilder;

        public BlockHeader LastBlockHeader { get; set; }

        public event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        public BlockHeaderPersister(
            IRepository repository,
            ISigner<BlockHeader> blockHeaderSigner,
            IGenesisBuilder genesisBuilder)
        {
            _repository = repository;
            _blockHeaderSigner = blockHeaderSigner;
            _genesisBuilder = genesisBuilder;
        }


		public async Task Update(BlockHeader blockHeader)
		{
			if(blockHeader == null) throw new ArgumentNullException(nameof(blockHeader));

			await _repository.AddBlockHeader(blockHeader);
		}

		public async Task Persist(params BlockHeader[] blockHeaders)
        {
            if (blockHeaders == null) throw new ArgumentNullException(nameof(blockHeaders));

            var blockHeadersToPersist = new List<BlockHeader>();

            if (LastBlockHeader == null)
            {
                // Persisting the Genesis block
                blockHeadersToPersist = blockHeaders.ToList();
            }
            else
            {
                blockHeadersToPersist = blockHeaders
                    .Where(bh => bh != null && bh.Index > LastBlockHeader?.Index)
                    .Distinct(bh => bh.Index)
                    .OrderBy(bh => bh.Index)
                    .ToList();
            }

            foreach (var blockHeader in blockHeadersToPersist.ToArray())
            {
                if (blockHeader.Hash == null)
                {
                    _blockHeaderSigner.Sign(blockHeader);
                }

                if (!Validate(blockHeader))
                {
                    blockHeadersToPersist.Remove(blockHeader);
                    break;
                }

                await _repository.AddBlockHeader(blockHeader);

                LastBlockHeader = blockHeader;

                await _repository.SetTotalBlockHeaderHeight(LastBlockHeader.Index);
            }

            if (blockHeadersToPersist.Count != 0)
            {
                OnBlockHeadersPersisted?.Invoke(this, blockHeadersToPersist.ToArray());
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
                if (blockHeader.Index != 0 || blockHeader.Hash != _genesisBuilder.Build().Hash)
                {
                    return false;
                }
            }

            return true;
        }
    }
}