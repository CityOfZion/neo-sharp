using System;
using NeoSharp.VM.Extensions;
using Newtonsoft.Json;

namespace NeoSharp.VM
{
    public abstract class IExecutionContext : IDisposable
    {
        /// <summary>
        /// ScriptHashLength
        /// </summary>
        public const int ScriptHashLength = 20;

        /// <summary>
        /// Engine
        /// </summary>
        [JsonIgnore]
        public readonly IExecutionEngine Engine;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        public abstract bool IsDisposed { get; }
        
        /// <summary>
        /// Evaluation Stack
        /// </summary>
        public abstract IStackItemsStack EvaluationStack { get; }
        
        /// <summary>
        /// Alt Stack
        /// </summary>
        public abstract IStackItemsStack AltStack { get; }
        
        /// <summary>
        /// Script Hash
        /// </summary>
        public abstract byte[] ScriptHash { get; }
        
        /// <summary>
        /// Next instruction
        /// </summary>
        public abstract EVMOpCode NextInstruction { get; }

        /// <summary>
        /// Get Instruction pointer
        /// </summary>
        public abstract int InstructionPointer { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        protected IExecutionContext(IExecutionEngine engine)
        {
            Engine = engine;
        }

        #region IDisposable Support

        /// <summary>
        /// Dispose logic
        /// </summary>
        /// <param name="disposing">False for cleanup native objects</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IExecutionContext()
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

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return "[" + ScriptHash.ToHexString() + "-" + InstructionPointer.ToString("x6") + "] " + NextInstruction.ToString();
        }
    }
}