namespace NeoSharp.VM
{
    public enum EVMState : byte
    {
        /// <summary>
        /// Normal state 
        /// </summary>
        None = 0,
        /// <summary>
        /// Virtual machine stopped
        /// </summary>
        Halt = 1,
        /// <summary>
        /// Virtual machine execution with errors
        /// </summary>
        Fault = 2
    };
}