using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class GetBlockHeadersMessage : GetBlocksMessage
    {
        public GetBlockHeadersMessage()
            : base(null)
        {
            Command = MessageCommand.getheaders;
        }

        public GetBlockHeadersMessage(UInt256 hashStart)
            : base(hashStart)
        {
            Command = MessageCommand.getheaders;
        }
    }
}