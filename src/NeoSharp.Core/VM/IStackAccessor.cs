using System.Collections;
using System.Numerics;

namespace NeoSharp.Core.VM
{
    public interface IStackAccessor
    {
        void Push(bool value);

        void Push(int value);

        void Push(uint value);

        void Push(long value);

        void Push(ulong value);

        void Push(byte[] value);

        void Push<T>(T item) where T : class;

        void Push<T>(T[] items) where T : class;

        T Peek<T>(int index = 0) where T : class;

        BigInteger PopBigInteger();

        byte[] PopByteArray();

        IEnumerable PopArray();

        T Pop<T>() where T : class;
        byte[] PeekByteArray(int index = 0);
    }
}