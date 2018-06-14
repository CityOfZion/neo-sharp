namespace NeoSharp.Core.SmartContract
{
    /// <summary>
    /// Represents the smart contract parameter type
    /// </summary>
    public enum ContractParameterType : byte
    {
        /// <summary>
        /// Signature
        /// </summary>
        Signature = 0x00,
        Boolean = 0x01,
        /// <summary>
        /// Integer
        /// </summary>
        Integer = 0x02,
        /// <summary>
        /// Hash 160
        /// </summary>
        Hash160 = 0x03,
        /// <summary>
        /// Hash 256
        /// </summary>
        Hash256 = 0x04,
        /// <summary>
        /// Byte array
        /// </summary>
        ByteArray = 0x05,
        PublicKey = 0x06,
        String = 0x07,

        Array = 0x10,

        InteropInterface = 0xf0,

        Void = 0xff
    }
}