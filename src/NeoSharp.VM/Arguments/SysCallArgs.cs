using System;

namespace NeoSharp.VM
{
    public class SysCallArgs : EventArgs
    {
        public enum EResult : byte
        {
            False = 0,
            True = 1,

            NotFound = 10,
            OutOfGas = 11,
        }

        /// <summary>
        /// Result
        /// </summary>
        public readonly EResult Result;

        /// <summary>
        /// Method
        /// </summary>
        public readonly string Method;

        /// <summary>
        /// Engine
        /// </summary>
        public readonly IExecutionEngine Engine;

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