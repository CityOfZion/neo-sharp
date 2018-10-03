using System;
using System.Runtime.CompilerServices;

namespace NeoSharp.Core.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// Convert Byte Array to Int64
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">Start index</param>
        /// <returns>Int64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInt64(this byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt64(value, startIndex);
        }

        /// <summary>
        /// Convert Byte Array to UInt16
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">Start index</param>
        /// <returns>UInt16</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(this byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt16(value, startIndex);
        }

        /// <summary>
        /// Convert Byte Array to UInt64
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">Start index</param>
        /// <returns>UInt64</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64(this byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt64(value, startIndex);
        }
    }
}