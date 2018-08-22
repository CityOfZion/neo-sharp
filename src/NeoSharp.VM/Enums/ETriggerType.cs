namespace NeoSharp.VM
{
    public enum ETriggerType : byte
    {
        /// <summary>
        /// The Verification trigger indicates that the contract is being invoked as a verification function. The verification function can accept multiple parameters, and should return a boolean value that indicates the validity of the transaction or block.
        /// </summary>
        Verification = 0x00,

        /// <summary>
        /// The VerificationR trigger indicates that the contract is being invoked as a verification function because it is specified as a target of an output of the transaction. The verification function accepts no parameter, and should return a boolean value that indicates the validity of the transaction.
        /// </summary>
        VerificationR = 0x01,

        /// <summary>
        /// The Application trigger indicates that the contract is being invoked as an application function. The application function can accept multiple parameters, change the states of the blockchain, and return any type of value.
        /// </summary>
        Application = 0x10,

        /// <summary>
        /// The ApplicationR trigger indicates that the default function received of the contract is being invoked because it is specified as a target of an output of the transaction. The received function accepts no parameter, changes the states of the blockchain, and returns any type of value.
        /// </summary>
        ApplicationR = 0x11
    }
}