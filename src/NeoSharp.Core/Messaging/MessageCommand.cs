namespace NeoSharp.Core.Messaging
{
    public enum MessageCommand : byte
    {
        notfound=0x00,

        addr = 0x01,
        alert = 0x02,
        block = 0x03,
        consensus = 0x04,
        filteradd = 0x05,
        filterclear = 0x06,
        filterload = 0x07,
        getaddr = 0x08,
        getblocks = 0x09,
        getdata = 0x0A,
        getheaders = 0x0B,
        headers = 0x0C,
        inv = 0x0D,
        mempool = 0x0E,
        merkleblock = 0x0F,
        ping = 0x10,
        pong = 0x11,
        reject = 0x12,
        tx = 0x13,
        verack = 0x14,
        version = 0x15,

        getdata_pri = 0x16,
        inv_pri = 0x17,
    }
}