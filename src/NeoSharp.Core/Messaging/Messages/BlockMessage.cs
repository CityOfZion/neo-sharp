using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockMessage : Message<Block>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BlockMessage()
        {
            Command = MessageCommand.block;
            Payload = new Block();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public BlockMessage(Block payload)
        {
            Command = MessageCommand.block;
            Payload = payload;
        }
    }
}