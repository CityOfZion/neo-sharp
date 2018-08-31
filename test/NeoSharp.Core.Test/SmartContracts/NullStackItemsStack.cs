using System;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullStackItemsStack : IStackItemsStack
    {
        public NullStackItemsStack(IExecutionEngine executionEngine) : base()
        {
        }

        public override int Count => 0;

        public override int Drop(int count = 0)
        {
            throw new NotImplementedException();
        }

        public override IStackItem Pop()
        {
            throw new NotImplementedException();
        }

        public override void Push(IStackItem item)
        {
            throw new NotImplementedException();
        }

        public override bool TryPeek(int index, out IStackItem obj)
        {
            throw new NotImplementedException();
        }

        public override bool TryPop<TStackItem>(out TStackItem item)
        {
            throw new NotImplementedException();
        }
    }
}
