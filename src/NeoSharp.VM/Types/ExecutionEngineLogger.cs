namespace NeoSharp.VM
{
    public class ExecutionEngineLogger
    {
        #region Properties

        /// <summary>
        /// Verbosity
        /// </summary>
        public readonly ELogVerbosity Verbosity;

        #endregion

        #region Events

        /// <summary>
        /// On StepInto
        /// </summary>
        public event delOnStepInto OnStepInto;
        /// <summary>
        /// On ExecutionContextChange
        /// </summary>
        public event delOnExecutionContextStackChange OnExecutionContextChange;
        /// <summary>
        /// On AltStackChange
        /// </summary>
        public event delOnStackItemsStackChange OnAltStackChange;
        /// <summary>
        /// On EvaluationStackChange
        /// </summary>
        public event delOnStackItemsStackChange OnEvaluationStackChange;
        /// <summary>
        /// On ResultStackChange
        /// </summary>
        public event delOnStackItemsStackChange OnResultStackChange;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="verbosity">Verbosity</param>
        public ExecutionEngineLogger(ELogVerbosity verbosity)
        {
            Verbosity = verbosity;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raise OnStepInto
        /// </summary>
        /// <param name="context">Context</param>
        public virtual void RaiseOnStepInto(IExecutionContext context)
        {
            OnStepInto?.Invoke(context);
        }
        /// <summary>
        /// Raise OnExecutionContextChange
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="item">Item</param>
        /// <param name="index">Index</param>
        /// <param name="operation">Operation</param>
        public virtual void RaiseOnExecutionContextChange(IStack<IExecutionContext> stack, IExecutionContext item, int index, ELogStackOperation operation)
        {
            OnExecutionContextChange?.Invoke(stack, item, index, operation);
        }
        /// <summary>
        /// Raise OnResultStackChange
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="item">Item</param>
        /// <param name="index">Index</param>
        /// <param name="operation">Operation</param>
        public virtual void RaiseOnResultStackChange(IStackItemsStack stack, IStackItem item, int index, ELogStackOperation operation)
        {
            OnResultStackChange?.Invoke(stack, item, index, operation);
        }
        /// <summary>
        /// Raise OnAltStackChange
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="item">Item</param>
        /// <param name="index">Index</param>
        /// <param name="operation">Operation</param>
        public virtual void RaiseOnAltStackChange(IStackItemsStack stack, IStackItem item, int index, ELogStackOperation operation)
        {
            OnAltStackChange?.Invoke(stack, item, index, operation);
        }
        /// <summary>
        /// Raise OnEvaluationStackChange
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="item">Item</param>
        /// <param name="index">Index</param>
        /// <param name="operation">Operation</param>
        public virtual void RaiseOnEvaluationStackChange(IStackItemsStack stack, IStackItem item, int index, ELogStackOperation operation)
        {
            OnEvaluationStackChange?.Invoke(stack, item, index, operation);
        }

        #endregion
    }
}