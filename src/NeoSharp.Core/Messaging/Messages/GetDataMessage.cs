using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class GetDataMessage : Message<GetDataPayload>
    {
        public GetDataMessage()
        {
            Command = MessageCommand.getdata;
            Payload = new GetDataPayload();
        }

        public GetDataMessage(InventoryType type, IEnumerable<UInt256> hashes)
        {
            Command = MessageCommand.getdata;
            Payload = new GetDataPayload { Type = type, Hashes = hashes.ToArray() };
        }
    }

    public class GetDataPayload : InventoryPayload { }
}