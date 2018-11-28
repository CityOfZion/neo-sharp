using System.Numerics;
using Neo.VM;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class IntegerStackItem : IntegerStackItemBase, INativeStackItemContainer
    {
        public StackItem NativeStackItem => _item;

        public override bool IsDisposed => false;

        private readonly Neo.VM.Types.Integer _item;

        public IntegerStackItem(Neo.VM.Types.Integer item) : base(item.GetBigInteger())
        {
            _item = item;
        }

        public IntegerStackItem(int value) : base(value)
        {
            _item = new Neo.VM.Types.Integer(new BigInteger(value));
        }

        public IntegerStackItem(long value) : base(value)
        {
            _item = new Neo.VM.Types.Integer(new BigInteger(value));
        }

        public IntegerStackItem(BigInteger value) : base(value)
        {
            _item = new Neo.VM.Types.Integer(value);
        }

        public IntegerStackItem(byte[] value) : base(new BigInteger(value))
        {
            _item = new Neo.VM.Types.Integer(new BigInteger(value));
        }
    }
}