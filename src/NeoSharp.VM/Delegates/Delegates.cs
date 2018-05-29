namespace NeoSharp.VM
{
    /// <summary>
    /// Delegate for receive operations
    /// </summary>
    /// <param name="context">Execution context</param>
    public delegate void delOnStepInto(IExecutionContext context);
    /// <summary>
    /// Delegate for receive ExecutionContextStack changes
    /// </summary>
    /// <param name="stack">Stack</param>
    /// <param name="item">ExecutionContext</param>
    /// <param name="index">Index</param>
    /// <param name="operation">Operation</param>
    public delegate void delOnExecutionContextStackChange(IStack<IExecutionContext> stack, IExecutionContext item, int index, ELogStackOperation operation);
    /// <summary>
    /// Delegate for receive StackItemStack changes
    /// </summary>
    /// <param name="stack">Stack</param>
    /// <param name="item">StackItem</param>
    /// <param name="index">Index</param>
    /// <param name="operation">Operation</param>
    public delegate void delOnStackItemsStackChange(IStackItemsStack stack, IStackItem item, int index, ELogStackOperation operation);
}