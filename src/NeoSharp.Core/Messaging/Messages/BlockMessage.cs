using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockMessage : Message<BlockPayload>
    {
        public BlockMessage()
        {
            Command = MessageCommand.block;
            Payload = new BlockPayload();
        }

        public BlockMessage(Block block)
        {
            Command = MessageCommand.block;
            Payload = new BlockPayload { Blocks = new[] { block } };
        }

        public BlockMessage(IEnumerable<Block> blocks)
        {
            Command = MessageCommand.block;
            Payload = new BlockPayload { Blocks = blocks.ToArray() };
        }
    }

    public class BlockPayload
    {
        [BinaryProperty(0)]
        public Block[] Blocks;
    }
}