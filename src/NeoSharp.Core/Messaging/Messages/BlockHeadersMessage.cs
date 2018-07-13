using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockHeadersMessage : Message<BlockHeadersPayload>
    {
        public BlockHeadersMessage()
        {
            Command = MessageCommand.headers;
            Payload = new BlockHeadersPayload { Headers = new BlockHeader[0] };
        }

        public BlockHeadersMessage(IEnumerable<BlockHeader> headers)
        {
            Command = MessageCommand.headers;
            Payload = new BlockHeadersPayload
            {
                Headers = headers.Select
                (
                    u =>
                    {
                        // We need to ensure that is sent without TX 

                        return u.Trim();
                    }
                )
                .ToArray()
            };
        }
    }

    public class BlockHeadersPayload
    {
        [BinaryProperty(0)]
        public BlockHeader[] Headers;
    }
}