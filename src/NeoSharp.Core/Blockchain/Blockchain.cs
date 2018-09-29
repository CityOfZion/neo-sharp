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

            _blockHeaderPersister.OnBlockHeadersPersisted += (_, blockHeaders) => this._blockchainContext.LastBlockHeader = blockHeaders.Last();
            _blockProcessor.OnBlockProcessed += (_, block) => this._blockchainContext.CurrentBlock = block;
        }

        public async Task InitializeBlockchain()
        {
            if (Interlocked.Exchange(ref _initialized, 1) != 0)
            {
                return;
            }

            var blockHeight = await this._blockRepository.GetTotalBlockHeight();
            var blockHeaderHeight = await this._blockRepository.GetTotalBlockHeaderHeight();

            this._blockchainContext.CurrentBlock = await this._blockRepository.GetBlock(blockHeight);
            this._blockchainContext.LastBlockHeader = await this._blockRepository.GetBlockHeader(blockHeaderHeight);

            this._blockHeaderPersister.LastBlockHeader = this._blockchainContext.LastBlockHeader;

            this._blockProcessor.Run(this._blockchainContext.CurrentBlock);
            if (this._blockchainContext.CurrentBlock == null || this._blockchainContext.LastBlockHeader == null)
            {
                await this._blockProcessor.AddBlock(this._genesisBuilder.Build());
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
