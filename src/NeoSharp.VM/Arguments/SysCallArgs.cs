using System;

namespace NeoSharp.VM
{
    public class SysCallArgs : EventArgs
    {
        /// <summary>
        /// Engine
        /// </summary>
        public IExecutionEngine Engine { get; }

        /// <summary>
        /// Hash Method
        /// </summary>
        public uint MethodHash { get; }

        /// <summary>
        /// Method
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Result
        /// </summary>
        public SysCallResult Result { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="methodName">Method name</param>
        /// <param name="methodHash">Method hash</param>
        /// <param name="result">Result</param>
        public SysCallArgs(IExecutionEngine engine, string methodName, uint methodHash, SysCallResult result)
        {
            Engine = engine;
            MethodName = methodName;
            MethodHash = methodHash;
            Result = result;
        }
    }
}