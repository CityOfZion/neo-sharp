using System;
using System.IO;

namespace NeoSharp.BinarySerialization
{
    public interface IBinaryDeserializer
    {
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        T Deserialize<T>(byte[] data) where T : new();
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        /// <returns>Return object</returns>
        object Deserialize(byte[] data, Type type);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <returns>Return object</returns>
        T Deserialize<T>(Stream stream) where T : new();
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <returns>Return object</returns>
        T Deserialize<T>(BinaryReader stream) where T : new();
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <returns>Return object</returns>
        object Deserialize(Stream stream, Type t);
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="obj">Object</param>
        void Deserialize(byte[] buffer, object obj);
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        void Deserialize(Stream stream, object obj);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <returns>Return object</returns>
        object Deserialize(BinaryReader stream, Type t);
    }
}