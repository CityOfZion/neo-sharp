using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class InventoryMessage : Message<InventoryPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InventoryMessage()
        {
            Command = MessageCommand.inv;
            Payload = new InventoryPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public InventoryMessage(InventoryPayload payload)
        {
            Command = MessageCommand.inv;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="hashes">Hashes</param>
        public InventoryMessage(InventoryType type, IEnumerable<UInt256> hashes)
        {
            Command = MessageCommand.inv;
            Payload = new InventoryPayload { Type = type, Hashes = hashes.ToArray() };
        }
    }

    public class InventoryPayload
    {
        // The max is defined on https://github.com/neo-project/neo/blob/6d5cf6311d8e132f5edca7c24ab3df38b3224412/neo/IO/Helper.cs#L121

        public const int MaxHashes = 0x10000000;

        [BinaryProperty(0)]
        public InventoryType Type;

        [BinaryProperty(1, MaxLength = MaxHashes)]
        public UInt256[] Hashes;
    }
}