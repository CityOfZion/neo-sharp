using System;
using System.Collections.Generic;
using System.Numerics;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullStack : Stack
    {
        public NullStack(ExecutionEngineBase executionEngine) : base()
        {
        }

        protected override ArrayStackItemBase CreateArray(IEnumerable<StackItemBase> items = null)
        {
            return null;
        }

        protected override BooleanStackItemBase CreateBool(bool value)
        {
            return null;
        }

        protected override ByteArrayStackItemBase CreateByteArray(byte[] data)
        {
            return null;
        }

        protected override IntegerStackItemBase CreateInteger(int value)
        {
            return null;
        }

        protected override IntegerStackItemBase CreateInteger(long value)
        {
            return null;
        }

        protected override IntegerStackItemBase CreateInteger(BigInteger value)
        {
            return null;
        }

        protected override InteropStackItemBase<T> CreateInterop<T>(T obj)
        {
            return null;
        }

        protected override MapStackItemBase CreateMap()
        {
            return null;
        }

        protected override ArrayStackItemBase CreateStruct(IEnumerable<StackItemBase> items = null)
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
