using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        void WriteStatePercent(string title, string msg, long value, long max)
        {
            _consoleWriter.Write(title + ": " + msg + " ");

            using (var pg = _consoleWriter.CreatePercent(max))
            {
                pg.Value = value;
            }
        }

        /// <summary>
        /// Show state
        /// </summary>
        [PromptCommand("state", Category = "Blockchain", Help = "Show current state")]
        private void StateCommand()
        {
            var memStr = _blockchain.MemoryPool.Count.ToString("###,###,###,###,##0");
            var blockStr = _blockProcessor.BlockPoolSize.ToString("###,###,###,###,##0");

            var headStr = _blockchain.LastBlockHeader.Index.ToString("###,###,###,###,##0");
            var blStr = _blockchain.CurrentBlock.Index.ToString("###,###,###,###,##0");
            var blIndex = 0.ToString("###,###,###,###,##0"); // TODO: Change me

            var numSpaces = new int[] { memStr.Length, blockStr.Length, blIndex.Length, headStr.Length, blStr.Length }.Max() + 1;

            _consoleWriter.WriteLine("Pools", ConsoleOutputStyle.Information);
            _consoleWriter.WriteLine("");

            WriteStatePercent(" Memory", memStr.PadLeft(numSpaces, ' '), _blockchain.MemoryPool.Count, _blockchain.MemoryPool.Capacity);
            WriteStatePercent(" Blocks", blockStr.PadLeft(numSpaces, ' '), _blockProcessor.BlockPoolSize, _blockProcessor.BlockPoolCapacity);

            _consoleWriter.WriteLine("");
            _consoleWriter.WriteLine("Heights", ConsoleOutputStyle.Information);
            _consoleWriter.WriteLine("");

            _consoleWriter.WriteLine("Headers: " + headStr.PadLeft(numSpaces, ' ') + " ");

            WriteStatePercent(" Blocks", blStr.PadLeft(numSpaces, ' '), _blockchain.CurrentBlock.Index, _blockchain.LastBlockHeader.Index);
            WriteStatePercent("  Index", blIndex.PadLeft(numSpaces, ' '), 0, _blockchain.CurrentBlock.Index);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        private async Task HeaderCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetBlockHeader(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        private async Task HeaderCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetBlockHeader(blockHash), output);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private async Task BlockCommand(uint blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetBlock(blockIndex), output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private async Task BlockCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetBlock(blockHash), output);
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        private async Task TxCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetTransaction(hash), output);
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
            var block = await _blockchain.GetBlock(blockIndex);
            _consoleWriter.WriteObject(block.Transactions?[txNumber], output);
        }

        /// <summary>
        /// Get asset by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private async Task AssetCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetAsset(hash), output);
        }

        /// <summary>
        /// Get asset by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private async Task AssetCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var assets = await _blockchain?.GetAssets();
            var result = assets.QueryResult(query, mode).ToArray();

            _consoleWriter.WriteObject(result, output);
        }

        /// <summary>
        /// Get contract by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private async Task ContractCommand(UInt160 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            _consoleWriter.WriteObject(await _blockchain?.GetContract(hash), output);
        }

        /// <summary>
        /// Get contract by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="mode">Regex/Contains</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private async Task ContractCommand(string query, EnumerableExtensions.QueryMode mode = EnumerableExtensions.QueryMode.Contains, PromptOutputStyle output = PromptOutputStyle.json)
        {
            var contracts = await _blockchain?.GetContracts();
            var result = contracts.QueryResult(query, mode).ToArray();

            _consoleWriter.WriteObject(result, output);
        }
    }
}