using NeoSharp.Application.Attributes;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        public enum BlockOutputStyle { all, header };

        /// <summary>
        /// Show state
        /// </summary>
        [PromptCommand("state", Category = "Blockchain", Help = "Show current state")]
        private void StateCommand()
        {

        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="blockIndex">Index</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(ulong blockIndex, BlockOutputStyle mode = BlockOutputStyle.header)
        {
            // TODO: Change this

            if (mode == BlockOutputStyle.all)
            {
                var block = Blockchain.GenesisBlock;
                _consoleWriter.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            }
            else
            {
                var header = new BlockHeader();
                _consoleWriter.WriteLine(JsonConvert.SerializeObject(header, Formatting.Indented));
            }
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="blockHash">Hash</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(UInt256 blockHash, BlockOutputStyle mode = BlockOutputStyle.header)
        {
            // TODO: Change this

            if (mode == BlockOutputStyle.all)
            {
                var block = Blockchain.GenesisBlock;
                _consoleWriter.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            }
            else
            {
                var header = new BlockHeader();
                _consoleWriter.WriteLine(JsonConvert.SerializeObject(header, Formatting.Indented));
            }
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        private void TxCommand(UInt256 hash)
        {
            // TODO: Change this

            var tx = new Transaction();
            _consoleWriter.WriteLine(tx.ToJson(true));
        }

        /// <summary>
        /// Get tx by block hash/ TxId
        /// </summary>
        /// <param name="blockIndex">Block Index</param>
        /// <param name="txNumber">TxNumber</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx by block num/tx number")]
        private void TxCommand(ulong blockIndex, ushort txNumber)
        {
            // TODO: Change this

            //var block = Blockchain.GenesisBlock;
            //var tx = block.Transactions[txNumber];

            var tx = new Transaction();
            _consoleWriter.WriteLine(tx.ToJson(true));
        }
    }
}