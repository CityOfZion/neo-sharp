using System.Runtime.CompilerServices;

namespace NeoSharp.VM
{
    public interface IExecutionEngine
    {
        /// <summary>
        /// Trigger
        /// </summary>
        ETriggerType Trigger { get; }

        /// <summary>
        /// Interop service
        /// </summary>
        InteropService InteropService { get; }

        /// <summary>
        /// Script table
        /// </summary>
        IScriptTable ScriptTable { get; }

        /// <summary>
        /// Logger
        /// </summary>
        ExecutionEngineLogger Logger { get; }

        /// <summary>
        /// Message Provider
        /// </summary>
        IMessageProvider MessageProvider { get; }

        /// <summary>
        /// Virtual Machine State
        /// </summary>
        EVMState State { get; }

        /// <summary>
        /// InvocationStack
        /// </summary>
        StackBase<ExecutionContextBase> InvocationStack { get; }

        /// <summary>
        /// ResultStack
        /// </summary>
        Stack ResultStack { get; }

        /// <summary>
        /// Is disposed
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Consumed Gas
        /// </summary>
        ulong ConsumedGas { get; }

        /// <summary>
        /// Gas Amount (ulong.MaxValue by default)
        /// </summary>
        ulong GasAmount { get; set; }

        ExecutionContextBase CurrentContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        ExecutionContextBase CallingContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        ExecutionContextBase EntryContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        /// <summary>
        /// Load script
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Script index in script cache</returns>
        int LoadScript(byte[] script);

        /// <summary>
        /// Load script
        /// </summary>
        /// <param name="scriptIndex">Script Index</param>
        /// <returns>True if is loaded</returns>
        bool LoadScript(int scriptIndex);

        /// <summary>
        /// Increase gas
        /// </summary>
        /// <param name="gas">Gas</param>
        bool IncreaseGas(ulong gas);

        /// <summary>
        /// Clean Execution engine state
        /// </summary>
        /// <param name="iteration">Iteration</param>
        void Clean(uint iteration = 0);

        /// <summary>
        /// Execute
        /// </summary>
        bool Execute();

        /// <summary>
        /// Step Into
        /// </summary>
        void StepInto();

        /// <summary>
        /// Step Into
        /// </summary>
        /// <param name="steps">Steps</param>
        void StepInto(int steps);

        /// <summary>
        /// Step Out
        /// </summary>
        void StepOut();

        /// <summary>
        /// Step Over
        /// </summary>
        void StepOver();
    }
}