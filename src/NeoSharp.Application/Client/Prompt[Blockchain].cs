using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        /// <summary>
        /// Show state
        /// </summary>
        [PromptCommand("state", Category = "Blockchain", Help = "Show current state")]
        private void StateCommand()
        {
            _consoleWriter.WriteLine("Memory pool:");

            using (var pg = _consoleWriter.CreatePercent(_blockchain.MemoryPool.Max))
            {
                pg.Value = _blockchain.MemoryPool.Count;
            }
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        private void HeaderCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetBlockHeader(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        private void HeaderCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetBlockHeader(blockHash), output);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetBlock(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetBlock(blockHash), output);
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        private void TxCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetTransaction(hash), output);
        }

        /// <summary>
        /// Get tx by block hash/ TxId
        /// </summary>
        /// <param name="blockIndex">Block Index</param>
        /// <param name="txNumber">TxNumber</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx by block num/tx number")]
        private async Task TxCommand(uint blockIndex, ushort txNumber, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var block = await this._blockchain.GetBlock(blockIndex);
            this._consoleWriter.WriteObject(block.Transactions?[txNumber], output);
        }

        /// <summary>
        /// Get asset by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private void AssetCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetAsset(hash), output);
        }

        /// <summary>
        /// Get asset by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private void AssetCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var result = _blockchain?.GetAssets().QueryResult(query, mode).ToArray();

            _consoleWriter.WriteObject(result, output);
        }

        /// <summary>
        /// Get contract by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private void ContractCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(_blockchain?.GetContract(hash), output);
        }

        /// <summary>
        /// Get contract by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private void ContractCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var result = _blockchain?.GetContracts().QueryResult(query, mode).ToArray();

            _consoleWriter.WriteObject(result, output);
        }
    }
}