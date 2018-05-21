namespace NeoSharp.Core.Types
{
    /// <summary>
    /// Define the behavior in case of exceeding the maximum
    /// </summary>
    public enum PoolMaxBehaviour : byte
    {
        /// <summary>
        /// Don't allow more items
        /// </summary>
        DontAllowMore,
        /// <summary>
        /// Remove from the end
        /// </summary>
        RemoveFromEnd
    }
}