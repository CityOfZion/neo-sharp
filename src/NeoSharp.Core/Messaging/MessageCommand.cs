using NeoSharp.Core.Caching;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Messaging
{
    public enum MessageCommand : byte
    {
        //notfound = 0x00,

        [ReflectionCache(typeof(AddrMessage))]
        addr = 0x01,
        //alert = 0x02,
        [ReflectionCache(typeof(BlockMessage))]
        block = 0x03,
        [ReflectionCache(typeof(ConsensusMessage))]
        consensus = 0x04,

        [ReflectionCache(typeof(FilterAddMessage))]
        filteradd = 0x05,
        [ReflectionCache(typeof(FilterClearMessage))]
        filterclear = 0x06,
        [ReflectionCache(typeof(FilterLoadMessage))]
        filterload = 0x07,

        [ReflectionCache(typeof(GetAddrMessage))]
        getaddr = 0x08,
        [ReflectionCache(typeof(GetBlocksMessage))]
        getblocks = 0x09,
        [ReflectionCache(typeof(GetDataMessage))]
        getdata = 0x0A,
        [ReflectionCache(typeof(GetBlockHeadersMessage))]
        getheaders = 0x0B,

        [ReflectionCache(typeof(BlockHeadersMessage))]
        headers = 0x0C,
        [ReflectionCache(typeof(InventoryMessage))]
        inv = 0x0D,
        [ReflectionCache(typeof(MemPoolMessage))]
        mempool = 0x0E,

        //merkleblock = 0x0F,
        //ping = 0x10,
        //pong = 0x11,
        //reject = 0x12,

        [ReflectionCache(typeof(TransactionMessage))]
        tx = 0x13,

        [ReflectionCache(typeof(VerAckMessage))]
        verack = 0x14,
        [ReflectionCache(typeof(VersionMessage))]
        version = 0x15,
    }
}