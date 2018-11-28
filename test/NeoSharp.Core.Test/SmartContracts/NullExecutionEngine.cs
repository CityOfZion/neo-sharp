using System.Collections.Generic;
using System.Numerics;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    public class NullExecutionEngine : ExecutionEngineBase
    {
        public EVMState PublicState { get; set; }
        public StackBase<ExecutionContextBase> PublicStack { get; set; }
        public Stack PublicStackItemsStack { get; set; }
        public bool PublicDisposed { get; set; }
        public uint PublicConsumedGas { get; set; }

        public override EVMState State { get { return PublicState; } }

        public override StackBase<ExecutionContextBase> InvocationStack { get { return PublicStack; } }

        public override Stack ResultStack { get { return PublicStackItemsStack; } }

        public override bool IsDisposed { get { return PublicDisposed; } }

        public override uint ConsumedGas { get { return PublicConsumedGas; } }

        public override void Clean(uint iteration = 0)
        {
        }

        public override bool IncreaseGas(long gas)
        {
            return true;
        }

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

        public override IntegerStackItemBase CreateInteger(byte[] value)
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
