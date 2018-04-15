namespace NeoSharp.Core.Network
{
    /// <summary>
    /// Define the object type in the list
    /// </summary>
    public enum InventoryType : byte
    {
        Tx = 0x01,
        Block = 0x02,
        Consensus = 0xe0
    }
}
