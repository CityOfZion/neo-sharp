using System;

namespace NeoSharp.VM
{
    public class InteropServiceEntry
    {
        /// <summary>
        /// Name
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Method Hash
        /// </summary>
        public uint MethodHash { get; }

        /// <summary>
        /// MethodHandler
        /// </summary>
        public Func<ExecutionEngineBase, bool> MethodHandler { get; }

        /// <summary>
        /// Gas cost
        /// </summary>
        public uint GasCost { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="methodHash">Method hash</param>
        /// <param name="methodHandler">Method handler</param>
        /// <param name="gasCost">Gas cost</param>
        public InteropServiceEntry(
            string methodName,
            uint methodHash,
            Func<ExecutionEngineBase, bool> methodHandler,
            uint gasCost)
        {
            MethodName = methodName;
            MethodHash = methodHash;
            MethodHandler = methodHandler;
            GasCost = gasCost;
        }
    }
}