using System;
namespace NeoSharp.Core.Wallet.Wrappers
{
    public interface IFileWrapper
    {

        /// <summary>
        /// Load the specified file into a string.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="fileName">File name.</param>
        string Load(string fileName);

        /// <summary>
        /// Check if the file exists
        /// </summary>
        /// <returns>Exists</returns>
        /// <param name="fileName">File name.</param>
        bool Exists(string fileName);


        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="content">Content.</param>
        /// <param name="fileName">File name.</param>
        void WriteToFile(string content, string fileName);
    }
}
