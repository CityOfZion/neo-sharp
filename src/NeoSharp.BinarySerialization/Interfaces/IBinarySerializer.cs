using System.IO;

namespace NeoSharp.BinarySerialization.Interfaces
{
    public interface IBinarySerializer
    {
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        byte[] Serialize(object obj);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        int Serialize(object obj, Stream stream);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        int Serialize(object obj, BinaryWriter stream);
    }
}