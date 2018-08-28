using System;

namespace NeoSharp.VM
{
    public class SysCallArgs : EventArgs
    {
        public enum EResult : byte
        {
            /// <summary>
            /// Wrong execution
            /// </summary>
            False = 0,

            /// <summary>
            /// Successful execution
            /// </summary>
            True = 1,

            /// <summary>
            /// The syscall is not found
            /// </summary>
            NotFound = 10,

            /// <summary>
            /// There is not enough gas for compute this syscall
            /// </summary>
            OutOfGas = 11,
        }

        /// <summary>
        /// Result
        /// </summary>
        public EResult Result { get; private set; }

        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Engine
        /// </summary>
        public IExecutionEngine Engine { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="method">Method</param>
        /// <param name="result">Result</param>
        public SysCallArgs(IExecutionEngine engine, string method, EResult result)
        {
            Engine = engine;
            Method = method;
            Result = result;
        }
    }
}