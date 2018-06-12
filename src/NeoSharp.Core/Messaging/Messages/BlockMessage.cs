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
            Payload = new BlockPayload { Block = block };
        }
    }

    public class BlockPayload
    {
        [BinaryProperty(0)]
        public Block Block;
    }
}