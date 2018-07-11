using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockMessage : Message<Block>
    {
        public BlockMessage()
        {
            Command = MessageCommand.block;
            Payload = new Block();
        }

        public BlockMessage(Block block)
        {
            Command = MessageCommand.block;
            Payload = block;
        }
    }
}