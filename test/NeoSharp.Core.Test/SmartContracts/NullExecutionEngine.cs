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
        public override ulong ConsumedGas { get { return PublicConsumedGas; } }
        public override ulong GasAmount { get; set; } = ulong.MaxValue;

        public override EVMState State { get { return PublicState; } }

        public override StackBase<ExecutionContextBase> InvocationStack { get { return PublicStack; } }

        public override Stack ResultStack { get { return PublicStackItemsStack; } }

        public override bool IsDisposed { get { return PublicDisposed; } }

        public override void Clean(uint iteration = 0) { }

        public override bool IncreaseGas(ulong gas) => true;

        public override bool Execute() => true;

        public override int GetHashCode() => base.GetHashCode();

        public override int LoadScript(byte[] script) => 0;

        public override bool LoadScript(int scriptIndex) => true;

        public override void StepInto() { }

        public override void StepOut() { }

        public override void StepOver() { }

        public override string ToString() { return base.ToString(); }

        protected override void Dispose(bool disposing) { base.Dispose(disposing); }
    }
}