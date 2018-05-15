using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class InventoryMessage : Message<InventoryPayload>
    {
        public InventoryMessage(InventoryType type, IEnumerable<UInt256> hashes)
        {
            Command = MessageCommand.headers;
            Payload = new InventoryPayload { Type = type, Hashes = hashes.ToArray() };
        }
    }

    public class InventoryPayload
    {
        [BinaryProperty(0)]
        public InventoryType Type;

        [BinaryProperty(1)]
        public UInt256[] Hashes;
    }
}