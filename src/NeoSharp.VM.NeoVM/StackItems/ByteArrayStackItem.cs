using Neo.VM;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class ByteArrayStackItem : ByteArrayStackItemBase, INativeStackItemContainer
    {
        public StackItem NativeStackItem => _item;

        public override bool IsDisposed => false;

        private readonly Neo.VM.Types.ByteArray _item;

        public ByteArrayStackItem(Neo.VM.Types.ByteArray item) : base(item.GetByteArray())
        {
            _item = item;
        }

        public ByteArrayStackItem(byte[] value) : base(value)
        {
            _item = new Neo.VM.Types.ByteArray(value);
        }
    }
}