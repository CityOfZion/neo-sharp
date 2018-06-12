using System.IO;

namespace NeoSharp.BinarySerialization
{
    public interface IBinarySerializer
    {
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        byte[] Serialize(object obj, BinarySerializerSettings settings = null);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        int Serialize(object obj, Stream stream, BinarySerializerSettings settings = null);
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        int Serialize(object obj, BinaryWriter stream, BinarySerializerSettings settings = null);
    }
}