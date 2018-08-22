namespace NeoSharp.VM.Types
{
    public class InteropServiceEntry
    {
        /// <summary>
        /// Delegate
        /// </summary>
        /// <param name="engine">Execution engine</param>
        /// <returns>Return false if something wrong</returns>
        public delegate bool delHandler(IExecutionEngine engine);

        /// <summary>
        /// Gas cost
        /// </summary>
        public readonly uint GasCost;

        /// <summary>
        /// Handler
        /// </summary>
        public readonly delHandler Handler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="gasCost">Gas cost</param>
        public InteropServiceEntry(delHandler handler, uint gasCost)
        {
            Handler = handler;
            GasCost = gasCost;
        }
    }
}