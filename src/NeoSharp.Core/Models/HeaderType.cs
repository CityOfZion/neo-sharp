namespace NeoSharp.Core.Models
{
    public enum HeaderType : byte
    {
        /// <summary>
        /// Block unavailable, no hashes, no TX data
        /// </summary>
        Header = 0,
        /// <summary>
        /// Block available, with TX hashes
        /// </summary>
        Extended = 1,
    }
}
