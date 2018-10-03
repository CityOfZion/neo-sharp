using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Application.Controllers
{
    public class PromptBlockchainController : IPromptController
    {
        #region Private fields

        private readonly IBlockPool _blockPool;
        private readonly ITransactionPool _transactionPool;
        private readonly IBlockchain _blockchain;
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionModel;
        private readonly IAssetRepository _assetModel;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IConsoleHandler _consoleHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="blockRepository">The Block Model.</param>
        /// <param name="transactionModel"></param>
        /// <param name="assetModel"></param>
        /// <param name="blockchainContext">The block chain context class.</param>
        /// <param name="blockPool">Block pool</param>
        /// <param name="transactionPool">Transaction Pool</param>
        /// <param name="consoleWriter">Console writter</param>
        public PromptBlockchainController(
            IBlockchain blockchain, 
            IBlockRepository blockRepository,
            ITransactionRepository transactionModel,
            IAssetRepository assetModel,
            IBlockchainContext blockchainContext,
            IBlockPool blockPool, 
            ITransactionPool transactionPool, 
            IConsoleHandler consoleHandler)
        {
            _blockchain = blockchain;
            _blockRepository = blockRepository;
            _transactionModel = transactionModel;
            _assetModel = assetModel;
            _blockchainContext = blockchainContext;
            _blockPool = blockPool;
            _transactionPool = transactionPool;
            _consoleHandler = consoleHandler;
        }

        void WriteStatePercent(string title, string msg, long? value, long? max)
        {
            if (!value.HasValue || !max.HasValue)
            {
                _consoleHandler.WriteLine(title + ": " + msg + " ");
                return;
            }

            _consoleHandler.Write(title + ": " + msg + " ");

            using (var pg = _consoleHandler.CreatePercent(max.Value))
            {
                pg.Value = value.Value;
            }
        }

        private string FormatState(long? value)
        {
            return value.HasValue ? value.Value.ToString("###,###,###,###,##0") : "?";
        }

        /// <summary>
        /// Show state
        /// </summary>
        [PromptCommand("state", Category = "Blockchain", Help = "Show current state")]
        public void StateCommand()
        {
            var memStr = FormatState(_transactionPool.Size);
            var blockStr = FormatState(_blockPool.Size);
            var headStr = FormatState(this._blockchainContext.LastBlockHeader?.Index);
            var blStr = FormatState(this._blockchainContext.CurrentBlock?.Index);
            var blIndex = FormatState(0); // TODO #398: Change me

            var numSpaces = new int[] { memStr.Length, blockStr.Length, blIndex.Length, headStr.Length, blStr.Length }.Max() + 1;

            _consoleHandler.WriteLine("Pools", ConsoleOutputStyle.Information);
            _consoleHandler.WriteLine("");

            WriteStatePercent(" Memory", memStr.PadLeft(numSpaces, ' '), _transactionPool.Size, _transactionPool.Capacity);
            WriteStatePercent(" Blocks", blockStr.PadLeft(numSpaces, ' '), _blockPool.Size, _blockPool.Capacity);

            _consoleHandler.WriteLine("");
            _consoleHandler.WriteLine("Heights", ConsoleOutputStyle.Information);
            _consoleHandler.WriteLine("");

            _consoleHandler.WriteLine("Headers: " + headStr.PadLeft(numSpaces, ' ') + " ");

            WriteStatePercent(" Blocks", blStr.PadLeft(numSpaces, ' '), this._blockchainContext.CurrentBlock?.Index, this._blockchainContext.LastBlockHeader?.Index);
            WriteStatePercent("  Index", blIndex.PadLeft(numSpaces, ' '), 0, this._blockchainContext.CurrentBlock?.Index);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        public async Task HeaderCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._blockRepository.GetBlockHeader(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        public async Task HeaderCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._blockRepository.GetBlockHeader(blockHash), output);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        public async Task BlockCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._blockRepository.GetBlock(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        public async Task BlockCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._blockRepository.GetBlock(blockHash), output);
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        public async Task TxCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._transactionModel.GetTransaction(hash), output);
        }

        /// <summary>
        /// Get tx by block hash/ TxId
        /// </summary>
        /// <param name="blockIndex">Block Index</param>
        /// <param name="txNumber">TxNumber</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx by block num/tx number")]
        public async Task TxCommand(uint blockIndex, ushort txNumber, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var block = await this._blockRepository.GetBlock(blockIndex);
            _consoleHandler.WriteObject(block.Transactions?[txNumber], output);
        }

        /// <summary>
        /// Get asset by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 0)]
        public async Task AssetCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleHandler.WriteObject(await this._assetModel.GetAsset(hash), output);
        }

        /// <summary>
        /// Get asset by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 1)]
        public async Task AssetCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var assets = await this._assetModel.GetAssets();
            var result = assets.QueryResult(query, mode).ToArray();

            _consoleHandler.WriteObject(result, output);
        }

        /// <summary>
        /// Get contract by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 0)]
        public Task ContractCommand(UInt160 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            throw new NotImplementedException();
            //_consoleHandler.WriteObject(await _blockchain?.GetContract(hash), output);
        }

        /// <summary>
        /// Get contract by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 1)]
        public Task ContractCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            throw new NotImplementedException();
            //var contracts = await _blockchain?.GetContracts();
            //var result = contracts.QueryResult(query, mode).ToArray();

            //_consoleHandler.WriteObject(result, output);
        }
    }
}