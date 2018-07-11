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
            Payload = new HeadersPayload { Headers = new HeaderPayload[0] };
        }

        public BlockHeadersMessage(IEnumerable<BlockHeaderBase> headers)
        {
            Command = MessageCommand.headers;
            Payload = new HeadersPayload
            {
                Headers = headers.Select
                (
                    u =>
                    {
                        if (u.Type == BlockHeaderBase.HeaderType.Extended)
                        {
                            if (u is BlockHeader header)
                            {
                                // We need to send 

                                return new HeaderPayload() { Header = header.GetBlockHeaderBase() };
                            }
                            else if (u is Block block)
                            {
                                // We need to send 

                                return new HeaderPayload() { Header = block.GetBlockHeaderBase() };
                            }
                        }

                        return new HeaderPayload() { Header = u };
                    }
                )
                .ToArray()
            };
        }
    }

    public class HeaderPayload
    {
        #region Serializable data

        [BinaryProperty(0)]
        public BlockHeaderBase Header;

        // if (reader.ReadByte() != 0) throw new FormatException();

        [BinaryProperty(1)]
        public byte FakeTxSize = 0;

        #endregion
    }

    public class HeadersPayload
    {
        #region Serializable data

        [BinaryProperty(0)]
        public HeaderPayload[] Headers;

        #endregion
    }
}