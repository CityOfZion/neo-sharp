using System;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullStack : Stack
    {
        public NullStack(ExecutionEngineBase executionEngine) : base()
        {
        }

        public override int Count => 0;

        public override int Drop(int count = 0)
        {
            throw new NotImplementedException();
        }

        public override StackItemBase Pop()
        {
            throw new NotImplementedException();
        }

        public override void Push(StackItemBase item)
        {
            throw new NotImplementedException();
        }

        public override bool TryPeek(int index, out StackItemBase obj)
        {
            throw new NotImplementedException();
        }

        public override bool TryPop<TStackItem>(out TStackItem item)
        {
            throw new NotImplementedException();
        }
    }
}
