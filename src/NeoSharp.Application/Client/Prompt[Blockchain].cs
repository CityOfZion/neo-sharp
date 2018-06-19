using NeoSharp.Application.Attributes;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Models;
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
        private void HeaderCommand(ulong blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var header = new BlockHeader();
            WriteObject(header, output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("header", Category = "Blockchain", Help = "Get header by index or by hash")]
        private void HeaderCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var header = new BlockHeader();
            WriteObject(header, output);
        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(ulong blockIndex, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var block = Blockchain.GenesisBlock;
            WriteObject(block, output);
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(UInt256 blockHash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var block = Blockchain.GenesisBlock;
            WriteObject(block, output);
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        private void TxCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var tx = new InvocationTransaction();
            WriteObject(tx, output);
        }

        /// <summary>
        /// Get tx by block hash/ TxId
        /// </summary>
        /// <param name="blockIndex">Block Index</param>
        /// <param name="txNumber">TxNumber</param>
        /// <param name="output">Output</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx by block num/tx number")]
        private void TxCommand(ulong blockIndex, ushort txNumber, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            //var block = Blockchain.GenesisBlock;
            //var tx = block.Transactions[txNumber];

            var tx = new InvocationTransaction();
            WriteObject(tx, output);
        }

        /// <summary>
        /// Get asset by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private void AssetCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var tx = new Asset();
            WriteObject(tx, output);
        }

        /// <summary>
        /// Get asset by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="output">Output</param>
        [PromptCommand("asset", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private void AssetCommand(string query, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var tx = new Asset();
            WriteObject(tx, output);
        }

        /// <summary>
        /// Get contract by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 0)]
        private void ContractCommand(UInt256 hash, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var tx = new Contract();
            WriteObject(tx, output);
        }

        /// <summary>
        /// Get contract by query
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="output">Output</param>
        [PromptCommand("contract", Category = "Blockchain", Help = "Get asset", Order = 1)]
        private void ContractCommand(string query, PromptOutputStyle output = PromptOutputStyle.json)
        {
            // TODO: Change this

            var tx = new Contract();
            WriteObject(tx, output);
        }
    }
}