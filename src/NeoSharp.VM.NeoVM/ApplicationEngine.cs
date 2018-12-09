using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Neo.VM;
using Neo.VM.Types;
using NeoSharp.Types.ExtensionMethods;
using NeoSharp.VM.NeoVM.StackItems;

namespace NeoSharp.VM.NeoVM
{
    public class ApplicationEngine : ExecutionEngineBase
    {
        private static readonly CryptoAdapter _crypto = new CryptoAdapter();

        private readonly ExecutionEngine _engine;
        private readonly StackExecutionContextWrapper _invocationStack;
        private readonly StackWrapper _resultStack;

        private const long Ratio = 100000;
        private ulong _gasConsumed;
        private ulong _gasAmount;
        private bool _isDisposed;
        private ContractScriptTable _contractScriptTable;

        private int _stackItemCount;
        private bool _isStackItemCountStrict = true;

        private List<byte[]> _scriptTable;

        private EVMState _state;

        public override ulong ConsumedGas => _gasConsumed;

        public override StackBase<ExecutionContextBase> InvocationStack => _invocationStack;

        public override Stack ResultStack => _resultStack;

        public override EVMState State => _state;

        public override void Clean(uint iteration = 0)
        {
            _gasConsumed = _gasAmount = 0;
            _state = EVMState.None;
        }

        public override bool IsDisposed => _isDisposed;

        #region Load script

        public override int LoadScript(byte[] script)
        {
            _scriptTable.Add(script);
            _engine.LoadScript(script);

            return _scriptTable.Count;
        }

        public override bool LoadScript(int scriptIndex)
        {
            if (_scriptTable.Count > scriptIndex)
            {
                _engine.LoadScript(_scriptTable[scriptIndex]);

                return true;
            }

            return false;
        }

        #endregion

        #region Limits

        /// <summary>
        /// Max value for SHL and SHR
        /// </summary>
        public const int MaxShlShr = ushort.MaxValue;
        /// <summary>
        /// Min value for SHL and SHR
        /// </summary>
        public const int MinShlShr = -MaxShlShr;
        /// <summary>
        /// Set the max size allowed size for BigInteger
        /// </summary>
        public const int MaxSizeForBigInteger = 32;
        /// <summary>
        /// Set the max Stack Size
        /// </summary>
        public const uint MaxStackSize = 2 * 1024;
        /// <summary>
        /// Set Max Item Size
        /// </summary>
        public const uint MaxItemSize = 1024 * 1024;
        /// <summary>
        /// Set Max Invocation Stack Size
        /// </summary>
        public const uint MaxInvocationStackSize = 1024;
        /// <summary>
        /// Set Max Array Size
        /// </summary>
        public const uint MaxArraySize = 1024;

        #endregion

        public ApplicationEngine(ExecutionEngineArgs args) : base(args)
        {
            _isDisposed = false;
            _gasConsumed = _gasAmount = 0;
            _state = EVMState.None;
            _scriptTable = new List<byte[]>();

            _contractScriptTable = new ContractScriptTable(args.ScriptTable);

            _engine = new ExecutionEngine(
                new ScriptContainerWrapper(args.MessageProvider),
                _crypto,
                _contractScriptTable,
                new InteropServiceAdapter(this, args.InteropService));

            _resultStack = new StackWrapper(_engine.ResultStack);
            _invocationStack = new StackExecutionContextWrapper(_engine.InvocationStack);
        }

        private bool CheckArraySize(OpCode nextInstruction)
        {
            int size;

            switch (nextInstruction)
            {
                case OpCode.PACK:
                case OpCode.NEWARRAY:
                case OpCode.NEWSTRUCT:
                    {
                        if (_engine.CurrentContext.EvaluationStack.Count == 0) return false;
                        size = (int)_engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                    }
                    break;
                case OpCode.SETITEM:
                    {
                        if (_engine.CurrentContext.EvaluationStack.Count < 3) return false;
                        if (!(_engine.CurrentContext.EvaluationStack.Peek(2) is Map map)) return true;
                        var key = _engine.CurrentContext.EvaluationStack.Peek(1);
                        if (key is ICollection) return false;
                        if (map.ContainsKey(key)) return true;
                        size = map.Count + 1;
                    }
                    break;
                case OpCode.APPEND:
                    {
                        if (_engine.CurrentContext.EvaluationStack.Count < 2) return false;
                        if (!(_engine.CurrentContext.EvaluationStack.Peek(1) is Array array)) return false;
                        size = array.Count + 1;
                    }
                    break;
                default:
                    return true;
            }

            return size <= MaxArraySize;
        }

        private bool CheckInvocationStack(OpCode nextInstruction)
        {
            switch (nextInstruction)
            {
                case OpCode.CALL:
                case OpCode.APPCALL:
                case OpCode.CALL_I:
                case OpCode.CALL_E:
                case OpCode.CALL_ED:
                    return _engine.InvocationStack.Count < MaxInvocationStackSize;
                default:
                    return true;
            }
        }

        private bool CheckItemSize(OpCode nextInstruction)
        {
            switch (nextInstruction)
            {
                case OpCode.PUSHDATA4:
                    {
                        if (_engine.CurrentContext.InstructionPointer + 4 >= _engine.CurrentContext.Script.Length)
                            return false;

                        var length = _engine.CurrentContext.Script.ToUInt32(CurrentContext.InstructionPointer + 1);

                        return length <= MaxItemSize;
                    }
                case OpCode.CAT:
                    {
                        if (_engine.CurrentContext.EvaluationStack.Count < 2) return false;

                        var length = _engine.CurrentContext.EvaluationStack.Peek().GetByteArray().Length + _engine.CurrentContext.EvaluationStack.Peek(1).GetByteArray().Length;

                        return length <= MaxItemSize;
                    }
                default:
                    return true;
            }
        }

        /// <summary>
        /// Check if the BigInteger is allowed for numeric operations
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return True if are allowed, otherwise False</returns>
        private static bool CheckBigInteger(BigInteger value)
        {
            return value.ToByteArray().Length <= MaxSizeForBigInteger;
        }

        /// <summary>
        /// Check if the BigInteger is allowed for numeric operations
        /// </summary> 
        private bool CheckBigIntegers(OpCode nextInstruction)
        {
            switch (nextInstruction)
            {
                case OpCode.SHL:
                    {
                        var ishift = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();

                        if ((ishift > MaxShlShr || ishift < MinShlShr))
                            return false;

                        var x = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x << (int)ishift))
                            return false;

                        break;
                    }
                case OpCode.SHR:
                    {
                        var ishift = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();

                        if ((ishift > MaxShlShr || ishift < MinShlShr))
                            return false;

                        var x = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x >> (int)ishift))
                            return false;

                        break;
                    }
                case OpCode.INC:
                    {
                        var x = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();

                        if (!CheckBigInteger(x) || !CheckBigInteger(x + 1))
                            return false;

                        break;
                    }
                case OpCode.DEC:
                    {
                        var x = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();

                        if (!CheckBigInteger(x) || (x.Sign <= 0 && !CheckBigInteger(x - 1)))
                            return false;

                        break;
                    }
                case OpCode.ADD:
                    {
                        var x2 = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        var x1 = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x2) || !CheckBigInteger(x1) || !CheckBigInteger(x1 + x2))
                            return false;

                        break;
                    }
                case OpCode.SUB:
                    {
                        var x2 = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        var x1 = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x2) || !CheckBigInteger(x1) || !CheckBigInteger(x1 - x2))
                            return false;

                        break;
                    }
                case OpCode.MUL:
                    {
                        var x2 = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        var x1 = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        var lx1 = x1.ToByteArray().Length;

                        if (lx1 > MaxSizeForBigInteger)
                            return false;

                        var lx2 = x2.ToByteArray().Length;

                        if ((lx1 + lx2) > MaxSizeForBigInteger)
                            return false;

                        break;
                    }
                case OpCode.DIV:
                    {
                        var x2 = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        var x1 = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x2) || !CheckBigInteger(x1))
                            return false;

                        break;
                    }
                case OpCode.MOD:
                    {
                        var x2 = _engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        var x1 = _engine.CurrentContext.EvaluationStack.Peek(1).GetBigInteger();

                        if (!CheckBigInteger(x2) || !CheckBigInteger(x1))
                            return false;

                        break;
                    }
            }

            return true;
        }

        private bool CheckStackSize(OpCode nextInstruction)
        {
            if (nextInstruction <= OpCode.PUSH16)
                _stackItemCount += 1;
            else
                switch (nextInstruction)
                {
                    case OpCode.JMPIF:
                    case OpCode.JMPIFNOT:
                    case OpCode.DROP:
                    case OpCode.NIP:
                    case OpCode.EQUAL:
                    case OpCode.BOOLAND:
                    case OpCode.BOOLOR:
                    case OpCode.CHECKMULTISIG:
                    case OpCode.REVERSE:
                    case OpCode.HASKEY:
                    case OpCode.THROWIFNOT:
                        _stackItemCount -= 1;
                        _isStackItemCountStrict = false;
                        break;
                    case OpCode.XSWAP:
                    case OpCode.ROLL:
                    case OpCode.CAT:
                    case OpCode.LEFT:
                    case OpCode.RIGHT:
                    case OpCode.AND:
                    case OpCode.OR:
                    case OpCode.XOR:
                    case OpCode.ADD:
                    case OpCode.SUB:
                    case OpCode.MUL:
                    case OpCode.DIV:
                    case OpCode.MOD:
                    case OpCode.SHL:
                    case OpCode.SHR:
                    case OpCode.NUMEQUAL:
                    case OpCode.NUMNOTEQUAL:
                    case OpCode.LT:
                    case OpCode.GT:
                    case OpCode.LTE:
                    case OpCode.GTE:
                    case OpCode.MIN:
                    case OpCode.MAX:
                    case OpCode.CHECKSIG:
                    case OpCode.CALL_ED:
                    case OpCode.CALL_EDT:
                        _stackItemCount -= 1;
                        break;
                    case OpCode.RET:
                    case OpCode.APPCALL:
                    case OpCode.TAILCALL:
                    case OpCode.NOT:
                    case OpCode.ARRAYSIZE:
                        _isStackItemCountStrict = false;
                        break;
                    case OpCode.SYSCALL:
                    case OpCode.PICKITEM:
                    case OpCode.SETITEM:
                    case OpCode.APPEND:
                    case OpCode.VALUES:
                        _stackItemCount = int.MaxValue;
                        _isStackItemCountStrict = false;
                        break;
                    case OpCode.DUPFROMALTSTACK:
                    case OpCode.DEPTH:
                    case OpCode.DUP:
                    case OpCode.OVER:
                    case OpCode.TUCK:
                    case OpCode.NEWMAP:
                        _stackItemCount += 1;
                        break;
                    case OpCode.XDROP:
                    case OpCode.REMOVE:
                        _stackItemCount -= 2;
                        _isStackItemCountStrict = false;
                        break;
                    case OpCode.SUBSTR:
                    case OpCode.WITHIN:
                    case OpCode.VERIFY:
                        _stackItemCount -= 2;
                        break;
                    case OpCode.UNPACK:
                        _stackItemCount += (int)_engine.CurrentContext.EvaluationStack.Peek().GetBigInteger();
                        _isStackItemCountStrict = false;
                        break;
                    case OpCode.NEWARRAY:
                    case OpCode.NEWSTRUCT:
                        _stackItemCount += ((Array)_engine.CurrentContext.EvaluationStack.Peek()).Count;
                        break;
                    case OpCode.KEYS:
                        _stackItemCount += ((Array)_engine.CurrentContext.EvaluationStack.Peek()).Count;
                        _isStackItemCountStrict = false;
                        break;
                }
            if (_stackItemCount <= MaxStackSize) return true;
            if (_isStackItemCountStrict) return false;
            _stackItemCount = GetItemCount(_engine.InvocationStack.SelectMany(p => p.EvaluationStack.Concat(p.AltStack)));
            if (_stackItemCount > MaxStackSize) return false;
            _isStackItemCountStrict = true;
            return true;
        }

        private bool CheckDynamicInvoke(OpCode nextInstruction)
        {
            switch (nextInstruction)
            {
                case OpCode.APPCALL:
                case OpCode.TAILCALL:
                    for (var i = CurrentContext.InstructionPointer + 1; i < CurrentContext.InstructionPointer + 21; i++)
                    {
                        if (_engine.CurrentContext.Script[i] != 0) return true;
                    }
                    // if we get this far it is a dynamic call
                    // now look at the current executing script
                    // to determine if it can do dynamic calls
                    return _contractScriptTable.GetScript(CurrentContext.ScriptHash, true) != null;
                case OpCode.CALL_ED:
                case OpCode.CALL_EDT:
                    return _contractScriptTable.GetScript(CurrentContext.ScriptHash, true) != null;
                default:
                    return true;
            }
        }

        #region Execute

        public override bool IncreaseGas(ulong gas)
        {
            _gasConsumed = checked(_gasConsumed + gas);

            if (_gasConsumed > _gasAmount)
            {
                _state = EVMState.FaultByGas;
                return false;
            }

            return true;
        }

        public override void StepOut() => _engine.StepOut();

        public override void StepOver() => _engine.StepOver();

        public override void StepInto(int steps = 1)
        {
            for (int x = 0; x < steps; x++)
            {
                if (Logger.Verbosity.HasFlag(ELogVerbosity.StepInto))
                {
                    Logger.RaiseOnStepInto(CurrentContext);
                }

                _engine.StepInto();
            }
        }

        public override bool Execute(ulong gas = ulong.MaxValue)
        {
            _gasAmount = gas;

            try
            {
                while (true)
                {
                    var nextOpCode = _engine.CurrentContext.InstructionPointer >= _engine.CurrentContext.Script.Length ? OpCode.RET : _engine.CurrentContext.NextInstruction;

                    if (!PreStepInto(nextOpCode))
                    {
                        _state = EVMState.Fault;
                        return false;
                    }

                    StepInto();

                    if (_engine.State.HasFlag(VMState.HALT) || _engine.State.HasFlag(VMState.FAULT))
                    {
                        _state = (EVMState)(_engine.State & ~VMState.BREAK);
                        break;
                    }

                    if (PostStepInto(nextOpCode)) continue;

                    _state = EVMState.Fault;
                    return false;
                }
            }
            catch
            {
                _state = EVMState.Fault;
                return false;
            }

            return _state == EVMState.Halt;
        }

        #endregion

        private static int GetItemCount(IEnumerable<StackItem> items)
        {
            var queue = new Queue<StackItem>(items);
            var counted = new List<StackItem>();
            var count = 0;

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                count++;

                switch (item)
                {
                    case Array array:
                        if (counted.Any(p => ReferenceEquals(p, array)))
                            continue;

                        counted.Add(array);

                        foreach (var arrayItem in array)
                            queue.Enqueue(arrayItem);

                        break;
                    case Map map:
                        if (counted.Any(p => ReferenceEquals(p, map)))
                            continue;

                        counted.Add(map);

                        foreach (var mapItem in map.Values)
                            queue.Enqueue(mapItem);

                        break;
                }
            }

            return count;
        }

        protected virtual ulong GetPrice(OpCode nextInstruction)
        {
            if (nextInstruction <= OpCode.PUSH16) return 0;

            switch (nextInstruction)
            {
                case OpCode.NOP:
                    return 0;
                case OpCode.APPCALL:
                case OpCode.TAILCALL:
                    return 10;
                case OpCode.SYSCALL:
                    return 0;
                case OpCode.SHA1:
                case OpCode.SHA256:
                    return 10;
                case OpCode.HASH160:
                case OpCode.HASH256:
                    return 20;
                case OpCode.CHECKSIG:
                case OpCode.VERIFY:
                    return 100;
                case OpCode.CHECKMULTISIG:
                    {
                        if (CurrentContext.EvaluationStack.Count == 0) return 1;

                        var item = _engine.CurrentContext.EvaluationStack.Peek();
                        var n = item is Array array ? array.Count : (int)item.GetBigInteger();

                        return n < 1 ? 1UL : (ulong)(100 * n);
                    }
                default: return 1;
            }
        }

        private bool PreStepInto(OpCode nextOpCode)
        {
            if (_engine.CurrentContext.InstructionPointer >= _engine.CurrentContext.Script.Length)
                return true;

            if (!IncreaseGas(GetPrice(nextOpCode) * Ratio)) return false;
            if (!CheckItemSize(nextOpCode)) return false;
            if (!CheckArraySize(nextOpCode)) return false;
            if (!CheckInvocationStack(nextOpCode)) return false;
            if (!CheckBigIntegers(nextOpCode)) return false;
            if (!CheckDynamicInvoke(nextOpCode)) return false;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            _isDisposed = true;

            Clean();
            _scriptTable.Clear();
        }

        private bool PostStepInto(OpCode nextOpCode) => CheckStackSize(nextOpCode);
    }
}