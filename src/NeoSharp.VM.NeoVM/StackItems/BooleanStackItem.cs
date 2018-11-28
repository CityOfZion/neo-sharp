using Neo.VM;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class BooleanStackItem : BooleanStackItemBase, INativeStackItemContainer
    {
        public StackItem NativeStackItem => _item;

        public override bool IsDisposed => false;

        private readonly Neo.VM.Types.Boolean _item;

        public BooleanStackItem(Neo.VM.Types.Boolean item) : base(item.GetBoolean())
        {
            _item = item;
        }

        public BooleanStackItem(bool value) : base(value)
        {
            _item = new Neo.VM.Types.Boolean(value);
        }
    }
}