namespace NeoSharp.VM
{
    public enum EStackItemType : byte
    {
        None = 0x00,

        Bool = 0x01,
        Integer = 0x02,
        ByteArray = 0x03,
        Interop = 0x04,

        Array = 0x05,
        Struct = 0x06,
        Map = 0x07
    }
}