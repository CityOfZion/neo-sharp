using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockHeadersMessage : Message<HeadersPayload>
    {
        public BlockHeadersMessage(IEnumerable<BlockHeader> headers)
        {
            Command = MessageCommand.headers;
            Payload = new HeadersPayload { Headers = headers.ToArray() };
        }
    }

    public class HeadersPayload
    {
        [BinaryProperty(0)]
        public BlockHeader[] Headers;
    }
}