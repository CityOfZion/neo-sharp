using Neo.VM;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class InteropStackItem<T> : InteropStackItemBase<T>, INativeStackItemContainer where T : class
    {
        public StackItem NativeStackItem => _item;

        public override bool IsDisposed => false;

        private readonly Neo.VM.Types.InteropInterface _item;

        public InteropStackItem(Neo.VM.Types.InteropInterface<T> item) : base((T)item)
        {
            _item = item;
        }

        public InteropStackItem(T value) : base(value)
        {
            _item = new Neo.VM.Types.InteropInterface<T>(value);
        }
    }
}