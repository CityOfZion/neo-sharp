using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Blockchain
{
    public class Blockchain : IBlockchain, IDisposable
    {
        #region Private fields
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly IBlockProcessor _blockProcessor;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IGenesisBuilder _genesisBuilder;
        private readonly IBlockRepository _blockRepository;

        private int _initialized;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockHeaderPersister">Block Header Persister</param>
        /// <param name="blockProcessor">Block Processor</param>
        /// <param name="blockchainContext">Block chain context class.</param>
        /// <param name="genesisBuilder">Genesis block generator.</param>
        /// <param name="blockRepository">The block model.</param>
        public Blockchain(
            IBlockHeaderPersister blockHeaderPersister,
            IBlockProcessor blockProcessor,
            IBlockchainContext blockchainContext,
            IGenesisBuilder genesisBuilder, 
            IBlockRepository blockRepository)
        {
            _blockHeaderPersister = blockHeaderPersister ?? throw new ArgumentNullException(nameof(blockHeaderPersister));
            _blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _genesisBuilder = genesisBuilder ?? throw new ArgumentNullException(nameof(genesisBuilder));
            _blockRepository = blockRepository;

            _blockHeaderPersister.OnBlockHeadersPersisted += (_, blockHeaders) => _blockchainContext.LastBlockHeader = blockHeaders.Last();
            _blockProcessor.OnBlockProcessed += (_, block) => _blockchainContext.CurrentBlock = block;
        }

        public async Task InitializeBlockchain()
        {
            if (Interlocked.Exchange(ref _initialized, 1) != 0)
            {
                return;
            }

            var blockHeight = await _blockRepository.GetTotalBlockHeight();
            var blockHeaderHeight = await _blockRepository.GetTotalBlockHeaderHeight();

            _blockchainContext.CurrentBlock = await _blockRepository.GetBlock(blockHeight);
            _blockchainContext.LastBlockHeader = await _blockRepository.GetBlockHeader(blockHeaderHeight);

            _blockHeaderPersister.LastBlockHeader = _blockchainContext.LastBlockHeader;

            _blockProcessor.Run(_blockchainContext.CurrentBlock);
            if (_blockchainContext.CurrentBlock == null || _blockchainContext.LastBlockHeader == null)
            {
                await _blockProcessor.AddBlock(_genesisBuilder.Build());
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_initialized == 1)
            {
            }
        }
    }
}
