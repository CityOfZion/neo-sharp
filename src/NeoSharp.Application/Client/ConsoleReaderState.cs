namespace NeoSharp.Application.Client
{
    public enum ConsoleReaderState
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        
        /// <summary>
        /// Reading line
        /// </summary>
        Reading,
        
        /// <summary>
        /// Reading line (with characters)
        /// </summary>
        ReadingDirty,
        
        /// <summary>
        /// Reading password
        /// </summary>
        ReadingPassword,

        /// <summary>
        /// Reading password (with characters)
        /// </summary>
        ReadingPasswordDirty,
    }
}