using System;
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

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        T Deserialize<T>(byte[] data, BinarySerializerSettings settings = null);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        T Deserialize<T>(Stream stream, BinarySerializerSettings settings = null);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        T Deserialize<T>(BinaryReader stream, BinarySerializerSettings settings = null);

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        object Deserialize(byte[] data, Type type, BinarySerializerSettings settings = null);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        object Deserialize(Stream stream, Type type, BinarySerializerSettings settings = null);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        object Deserialize(BinaryReader stream, Type type, BinarySerializerSettings settings = null);
    }
}