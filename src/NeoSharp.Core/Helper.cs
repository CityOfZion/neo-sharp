using System;
using System.Runtime.CompilerServices;

namespace NeoSharp.Core
{
    public static class Helper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToInt32(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt32(value, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ToInt64(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ushort ToUInt16(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt16(value, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint ToUInt32(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt32(value, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong ToUInt64(this byte[] value, int startIndex)
        {
            return BitConverter.ToUInt64(value, startIndex);
        }
    }
}
