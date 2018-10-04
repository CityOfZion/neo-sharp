using System;
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
        private readonly IBlockProcessor _blockProcessor;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IGenesisBuilder _genesisBuilder;
        private readonly IBlockRepository _blockRepository;

        private int _initialized;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockProcessor">Block Processor</param>
        /// <param name="blockchainContext">Block chain context class.</param>
        /// <param name="genesisBuilder">Genesis block generator.</param>
        /// <param name="blockRepository">The block model.</param>
        public Blockchain(
            IBlockProcessor blockProcessor,
            IBlockchainContext blockchainContext,
            IGenesisBuilder genesisBuilder, 
            IBlockRepository blockRepository)
        {
            _blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _genesisBuilder = genesisBuilder ?? throw new ArgumentNullException(nameof(genesisBuilder));
            _blockRepository = blockRepository;
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

            _blockProcessor.Run();
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
