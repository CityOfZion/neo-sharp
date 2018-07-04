using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class BlockHeadersMessage : Message<HeadersPayload>
    {
        public BlockHeadersMessage()
        {
            Command = MessageCommand.headers;
            Payload = new HeadersPayload { Headers = new HeaderPayload[] { } };
        }

        public BlockHeadersMessage(IEnumerable<BlockHeader> headers)
        {
            Command = MessageCommand.headers;
            Payload = new HeadersPayload { Headers = headers.Select(u => new HeaderPayload() { Dummy = 0, Header = u }).ToArray() };
        }
    }

    public class HeaderPayload
    {
        #region Serializable data

        [BinaryProperty(0)]
        public BlockHeaderBase Header;

        // TODO: if (reader.ReadByte() != 0) throw new FormatException();

        [BinaryProperty(1)]
        public byte Dummy;

        #endregion
    }

    public class HeadersPayload
    {
        [BinaryProperty(0)]
        public HeaderPayload[] Headers;
    }
}