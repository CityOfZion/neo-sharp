using System.Collections.Generic;
using System.Numerics;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullExecutionEngine : IExecutionEngine
    {
        public EVMState PublicState { get; set; }
        public IStack<IExecutionContext> PublicStack { get; set; }
        public IStackItemsStack PublicStackItemsStack { get; set; }
        public bool PublicDisposed { get; set; }
        public uint PublicConsumedGas { get; set; }

        public override EVMState State { get { return PublicState; } }

        public override IStack<IExecutionContext> InvocationStack { get { return PublicStack; } }

        public override IStackItemsStack ResultStack { get { return PublicStackItemsStack; } }

        public override bool IsDisposed { get { return PublicDisposed; } }

        public override uint ConsumedGas { get { return PublicConsumedGas; } }

        public override void Clean(uint iteration = 0)
        {
        }

        public override bool IncreaseGas(uint gas)
        {
            return true;
        }

        public override IArrayStackItem CreateArray(IEnumerable<IStackItem> items = null)
        {
            return null;
        }

        public override IBooleanStackItem CreateBool(bool value)
        {
            return null;
        }

        public override IByteArrayStackItem CreateByteArray(byte[] data)
        {
            return null;
        }

        public override IIntegerStackItem CreateInteger(int value)
        {
            return null;
        }

        public override IIntegerStackItem CreateInteger(long value)
        {
            return null;
        }

        public override IIntegerStackItem CreateInteger(BigInteger value)
        {
            return null;
        }

        public override IIntegerStackItem CreateInteger(byte[] value)
        {
            return null;
        }

        public override IInteropStackItem CreateInterop(object obj)
        {
            return null;
        }

        public override IMapStackItem CreateMap()
        {
            return null;
        }

        public override IArrayStackItem CreateStruct(IEnumerable<IStackItem> items = null)
        {
            return null;
        }

        public override bool Execute(uint gas = uint.MaxValue)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override int LoadScript(byte[] script)
        {
            return 0;
        }

        public override bool LoadScript(int scriptIndex)
        {
            return true;
        }

        public override void StepInto(int steps = 1)
        {
        }

        public override void StepOut()
        {
        }

        public override void StepOver()
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
