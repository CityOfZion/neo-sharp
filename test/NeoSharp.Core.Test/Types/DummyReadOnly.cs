using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Test.Types
{
    class DummyReadOnly
    {
        [BinaryProperty(0, ValueHandlerLogic = ValueHandlerLogicType.JustConsume)]
        public int A { get; } = 12;
    }
}