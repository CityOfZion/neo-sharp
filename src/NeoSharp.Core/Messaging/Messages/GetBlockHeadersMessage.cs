using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class GetBlockHeadersMessage : GetBlockHashesMessage
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