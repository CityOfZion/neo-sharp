namespace NeoSharp.VM
{
    public enum SysCallResult : byte
    {
        /// <summary>
        /// Wrong execution
        /// </summary>
        False = 0,

        /// <summary>
        /// Successful execution
        /// </summary>
        True = 1,

        /// <summary>
        /// The syscall is not found
        /// </summary>
        NotFound = 10,

        /// <summary>
        /// There is not enough gas for compute this syscall
        /// </summary>
        OutOfGas = 11,
    }
}