using System;
using System.Collections.Generic;
using System.Numerics;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullStack : Stack
    {
        public override ArrayStackItemBase CreateArray(IEnumerable<StackItemBase> items = null)
        {
            return null;
        }

        public override BooleanStackItemBase CreateBool(bool value)
        {
            return null;
        }

        public override ByteArrayStackItemBase CreateByteArray(byte[] data)
        {
            return null;
        }

        public override IntegerStackItemBase CreateInteger(int value)
        {
            return null;
        }

        public override IntegerStackItemBase CreateInteger(long value)
        {
            return null;
        }

        public override IntegerStackItemBase CreateInteger(BigInteger value)
        {
            return null;
        }

        public override InteropStackItemBase<T> CreateInterop<T>(T obj)
        {
            return null;
        }

        public override MapStackItemBase CreateMap()
        {
            return null;
        }

        public override ArrayStackItemBase CreateStruct(IEnumerable<StackItemBase> items = null)
        {
            return null;
        }

        public override int Count => 0;

        public override void Push(StackItemBase item)
        {
            throw new NotImplementedException();
        }

        public override bool TryPeek(int index, out StackItemBase item)
        {
            throw new NotImplementedException();
        }

        public override bool TryPop(out StackItemBase item)
        {
            throw new NotImplementedException();
        }
    }
}
