using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NeoSharp.VM
{
    public abstract class ExecutionEngineBase : IDisposable
    {
        #region Public fields from arguments

        /// <summary>
        /// Gas ratio for computation
        /// </summary>
        public const ulong GasRatio = 100000;

        /// <summary>
        /// Trigger
        /// </summary>
        public ETriggerType Trigger { get; }

        /// <summary>
        /// Interop service
        /// </summary>
        public InteropService InteropService { get; }

        /// <summary>
        /// Script table
        /// </summary>
        public IScriptTable ScriptTable { get; }

        /// <summary>
        /// Logger
        /// </summary>
        public ExecutionEngineLogger Logger { get; }

        /// <summary>
        /// Message Provider
        /// </summary>
        public IMessageProvider MessageProvider { get; }

        #endregion

        /// <summary>
        /// Virtual Machine State
        /// </summary>
        public abstract EVMState State { get; }

        /// <summary>
        /// InvocationStack
        /// </summary>
        public abstract StackBase<ExecutionContextBase> InvocationStack { get; }

        /// <summary>
        /// ResultStack
        /// </summary>
        public abstract Stack ResultStack { get; }

        /// <summary>
        /// Is disposed
        /// </summary>
        public abstract bool IsDisposed { get; }

        /// <summary>
        /// Consumed Gas
        /// </summary>
        public abstract ulong ConsumedGas { get; }

        #region Shortcuts

        public ExecutionContextBase CurrentContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InvocationStack.TryPeek(0, out var i) ? i : null;
        }

        public ExecutionContextBase CallingContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InvocationStack.TryPeek(1, out var i) ? i : null;
        }

        public ExecutionContextBase EntryContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => InvocationStack.TryPeek(-1, out var i) ? i : null;
        }

        #endregion

        /// <summary>
        /// For unit testing only
        /// </summary>
        protected ExecutionEngineBase() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">Arguments</param>
        protected ExecutionEngineBase(ExecutionEngineArgs e)
        {
            if (e == null) return;

            InteropService = e.InteropService;
            ScriptTable = e.ScriptTable;
            MessageProvider = e.MessageProvider;
            Trigger = e.Trigger;
            Logger = e.Logger;
        }

        #region Load Script

        /// <summary>
        /// Load script
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Script index in script cache</returns>
        public abstract int LoadScript(byte[] script);

        /// <summary>
        /// Load script
        /// </summary>
        /// <param name="scriptIndex">Script Index</param>
        /// <returns>True if is loaded</returns>
        public abstract bool LoadScript(int scriptIndex);

        #endregion

        #region Execution

        /// <summary>
        /// Increase gas
        /// </summary>
        /// <param name="gas">Gas</param>
        public abstract bool IncreaseGas(ulong gas);

        /// <summary>
        /// Clean Execution engine state
        /// </summary>
        /// <param name="iteration">Iteration</param>
        public abstract void Clean(uint iteration = 0);

        /// <summary>
        /// Execute (until x of Gas)
        /// </summary>
        /// <param name="gas">Gas</param>
        public abstract bool Execute(ulong gas = ulong.MaxValue);

        /// <summary>
        /// Step Into
        /// </summary>
        /// <param name="steps">Steps</param>
        public abstract void StepInto(int steps = 1);

        /// <summary>
        /// Step Out
        /// </summary>
        public abstract void StepOut();

        /// <summary>
        /// Step Over
        /// </summary>
        public abstract void StepOver();

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Dispose logic
        /// </summary>
        /// <param name="disposing">False for cleanup native objects</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ExecutionEngineBase()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}