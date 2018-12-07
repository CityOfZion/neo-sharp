namespace NeoSharp.VM
{
    public enum ETriggerType : byte
    {
        /// <summary>
        /// The Verification trigger indicates that the contract is being invoked as a verification function. The verification function can accept multiple parameters, and should return a boolean value that indicates the validity of the transaction or block.
        /// </summary>
        Verification = 0x00,

        /// <summary>
        /// The Application trigger indicates that the contract is being invoked as an application function. The application function can accept multiple parameters, change the states of the blockchain, and return any type of value.
        /// </summary>
        Application = 0x10,
    }
}