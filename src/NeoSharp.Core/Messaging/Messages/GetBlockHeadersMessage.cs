using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class GetBlockHeadersMessage : Message<GetBlocksPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GetBlockHeadersMessage()
        {
            Command = MessageCommand.getheaders;
            Payload = new GetBlocksPayload
            {
                HashStart = new UInt256[] { }
            };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public GetBlockHeadersMessage(GetBlocksPayload payload)
        {
            Command = MessageCommand.getheaders;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hashStart">Hash start</param>
        public GetBlockHeadersMessage(UInt256 hashStart)
        {
            Command = MessageCommand.getheaders;
            Payload = new GetBlocksPayload
            {
                HashStart = hashStart == null ? new UInt256[] { } : new[] { hashStart }
            };
        }
    }
}