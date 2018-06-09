namespace NeoSharp.VM
{
    public enum ELogStackOperation : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Pop item to stack
        /// </summary>
        Pop = 0x01,
        /// <summary>
        /// Drop item from stack
        /// </summary>
        Drop = 0x02,
        /// <summary>
        /// Push item to stack
        /// </summary>
        Push = 0x03,
        /// <summary>
        /// Peek item from stack
        /// </summary>
        Peek = 0x04,
        /// <summary>
        /// TryPeek item from stack
        /// </summary>
        TryPeek = 0x05,
        /// <summary>
        /// Remove item from stack
        /// </summary>
        Remove = 0x06,
        /// <summary>
        /// Insert item to stack
        /// </summary>
        Insert = 0x07
    }
}