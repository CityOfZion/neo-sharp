namespace NeoSharp.VM
{
    public interface IVMFactory
    {
        /// <summary>
        /// Create ExecutionEngine
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Return ExecutionEngine</returns>
        IExecutionEngine Create(ExecutionEngineArgs args);
    }
}