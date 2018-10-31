using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class GetDataMessage : Message<GetDataPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GetDataMessage()
        {
            Command = MessageCommand.getdata;
            Payload = new GetDataPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public GetDataMessage(GetDataPayload payload)
        {
            Command = MessageCommand.getdata;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="hashes">Hashes</param>
        public GetDataMessage(InventoryType type, IEnumerable<UInt256> hashes)
        {
            Command = MessageCommand.getdata;
            Payload = new GetDataPayload { Type = type, Hashes = hashes.ToArray() };
        }
    }

    public class GetDataPayload : InventoryPayload { }
}