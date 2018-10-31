using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockHeadersMessage : Message<BlockHeadersPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BlockHeadersMessage()
        {
            Command = MessageCommand.headers;
            Payload = new BlockHeadersPayload { Headers = new BlockHeader[0] };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public BlockHeadersMessage(BlockHeadersPayload payload)
        {
            Command = MessageCommand.headers;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headers">Headers</param>
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